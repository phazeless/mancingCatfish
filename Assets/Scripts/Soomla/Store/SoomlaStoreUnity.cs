using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace Soomla.Store
{
	public class SoomlaStoreUnity : SoomlaStore, IStoreListener
	{
		protected override void _loadBillingService()
		{
			StandardPurchasingModule standardPurchasingModule = StandardPurchasingModule.Instance();
			standardPurchasingModule.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
			ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(standardPurchasingModule, new IPurchasingModule[0]);
			configurationBuilder.Configure<IGooglePlayConfiguration>();
			foreach (PurchasableVirtualItem purchasableVirtualItem in StoreInfo.PurchasableItems.Values)
			{
				if (purchasableVirtualItem.PurchaseType is PurchaseWithMarket)
				{
					PurchaseWithMarket purchaseWithMarket = purchasableVirtualItem.PurchaseType as PurchaseWithMarket;
					bool flag = (purchasableVirtualItem is SingleUsePackVG || purchasableVirtualItem is SingleUseVG || purchasableVirtualItem is VirtualCurrencyPack) && !(purchasableVirtualItem is LifetimeVG);
					ProductType type = (!flag) ? ProductType.NonConsumable : ProductType.Consumable;
					configurationBuilder.AddProduct(purchaseWithMarket.MarketItem.ProductId, type);
				}
			}
			UnityPurchasing.Initialize(this, configurationBuilder);
			if (!this.hasInitialized)
			{
				this.hasInitialized = true;
				StoreEvents.Instance.onSoomlaStoreInitialized(string.Empty, true);
				StoreInventory.RefreshLocalInventory();
			}
		}

		protected override void _buyMarketItem(string productId, string payload)
		{
			if (!this.HasUnityStoreModuleInitialized)
			{
				UnityEngine.Debug.LogWarning("Trying to call '_buyMarketItem' item while Unity Store Module has not intialized.  (Possible reason: No Internet Connection)");
				return;
			}
			PurchasableVirtualItem purchasableItemWithProductId = StoreInfo.GetPurchasableItemWithProductId(productId);
			JSONObject jsonobject = new JSONObject();
			if (purchasableItemWithProductId != null)
			{
				jsonobject.AddField("itemId", StoreInfo.GetPurchasableItemWithProductId(productId).ItemId);
				jsonobject.AddField("payload", payload);
			}
			StoreEvents.Instance.onMarketPurchaseStarted(jsonobject.print(false));
			this.unityStoreController.InitiatePurchase(this.unityStoreController.products.WithStoreSpecificID(productId), payload);
		}

		protected override void _restoreTransactions()
		{
			if (!this.HasUnityStoreModuleInitialized)
			{
				UnityEngine.Debug.LogWarning("Trying to call '_restoreTransactions' while Unity Store Module has not intialized. (Possible reason: No Internet Connection)");
				return;
			}
			StoreEvents.Instance.onRestoreTransactionsStarted(string.Empty);
			this._refreshMarketItemsDetails();
		}

		protected override void _refreshInventory()
		{
			this._restoreTransactions();
		}

		protected override void _refreshMarketItemsDetails()
		{
			JSONObject jsonobject = new JSONObject();
			StoreEvents.Instance.onMarketItemsRefreshStarted(jsonobject.print(false));
			if (!this.HasUnityStoreModuleInitialized)
			{
				jsonobject.AddField("errorMessage", new JSONObject("unityStoreController has not been initialized yet! Will try to refresh once it has initialized...", -2, false, false));
				StoreEvents.Instance.onMarketItemsRefreshFailed(jsonobject.print(false));
			}
			else
			{
				Product[] all = this.unityStoreController.products.all;
				foreach (Product product in all)
				{
					foreach (PurchasableVirtualItem purchasableVirtualItem in StoreInfo.PurchasableItems.Values)
					{
						if (purchasableVirtualItem.PurchaseType is PurchaseWithMarket)
						{
							PurchaseWithMarket purchaseWithMarket = purchasableVirtualItem.PurchaseType as PurchaseWithMarket;
							if (purchaseWithMarket.MarketItem.ProductId == product.definition.storeSpecificId)
							{
								purchaseWithMarket.MarketItem.MarketTitle = product.metadata.localizedTitle;
								purchaseWithMarket.MarketItem.MarketDescription = product.metadata.localizedDescription;
								purchaseWithMarket.MarketItem.MarketPriceAndCurrency = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
								purchaseWithMarket.MarketItem.MarketCurrencyCode = product.metadata.isoCurrencyCode;
								jsonobject.Add(purchaseWithMarket.MarketItem.toJSONObject());
							}
						}
					}
				}
				StoreEvents.Instance.onMarketItemsRefreshFinished(jsonobject.print(false));
			}
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			this.unityStoreController = controller;
			this.unityAppleExtensions = extensions.GetExtension<IAppleExtensions>();
			this._refreshInventory();
			StoreInventory.RefreshLocalInventory();
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			UnityEngine.Debug.LogError("Unity IAP failed, error: " + error);
		}

		public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
		{
			PurchasableVirtualItem purchasableItemWithProductId = StoreInfo.GetPurchasableItemWithProductId(i.definition.storeSpecificId);
			JSONObject jsonobject = new JSONObject();
			if (purchasableItemWithProductId != null)
			{
				jsonobject.AddField("itemId", purchasableItemWithProductId.ItemId);
				jsonobject.AddField("payload", string.Empty);
			}
			StoreEvents.Instance.onMarketPurchaseCancelled(jsonobject.print(false));
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			if (this.DetermineIfValidPurchase(e))
			{
				this.AppLovinTrackIAP(e);
				string storeSpecificId = e.purchasedProduct.definition.storeSpecificId;
				PurchasableVirtualItem purchasableItemWithProductId = StoreInfo.GetPurchasableItemWithProductId(storeSpecificId);
				JSONObject jsonobject = new JSONObject();
				jsonobject.AddField("itemId", purchasableItemWithProductId.ItemId);
				jsonobject.AddField("payload", string.Empty);
				purchasableItemWithProductId.Give(1);
				StoreEvents.Instance.onMarketPurchase(jsonobject.print(false));
				try
				{
					string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA1nYbyafW+D3EC66awAtpa+8cpuKI+Ic6+02p7SIYBturPTX/s0nw1opZ6cIiGFlmp7VkT335XaAN5tAP3CcRQNolwXeHFGkSBO7Liv4xTk0n0nVsiNWxjOd8Uz59c+C353cB9uRhN22EgOttmB9QYLxZWUojVnS1cOI8C+05SfCl0HBqa7KZ4EoLtH517GdoKU3/jssMGpapDm61wAxvdNO7d2bA0q9oFdOLziyDYpRT5J7Sms8zs+Q1PNBO9Rmgr71cBWDeo9tnIQZ52Hfj8ZJX7TFPNpzEmu797aOyhrGUVUGZthspcNu5jEFs3KxuqZwsNlIihQbtfLBA0df1NQIDAQAB";
					GoogleReceiptAndSignature googleReceiptAndSignature = new GoogleReceiptAndSignature(e.purchasedProduct.receipt);
					LAnalyticsIAPReceiptDataAndroid receiptData = new LAnalyticsIAPReceiptDataAndroid(storeSpecificId, publicKey, googleReceiptAndSignature.receipt, googleReceiptAndSignature.signature, e.purchasedProduct.metadata.localizedPrice, e.purchasedProduct.metadata.isoCurrencyCode);
					LionAnalytics.Rev_TrackPurchase(receiptData);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.Log("LionAnalytics: Failed to Track Purchase event, error: " + ex.Message);
				}
			}
			return PurchaseProcessingResult.Complete;
		}

		private bool HasUnityStoreModuleInitialized
		{
			get
			{
				return this.unityStoreController != null;
			}
		}

		private void AppLovinTrackIAP(PurchaseEventArgs e)
		{
		}

		private bool DetermineIfValidPurchase(PurchaseEventArgs e)
		{
			bool result = true;
			//CrossPlatformValidator crossPlatformValidator = new CrossPlatformValidator(GooglePlayTangle.Data(), null, Application.identifier);
			//try
			//{
			//	crossPlatformValidator.Validate(e.purchasedProduct.receipt);
			//}
			//catch (IAPSecurityException)
			//{
			//	UnityEngine.Debug.LogWarning("User tried to purchase through cheating.");
			//	result = false;
			//}
			return result;
		}

		private IStoreController unityStoreController;

		private IAppleExtensions unityAppleExtensions;

		private bool hasInitialized;
	}
}
