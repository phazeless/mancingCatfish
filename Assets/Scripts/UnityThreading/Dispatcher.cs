using System;
using System.Collections.Generic;
using System.Threading;

namespace UnityThreading
{
	public class Dispatcher : DispatcherBase
	{
		public Dispatcher() : this(true)
		{
		}

		public Dispatcher(bool setThreadDefaults)
		{
			if (!setThreadDefaults)
			{
				return;
			}
			if (Dispatcher.currentDispatcher != null)
			{
				throw new InvalidOperationException("Only one Dispatcher instance allowed per thread.");
			}
			Dispatcher.currentDispatcher = this;
			if (Dispatcher.mainDispatcher == null)
			{
				Dispatcher.mainDispatcher = this;
			}
		}

		public static Task CurrentTask
		{
			get
			{
				if (Dispatcher.currentTask == null)
				{
					throw new InvalidOperationException("No task is currently running.");
				}
				return Dispatcher.currentTask;
			}
		}

		public static Dispatcher Current
		{
			get
			{
				if (Dispatcher.currentDispatcher == null)
				{
					throw new InvalidOperationException("No Dispatcher found for the current thread, please create a new Dispatcher instance before calling this property.");
				}
				return Dispatcher.currentDispatcher;
			}
			set
			{
				if (Dispatcher.currentDispatcher != null)
				{
					Dispatcher.currentDispatcher.Dispose();
				}
				Dispatcher.currentDispatcher = value;
			}
		}

		public static Dispatcher CurrentNoThrow
		{
			get
			{
				return Dispatcher.currentDispatcher;
			}
		}

		public static Dispatcher Main
		{
			get
			{
				if (Dispatcher.mainDispatcher == null)
				{
					throw new InvalidOperationException("No Dispatcher found for the main thread, please create a new Dispatcher instance before calling this property.");
				}
				return Dispatcher.mainDispatcher;
			}
		}

		public static Dispatcher MainNoThrow
		{
			get
			{
				return Dispatcher.mainDispatcher;
			}
		}

		public static Func<T> CreateSafeFunction<T>(Func<T> function)
		{
			return delegate()
			{
				T result;
				try
				{
					result = function();
				}
				catch
				{
					Dispatcher.CurrentTask.Abort();
					result = default(T);
				}
				return result;
			};
		}

		public static Action CreateSafeAction<T>(Action action)
		{
			return delegate()
			{
				try
				{
					action();
				}
				catch
				{
					Dispatcher.CurrentTask.Abort();
				}
			};
		}

		public void ProcessTasks()
		{
			if (this.dataEvent.InterWaitOne(0))
			{
				this.ProcessTasksInternal();
			}
		}

		public bool ProcessTasks(WaitHandle exitHandle)
		{
			if (WaitHandle.WaitAny(new WaitHandle[]
			{
				exitHandle,
				this.dataEvent
			}) == 0)
			{
				return false;
			}
			this.ProcessTasksInternal();
			return true;
		}

		public bool ProcessNextTask()
		{
			object taskListSyncRoot = this.taskListSyncRoot;
			Task task;
			lock (taskListSyncRoot)
			{
				if (this.taskList.Count == 0)
				{
					return false;
				}
				task = this.taskList.Dequeue();
			}
			this.ProcessSingleTask(task);
			if (this.TaskCount == 0)
			{
				this.dataEvent.Reset();
			}
			return true;
		}

		public bool ProcessNextTask(WaitHandle exitHandle)
		{
			if (WaitHandle.WaitAny(new WaitHandle[]
			{
				exitHandle,
				this.dataEvent
			}) == 0)
			{
				return false;
			}
			object taskListSyncRoot = this.taskListSyncRoot;
			Task task;
			lock (taskListSyncRoot)
			{
				if (this.taskList.Count == 0)
				{
					return false;
				}
				task = this.taskList.Dequeue();
			}
			this.ProcessSingleTask(task);
			if (this.TaskCount == 0)
			{
				this.dataEvent.Reset();
			}
			return true;
		}

		private void ProcessTasksInternal()
		{
			object taskListSyncRoot = this.taskListSyncRoot;
			List<Task> list;
			lock (taskListSyncRoot)
			{
				list = new List<Task>(this.taskList);
				this.taskList.Clear();
			}
			while (list.Count != 0)
			{
				Task task = list[0];
				list.RemoveAt(0);
				this.ProcessSingleTask(task);
			}
			if (this.TaskCount == 0)
			{
				this.dataEvent.Reset();
			}
		}

		private void ProcessSingleTask(Task task)
		{
			this.RunTask(task);
			if (this.TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
			{
				object taskListSyncRoot = this.taskListSyncRoot;
				lock (taskListSyncRoot)
				{
					base.ReorderTasks();
				}
			}
		}

		internal void RunTask(Task task)
		{
			Task task2 = Dispatcher.currentTask;
			Dispatcher.currentTask = task;
			Dispatcher.currentTask.DoInternal();
			Dispatcher.currentTask = task2;
		}

		protected override void CheckAccessLimitation()
		{
			if (this.AllowAccessLimitationChecks && Dispatcher.currentDispatcher == this)
			{
				throw new InvalidOperationException("Dispatching a Task with the Dispatcher associated to the current thread is prohibited. You can run these Tasks without the need of a Dispatcher.");
			}
		}

		public override void Dispose()
		{
			for (;;)
			{
				object taskListSyncRoot = this.taskListSyncRoot;
				lock (taskListSyncRoot)
				{
					if (this.taskList.Count == 0)
					{
						break;
					}
					Dispatcher.currentTask = this.taskList.Dequeue();
				}
				Dispatcher.currentTask.Dispose();
			}
			this.dataEvent.Close();
			this.dataEvent = null;
			if (Dispatcher.currentDispatcher == this)
			{
				Dispatcher.currentDispatcher = null;
			}
			if (Dispatcher.mainDispatcher == this)
			{
				Dispatcher.mainDispatcher = null;
			}
		}

		[ThreadStatic]
		private static Task currentTask;

		[ThreadStatic]
		internal static Dispatcher currentDispatcher;

		protected static Dispatcher mainDispatcher;
	}
}
