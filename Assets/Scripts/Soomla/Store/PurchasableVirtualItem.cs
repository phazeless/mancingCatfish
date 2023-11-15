using System;

namespace Soomla.Store
{
	public abstract class PurchasableVirtualItem : VirtualItem
	{
		protected PurchasableVirtualItem(string name, string description, string itemId, PurchaseType purchaseType) : base(name, description, itemId)
		{
			this.PurchaseType = purchaseType;
			if (this.PurchaseType != null)
			{
				this.PurchaseType.AssociatedItem = this;
			}
		}

		protected PurchasableVirtualItem(JSONObject jsonItem) : base(jsonItem)
		{
			JSONObject jsonobject = jsonItem["purchasableItem"];
			string str = jsonobject["purchaseType"].str;
			if (str == "market")
			{
				JSONObject jsonObject = jsonobject["marketItem"];
				this.PurchaseType = new PurchaseWithMarket(new MarketItem(jsonObject));
			}
			else if (str == "virtualItem")
			{
				string str2 = jsonobject["pvi_itemId"].str;
				int amount = Convert.ToInt32(jsonobject["pvi_amount"].n);
				this.PurchaseType = new PurchaseWithVirtualItem(str2, amount);
			}
			else
			{
				SoomlaUtils.LogError("SOOMLA PurchasableVirtualItem", "Couldn't determine what type of class is the given purchaseType.");
			}
			if (this.PurchaseType != null)
			{
				this.PurchaseType.AssociatedItem = this;
			}
		}

		public bool CanAfford()
		{
			return this.PurchaseType.CanAfford();
		}

		public void Buy(string payload)
		{
			if (!this.canBuy())
			{
				return;
			}
			this.PurchaseType.Buy(payload);
		}

		protected abstract bool canBuy();

		public override JSONObject toJSONObject()
		{
			JSONObject jsonobject = base.toJSONObject();
			try
			{
				JSONObject jsonobject2 = new JSONObject(JSONObject.Type.OBJECT);
				if (this.PurchaseType is PurchaseWithMarket)
				{
					jsonobject2.AddField("purchaseType", "market");
					MarketItem marketItem = ((PurchaseWithMarket)this.PurchaseType).MarketItem;
					jsonobject2.AddField("marketItem", marketItem.toJSONObject());
				}
				else if (this.PurchaseType is PurchaseWithVirtualItem)
				{
					jsonobject2.AddField("purchaseType", "virtualItem");
					jsonobject2.AddField("pvi_itemId", ((PurchaseWithVirtualItem)this.PurchaseType).TargetItemId);
					jsonobject2.AddField("pvi_amount", ((PurchaseWithVirtualItem)this.PurchaseType).Amount);
				}
				jsonobject.AddField("purchasableItem", jsonobject2);
			}
			catch (Exception ex)
			{
				SoomlaUtils.LogError("SOOMLA PurchasableVirtualItem", "An error occurred while generating JSON object. " + ex.Message);
			}
			return jsonobject;
		}

		private const string TAG = "SOOMLA PurchasableVirtualItem";

		public PurchaseType PurchaseType;
	}
}
