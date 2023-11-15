using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace UnityThreading
{
	public abstract class DispatcherBase : IDisposable
	{
		public DispatcherBase()
		{
		}

		public bool IsWorking
		{
			get
			{
				return this.dataEvent.InterWaitOne(0);
			}
		}

		public virtual int TaskCount
		{
			get
			{
				object obj = this.taskListSyncRoot;
				int count;
				lock (obj)
				{
					count = this.taskList.Count;
				}
				return count;
			}
		}

		public void Lock()
		{
			object obj = this.taskListSyncRoot;
			lock (obj)
			{
				this.lockCount++;
			}
		}

		public void Unlock()
		{
			object obj = this.taskListSyncRoot;
			lock (obj)
			{
				this.lockCount--;
				if (this.lockCount == 0 && this.delayedTaskList.Count > 0)
				{
					while (this.delayedTaskList.Count > 0)
					{
						this.taskList.Enqueue(this.delayedTaskList.Dequeue());
					}
					if (this.TaskSortingSystem == TaskSortingSystem.ReorderWhenAdded || this.TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
					{
						this.ReorderTasks();
					}
					this.TasksAdded();
				}
			}
		}

		public Task<T> Dispatch<T>(Func<T> function)
		{
			this.CheckAccessLimitation();
			Task<T> task = new Task<T>(function);
			this.AddTask(task);
			return task;
		}

		public Task Dispatch(Action action)
		{
			this.CheckAccessLimitation();
			Task task = Task.Create(action);
			this.AddTask(task);
			return task;
		}

		public Task Dispatch(Task task)
		{
			this.CheckAccessLimitation();
			this.AddTask(task);
			return task;
		}

		internal virtual void AddTask(Task task)
		{
			object obj = this.taskListSyncRoot;
			lock (obj)
			{
				if (this.lockCount > 0)
				{
					this.delayedTaskList.Enqueue(task);
					return;
				}
				this.taskList.Enqueue(task);
				if (this.TaskSortingSystem == TaskSortingSystem.ReorderWhenAdded || this.TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
				{
					this.ReorderTasks();
				}
			}
			this.TasksAdded();
		}

		internal void AddTasks(IEnumerable<Task> tasks)
		{
			object obj = this.taskListSyncRoot;
			lock (obj)
			{
				if (this.lockCount > 0)
				{
					foreach (Task item in tasks)
					{
						this.delayedTaskList.Enqueue(item);
					}
					return;
				}
				foreach (Task item2 in tasks)
				{
					this.taskList.Enqueue(item2);
				}
				if (this.TaskSortingSystem == TaskSortingSystem.ReorderWhenAdded || this.TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
				{
					this.ReorderTasks();
				}
			}
			this.TasksAdded();
		}

		internal virtual void TasksAdded()
		{
			this.dataEvent.Set();
		}

		protected void ReorderTasks()
		{
			this.taskList = new Queue<Task>(from t in this.taskList
			orderby t.Priority
			select t);
		}

		internal IEnumerable<Task> SplitTasks(int divisor)
		{
			if (divisor == 0)
			{
				divisor = 2;
			}
			int count = this.TaskCount / divisor;
			return this.IsolateTasks(count);
		}

		internal IEnumerable<Task> IsolateTasks(int count)
		{
			Queue<Task> queue = new Queue<Task>();
			if (count == 0)
			{
				count = this.taskList.Count;
			}
			object obj = this.taskListSyncRoot;
			lock (obj)
			{
				int num = 0;
				while (num < count && num < this.taskList.Count)
				{
					queue.Enqueue(this.taskList.Dequeue());
					num++;
				}
				if (this.TaskCount == 0)
				{
					this.dataEvent.Reset();
				}
			}
			return queue;
		}

		protected abstract void CheckAccessLimitation();

		public virtual void Dispose()
		{
			for (;;)
			{
				object obj = this.taskListSyncRoot;
				Task task;
				lock (obj)
				{
					if (this.taskList.Count == 0)
					{
						break;
					}
					task = this.taskList.Dequeue();
				}
				task.Dispose();
			}
			this.dataEvent.Close();
			this.dataEvent = null;
		}

		protected int lockCount;

		protected object taskListSyncRoot = new object();

		protected Queue<Task> taskList = new Queue<Task>();

		protected Queue<Task> delayedTaskList = new Queue<Task>();

		protected ManualResetEvent dataEvent = new ManualResetEvent(false);

		public bool AllowAccessLimitationChecks;

		public TaskSortingSystem TaskSortingSystem;
	}
}
