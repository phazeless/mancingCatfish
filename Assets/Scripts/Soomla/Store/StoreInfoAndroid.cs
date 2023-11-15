using System;
using UnityEngine;

namespace Soomla.Store
{
	public class StoreInfoAndroid : StoreInfo
	{
		protected override void _setStoreAssets(IStoreAssets storeAssets)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "pushing IStoreAssets to StoreInfo on java side");
			AndroidJNI.PushLocalFrame(100);
			string text = StoreInfo.IStoreAssetsToJSON(storeAssets);
			int version = storeAssets.GetVersion();
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StoreInfo"))
			{
				androidJavaClass.CallStatic("setStoreAssets", new object[]
				{
					version,
					text
				});
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "done! (pushing data to StoreAssets on java side)");
		}

		protected override void loadNativeFromDB()
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.data.StoreInfo"))
			{
				androidJavaClass.CallStatic<bool>("loadFromDB", new object[0]);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
	}
}
