using System;

namespace Soomla
{
	public class SoomlaEvent
	{
		public SoomlaEvent()
		{
		}

		public SoomlaEvent(object sender) : this(sender, string.Empty)
		{
		}

		public SoomlaEvent(object sender, string payload)
		{
			this.Sender = sender;
			this.Payload = payload;
		}

		public readonly object Sender;

		public readonly string Payload;
	}
}
