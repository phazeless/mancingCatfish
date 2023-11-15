using System;
using System.Collections;

namespace UnityThreading
{
	public sealed class EnumeratableActionThread : ThreadBase
	{
		public EnumeratableActionThread(Func<ThreadBase, IEnumerator> enumeratableAction) : this(enumeratableAction, true)
		{
		}

		public EnumeratableActionThread(Func<ThreadBase, IEnumerator> enumeratableAction, bool autoStartThread) : base("EnumeratableActionThread", Dispatcher.Current, false)
		{
			this.enumeratableAction = enumeratableAction;
			if (autoStartThread)
			{
				base.Start();
			}
		}

		protected override IEnumerator Do()
		{
			return this.enumeratableAction(this);
		}

		private Func<ThreadBase, IEnumerator> enumeratableAction;
	}
}
