using System;

namespace Soomla.Store
{
	public class VirtualCurrencyStorage : VirtualItemStorage
	{
		protected VirtualCurrencyStorage()
		{
			VirtualItemStorage.TAG = "SOOMLA VirtualCurrencyStorage";
		}

		private static VirtualCurrencyStorage instance
		{
			get
			{
				return VirtualCurrencyStorage._instance = ((VirtualCurrencyStorage._instance != null) ? VirtualCurrencyStorage._instance : new VirtualCurrencyStorageUnity());
			}
		}

		public static int GetBalance(VirtualItem item)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "fetching balance for virtual item with itemId: " + item.ItemId);
			return VirtualCurrencyStorage.instance._getBalance(item);
		}

		public static int SetBalance(VirtualItem item, int balance)
		{
			return VirtualCurrencyStorage.SetBalance(item, balance, true);
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
			return VirtualCurrencyStorage.instance._setBalance(item, balance, notify);
		}

		public static int Add(VirtualItem item, int amount)
		{
			return VirtualCurrencyStorage.Add(item, amount, true);
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
			return VirtualCurrencyStorage.instance._add(item, amount, notify);
		}

		public static int Remove(VirtualItem item, int amount)
		{
			return VirtualCurrencyStorage.Remove(item, amount, true);
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
			return VirtualCurrencyStorage.instance._remove(item, amount, true);
		}

		private static VirtualCurrencyStorage _instance;
	}
}
