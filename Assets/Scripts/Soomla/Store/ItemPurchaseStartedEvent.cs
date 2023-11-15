using System;

namespace Soomla.Store
{
	public class ItemPurchaseStartedEvent : SoomlaEvent
	{
		public ItemPurchaseStartedEvent(PurchasableVirtualItem item) : this(item, null)
		{
		}

		public ItemPurchaseStartedEvent(PurchasableVirtualItem item, object sender) : base(sender)
		{
			this.mItem = item;
		}

		public PurchasableVirtualItem getItem()
		{
			return this.mItem;
		}

		private PurchasableVirtualItem mItem;
	}
}
