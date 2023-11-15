using System;

namespace Soomla.Store
{
	public class InsufficientFundsException : Exception
	{
		public InsufficientFundsException() : base("You tried to buy something but you don't have enough funds to buy it.")
		{
		}

		public InsufficientFundsException(string itemId) : base("You tried to buy with itemId: " + itemId + " but you don't have enough funds to buy it.")
		{
		}
	}
}
