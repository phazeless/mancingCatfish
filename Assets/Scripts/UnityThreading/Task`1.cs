using System;
using System.Collections;
using System.Reflection;

namespace UnityThreading
{
	public class Task<T> : Task
	{
		public Task(Func<Task, T> function)
		{
			this.function = function;
		}

		public Task(Func<T> function)
		{
			this.function = ((Task t) => function());
		}

		public Task(Action<Task> action)
		{
			this.function = delegate(Task t)
			{
				action(t);
				return default(T);
			};
		}

		public Task(Action action)
		{
			this.function = delegate(Task t)
			{
				action();
				return default(T);
			};
		}

		public Task(IEnumerator enumerator)
		{
			this.function = ((Task t) => (T)((object)enumerator));
		}

		public Task(Type type, string methodName, params object[] args)
		{
			MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
			if (methodInfo == null)
			{
				throw new ArgumentException("methodName", "Fitting method with the given name was not found.");
			}
			this.function = ((Task t) => (T)((object)methodInfo.Invoke(null, args)));
		}

		public Task(object that, string methodName, params object[] args)
		{
			MethodInfo methodInfo = that.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
			if (methodInfo == null)
			{
				throw new ArgumentException("methodName", "Fitting method with the given name was not found.");
			}
			this.function = ((Task t) => (T)((object)methodInfo.Invoke(that, args)));
		}

		protected override IEnumerator Do()
		{
			this.result = this.function(this);
			if (this.result is IEnumerator)
			{
				return (IEnumerator)((object)this.result);
			}
			return null;
		}

		public override TResult Wait<TResult>()
		{
			this.Priority--;
			return (TResult)((object)this.Result);
		}

		public override TResult WaitForSeconds<TResult>(float seconds)
		{
			this.Priority--;
			return this.WaitForSeconds<TResult>(seconds, default(TResult));
		}

		public override TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue)
		{
			if (!base.HasEnded)
			{
				base.WaitForSeconds(seconds);
			}
			if (base.IsSucceeded)
			{
				return (TResult)((object)this.result);
			}
			return defaultReturnValue;
		}

		public override object RawResult
		{
			get
			{
				if (!base.IsEnding)
				{
					base.Wait();
				}
				return this.result;
			}
		}

		public T Result
		{
			get
			{
				if (!base.IsEnding)
				{
					base.Wait();
				}
				return this.result;
			}
		}

		public new Task<T> Run(DispatcherBase target)
		{
			base.Run(target);
			return this;
		}

		public new Task<T> Run()
		{
			base.Run();
			return this;
		}

		private Func<Task, T> function;

		private T result;
	}
}
