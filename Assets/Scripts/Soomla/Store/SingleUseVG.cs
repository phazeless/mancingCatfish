using System;

namespace Soomla.Store
{
	public class SingleUseVG : VirtualGood
	{
		public SingleUseVG(string name, string description, string itemId, PurchaseType purchaseType) : base(name, description, itemId, purchaseType)
		{
		}

		public SingleUseVG(JSONObject jsonVg) : base(jsonVg)
		{
		}

		public override JSONObject toJSONObject()
		{
			return base.toJSONObject();
		}

		public override int Give(int amount, bool notify)
		{
			return VirtualGoodsStorage.Add(this, amount, notify);
		}

		public override int Take(int amount, bool notify)
		{
			return VirtualGoodsStorage.Remove(this, amount, notify);
		}

		protected override bool canBuy()
		{
			return true;
		}
	}
}
