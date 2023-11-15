using System;
using System.Collections;
using System.Threading;

namespace UnityThreading
{
	public sealed class TickThread : ThreadBase
	{
		public TickThread(Action action, int tickLengthInMilliseconds) : this(action, tickLengthInMilliseconds, true)
		{
		}

		public TickThread(Action action, int tickLengthInMilliseconds, bool autoStartThread) : base("TickThread", Dispatcher.CurrentNoThrow, false)
		{
			this.tickLengthInMilliseconds = tickLengthInMilliseconds;
			this.action = action;
			if (autoStartThread)
			{
				base.Start();
			}
		}

		protected override IEnumerator Do()
		{
			while (!this.exitEvent.InterWaitOne(0))
			{
				this.action();
				if (WaitHandle.WaitAny(new WaitHandle[]
				{
					this.exitEvent,
					this.tickEvent
				}, this.tickLengthInMilliseconds) == 0)
				{
					return null;
				}
			}
			return null;
		}

		private Action action;

		private int tickLengthInMilliseconds;

		private ManualResetEvent tickEvent = new ManualResetEvent(false);
	}
}
