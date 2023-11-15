using System;
using System.Collections;
using System.Collections.Generic;
using Soomla.Singletons;
using UnityEngine;

namespace Soomla.Store
{
	public class StoreEvents : CodeGeneratedSingleton
	{
		protected override bool DontDestroySingleton
		{
			get
			{
				return true;
			}
		}

		public void RunLater(StoreEvents.RunLaterDelegate runLaterDelegate)
		{
			base.StartCoroutine(this.RunLaterPriv(0.1f, runLaterDelegate));
		}

		private IEnumerator RunLaterPriv(float delay, StoreEvents.RunLaterDelegate runLaterDelegate)
		{
			float pauseEndTime = Time.realtimeSinceStartup + delay;
			while (Time.realtimeSinceStartup < pauseEndTime)
			{
				yield return null;
			}
			runLaterDelegate();
			yield break;
		}

		public static void Initialize()
		{
			if (StoreEvents.Instance == null)
			{
				CoreEvents.Initialize();
				StoreEvents.Instance = UnitySingleton.GetSynchronousCodeGeneratedInstance<StoreEvents>();
				SoomlaUtils.LogDebug("SOOMLA StoreEvents", "Initializing StoreEvents ...");
				
				StoreEvents.sep = new StoreEventPusherAndroid();
			}
		}

		public void onBillingSupported(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onBillingSupported");
			StoreEvents.OnBillingSupported();
		}

		public void onBillingNotSupported(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onBillingNotSupported");
			StoreEvents.OnBillingNotSupported();
		}

		public void onCurrencyBalanceChanged(string message)
		{
			this.onCurrencyBalanceChanged(message, false);
		}

		public void onCurrencyBalanceChanged(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onCurrencyBalanceChanged:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			VirtualCurrency virtualCurrency = (VirtualCurrency)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			int num = (int)jsonobject["balance"].n;
			int num2 = (int)jsonobject["amountAdded"].n;
			StoreInventory.RefreshOnCurrencyBalanceChanged(virtualCurrency, num, num2);
			StoreEvents.OnCurrencyBalanceChanged(virtualCurrency, num, num2);
			if (alsoPush)
			{
				StoreEvents.sep.PushEventOnCurrencyBalanceChanged(virtualCurrency, num, num2);
			}
		}

		public void onGoodBalanceChanged(string message)
		{
			this.onGoodBalanceChanged(message, false);
		}

		public void onGoodBalanceChanged(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onGoodBalanceChanged:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			VirtualGood virtualGood = (VirtualGood)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			int num = (int)jsonobject["balance"].n;
			int num2 = (int)jsonobject["amountAdded"].n;
			StoreInventory.RefreshOnGoodBalanceChanged(virtualGood, num, num2);
			StoreEvents.OnGoodBalanceChanged(virtualGood, num, num2);
			if (alsoPush)
			{
				StoreEvents.sep.PushEventOnGoodBalanceChanged(virtualGood, num, num2);
			}
		}

		public void onGoodEquipped(string message)
		{
			this.onGoodEquipped(message, false);
		}

		public void onGoodEquipped(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onVirtualGoodEquipped:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			StoreInventory.RefreshOnGoodEquipped(equippableVG);
			StoreEvents.OnGoodEquipped(equippableVG);
			if (alsoPush)
			{
				StoreEvents.sep.PushEventOnGoodEquipped(equippableVG);
			}
		}

		public void onGoodUnequipped(string message)
		{
			this.onGoodUnequipped(message, false);
		}

		public void onGoodUnequipped(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onVirtualGoodUnEquipped:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			StoreInventory.RefreshOnGoodUnEquipped(equippableVG);
			StoreEvents.OnGoodUnEquipped(equippableVG);
			if (alsoPush)
			{
				StoreEvents.sep.PushEventOnGoodUnequipped(equippableVG);
			}
		}

		public void onGoodUpgrade(string message)
		{
			this.onGoodUpgrade(message, false);
		}

		public void onGoodUpgrade(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onGoodUpgrade:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			VirtualGood virtualGood = (VirtualGood)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			UpgradeVG upgradeVG = null;
			if (jsonobject.HasField("upgradeItemId") && !string.IsNullOrEmpty(jsonobject["upgradeItemId"].str))
			{
				upgradeVG = (UpgradeVG)StoreInfo.GetItemByItemId(jsonobject["upgradeItemId"].str);
			}
			StoreInventory.RefreshOnGoodUpgrade(virtualGood, upgradeVG);
			StoreEvents.OnGoodUpgrade(virtualGood, upgradeVG);
			if (alsoPush)
			{
				StoreEvents.sep.PushEventOnGoodUpgrade(virtualGood, upgradeVG);
			}
		}

		public void onItemPurchased(string message)
		{
			this.onItemPurchased(message, false);
		}

		public void onItemPurchased(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onItemPurchased:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			PurchasableVirtualItem purchasableVirtualItem = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			string text = string.Empty;
			if (jsonobject.HasField("payload"))
			{
				text = jsonobject["payload"].str;
			}
			StoreEvents.OnItemPurchased(purchasableVirtualItem, text);
			if (alsoPush)
			{
				StoreEvents.sep.PushEventOnItemPurchased(purchasableVirtualItem, text);
			}
		}

		public void onItemPurchaseStarted(string message)
		{
			this.onItemPurchaseStarted(message, false);
		}

		public void onItemPurchaseStarted(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onItemPurchaseStarted:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			PurchasableVirtualItem purchasableVirtualItem = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			StoreEvents.OnItemPurchaseStarted(purchasableVirtualItem);
			if (alsoPush)
			{
				StoreEvents.sep.PushEventOnItemPurchaseStarted(purchasableVirtualItem);
			}
		}

		public void onMarketPurchaseCancelled(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketPurchaseCancelled: " + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			PurchasableVirtualItem obj = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			StoreEvents.OnMarketPurchaseCancelled(obj);
		}

		public void onMarketPurchaseDeferred(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketPurchaseDeferred: " + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			PurchasableVirtualItem arg = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			string arg2 = string.Empty;
			if (jsonobject.HasField("payload"))
			{
				arg2 = jsonobject["payload"].str;
			}
			StoreEvents.OnMarketPurchaseDeferred(arg, arg2);
		}

		public void onMarketPurchase(string message)
		{
			UnityEngine.Debug.Log("SOOMLA/UNITY onMarketPurchase:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			PurchasableVirtualItem arg = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			string arg2 = string.Empty;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (jsonobject.HasField("payload"))
			{
				arg2 = jsonobject["payload"].str;
			}
			if (jsonobject.HasField("extra"))
			{
				JSONObject jsonobject2 = jsonobject["extra"];
				if (jsonobject2.keys != null)
				{
					foreach (string text in jsonobject2.keys)
					{
						if (jsonobject2[text] != null)
						{
							dictionary.Add(text, jsonobject2[text].str);
						}
					}
				}
			}
			StoreEvents.OnMarketPurchase(arg, arg2, dictionary);
		}

		public void onMarketPurchaseStarted(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketPurchaseStarted: " + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			PurchasableVirtualItem obj = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			StoreEvents.OnMarketPurchaseStarted(obj);
		}

		public void onMarketRefund(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketRefund:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			PurchasableVirtualItem obj = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			StoreEvents.OnMarketRefund(obj);
		}

		public void onRestoreTransactionsFinished(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onRestoreTransactionsFinished:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			bool b = jsonobject["success"].b;
			StoreEvents.OnRestoreTransactionsFinished(b);
		}

		public void onRestoreTransactionsStarted(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onRestoreTransactionsStarted");
			StoreEvents.OnRestoreTransactionsStarted();
		}

		public void onMarketItemsRefreshStarted(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketItemsRefreshStarted");
			StoreEvents.OnMarketItemsRefreshStarted();
		}

		public void onMarketItemsRefreshFailed(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketItemsRefreshFailed");
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			string str = jsonobject["errorMessage"].str;
			StoreEvents.OnMarketItemsRefreshFailed(str);
		}

		public void onMarketItemsRefreshFinished(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketItemsRefreshFinished: " + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			List<VirtualItem> list = new List<VirtualItem>();
			List<MarketItem> list2 = new List<MarketItem>();
			foreach (JSONObject jsonobject2 in jsonobject.list)
			{
				string str = jsonobject2["productId"].str;
				string str2 = jsonobject2["marketPrice"].str;
				string str3 = jsonobject2["marketTitle"].str;
				string str4 = jsonobject2["marketDesc"].str;
				string str5 = jsonobject2["marketCurrencyCode"].str;
				long marketPriceMicros = Convert.ToInt64(jsonobject2["marketPriceMicros"].n);
				try
				{
					PurchasableVirtualItem purchasableItemWithProductId = StoreInfo.GetPurchasableItemWithProductId(str);
					MarketItem marketItem = ((PurchaseWithMarket)purchasableItemWithProductId.PurchaseType).MarketItem;
					marketItem.MarketPriceAndCurrency = str2;
					marketItem.MarketTitle = str3;
					marketItem.MarketDescription = str4;
					marketItem.MarketCurrencyCode = str5;
					marketItem.MarketPriceMicros = marketPriceMicros;
					list2.Add(marketItem);
					list.Add(purchasableItemWithProductId);
				}
				catch (VirtualItemNotFoundException ex)
				{
					SoomlaUtils.LogDebug("SOOMLA StoreEvents", ex.Message);
				}
			}
			if (list.Count > 0)
			{
				StoreInfo.Save(list, false);
			}
			StoreEvents.OnMarketItemsRefreshFinished(list2);
		}

		public void onUnexpectedStoreError(string message)
		{
			this.onUnexpectedStoreError(message, false);
		}

		public void onUnexpectedStoreError(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY OnUnexpectedStoreError");
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			int num = (int)jsonobject["errorCode"].n;
			StoreEvents.OnUnexpectedStoreError(num);
			if (alsoPush)
			{
				StoreEvents.sep.PushEventUnexpectedStoreError(num);
			}
		}

		public void onVerificationStarted(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onVerificationStarted: " + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			PurchasableVirtualItem obj = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jsonobject["itemId"].str);
			StoreEvents.OnVerificationStarted(obj);
		}

		public void onSoomlaStoreInitialized(string message)
		{
			this.onSoomlaStoreInitialized(message, false);
		}

		public void onSoomlaStoreInitialized(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onSoomlaStoreInitialized");
			StoreInventory.RefreshLocalInventory();
			StoreEvents.OnSoomlaStoreInitialized();
			if (alsoPush)
			{
				StoreEvents.sep.PushEventSoomlaStoreInitialized();
			}
		}

		public void onIabServiceStarted(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onIabServiceStarted");
			StoreEvents.OnIabServiceStarted();
		}

		public void onIabServiceStopped(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onIabServiceStopped");
			StoreEvents.OnIabServiceStopped();
		}

		private const string TAG = "SOOMLA StoreEvents";

		public static StoreEvents Instance = null;

		private static StoreEvents.StoreEventPusher sep = null;

		public static StoreEvents.Action OnBillingNotSupported = delegate()
		{
		};

		public static StoreEvents.Action OnBillingSupported = delegate()
		{
		};

		public static Action<VirtualCurrency, int, int> OnCurrencyBalanceChanged = delegate(VirtualCurrency A_0, int A_1, int A_2)
		{
		};

		public static Action<VirtualGood, int, int> OnGoodBalanceChanged = delegate(VirtualGood A_0, int A_1, int A_2)
		{
		};

		public static Action<EquippableVG> OnGoodEquipped = delegate(EquippableVG A_0)
		{
		};

		public static Action<EquippableVG> OnGoodUnEquipped = delegate(EquippableVG A_0)
		{
		};

		public static Action<VirtualGood, UpgradeVG> OnGoodUpgrade = delegate(VirtualGood A_0, UpgradeVG A_1)
		{
		};

		public static Action<PurchasableVirtualItem, string> OnItemPurchased = delegate(PurchasableVirtualItem A_0, string A_1)
		{
		};

		public static Action<PurchasableVirtualItem> OnItemPurchaseStarted = delegate(PurchasableVirtualItem A_0)
		{
		};

		public static Action<PurchasableVirtualItem> OnMarketPurchaseCancelled = delegate(PurchasableVirtualItem A_0)
		{
		};

		public static Action<PurchasableVirtualItem, string> OnMarketPurchaseDeferred = delegate(PurchasableVirtualItem A_0, string A_1)
		{
		};

		public static Action<PurchasableVirtualItem, string, Dictionary<string, string>> OnMarketPurchase = delegate(PurchasableVirtualItem A_0, string A_1, Dictionary<string, string> A_2)
		{
		};

		public static Action<PurchasableVirtualItem> OnMarketPurchaseStarted = delegate(PurchasableVirtualItem A_0)
		{
		};

		public static Action<PurchasableVirtualItem> OnMarketRefund = delegate(PurchasableVirtualItem A_0)
		{
		};

		public static Action<bool> OnRestoreTransactionsFinished = delegate(bool A_0)
		{
		};

		public static StoreEvents.Action OnRestoreTransactionsStarted = delegate()
		{
		};

		public static StoreEvents.Action OnMarketItemsRefreshStarted = delegate()
		{
		};

		public static Action<string> OnMarketItemsRefreshFailed = delegate(string A_0)
		{
		};

		public static Action<List<MarketItem>> OnMarketItemsRefreshFinished = delegate(List<MarketItem> A_0)
		{
		};

		public static Action<int> OnUnexpectedStoreError = delegate(int A_0)
		{
		};

		public static Action<PurchasableVirtualItem> OnVerificationStarted = delegate(PurchasableVirtualItem A_0)
		{
		};

		public static StoreEvents.Action OnSoomlaStoreInitialized = delegate()
		{
		};

		public static StoreEvents.Action OnIabServiceStarted = delegate()
		{
		};

		public static StoreEvents.Action OnIabServiceStopped = delegate()
		{
		};

		public delegate void RunLaterDelegate();

		public delegate void Action();

		public class StoreEventPusher
		{
			public void PushEventSoomlaStoreInitialized()
			{
				this._pushEventSoomlaStoreInitialized(string.Empty);
			}

			public void PushEventUnexpectedStoreError(int errorCode)
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("errorCode", errorCode);
				this._pushEventUnexpectedStoreError(jsonobject.print(false));
			}

			public void PushEventOnCurrencyBalanceChanged(VirtualCurrency currency, int balance, int amountAdded)
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("itemId", currency.ItemId);
				jsonobject.AddField("balance", balance);
				jsonobject.AddField("amountAdded", amountAdded);
				this._pushEventCurrencyBalanceChanged(jsonobject.print(false));
			}

			public void PushEventOnGoodBalanceChanged(VirtualGood good, int balance, int amountAdded)
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("itemId", good.ItemId);
				jsonobject.AddField("balance", balance);
				jsonobject.AddField("amountAdded", amountAdded);
				this._pushEventGoodBalanceChanged(jsonobject.print(false));
			}

			public void PushEventOnGoodEquipped(EquippableVG good)
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("itemId", good.ItemId);
				this._pushEventGoodEquipped(jsonobject.print(false));
			}

			public void PushEventOnGoodUnequipped(EquippableVG good)
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("itemId", good.ItemId);
				this._pushEventGoodUnequipped(jsonobject.print(false));
			}

			public void PushEventOnGoodUpgrade(VirtualGood good, UpgradeVG upgrade)
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("itemId", good.ItemId);
				jsonobject.AddField("upgradeItemId", (!(upgrade == null)) ? upgrade.ItemId : null);
				this._pushEventGoodUpgrade(jsonobject.print(false));
			}

			public void PushEventOnItemPurchased(PurchasableVirtualItem item, string payload)
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("itemId", item.ItemId);
				jsonobject.AddField("payload", payload);
				this._pushEventItemPurchased(jsonobject.print(false));
			}

			public void PushEventOnItemPurchaseStarted(PurchasableVirtualItem item)
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("itemId", item.ItemId);
				this._pushEventItemPurchaseStarted(jsonobject.print(false));
			}

			protected virtual void _pushEventSoomlaStoreInitialized(string message)
			{
			}

			protected virtual void _pushEventUnexpectedStoreError(string message)
			{
			}

			protected virtual void _pushEventCurrencyBalanceChanged(string message)
			{
			}

			protected virtual void _pushEventGoodBalanceChanged(string message)
			{
			}

			protected virtual void _pushEventGoodEquipped(string message)
			{
			}

			protected virtual void _pushEventGoodUnequipped(string message)
			{
			}

			protected virtual void _pushEventGoodUpgrade(string message)
			{
			}

			protected virtual void _pushEventItemPurchased(string message)
			{
			}

			protected virtual void _pushEventItemPurchaseStarted(string message)
			{
			}
		}
	}
}
