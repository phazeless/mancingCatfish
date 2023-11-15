using System;

namespace Soomla.Store
{
	public abstract class PurchaseType
	{
		public PurchaseType()
		{
		}

		public abstract void Buy(string payload);

		public abstract bool CanAfford();

		public abstract string GetPrice();

		public PurchasableVirtualItem AssociatedItem;
	}
}
