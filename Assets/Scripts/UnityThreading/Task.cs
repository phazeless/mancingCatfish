using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace UnityThreading
{
	public abstract class Task
	{
		public Task()
		{
		}

		~Task()
		{
			this.Dispose();
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event TaskEndedEventHandler taskEnded;

		public event TaskEndedEventHandler TaskEnded;

		private void End()
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				this.endingEvent.Set();
				if (this.taskEnded != null)
				{
					this.taskEnded(this);
				}
				this.endedEvent.Set();
				if (Task.current == this)
				{
					Task.current = null;
				}
				this.hasEnded = true;
			}
		}

		protected abstract IEnumerator Do();

		public static Task Current
		{
			get
			{
				return Task.current;
			}
		}

		public bool ShouldAbort
		{
			get
			{
				return this.abortEvent.InterWaitOne(0);
			}
		}

		public bool HasEnded
		{
			get
			{
				return this.hasEnded || this.endedEvent.InterWaitOne(0);
			}
		}

		public bool IsEnding
		{
			get
			{
				return this.endingEvent.InterWaitOne(0);
			}
		}

		public bool IsSucceeded
		{
			get
			{
				return this.endingEvent.InterWaitOne(0) && !this.abortEvent.InterWaitOne(0);
			}
		}

		public bool IsFailed
		{
			get
			{
				return this.endingEvent.InterWaitOne(0) && this.abortEvent.InterWaitOne(0);
			}
		}

		public void Abort()
		{
			this.abortEvent.Set();
			if (!this.hasStarted)
			{
				this.End();
			}
		}

		public void AbortWait()
		{
			this.Abort();
			if (!this.hasStarted)
			{
				return;
			}
			this.Wait();
		}

		public void AbortWaitForSeconds(float seconds)
		{
			this.Abort();
			if (!this.hasStarted)
			{
				return;
			}
			this.WaitForSeconds(seconds);
		}

		public void Wait()
		{
			if (this.hasEnded)
			{
				return;
			}
			this.Priority--;
			this.endedEvent.WaitOne();
		}

		public void WaitForSeconds(float seconds)
		{
			if (this.hasEnded)
			{
				return;
			}
			this.Priority--;
			this.endedEvent.InterWaitOne(TimeSpan.FromSeconds((double)seconds));
		}

		public abstract TResult Wait<TResult>();

		public abstract TResult WaitForSeconds<TResult>(float seconds);

		public abstract object RawResult { get; }

		public abstract TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue);

		internal void DoInternal()
		{
			Task.current = this;
			this.hasStarted = true;
			if (!this.ShouldAbort)
			{
				try
				{
					IEnumerator enumerator = this.Do();
					if (enumerator == null)
					{
						this.End();
						return;
					}
					this.RunEnumerator(enumerator);
				}
				catch (Exception ex)
				{
					this.Abort();
					if (string.IsNullOrEmpty(this.Name))
					{
						UnityEngine.Debug.LogError("Error while processing task:\n" + ex.ToString());
					}
					else
					{
						UnityEngine.Debug.LogError("Error while processing task '" + this.Name + "':\n" + ex.ToString());
					}
				}
			}
			this.End();
		}

		private void RunEnumerator(IEnumerator enumerator)
		{
			ThreadBase currentThread = ThreadBase.CurrentThread;
			for (;;)
			{
				if (enumerator.Current is Task)
				{
					Task taskBase = (Task)enumerator.Current;
					currentThread.DispatchAndWait(taskBase);
				}
				else if (enumerator.Current is SwitchTo)
				{
					SwitchTo switchTo = (SwitchTo)enumerator.Current;
					if (switchTo.Target == SwitchTo.TargetType.Main && currentThread != null)
					{
						Task taskBase2 = Task.Create(delegate()
						{
							if (enumerator.MoveNext() && !this.ShouldAbort)
							{
								this.RunEnumerator(enumerator);
							}
						});
						currentThread.DispatchAndWait(taskBase2);
					}
					else if (switchTo.Target == SwitchTo.TargetType.Thread && currentThread == null)
					{
						break;
					}
				}
				if (!enumerator.MoveNext() || this.ShouldAbort)
				{
					return;
				}
			}
		}

		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (this.hasStarted)
			{
				this.Wait();
			}
			this.endingEvent.Close();
			this.endedEvent.Close();
			this.abortEvent.Close();
		}

		public Task Run(DispatcherBase target)
		{
			if (target == null)
			{
				return this.Run();
			}
			target.Dispatch(this);
			return this;
		}

		public Task Run()
		{
			this.Run(UnityThreadHelper.TaskDistributor);
			return this;
		}

		public static Task Create(Action<Task> action)
		{
			return new Task<Task.Unit>(action);
		}

		public static Task Create(Action action)
		{
			return new Task<Task.Unit>(action);
		}

		public static Task<T> Create<T>(Func<Task, T> func)
		{
			return new Task<T>(func);
		}

		public static Task<T> Create<T>(Func<T> func)
		{
			return new Task<T>(func);
		}

		public static Task Create(IEnumerator enumerator)
		{
			return new Task<IEnumerator>(enumerator);
		}

		public static Task<T> Create<T>(Type type, string methodName, params object[] args)
		{
			return new Task<T>(type, methodName, args);
		}

		public static Task<T> Create<T>(object that, string methodName, params object[] args)
		{
			return new Task<T>(that, methodName, args);
		}

		private object syncRoot = new object();

		private bool hasEnded;

		public string Name;

		public volatile int Priority;

		private ManualResetEvent abortEvent = new ManualResetEvent(false);

		private ManualResetEvent endedEvent = new ManualResetEvent(false);

		private ManualResetEvent endingEvent = new ManualResetEvent(false);

		private bool hasStarted;

		[ThreadStatic]
		private static Task current;

		private bool disposed;

		public struct Unit
		{
		}
	}
}
