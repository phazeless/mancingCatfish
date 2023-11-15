using System;

namespace Soomla.Store
{
	public class SingleUsePackVG : VirtualGood
	{
		public SingleUsePackVG(string goodItemId, int amount, string name, string description, string itemId, PurchaseType purchaseType) : base(name, description, itemId, purchaseType)
		{
			this.GoodItemId = goodItemId;
			this.GoodAmount = amount;
		}

		public SingleUsePackVG(JSONObject jsonItem) : base(jsonItem)
		{
			this.GoodItemId = jsonItem["good_itemId"].str;
			this.GoodAmount = Convert.ToInt32(jsonItem["good_amount"].n);
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jsonobject = base.toJSONObject();
			jsonobject.AddField("good_itemId", this.GoodItemId);
			jsonobject.AddField("good_amount", this.GoodAmount);
			return jsonobject;
		}

		public override int Give(int amount, bool notify)
		{
			SingleUseVG item = null;
			try
			{
				item = (SingleUseVG)StoreInfo.GetItemByItemId(this.GoodItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(SingleUsePackVG.TAG, "SingleUseVG with itemId: " + this.GoodItemId + " doesn't exist! Can't give this pack.");
				return 0;
			}
			return VirtualGoodsStorage.Add(item, this.GoodAmount * amount, notify);
		}

		public override int Take(int amount, bool notify)
		{
			SingleUseVG item = null;
			try
			{
				item = (SingleUseVG)StoreInfo.GetItemByItemId(this.GoodItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(SingleUsePackVG.TAG, "SingleUseVG with itemId: " + this.GoodItemId + " doesn't exist! Can't give this pack.");
				return 0;
			}
			return VirtualGoodsStorage.Remove(item, this.GoodAmount * amount, notify);
		}

		public override int ResetBalance(int balance, bool notify)
		{
			SoomlaUtils.LogError(SingleUsePackVG.TAG, "Someone tried to reset balance of GoodPack. That's not right.");
			return 0;
		}

		public override int GetBalance()
		{
			SoomlaUtils.LogError(SingleUsePackVG.TAG, "Someone tried to check balance of GoodPack. That's not right.");
			return 0;
		}

		protected override bool canBuy()
		{
			return true;
		}

		private static string TAG = "SOOMLA SingleUsePackVG";

		public string GoodItemId;

		public int GoodAmount;
	}
}
