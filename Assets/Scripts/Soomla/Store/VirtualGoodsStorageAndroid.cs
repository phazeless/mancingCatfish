using System;
using UnityEngine;

namespace Soomla.Store
{
	public class VirtualGoodsStorageAndroid : VirtualGoodsStorage
	{
		protected override void _removeUpgrades(VirtualGood good, bool notify)
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualGoodsStorage", new object[0]))
				{
					androidJavaObject.Call("removeUpgrades", new object[]
					{
						good.ItemId,
						notify
					});
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _assignCurrentUpgrade(VirtualGood good, UpgradeVG upgradeVG, bool notify)
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualGoodsStorage", new object[0]))
				{
					androidJavaObject.Call("assignCurrentUpgrade", new object[]
					{
						good.ItemId,
						upgradeVG.ItemId,
						notify
					});
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override UpgradeVG _getCurrentUpgrade(VirtualGood good)
		{
			AndroidJNI.PushLocalFrame(100);
			string text;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualGoodsStorage", new object[0]))
				{
					text = androidJavaObject.Call<string>("getCurrentUpgrade", new object[]
					{
						good.ItemId
					});
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			if (!string.IsNullOrEmpty(text))
			{
				return (UpgradeVG)StoreInfo.GetItemByItemId(text);
			}
			return null;
		}

		protected override bool _isEquipped(EquippableVG good)
		{
			AndroidJNI.PushLocalFrame(100);
			bool result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualGoodsStorage", new object[0]))
				{
					result = androidJavaObject.Call<bool>("isEquipped", new object[]
					{
						good.ItemId
					});
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return result;
		}

		protected override void _equip(EquippableVG good, bool notify)
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualGoodsStorage", new object[0]))
				{
					androidJavaObject.Call("equip", new object[]
					{
						good.ItemId,
						notify
					});
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _unequip(EquippableVG good, bool notify)
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualGoodsStorage", new object[0]))
				{
					androidJavaObject.Call("unequip", new object[]
					{
						good.ItemId,
						notify
					});
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override int _getBalance(VirtualItem item)
		{
			AndroidJNI.PushLocalFrame(100);
			int result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualGoodsStorage", new object[0]))
				{
					result = androidJavaObject.Call<int>("getBalance", new object[]
					{
						item.ItemId
					});
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return result;
		}

		protected override int _setBalance(VirtualItem item, int balance, bool notify)
		{
			AndroidJNI.PushLocalFrame(100);
			int result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualGoodsStorage", new object[0]))
				{
					result = androidJavaObject.Call<int>("setBalance", new object[]
					{
						item.ItemId,
						balance,
						notify
					});
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return result;
		}

		protected override int _add(VirtualItem item, int amount, bool notify)
		{
			AndroidJNI.PushLocalFrame(100);
			int result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualGoodsStorage", new object[0]))
				{
					result = androidJavaObject.Call<int>("add", new object[]
					{
						item.ItemId,
						amount,
						notify
					});
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return result;
		}

		protected override int _remove(VirtualItem item, int amount, bool notify)
		{
			AndroidJNI.PushLocalFrame(100);
			int result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualGoodsStorage", new object[0]))
				{
					result = androidJavaObject.Call<int>("remove", new object[]
					{
						item.ItemId,
						amount,
						notify
					});
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return result;
		}
	}
}
