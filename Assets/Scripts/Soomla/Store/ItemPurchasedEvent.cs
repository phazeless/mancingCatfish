using System;

namespace Soomla.Store
{
	public class ItemPurchasedEvent : SoomlaEvent
	{
		public ItemPurchasedEvent(PurchasableVirtualItem item, string payload) : this(item, payload, null)
		{
		}

		public ItemPurchasedEvent(PurchasableVirtualItem item, string payload, object sender) : base(sender)
		{
			this.mItem = item;
			this.mPayload = payload;
		}

		public PurchasableVirtualItem getItem()
		{
			return this.mItem;
		}

		public string getPayload()
		{
			return this.mPayload;
		}

		private PurchasableVirtualItem mItem;

		private string mPayload;
	}
}
