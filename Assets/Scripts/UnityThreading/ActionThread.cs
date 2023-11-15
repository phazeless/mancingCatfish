using System;
using System.Collections;

namespace UnityThreading
{
	public sealed class ActionThread : ThreadBase
	{
		public ActionThread(Action<ActionThread> action) : this(action, true)
		{
		}

		public ActionThread(Action<ActionThread> action, bool autoStartThread) : base("ActionThread", Dispatcher.Current, false)
		{
			this.action = action;
			if (autoStartThread)
			{
				base.Start();
			}
		}

		protected override IEnumerator Do()
		{
			this.action(this);
			return null;
		}

		private Action<ActionThread> action;
	}
}
