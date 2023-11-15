using System;

namespace Soomla.Store
{
	public class GoodUpgradeEvent : SoomlaEvent
	{
		public GoodUpgradeEvent(VirtualGood item, UpgradeVG upgradeVGItem) : this(item, upgradeVGItem, null)
		{
		}

		public GoodUpgradeEvent(VirtualGood item, UpgradeVG upgradeVGItem, object sender) : base(sender)
		{
			this.mItem = item;
			this.mCurrentUpgradeItem = upgradeVGItem;
		}

		public VirtualGood getGoodItem()
		{
			return this.mItem;
		}

		public UpgradeVG getCurrentUpgrade()
		{
			return this.mCurrentUpgradeItem;
		}

		private VirtualGood mItem;

		private UpgradeVG mCurrentUpgradeItem;
	}
}
