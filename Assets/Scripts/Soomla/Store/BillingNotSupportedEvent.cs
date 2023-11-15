using System;

namespace Soomla.Store
{
	public class BillingNotSupportedEvent : SoomlaEvent
	{
		public BillingNotSupportedEvent() : this(null)
		{
		}

		public BillingNotSupportedEvent(object sender) : base(sender)
		{
		}
	}
}
