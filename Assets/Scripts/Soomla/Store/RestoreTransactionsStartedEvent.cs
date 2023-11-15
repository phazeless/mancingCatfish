using System;

namespace Soomla.Store
{
	public class RestoreTransactionsStartedEvent : SoomlaEvent
	{
		public RestoreTransactionsStartedEvent() : this(null)
		{
		}

		public RestoreTransactionsStartedEvent(object sender) : base(sender)
		{
		}
	}
}
