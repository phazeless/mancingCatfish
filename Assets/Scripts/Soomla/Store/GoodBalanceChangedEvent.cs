using System;

namespace Soomla.Store
{
	public class GoodBalanceChangedEvent : SoomlaEvent
	{
		public GoodBalanceChangedEvent(VirtualGood item, int balance, int amountAdded) : this(item, balance, amountAdded, null)
		{
		}

		public GoodBalanceChangedEvent(VirtualGood item, int balance, int amountAdded, object sender) : base(sender)
		{
			this.mItem = item;
			this.mBalance = balance;
			this.mAmountAdded = amountAdded;
		}

		public VirtualGood getGoodItemId()
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

		private VirtualGood mItem;

		private int mBalance;

		private int mAmountAdded;
	}
}
