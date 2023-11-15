using System;

namespace Soomla.Store
{
	public class VirtualCurrency : VirtualItem
	{
		public VirtualCurrency(string name, string description, string itemId) : base(name, description, itemId)
		{
		}

		public VirtualCurrency(JSONObject jsonVc) : base(jsonVc)
		{
		}

		public override JSONObject toJSONObject()
		{
			return base.toJSONObject();
		}

		public override int Give(int amount, bool notify)
		{
			return VirtualCurrencyStorage.Add(this, amount, notify);
		}

		public override int Take(int amount, bool notify)
		{
			return VirtualCurrencyStorage.Remove(this, amount, notify);
		}

		public override int ResetBalance(int balance, bool notify)
		{
			return VirtualCurrencyStorage.SetBalance(this, balance, notify);
		}

		public override int GetBalance()
		{
			return VirtualCurrencyStorage.GetBalance(this);
		}
	}
}
