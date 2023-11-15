using System;

namespace Soomla.Store
{
	public class LifetimeVG : VirtualGood
	{
		public LifetimeVG(string name, string description, string itemId, PurchaseType purchaseType) : base(name, description, itemId, purchaseType)
		{
		}

		public LifetimeVG(JSONObject jsonVg) : base(jsonVg)
		{
		}

		public override JSONObject toJSONObject()
		{
			return base.toJSONObject();
		}

		public override int Give(int amount, bool notify)
		{
			if (amount > 1)
			{
				SoomlaUtils.LogDebug(LifetimeVG.TAG, "You tried to give more than one LifetimeVG.Will try to give one anyway.");
				amount = 1;
			}
			int balance = VirtualGoodsStorage.GetBalance(this);
			if (balance < 1)
			{
				return VirtualGoodsStorage.Add(this, amount, notify);
			}
			return 1;
		}

		public override int Take(int amount, bool notify)
		{
			if (amount > 1)
			{
				amount = 1;
			}
			int balance = VirtualGoodsStorage.GetBalance(this);
			if (balance > 0)
			{
				return VirtualGoodsStorage.Remove(this, amount, notify);
			}
			return 0;
		}

		protected override bool canBuy()
		{
			int balance = VirtualGoodsStorage.GetBalance(this);
			return balance < 1;
		}

		private static string TAG = "SOOMLA LifetimeVG";
	}
}
