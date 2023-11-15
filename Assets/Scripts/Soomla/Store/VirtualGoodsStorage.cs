using System;

namespace Soomla.Store
{
	public class VirtualGoodsStorage : VirtualItemStorage
	{
		protected VirtualGoodsStorage()
		{
			VirtualItemStorage.TAG = "SOOMLA VirtualGoodsStorage";
		}

		private static VirtualGoodsStorage instance
		{
			get
			{
				return VirtualGoodsStorage._instance = ((VirtualGoodsStorage._instance != null) ? VirtualGoodsStorage._instance : new VirtualGoodsStorageUnity());
			}
		}

		public static void RemoveUpgrades(VirtualGood good)
		{
			VirtualGoodsStorage.RemoveUpgrades(good, true);
		}

		public static void RemoveUpgrades(VirtualGood good, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "Removing upgrade information from virtual good: " + good.ItemId);
			VirtualGoodsStorage.instance._removeUpgrades(good, notify);
		}

		public static void AssignCurrentUpgrade(VirtualGood good, UpgradeVG upgradeVG)
		{
			VirtualGoodsStorage.AssignCurrentUpgrade(good, upgradeVG, true);
		}

		public static void AssignCurrentUpgrade(VirtualGood good, UpgradeVG upgradeVG, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "Assigning upgrade " + upgradeVG.ItemId + " to virtual good: " + good.ItemId);
			VirtualGoodsStorage.instance._assignCurrentUpgrade(good, upgradeVG, notify);
		}

		public static UpgradeVG GetCurrentUpgrade(VirtualGood good)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "Fetching upgrade to virtual good: " + good.ItemId);
			return VirtualGoodsStorage.instance._getCurrentUpgrade(good);
		}

		public static bool IsEquipped(EquippableVG good)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "checking if virtual good with itemId: " + good.ItemId + " is equipped.");
			return VirtualGoodsStorage.instance._isEquipped(good);
		}

		public static void Equip(EquippableVG good)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "equipping: " + good.ItemId);
			VirtualGoodsStorage.Equip(good);
		}

		public static void Equip(EquippableVG good, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "equipping: " + good.ItemId);
			VirtualGoodsStorage.instance._equip(good, notify);
		}

		public static void UnEquip(EquippableVG good)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "unequipping: " + good.ItemId);
			VirtualGoodsStorage.UnEquip(good, true);
		}

		public static void UnEquip(EquippableVG good, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "unequipping: " + good.ItemId);
			VirtualGoodsStorage.instance._unequip(good, notify);
		}

		public static int GetBalance(VirtualItem item)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "fetching balance for virtual item with itemId: " + item.ItemId);
			return VirtualGoodsStorage.instance._getBalance(item);
		}

		public static int SetBalance(VirtualItem item, int balance)
		{
			return VirtualGoodsStorage.SetBalance(item, balance, true);
		}

		public static int SetBalance(VirtualItem item, int balance, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, string.Concat(new object[]
			{
				"setting balance ",
				balance,
				" to ",
				item.ItemId,
				"."
			}));
			return VirtualGoodsStorage.instance._setBalance(item, balance, notify);
		}

		public static int Add(VirtualItem item, int amount)
		{
			return VirtualGoodsStorage.Add(item, amount, true);
		}

		public static int Add(VirtualItem item, int amount, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, string.Concat(new object[]
			{
				"adding ",
				amount,
				" ",
				item.ItemId
			}));
			return VirtualGoodsStorage.instance._add(item, amount, notify);
		}

		public static int Remove(VirtualItem item, int amount)
		{
			return VirtualGoodsStorage.Remove(item, amount, true);
		}

		public static int Remove(VirtualItem item, int amount, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, string.Concat(new object[]
			{
				"Removing ",
				amount,
				" ",
				item.ItemId,
				"."
			}));
			return VirtualGoodsStorage.instance._remove(item, amount, true);
		}

		protected virtual void _removeUpgrades(VirtualGood good, bool notify)
		{
		}

		protected virtual void _assignCurrentUpgrade(VirtualGood good, UpgradeVG upgradeVG, bool notify)
		{
		}

		protected virtual UpgradeVG _getCurrentUpgrade(VirtualGood good)
		{
			return null;
		}

		protected virtual bool _isEquipped(EquippableVG good)
		{
			return false;
		}

		protected virtual void _equip(EquippableVG good, bool notify)
		{
		}

		protected virtual void _unequip(EquippableVG good, bool notify)
		{
		}

		private static VirtualGoodsStorage _instance;
	}
}
