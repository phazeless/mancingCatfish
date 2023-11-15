using System;

namespace Soomla.Store
{
	public class PurchaseWithVirtualItem : PurchaseType
	{
		public PurchaseWithVirtualItem(string targetItemId, int amount)
		{
			this.TargetItemId = targetItemId;
			this.Amount = amount;
		}

		public override void Buy(string payload)
		{
			SoomlaUtils.LogDebug("SOOMLA PurchaseWithVirtualItem", string.Concat(new object[]
			{
				"Trying to buy a ",
				this.AssociatedItem.Name,
				" with ",
				this.Amount,
				" pieces of ",
				this.TargetItemId
			}));
			VirtualItem targetVirtualItem = this.getTargetVirtualItem();
			if (targetVirtualItem == null)
			{
				return;
			}
			JSONObject eventJSON = new JSONObject();
			eventJSON.AddField("itemId", this.AssociatedItem.ItemId);
			StoreEvents.Instance.onItemPurchaseStarted(eventJSON.print(false), true);
			if (!this.checkTargetBalance(targetVirtualItem))
			{
				throw new InsufficientFundsException(this.TargetItemId);
			}
			targetVirtualItem.Take(this.Amount);
			this.AssociatedItem.Give(1);
			StoreEvents.Instance.RunLater(delegate
			{
				eventJSON = new JSONObject();
				eventJSON.AddField("itemId", this.AssociatedItem.ItemId);
				eventJSON.AddField("payload", payload);
				StoreEvents.Instance.onItemPurchased(eventJSON.print(false), true);
			});
		}

		public override bool CanAfford()
		{
			SoomlaUtils.LogDebug("SOOMLA PurchaseWithVirtualItem", string.Concat(new object[]
			{
				"Checking affordability of ",
				this.AssociatedItem.Name,
				" with ",
				this.Amount,
				" pieces of ",
				this.TargetItemId
			}));
			VirtualItem targetVirtualItem = this.getTargetVirtualItem();
			return this.checkTargetBalance(targetVirtualItem);
		}

		public override string GetPrice()
		{
			return this.Amount.ToString();
		}

		private VirtualItem getTargetVirtualItem()
		{
			VirtualItem result = null;
			try
			{
				result = StoreInfo.GetItemByItemId(this.TargetItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError("SOOMLA PurchaseWithVirtualItem", "Target virtual item doesn't exist !");
			}
			return result;
		}

		private bool checkTargetBalance(VirtualItem item)
		{
			int itemBalance = StoreInventory.GetItemBalance(item.ItemId);
			return itemBalance >= this.Amount;
		}

		private const string TAG = "SOOMLA PurchaseWithVirtualItem";

		public string TargetItemId;

		public int Amount;
	}
}
