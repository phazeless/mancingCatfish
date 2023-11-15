using System;
using UnityEngine;

namespace Soomla.Store
{
	public class VirtualCurrencyStorageAndroid : VirtualCurrencyStorage
	{
		protected override int _getBalance(VirtualItem item)
		{
			AndroidJNI.PushLocalFrame(100);
			int result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StorageManager"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualCurrencyStorage", new object[0]))
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
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualCurrencyStorage", new object[0]))
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
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualCurrencyStorage", new object[0]))
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
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getVirtualCurrencyStorage", new object[0]))
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
