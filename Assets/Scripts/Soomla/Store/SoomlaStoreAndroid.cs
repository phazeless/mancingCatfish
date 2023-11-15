using System;
using UnityEngine;

namespace Soomla.Store
{
	public class SoomlaStoreAndroid : SoomlaStore
	{
		protected override void _loadBillingService()
		{
			if (StoreSettings.GPlayBP)
			{
				if (string.IsNullOrEmpty(StoreSettings.AndroidPublicKey) || StoreSettings.AndroidPublicKey == StoreSettings.AND_PUB_KEY_DEFAULT)
				{
					SoomlaUtils.LogError("SOOMLA SoomlaStore", "You chose Google Play billing service, but publicKey is not set!! Stopping here!!");
					throw new ExitGUIException();
				}
				if (StoreSettings.PlaySsvValidation)
				{
					if (string.IsNullOrEmpty(StoreSettings.PlayClientId) || StoreSettings.PlayClientId == StoreSettings.PLAY_CLIENT_ID_DEFAULT)
					{
						SoomlaUtils.LogError("SOOMLA SoomlaStore", "You chose Google Play Receipt Validation, but clientId is not set!! Stopping here!!");
						throw new ExitGUIException();
					}
					if (string.IsNullOrEmpty(StoreSettings.PlayClientSecret) || StoreSettings.PlayClientSecret == StoreSettings.PLAY_CLIENT_SECRET_DEFAULT)
					{
						SoomlaUtils.LogError("SOOMLA SoomlaStore", "You chose Google Play Receipt Validation, but clientSecret is not set!! Stopping here!!");
						throw new ExitGUIException();
					}
					if (string.IsNullOrEmpty(StoreSettings.PlayRefreshToken) || StoreSettings.PlayRefreshToken == StoreSettings.PLAY_REFRESH_TOKEN_DEFAULT)
					{
						SoomlaUtils.LogError("SOOMLA SoomlaStore", "You chose Google Play Receipt Validation, but refreshToken is not set!! Stopping here!!");
						throw new ExitGUIException();
					}
				}
			}
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.store.SoomlaStore"))
			{
				SoomlaStoreAndroid.jniSoomlaStore = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
				if (!SoomlaStoreAndroid.jniSoomlaStore.Call<bool>("loadBillingService", new object[0]))
				{
					SoomlaUtils.LogError("SOOMLA SoomlaStore", "Couldn't load billing service! Billing functions won't work.");
				}
			}
			if (StoreSettings.GPlayBP)
			{
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.soomla.store.billing.google.GooglePlayIabService"))
				{
					AndroidJavaObject androidJavaObject = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
					androidJavaObject.Call("setPublicKey", new object[]
					{
						StoreSettings.AndroidPublicKey
					});
					if (StoreSettings.PlaySsvValidation)
					{
						using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.util.HashMap", new object[0]))
						{
							IntPtr methodID = AndroidJNIHelper.GetMethodID(androidJavaObject2.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
							object[] array = new object[2];
							using (AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.lang.String", new object[]
							{
								"clientId"
							}))
							{
								using (AndroidJavaObject androidJavaObject4 = new AndroidJavaObject("java.lang.String", new object[]
								{
									StoreSettings.PlayClientId
								}))
								{
									array[0] = androidJavaObject3;
									array[1] = androidJavaObject4;
									AndroidJNI.CallObjectMethod(androidJavaObject2.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
								}
							}
							using (AndroidJavaObject androidJavaObject5 = new AndroidJavaObject("java.lang.String", new object[]
							{
								"clientSecret"
							}))
							{
								using (AndroidJavaObject androidJavaObject6 = new AndroidJavaObject("java.lang.String", new object[]
								{
									StoreSettings.PlayClientSecret
								}))
								{
									array[0] = androidJavaObject5;
									array[1] = androidJavaObject6;
									AndroidJNI.CallObjectMethod(androidJavaObject2.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
								}
							}
							using (AndroidJavaObject androidJavaObject7 = new AndroidJavaObject("java.lang.String", new object[]
							{
								"refreshToken"
							}))
							{
								using (AndroidJavaObject androidJavaObject8 = new AndroidJavaObject("java.lang.String", new object[]
								{
									StoreSettings.PlayRefreshToken
								}))
								{
									array[0] = androidJavaObject7;
									array[1] = androidJavaObject8;
									AndroidJNI.CallObjectMethod(androidJavaObject2.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
								}
							}
							using (AndroidJavaObject androidJavaObject9 = new AndroidJavaObject("java.lang.String", new object[]
							{
								"verifyOnServerFailure"
							}))
							{
								using (AndroidJavaObject androidJavaObject10 = new AndroidJavaObject("java.lang.Boolean", new object[]
								{
									StoreSettings.PlayVerifyOnServerFailure
								}))
								{
									array[0] = androidJavaObject9;
									array[1] = androidJavaObject10;
									AndroidJNI.CallObjectMethod(androidJavaObject2.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
								}
							}
							androidJavaObject.Call("configVerifyPurchases", new object[]
							{
								androidJavaObject2
							});
						}
					}
					androidJavaClass2.SetStatic<bool>("AllowAndroidTestPurchases", StoreSettings.AndroidTestPurchases);
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _buyMarketItem(string productId, string payload)
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaObject androidJavaObject = AndroidJNIHandler.CallStatic<AndroidJavaObject>(new AndroidJavaClass("com.soomla.store.data.StoreInfo"), "getPurchasableItem", productId))
			{
				AndroidJNIHandler.CallVoid(SoomlaStoreAndroid.jniSoomlaStore, "buyWithMarket", androidJavaObject.Call<AndroidJavaObject>("getPurchaseType", new object[0]).Call<AndroidJavaObject>("getMarketItem", new object[0]), payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _refreshInventory()
		{
			AndroidJNI.PushLocalFrame(100);
			SoomlaStoreAndroid.jniSoomlaStore.Call("refreshInventory", new object[0]);
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _refreshMarketItemsDetails()
		{
			AndroidJNI.PushLocalFrame(100);
			SoomlaStoreAndroid.jniSoomlaStore.Call("refreshMarketItemsDetails", new object[0]);
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _restoreTransactions()
		{
			AndroidJNI.PushLocalFrame(100);
			SoomlaStoreAndroid.jniSoomlaStore.Call("restoreTransactions", new object[0]);
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _startIabServiceInBg()
		{
			AndroidJNI.PushLocalFrame(100);
			SoomlaStoreAndroid.jniSoomlaStore.Call("startIabServiceInBg", new object[0]);
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _stopIabServiceInBg()
		{
			AndroidJNI.PushLocalFrame(100);
			SoomlaStoreAndroid.jniSoomlaStore.Call("stopIabServiceInBg", new object[0]);
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		private static AndroidJavaObject jniSoomlaStore;
	}
}
