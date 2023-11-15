using System;

namespace Soomla.Store
{
	public abstract class VirtualGood : PurchasableVirtualItem
	{
		public VirtualGood(string name, string description, string itemId, PurchaseType purchaseType) : base(name, description, itemId, purchaseType)
		{
		}

		public VirtualGood(JSONObject jsonVg) : base(jsonVg)
		{
		}

		public override JSONObject toJSONObject()
		{
			return base.toJSONObject();
		}

		public override int ResetBalance(int balance, bool notify)
		{
			return VirtualGoodsStorage.SetBalance(this, balance, notify);
		}

		public override int GetBalance()
		{
			return VirtualGoodsStorage.GetBalance(this);
		}
	}
}
