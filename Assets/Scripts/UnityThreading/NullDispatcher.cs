using System;

namespace UnityThreading
{
	public class NullDispatcher : DispatcherBase
	{
		protected override void CheckAccessLimitation()
		{
		}

		internal override void AddTask(Task task)
		{
			task.DoInternal();
		}

		public static NullDispatcher Null = new NullDispatcher();
	}
}
