using System;

namespace Soomla.Store
{
	public class IabServiceStoppedEvent : SoomlaEvent
	{
		public IabServiceStoppedEvent() : this(null)
		{
		}

		public IabServiceStoppedEvent(object sender) : base(sender)
		{
		}
	}
}
