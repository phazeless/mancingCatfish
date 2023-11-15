using System;

namespace Soomla.Store
{
	public class MarketPurchaseStartedEvent : SoomlaEvent
	{
		public MarketPurchaseStartedEvent(PurchasableVirtualItem purchasableVirtualItem) : this(purchasableVirtualItem, false, null)
		{
		}

		public MarketPurchaseStartedEvent(PurchasableVirtualItem purchasableVirtualItem, bool fraudProtection) : this(purchasableVirtualItem, fraudProtection, null)
		{
		}

		public MarketPurchaseStartedEvent(PurchasableVirtualItem purchasableVirtualItem, object sender) : this(purchasableVirtualItem, false, sender)
		{
		}

		public MarketPurchaseStartedEvent(PurchasableVirtualItem purchasableVirtualItem, bool fraudProtection, object sender) : base(sender)
		{
			this.mPurchasableVirtualItem = purchasableVirtualItem;
			this.mFraudProtection = fraudProtection;
		}

		public PurchasableVirtualItem getPurchasableVirtualItem()
		{
			return this.mPurchasableVirtualItem;
		}

		public bool isFraudProtection()
		{
			return this.mFraudProtection;
		}

		private PurchasableVirtualItem mPurchasableVirtualItem;

		private bool mFraudProtection;
	}
}
