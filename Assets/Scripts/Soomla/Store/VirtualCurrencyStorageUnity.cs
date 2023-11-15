using System;

namespace Soomla.Store
{
	public class VirtualCurrencyStorageUnity : VirtualCurrencyStorage
	{
		private static string keyCurrencyBalance(string itemId)
		{
			return "currency." + itemId + ".balance";
		}

		protected string keyBalance(string itemId)
		{
			return VirtualCurrencyStorageUnity.keyCurrencyBalance(itemId);
		}

		protected void postBalanceChangeEvent(VirtualItem item, int balance, int amountAdded)
		{
			JSONObject jsonobject = new JSONObject();
			jsonobject.AddField("itemId", item.ItemId);
			jsonobject.AddField("balance", balance);
			jsonobject.AddField("amountAdded", amountAdded);
			StoreEvents.Instance.onCurrencyBalanceChanged(jsonobject.print(false));
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
