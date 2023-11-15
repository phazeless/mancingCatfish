using System;

namespace Soomla.Store
{
	public class MarketItemsRefreshStartedEvent : SoomlaEvent
	{
		public MarketItemsRefreshStartedEvent() : this(null)
		{
		}

		public MarketItemsRefreshStartedEvent(object sender) : base(sender)
		{
		}
	}
}
