using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace UnityThreading
{
	public abstract class ThreadBase : IDisposable
	{
		public ThreadBase(string threadName) : this(threadName, true)
		{
		}

		public ThreadBase(string threadName, bool autoStartThread) : this(threadName, Dispatcher.CurrentNoThrow, autoStartThread)
		{
		}

		public ThreadBase(string threadName, Dispatcher targetDispatcher) : this(threadName, targetDispatcher, true)
		{
		}

		public ThreadBase(string threadName, Dispatcher targetDispatcher, bool autoStartThread)
		{
			this.threadName = threadName;
			this.targetDispatcher = targetDispatcher;
			if (autoStartThread)
			{
				this.Start();
			}
		}

		public static int AvailableProcessors
		{
			get
			{
				return SystemInfo.processorCount;
			}
		}

		public static ThreadBase CurrentThread
		{
			get
			{
				return ThreadBase.currentThread;
			}
		}

		public bool IsAlive
		{
			get
			{
				return this.thread != null && this.thread.IsAlive;
			}
		}

		public bool ShouldStop
		{
			get
			{
				return this.exitEvent.InterWaitOne(0);
			}
		}

		public void Start()
		{
			if (this.thread != null)
			{
				this.Abort();
			}
			this.exitEvent.Reset();
			this.thread = new Thread(new ThreadStart(this.DoInternal));
			this.thread.Name = this.threadName;
			this.thread.Priority = this.priority;
			this.thread.Start();
		}

		public void Exit()
		{
			if (this.thread != null)
			{
				this.exitEvent.Set();
			}
		}

		public void Abort()
		{
			this.Exit();
			if (this.thread != null)
			{
				this.thread.Join();
			}
		}

		public void AbortWaitForSeconds(float seconds)
		{
			this.Exit();
			if (this.thread != null)
			{
				this.thread.Join((int)(seconds * 1000f));
				if (this.thread.IsAlive)
				{
					this.thread.Abort();
				}
			}
		}

		public Task<T> Dispatch<T>(Func<T> function)
		{
			return this.targetDispatcher.Dispatch<T>(function);
		}

		public T DispatchAndWait<T>(Func<T> function)
		{
			Task<T> task = this.Dispatch<T>(function);
			task.Wait();
			return task.Result;
		}

		public T DispatchAndWait<T>(Func<T> function, float timeOutSeconds)
		{
			Task<T> task = this.Dispatch<T>(function);
			task.WaitForSeconds(timeOutSeconds);
			return task.Result;
		}

		public Task Dispatch(Action action)
		{
			return this.targetDispatcher.Dispatch(action);
		}

		public void DispatchAndWait(Action action)
		{
			Task task = this.Dispatch(action);
			task.Wait();
		}

		public void DispatchAndWait(Action action, float timeOutSeconds)
		{
			Task task = this.Dispatch(action);
			task.WaitForSeconds(timeOutSeconds);
		}

		public Task Dispatch(Task taskBase)
		{
			return this.targetDispatcher.Dispatch(taskBase);
		}

		public void DispatchAndWait(Task taskBase)
		{
			Task task = this.Dispatch(taskBase);
			task.Wait();
		}

		public void DispatchAndWait(Task taskBase, float timeOutSeconds)
		{
			Task task = this.Dispatch(taskBase);
			task.WaitForSeconds(timeOutSeconds);
		}

		protected void DoInternal()
		{
			ThreadBase.currentThread = this;
			IEnumerator enumerator = this.Do();
			if (enumerator == null)
			{
				return;
			}
			this.RunEnumerator(enumerator);
		}

		private void RunEnumerator(IEnumerator enumerator)
		{
			for (;;)
			{
				if (enumerator.Current is Task)
				{
					Task taskBase = (Task)enumerator.Current;
					this.DispatchAndWait(taskBase);
				}
				else if (enumerator.Current is SwitchTo)
				{
					SwitchTo switchTo = (SwitchTo)enumerator.Current;
					if (switchTo.Target == SwitchTo.TargetType.Main && ThreadBase.CurrentThread != null)
					{
						Task taskBase2 = Task.Create(delegate()
						{
							if (enumerator.MoveNext() && !this.ShouldStop)
							{
								this.RunEnumerator(enumerator);
							}
						});
						this.DispatchAndWait(taskBase2);
					}
					else if (switchTo.Target == SwitchTo.TargetType.Thread && ThreadBase.CurrentThread == null)
					{
						break;
					}
				}
				if (!enumerator.MoveNext() || this.ShouldStop)
				{
					return;
				}
			}
		}

		protected abstract IEnumerator Do();

		public virtual void Dispose()
		{
			this.AbortWaitForSeconds(1f);
		}

		public System.Threading.ThreadPriority Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
				if (this.thread != null)
				{
					this.thread.Priority = this.priority;
				}
			}
		}

		protected Dispatcher targetDispatcher;

		protected Thread thread;

		protected ManualResetEvent exitEvent = new ManualResetEvent(false);

		[ThreadStatic]
		private static ThreadBase currentThread;

		private string threadName;

		private System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.BelowNormal;
	}
}
