using System;

namespace Soomla.Store
{
	public class GoodEquippedEvent : SoomlaEvent
	{
		public GoodEquippedEvent(EquippableVG item) : this(item, null)
		{
			this.mItem = item;
		}

		public GoodEquippedEvent(EquippableVG item, object sender) : base(sender)
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
