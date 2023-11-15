using System;

namespace Soomla.Store
{
	public class CurrencyBalanceChangedEvent : SoomlaEvent
	{
		public CurrencyBalanceChangedEvent(VirtualCurrency item, int balance, int amountAdded) : this(item, balance, amountAdded, null)
		{
		}

		public CurrencyBalanceChangedEvent(VirtualCurrency item, int balance, int amountAdded, object sender) : base(sender)
		{
			this.mItem = item;
			this.mBalance = balance;
			this.mAmountAdded = amountAdded;
		}

		public VirtualCurrency getCurrencyItem()
		{
			return this.mItem;
		}

		public int getBalance()
		{
			return this.mBalance;
		}

		public int getAmountAdded()
		{
			return this.mAmountAdded;
		}

		private VirtualCurrency mItem;

		private int mBalance;

		private int mAmountAdded;
	}
}
