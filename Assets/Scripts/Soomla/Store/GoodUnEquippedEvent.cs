using System;

namespace Soomla.Store
{
	public class GoodUnEquippedEvent : SoomlaEvent
	{
		public GoodUnEquippedEvent(EquippableVG item) : this(item, null)
		{
		}

		public GoodUnEquippedEvent(EquippableVG item, object sender) : base(sender)
		{
			this.mItem = item;
		}

		public EquippableVG getGoodItem()
		{
			return this.mItem;
		}

		private EquippableVG mItem;
	}
}
