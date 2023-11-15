using System;

namespace Soomla.Store
{
	public class IabServiceStartedEvent : SoomlaEvent
	{
		public IabServiceStartedEvent() : this(null)
		{
		}

		public IabServiceStartedEvent(object sender) : base(sender)
		{
		}
	}
}
