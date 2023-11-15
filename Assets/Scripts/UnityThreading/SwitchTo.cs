using System;

namespace UnityThreading
{
	public class SwitchTo
	{
		private SwitchTo(SwitchTo.TargetType target)
		{
			this.Target = target;
		}

		public SwitchTo.TargetType Target { get; private set; }

		public static readonly SwitchTo MainThread = new SwitchTo(SwitchTo.TargetType.Main);

		public static readonly SwitchTo Thread = new SwitchTo(SwitchTo.TargetType.Thread);

		public enum TargetType
		{
			Main,
			Thread
		}
	}
}
