using System;

namespace Soomla.Store
{
	public class VirtualCurrencyPack : PurchasableVirtualItem
	{
		public VirtualCurrencyPack(string name, string description, string itemId, int currencyAmount, string currencyItemId, PurchaseType purchaseType) : base(name, description, itemId, purchaseType)
		{
			this.CurrencyAmount = currencyAmount;
			this.CurrencyItemId = currencyItemId;
		}

		public VirtualCurrencyPack(JSONObject jsonItem) : base(jsonItem)
		{
			this.CurrencyAmount = Convert.ToInt32(jsonItem["currency_amount"].n);
			this.CurrencyItemId = jsonItem["currency_itemId"].str;
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jsonobject = base.toJSONObject();
			jsonobject.AddField("currency_amount", this.CurrencyAmount);
			jsonobject.AddField("currency_itemId", this.CurrencyItemId);
			return jsonobject;
		}

		public override int Give(int amount, bool notify)
		{
			VirtualCurrency item = null;
			try
			{
				item = (VirtualCurrency)StoreInfo.GetItemByItemId(this.CurrencyItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(VirtualCurrencyPack.TAG, "VirtualCurrency with itemId: " + this.CurrencyItemId + " doesn't exist! Can't give this pack.");
				return 0;
			}
			return VirtualCurrencyStorage.Add(item, this.CurrencyAmount * amount, notify);
		}

		public override int Take(int amount, bool notify)
		{
			VirtualCurrency item = null;
			try
			{
				item = (VirtualCurrency)StoreInfo.GetItemByItemId(this.CurrencyItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(VirtualCurrencyPack.TAG, "VirtualCurrency with itemId: " + this.CurrencyItemId + " doesn't exist! Can't take this pack.");
				return 0;
			}
			return VirtualCurrencyStorage.Remove(item, this.CurrencyAmount * amount, notify);
		}

		public override int ResetBalance(int balance, bool notify)
		{
			SoomlaUtils.LogError(VirtualCurrencyPack.TAG, "Someone tried to reset balance of CurrencyPack. That's not right.");
			return 0;
		}

		public override int GetBalance()
		{
			SoomlaUtils.LogError(VirtualCurrencyPack.TAG, "Someone tried to check balance of CurrencyPack. That's not right.");
			return 0;
		}

		protected override bool canBuy()
		{
			return true;
		}

		private static string TAG = "SOOMLA VirtualCurrencyPack";

		public int CurrencyAmount;

		public string CurrencyItemId;
	}
}
