using System;

namespace Soomla.Store
{
	public class MarketPurchaseCancelledEvent : SoomlaEvent
	{
		public MarketPurchaseCancelledEvent(PurchasableVirtualItem purchasableVirtualItem) : this(purchasableVirtualItem, null)
		{
		}

		public MarketPurchaseCancelledEvent(PurchasableVirtualItem purchasableVirtualItem, object sender) : base(sender)
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
