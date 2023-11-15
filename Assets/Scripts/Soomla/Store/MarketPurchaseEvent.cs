using System;
using System.Collections.Generic;

namespace Soomla.Store
{
	public class MarketPurchaseEvent : SoomlaEvent
	{
		public MarketPurchaseEvent(PurchasableVirtualItem purchasableVirtualItem, string payload, Dictionary<string, string> extraInfo) : this(purchasableVirtualItem, payload, extraInfo, null)
		{
		}

		public MarketPurchaseEvent(PurchasableVirtualItem purchasableVirtualItem, string payload, Dictionary<string, string> extraInfo, object sender) : base(sender)
		{
			this.PurchasableVirtualItem = purchasableVirtualItem;
			this.Payload = payload;
			this.ExtraInfo = extraInfo;
		}

		public readonly PurchasableVirtualItem PurchasableVirtualItem;

		public new readonly string Payload;

		public readonly Dictionary<string, string> ExtraInfo;
	}
}
