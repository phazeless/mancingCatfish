using System;
using System.Collections.Generic;

namespace Soomla.Store
{
	public class StoreInventory
	{
		public static bool CanAfford(string itemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Checking can afford: " + itemId);
			PurchasableVirtualItem purchasableVirtualItem = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(itemId);
			return purchasableVirtualItem.CanAfford();
		}

		public static void BuyItem(string itemId)
		{
			StoreInventory.BuyItem(itemId, string.Empty);
		}

		public static void BuyItem(string itemId, string payload)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Buying: " + itemId);
			PurchasableVirtualItem purchasableVirtualItem = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(itemId);
			purchasableVirtualItem.Buy(payload);
		}

		public static int GetItemBalance(string itemId)
		{
			int result;
			if (StoreInventory.localItemBalances.TryGetValue(itemId, out result))
			{
				return result;
			}
			VirtualItem itemByItemId = StoreInfo.GetItemByItemId(itemId);
			return itemByItemId.GetBalance();
		}

		public static void GiveItem(string itemId, int amount)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", string.Concat(new object[]
			{
				"Giving: ",
				amount,
				" pieces of: ",
				itemId
			}));
			VirtualItem itemByItemId = StoreInfo.GetItemByItemId(itemId);
			itemByItemId.Give(amount);
		}

		public static void TakeItem(string itemId, int amount)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", string.Concat(new object[]
			{
				"Taking: ",
				amount,
				" pieces of: ",
				itemId
			}));
			VirtualItem itemByItemId = StoreInfo.GetItemByItemId(itemId);
			itemByItemId.Take(amount);
		}

		public static void EquipVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Equipping: " + goodItemId);
			EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(goodItemId);
			try
			{
				equippableVG.Equip();
			}
			catch (NotEnoughGoodsException ex)
			{
				SoomlaUtils.LogError("SOOMLA StoreInventory", "UNEXPECTED! Couldn't equip something");
				throw ex;
			}
		}

		public static void UnEquipVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "UnEquipping: " + goodItemId);
			EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(goodItemId);
			equippableVG.Unequip();
		}

		public static bool IsVirtualGoodEquipped(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Checking if " + goodItemId + " is equipped");
			EquippableVG good = (EquippableVG)StoreInfo.GetItemByItemId(goodItemId);
			return VirtualGoodsStorage.IsEquipped(good);
		}

		public static EquippableVG GetEquippedVirtualGood(VirtualCategory category)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Checking equipped goood in " + category.Name + " category");
			foreach (string text in category.GoodItemIds)
			{
				EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(text);
				if (equippableVG != null && equippableVG.Equipping == EquippableVG.EquippingModel.CATEGORY && VirtualGoodsStorage.IsEquipped(equippableVG) && StoreInfo.GetCategoryForVirtualGood(text) == category)
				{
					return equippableVG;
				}
			}
			SoomlaUtils.LogError("SOOMLA StoreInventory", "There is no virtual good equipped in " + category.Name + " category");
			return null;
		}

		public static int GetGoodUpgradeLevel(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Checking " + goodItemId + " upgrade level");
			VirtualGood virtualGood = (VirtualGood)StoreInfo.GetItemByItemId(goodItemId);
			if (virtualGood == null)
			{
				SoomlaUtils.LogError("SOOMLA StoreInventory", "You tried to get the level of a non-existant virtual good.");
				return 0;
			}
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(virtualGood);
			if (currentUpgrade == null)
			{
				return 0;
			}
			UpgradeVG upgradeVG = StoreInfo.GetFirstUpgradeForVirtualGood(goodItemId);
			int num = 1;
			while (upgradeVG.ItemId != currentUpgrade.ItemId)
			{
				upgradeVG = (UpgradeVG)StoreInfo.GetItemByItemId(upgradeVG.NextItemId);
				num++;
			}
			return num;
		}

		public static string GetGoodCurrentUpgrade(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Checking " + goodItemId + " current upgrade");
			VirtualGood good = (VirtualGood)StoreInfo.GetItemByItemId(goodItemId);
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(good);
			if (currentUpgrade == null)
			{
				return string.Empty;
			}
			return currentUpgrade.ItemId;
		}

		public static void UpgradeGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "SOOMLA/UNITY Calling UpgradeGood with: " + goodItemId);
			VirtualGood good = (VirtualGood)StoreInfo.GetItemByItemId(goodItemId);
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(good);
			if (currentUpgrade != null)
			{
				string nextItemId = currentUpgrade.NextItemId;
				if (string.IsNullOrEmpty(nextItemId))
				{
					return;
				}
				UpgradeVG upgradeVG = (UpgradeVG)StoreInfo.GetItemByItemId(nextItemId);
				upgradeVG.Buy(string.Empty);
			}
			else
			{
				UpgradeVG firstUpgradeForVirtualGood = StoreInfo.GetFirstUpgradeForVirtualGood(goodItemId);
				if (firstUpgradeForVirtualGood != null)
				{
					firstUpgradeForVirtualGood.Buy(string.Empty);
				}
			}
		}

		public static void RemoveGoodUpgrades(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "SOOMLA/UNITY Calling RemoveGoodUpgrades with: " + goodItemId);
			List<UpgradeVG> upgradesForVirtualGood = StoreInfo.GetUpgradesForVirtualGood(goodItemId);
			foreach (UpgradeVG item in upgradesForVirtualGood)
			{
				VirtualGoodsStorage.Remove(item, 1, true);
			}
			VirtualGood good = (VirtualGood)StoreInfo.GetItemByItemId(goodItemId);
			VirtualGoodsStorage.RemoveUpgrades(good);
		}

		public static void RefreshLocalInventory()
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Refreshing local inventory");
			StoreInventory.localItemBalances = new Dictionary<string, int>();
			StoreInventory.localUpgrades = new Dictionary<string, StoreInventory.LocalUpgrade>();
			StoreInventory.localEquippedGoods = new HashSet<string>();
			foreach (VirtualCurrency virtualCurrency in StoreInfo.Currencies)
			{
				StoreInventory.localItemBalances[virtualCurrency.ItemId] = VirtualCurrencyStorage.GetBalance(virtualCurrency);
			}
			foreach (VirtualGood virtualGood in StoreInfo.Goods)
			{
				StoreInventory.localItemBalances[virtualGood.ItemId] = VirtualGoodsStorage.GetBalance(virtualGood);
				UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(virtualGood);
				if (currentUpgrade != null)
				{
					int goodUpgradeLevel = StoreInventory.GetGoodUpgradeLevel(virtualGood.ItemId);
					StoreInventory.localUpgrades.AddOrUpdate(virtualGood.ItemId, new StoreInventory.LocalUpgrade
					{
						itemId = currentUpgrade.ItemId,
						level = goodUpgradeLevel
					});
				}
				if (virtualGood is EquippableVG && VirtualGoodsStorage.IsEquipped((EquippableVG)virtualGood))
				{
					StoreInventory.localEquippedGoods.Add(virtualGood.ItemId);
				}
			}
		}

		public static void RefreshOnGoodUpgrade(VirtualGood vg, UpgradeVG uvg)
		{
			if (uvg == null)
			{
				StoreInventory.localUpgrades.Remove(vg.ItemId);
			}
			else
			{
				int goodUpgradeLevel = StoreInventory.GetGoodUpgradeLevel(vg.ItemId);
				StoreInventory.LocalUpgrade localUpgrade;
				if (StoreInventory.localUpgrades.TryGetValue(vg.ItemId, out localUpgrade))
				{
					localUpgrade.itemId = uvg.ItemId;
					localUpgrade.level = goodUpgradeLevel;
				}
				else
				{
					StoreInventory.localUpgrades.Add(vg.ItemId, new StoreInventory.LocalUpgrade
					{
						itemId = uvg.ItemId,
						level = goodUpgradeLevel
					});
				}
			}
		}

		public static void RefreshOnGoodEquipped(EquippableVG equippable)
		{
			StoreInventory.localEquippedGoods.Add(equippable.ItemId);
		}

		public static void RefreshOnGoodUnEquipped(EquippableVG equippable)
		{
			StoreInventory.localEquippedGoods.Remove(equippable.ItemId);
		}

		public static void RefreshOnCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded)
		{
			StoreInventory.UpdateLocalBalance(virtualCurrency.ItemId, balance);
		}

		public static void RefreshOnGoodBalanceChanged(VirtualGood good, int balance, int amountAdded)
		{
			StoreInventory.UpdateLocalBalance(good.ItemId, balance);
		}

		private static void UpdateLocalBalance(string itemId, int balance)
		{
			StoreInventory.localItemBalances[itemId] = balance;
		}

		protected const string TAG = "SOOMLA StoreInventory";

		private static Dictionary<string, int> localItemBalances;

		private static Dictionary<string, StoreInventory.LocalUpgrade> localUpgrades;

		private static HashSet<string> localEquippedGoods;

		private class LocalUpgrade
		{
			public int level;

			public string itemId;
		}
	}
}
