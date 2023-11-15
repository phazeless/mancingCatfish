using System;

namespace Soomla.Store
{
	public class PurchaseWithMarket : PurchaseType
	{
		public PurchaseWithMarket(string productId, double price)
		{
			this.MarketItem = new MarketItem(productId, price);
		}

		public PurchaseWithMarket(MarketItem marketItem)
		{
			this.MarketItem = marketItem;
		}

		public override void Buy(string payload)
		{
			SoomlaUtils.LogDebug("SOOMLA PurchaseWithMarket", "Starting in-app purchase for productId: " + this.MarketItem.ProductId);
			JSONObject jsonobject = new JSONObject();
			jsonobject.AddField("itemId", this.AssociatedItem.ItemId);
			StoreEvents.Instance.onItemPurchaseStarted(jsonobject.print(false), true);
			SoomlaStore.BuyMarketItem(this.MarketItem.ProductId, payload);
		}

		public override bool CanAfford()
		{
			return true;
		}

		public override string GetPrice()
		{
			return this.MarketItem.Price.ToString();
		}

		private const string TAG = "SOOMLA PurchaseWithMarket";

		public MarketItem MarketItem;
	}
}
