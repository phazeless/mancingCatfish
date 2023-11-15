using System;

namespace Soomla
{
	public class MarketItemsRefreshFailedEvent : SoomlaEvent
	{
		public MarketItemsRefreshFailedEvent(string errorMessage) : this(errorMessage, null)
		{
		}

		public MarketItemsRefreshFailedEvent(string errorMessage, object sender) : base(sender)
		{
			this.ErrorMessage = errorMessage;
		}

		public string ErrorMessage;
	}
}
