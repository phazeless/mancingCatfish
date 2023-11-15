using System;

namespace Soomla.Store
{
	public class MarketRefundEvent : SoomlaEvent
	{
		public MarketRefundEvent(PurchasableVirtualItem purchasableVirtualItem) : this(purchasableVirtualItem, null)
		{
		}

		public MarketRefundEvent(PurchasableVirtualItem purchasableVirtualItem, object sender) : base(sender)
		{
			this.mPurchasableVirtualItem = purchasableVirtualItem;
		}

		public PurchasableVirtualItem getPurchasableVirtualItem()
		{
			return this.mPurchasableVirtualItem;
		}

		private PurchasableVirtualItem mPurchasableVirtualItem;
	}
}
