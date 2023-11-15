using System;

namespace Soomla.Store
{
	public class UpgradeVG : LifetimeVG
	{
		public UpgradeVG(string goodItemId, string nextItemId, string prevItemId, string name, string description, string itemId, PurchaseType purchaseType) : base(name, description, itemId, purchaseType)
		{
			this.GoodItemId = goodItemId;
			this.PrevItemId = prevItemId;
			this.NextItemId = nextItemId;
		}

		public UpgradeVG(JSONObject jsonItem) : base(jsonItem)
		{
			this.GoodItemId = jsonItem["good_itemId"].str;
			this.PrevItemId = jsonItem["prev_itemId"].str;
			this.NextItemId = jsonItem["next_itemId"].str;
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jsonobject = base.toJSONObject();
			jsonobject.AddField("good_itemId", this.GoodItemId);
			jsonobject.AddField("prev_itemId", (!string.IsNullOrEmpty(this.PrevItemId)) ? this.PrevItemId : string.Empty);
			jsonobject.AddField("next_itemId", (!string.IsNullOrEmpty(this.NextItemId)) ? this.NextItemId : string.Empty);
			return jsonobject;
		}

		protected override bool canBuy()
		{
			VirtualGood good = null;
			try
			{
				good = (VirtualGood)StoreInfo.GetItemByItemId(this.GoodItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(UpgradeVG.TAG, "VirtualGood with itemId: " + this.GoodItemId + " doesn't exist! Returning NO (can't buy).");
				return false;
			}
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(good);
			return ((currentUpgrade == null && string.IsNullOrEmpty(this.PrevItemId)) || (currentUpgrade != null && (currentUpgrade.NextItemId == base.ItemId || currentUpgrade.PrevItemId == base.ItemId))) && base.canBuy();
		}

		public override int Give(int amount, bool notify)
		{
			SoomlaUtils.LogDebug(UpgradeVG.TAG, "Assigning " + this.Name + " to: " + this.GoodItemId);
			VirtualGood good = null;
			try
			{
				good = (VirtualGood)StoreInfo.GetItemByItemId(this.GoodItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(UpgradeVG.TAG, "VirtualGood with itemId: " + this.GoodItemId + " doesn't exist! Can't upgrade.");
				return 0;
			}
			VirtualGoodsStorage.AssignCurrentUpgrade(good, this, notify);
			return base.Give(amount, notify);
		}

		public override int Take(int amount, bool notify)
		{
			VirtualGood virtualGood = null;
			try
			{
				virtualGood = (VirtualGood)StoreInfo.GetItemByItemId(this.GoodItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(UpgradeVG.TAG, "VirtualGood with itemId: " + this.GoodItemId + " doesn't exist! Can't downgrade.");
				return 0;
			}
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(virtualGood);
			if (currentUpgrade != this)
			{
				SoomlaUtils.LogError(UpgradeVG.TAG, "You can't take an upgrade that's not currently assigned.The UpgradeVG " + this.Name + " is not assigned to the VirtualGood: " + virtualGood.Name);
				return 0;
			}
			if (!string.IsNullOrEmpty(this.PrevItemId))
			{
				UpgradeVG upgradeVG = null;
				try
				{
					upgradeVG = (UpgradeVG)StoreInfo.GetItemByItemId(this.PrevItemId);
				}
				catch (VirtualItemNotFoundException)
				{
					SoomlaUtils.LogError(UpgradeVG.TAG, "Previous UpgradeVG with itemId: " + this.PrevItemId + " doesn't exist! Can't downgrade.");
					return 0;
				}
				SoomlaUtils.LogDebug(UpgradeVG.TAG, "Downgrading " + virtualGood.Name + " to: " + upgradeVG.Name);
				VirtualGoodsStorage.AssignCurrentUpgrade(virtualGood, upgradeVG, notify);
			}
			else
			{
				SoomlaUtils.LogDebug(UpgradeVG.TAG, "Downgrading " + virtualGood.Name + " to NO-UPGRADE");
				VirtualGoodsStorage.RemoveUpgrades(virtualGood, notify);
			}
			return base.Take(amount, notify);
		}

		private static string TAG = "SOOMLA UpgradeVG";

		public string GoodItemId;

		public string NextItemId;

		public string PrevItemId;
	}
}
