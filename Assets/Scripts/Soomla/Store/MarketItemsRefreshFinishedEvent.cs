using System;
using System.Collections.Generic;

namespace Soomla.Store
{
	public class MarketItemsRefreshFinishedEvent : SoomlaEvent
	{
		public MarketItemsRefreshFinishedEvent(List<MarketItem> marketItems) : this(marketItems, null)
		{
		}

		public MarketItemsRefreshFinishedEvent(List<MarketItem> marketItems, object sender) : base(sender)
		{
			this.mMarketItems = marketItems;
		}

		public List<MarketItem> getMarketItems()
		{
			return this.mMarketItems;
		}

		private List<MarketItem> mMarketItems;
	}
}
