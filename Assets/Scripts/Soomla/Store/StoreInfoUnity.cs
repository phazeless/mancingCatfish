using System;
using System.Reflection;

namespace Soomla.Store
{
	public class StoreInfoUnity : StoreInfo
	{
		protected override void _setStoreAssets(IStoreAssets storeAssets)
		{
			string val = StoreInfo.IStoreAssetsToJSON(storeAssets);
			KeyValueStorage.SetValue(this.keyMetaStoreInfo(), val);
		}

		private string keyMetaStoreInfo()
		{
			MethodInfo method = typeof(StoreInfo).GetMethod("keyMetaStoreInfo", BindingFlags.Static | BindingFlags.NonPublic);
			return (string)method.Invoke(this, new object[0]);
		}
	}
}
