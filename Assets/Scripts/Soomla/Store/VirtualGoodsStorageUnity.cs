using System;

namespace Soomla.Store
{
	public class VirtualGoodsStorageUnity : VirtualGoodsStorage
	{
		protected override void _removeUpgrades(VirtualGood good, bool notify)
		{
			string itemId = good.ItemId;
			string key = VirtualGoodsStorageUnity.keyGoodUpgrade(itemId);
			EncryptedPlayerPrefs.DeleteKey(key);
			if (notify)
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("itemId", good.ItemId);
				StoreEvents.Instance.onGoodUpgrade(jsonobject.print(false));
			}
		}

		protected override void _assignCurrentUpgrade(VirtualGood good, UpgradeVG upgradeVG, bool notify)
		{
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(good);
			if (currentUpgrade != null && currentUpgrade.ItemId == upgradeVG.ItemId)
			{
				return;
			}
			string itemId = good.ItemId;
			string key = VirtualGoodsStorageUnity.keyGoodUpgrade(itemId);
			string itemId2 = upgradeVG.ItemId;
			EncryptedPlayerPrefs.SetString(key, itemId2, true);
			if (notify)
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("itemId", good.ItemId);
				jsonobject.AddField("upgradeItemId", upgradeVG.ItemId);
				StoreEvents.Instance.onGoodUpgrade(jsonobject.print(false));
			}
		}

		protected override UpgradeVG _getCurrentUpgrade(VirtualGood good)
		{
			string itemId = good.ItemId;
			string key = VirtualGoodsStorageUnity.keyGoodUpgrade(itemId);
			string @string = EncryptedPlayerPrefs.GetString(key);
			if (string.IsNullOrEmpty(@string))
			{
				SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "You tried to fetch the current upgrade of " + good.ItemId + " but there's no upgrade for it.");
				return null;
			}
			try
			{
				return (UpgradeVG)StoreInfo.GetItemByItemId(@string);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(VirtualItemStorage.TAG, "The current upgrade's itemId from the DB is not found in StoreInfo.");
			}
			catch (InvalidCastException)
			{
				SoomlaUtils.LogError(VirtualItemStorage.TAG, "The current upgrade's itemId from the DB is not an UpgradeVG.");
			}
			return null;
		}

		protected override bool _isEquipped(EquippableVG good)
		{
			string itemId = good.ItemId;
			string key = VirtualGoodsStorageUnity.keyGoodEquipped(itemId);
			string @string = EncryptedPlayerPrefs.GetString(key);
			return !string.IsNullOrEmpty(@string);
		}

		protected override void _equip(EquippableVG good, bool notify)
		{
			if (VirtualGoodsStorage.IsEquipped(good))
			{
				return;
			}
			this.equipPriv(good, true, notify);
		}

		protected override void _unequip(EquippableVG good, bool notify)
		{
			if (!VirtualGoodsStorage.IsEquipped(good))
			{
				return;
			}
			this.equipPriv(good, false, notify);
		}

		private void equipPriv(EquippableVG good, bool equip, bool notify)
		{
			string itemId = good.ItemId;
			string key = VirtualGoodsStorageUnity.keyGoodEquipped(itemId);
			if (equip)
			{
				EncryptedPlayerPrefs.SetString(key, "yes", true);
				if (notify)
				{
					JSONObject jsonobject = new JSONObject();
					jsonobject.AddField("itemId", good.ItemId);
					StoreEvents.Instance.onGoodEquipped(jsonobject.print(false));
				}
			}
			else
			{
				EncryptedPlayerPrefs.DeleteKey(key);
				if (notify)
				{
					JSONObject jsonobject2 = new JSONObject();
					jsonobject2.AddField("itemId", good.ItemId);
					StoreEvents.Instance.onGoodUnequipped(jsonobject2.print(false));
				}
			}
		}

		private static string keyGoodBalance(string itemId)
		{
			return "good." + itemId + ".balance";
		}

		private static string keyGoodEquipped(string itemId)
		{
			return "good." + itemId + ".equipped";
		}

		private static string keyGoodUpgrade(string itemId)
		{
			return "good." + itemId + ".currentUpgrade";
		}

		protected string keyBalance(string itemId)
		{
			return VirtualGoodsStorageUnity.keyGoodBalance(itemId);
		}

		protected void postBalanceChangeEvent(VirtualItem item, int balance, int amountAdded)
		{
			JSONObject jsonobject = new JSONObject();
			jsonobject.AddField("itemId", item.ItemId);
			jsonobject.AddField("balance", balance);
			jsonobject.AddField("amountAdded", amountAdded);
			StoreEvents.Instance.onGoodBalanceChanged(jsonobject.print(false));
		}

		protected override int _getBalance(VirtualItem item)
		{
			string itemId = item.ItemId;
			string key = this.keyBalance(itemId);
			string @string = EncryptedPlayerPrefs.GetString(key);
			int num = 0;
			if (!string.IsNullOrEmpty(@string))
			{
				num = int.Parse(@string);
			}
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, string.Concat(new object[]
			{
				"the balance for ",
				item.ItemId,
				" is ",
				num
			}));
			return num;
		}

		protected override int _setBalance(VirtualItem item, int balance, bool notify)
		{
			int num = this._getBalance(item);
			if (num == balance)
			{
				return balance;
			}
			string itemId = item.ItemId;
			string value = string.Empty + balance;
			string key = this.keyBalance(itemId);
			EncryptedPlayerPrefs.SetString(key, value, true);
			if (notify)
			{
				this.postBalanceChangeEvent(item, balance, 0);
			}
			return balance;
		}

		protected override int _add(VirtualItem item, int amount, bool notify)
		{
			string itemId = item.ItemId;
			int num = this._getBalance(item);
			if (num < 0)
			{
				num = 0;
				amount = 0;
			}
			string value = string.Empty + (num + amount);
			string key = this.keyBalance(itemId);
			EncryptedPlayerPrefs.SetString(key, value, true);
			if (notify)
			{
				this.postBalanceChangeEvent(item, num + amount, amount);
			}
			return num + amount;
		}

		protected override int _remove(VirtualItem item, int amount, bool notify)
		{
			string itemId = item.ItemId;
			int num = this._getBalance(item) - amount;
			if (num < 0)
			{
				num = 0;
				amount = 0;
			}
			string value = string.Empty + num;
			string key = this.keyBalance(itemId);
			EncryptedPlayerPrefs.SetString(key, value, true);
			if (notify)
			{
				this.postBalanceChangeEvent(item, num, -1 * amount);
			}
			return num;
		}
	}
}
