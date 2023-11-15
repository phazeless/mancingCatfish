using System;
using System.Linq;
using System.Threading;

namespace UnityThreading
{
	public class TaskDistributor : DispatcherBase
	{
		public TaskDistributor(string name) : this(name, 0)
		{
		}

		public TaskDistributor(string name, int workerThreadCount) : this(name, workerThreadCount, true)
		{
		}

		public TaskDistributor(string name, int workerThreadCount, bool autoStart)
		{
			this.name = name;
			if (workerThreadCount <= 0)
			{
				workerThreadCount = ThreadBase.AvailableProcessors * 2;
			}
			this.workerThreads = new TaskWorker[workerThreadCount];
			object obj = this.workerThreads;
			lock (obj)
			{
				for (int i = 0; i < workerThreadCount; i++)
				{
					this.workerThreads[i] = new TaskWorker(name, this);
				}
			}
			if (TaskDistributor.mainTaskDistributor == null)
			{
				TaskDistributor.mainTaskDistributor = this;
			}
			if (autoStart)
			{
				this.Start();
			}
		}

		internal WaitHandle NewDataWaitHandle
		{
			get
			{
				return this.dataEvent;
			}
		}

		public static TaskDistributor Main
		{
			get
			{
				if (TaskDistributor.mainTaskDistributor == null)
				{
					throw new InvalidOperationException("No default TaskDistributor found, please create a new TaskDistributor instance before calling this property.");
				}
				return TaskDistributor.mainTaskDistributor;
			}
		}

		public static TaskDistributor MainNoThrow
		{
			get
			{
				return TaskDistributor.mainTaskDistributor;
			}
		}

		public override int TaskCount
		{
			get
			{
				int num = base.TaskCount;
				object obj = this.workerThreads;
				lock (obj)
				{
					for (int i = 0; i < this.workerThreads.Length; i++)
					{
						num += this.workerThreads[i].Dispatcher.TaskCount;
					}
				}
				return num;
			}
		}

		public void Start()
		{
			object obj = this.workerThreads;
			lock (obj)
			{
				for (int i = 0; i < this.workerThreads.Length; i++)
				{
					if (!this.workerThreads[i].IsAlive)
					{
						this.workerThreads[i].Start();
					}
				}
			}
		}

		public void SpawnAdditionalWorkerThread()
		{
			object obj = this.workerThreads;
			lock (obj)
			{
				Array.Resize<TaskWorker>(ref this.workerThreads, this.workerThreads.Length + 1);
				this.workerThreads[this.workerThreads.Length - 1] = new TaskWorker(this.name, this);
				this.workerThreads[this.workerThreads.Length - 1].Priority = this.priority;
				this.workerThreads[this.workerThreads.Length - 1].Start();
			}
		}

		internal void FillTasks(Dispatcher target)
		{
			target.AddTasks(base.IsolateTasks(1));
		}

		protected override void CheckAccessLimitation()
		{
			if (this.MaxAdditionalWorkerThreads > 0 || !this.AllowAccessLimitationChecks)
			{
				return;
			}
			if (ThreadBase.CurrentThread != null && ThreadBase.CurrentThread is TaskWorker && ((TaskWorker)ThreadBase.CurrentThread).TaskDistributor == this)
			{
				throw new InvalidOperationException("Access to TaskDistributor prohibited when called from inside a TaskDistributor thread. Dont dispatch new Tasks through the same TaskDistributor. If you want to distribute new tasks create a new TaskDistributor and use the new created instance. Remember to dispose the new instance to prevent thread spamming.");
			}
		}

		internal override void TasksAdded()
		{
			if (this.MaxAdditionalWorkerThreads > 0)
			{
				if (this.workerThreads.All((TaskWorker worker) => worker.Dispatcher.TaskCount > 0 || worker.IsWorking) || this.taskList.Count > this.workerThreads.Length)
				{
					Interlocked.Decrement(ref this.MaxAdditionalWorkerThreads);
					this.SpawnAdditionalWorkerThread();
				}
			}
			base.TasksAdded();
		}

		public override void Dispose()
		{
			if (this.isDisposed)
			{
				return;
			}
			for (;;)
			{
				object taskListSyncRoot = this.taskListSyncRoot;
				Task task;
				lock (taskListSyncRoot)
				{
					if (this.taskList.Count == 0)
					{
						break;
					}
					task = this.taskList.Dequeue();
				}
				task.Dispose();
			}
			object obj = this.workerThreads;
			lock (obj)
			{
				for (int i = 0; i < this.workerThreads.Length; i++)
				{
					this.workerThreads[i].Dispose();
				}
				this.workerThreads = new TaskWorker[0];
			}
			this.dataEvent.Close();
			this.dataEvent = null;
			if (TaskDistributor.mainTaskDistributor == this)
			{
				TaskDistributor.mainTaskDistributor = null;
			}
			this.isDisposed = true;
		}

		public ThreadPriority Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
				foreach (TaskWorker taskWorker in this.workerThreads)
				{
					taskWorker.Priority = value;
				}
			}
		}

		private TaskWorker[] workerThreads;

		private static TaskDistributor mainTaskDistributor;

		public int MaxAdditionalWorkerThreads;

		private string name;

		private bool isDisposed;

		private ThreadPriority priority = ThreadPriority.BelowNormal;
	}
}
