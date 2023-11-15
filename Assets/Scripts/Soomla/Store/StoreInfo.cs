using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soomla.Store
{
	public class StoreInfo
	{
		private static StoreInfo instance
		{
			get
			{
				return StoreInfo._instance = ((StoreInfo._instance != null) ? StoreInfo._instance : new StoreInfoUnity());
			}
		}

		private static bool assetsArrayHasMarketIdDuplicates(PurchasableVirtualItem[] assetsArray)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (PurchasableVirtualItem purchasableVirtualItem in assetsArray)
			{
				if (purchasableVirtualItem.PurchaseType.GetType() == typeof(PurchaseWithMarket))
				{
					string productId = ((PurchaseWithMarket)purchasableVirtualItem.PurchaseType).MarketItem.ProductId;
					if (hashSet.Contains(productId))
					{
						return false;
					}
					hashSet.Add(productId);
				}
			}
			return true;
		}

		private static void validateStoreAssets(IStoreAssets storeAssets)
		{
			if (storeAssets == null)
			{
				throw new ArgumentException("The given store assets can't be null!");
			}
			if (storeAssets.GetCurrencies() == null || storeAssets.GetCurrencyPacks() == null || storeAssets.GetGoods() == null || storeAssets.GetCategories() == null)
			{
				throw new ArgumentException("All IStoreAssets methods shouldn't return NULL-pointer references!");
			}
			if (!StoreInfo.assetsArrayHasMarketIdDuplicates(storeAssets.GetGoods()) || !StoreInfo.assetsArrayHasMarketIdDuplicates(storeAssets.GetCurrencyPacks()))
			{
				throw new ArgumentException("The given store assets has duplicates at marketItem productId!");
			}
		}

		public static void SetStoreAssets(IStoreAssets storeAssets)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Setting store assets in SoomlaInfo");
			try
			{
				StoreInfo.validateStoreAssets(storeAssets);
				StoreInfo.instance._setStoreAssets(storeAssets);
				StoreInfo.initializeFromDB();
			}
			catch (ArgumentException ex)
			{
				SoomlaUtils.LogError("SOOMLA/UNITY StoreInfo", ex.Message);
			}
		}

		public static VirtualItem GetItemByItemId(string itemId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch an item with itemId: " + itemId);
			VirtualItem result;
			if (StoreInfo.VirtualItems != null && StoreInfo.VirtualItems.TryGetValue(itemId, out result))
			{
				return result;
			}
			throw new VirtualItemNotFoundException("itemId", itemId);
		}

		public static PurchasableVirtualItem GetPurchasableItemWithProductId(string productId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch a purchasable item with productId: " + productId);
			PurchasableVirtualItem result;
			if (StoreInfo.PurchasableItems != null && StoreInfo.PurchasableItems.TryGetValue(productId, out result))
			{
				return result;
			}
			throw new VirtualItemNotFoundException("productId", productId);
		}

		public static VirtualCategory GetCategoryForVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch a category for a good with itemId: " + goodItemId);
			VirtualCategory result;
			if (StoreInfo.GoodsCategories != null && StoreInfo.GoodsCategories.TryGetValue(goodItemId, out result))
			{
				return result;
			}
			throw new VirtualItemNotFoundException("goodItemId of category", goodItemId);
		}

		public static UpgradeVG GetFirstUpgradeForVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch first upgrade of a good with itemId: " + goodItemId);
			List<UpgradeVG> source;
			if (StoreInfo.GoodsUpgrades != null && StoreInfo.GoodsUpgrades.TryGetValue(goodItemId, out source))
			{
				return source.FirstOrDefault((UpgradeVG up) => string.IsNullOrEmpty(up.PrevItemId));
			}
			return null;
		}

		public static UpgradeVG GetLastUpgradeForVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch last upgrade of a good with itemId: " + goodItemId);
			List<UpgradeVG> source;
			if (StoreInfo.GoodsUpgrades != null && StoreInfo.GoodsUpgrades.TryGetValue(goodItemId, out source))
			{
				return source.FirstOrDefault((UpgradeVG up) => string.IsNullOrEmpty(up.NextItemId));
			}
			return null;
		}

		public static List<UpgradeVG> GetUpgradesForVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch upgrades of a good with itemId: " + goodItemId);
			List<UpgradeVG> result;
			if (StoreInfo.GoodsUpgrades != null && StoreInfo.GoodsUpgrades.TryGetValue(goodItemId, out result))
			{
				return result;
			}
			return null;
		}

		public static void Save()
		{
			string text = StoreInfo.toJSONObject().print(false);
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "saving StoreInfo to DB. json is: " + text);
			string key = StoreInfo.keyMetaStoreInfo();
			KeyValueStorage.SetValue(key, text);
			StoreInfo.instance.loadNativeFromDB();
		}

		public static void Save(VirtualItem virtualItem, bool saveToDB = true)
		{
			StoreInfo.replaceVirtualItem(virtualItem);
			if (saveToDB)
			{
				StoreInfo.Save();
			}
		}

		public static void Save(List<VirtualItem> virtualItems, bool saveToDB = true)
		{
			if (virtualItems == null && virtualItems.Count == 0)
			{
				return;
			}
			foreach (VirtualItem virtualItem in virtualItems)
			{
				StoreInfo.replaceVirtualItem(virtualItem);
			}
			if (saveToDB)
			{
				StoreInfo.Save();
			}
		}

		protected virtual void _setStoreAssets(IStoreAssets storeAssets)
		{
		}

		protected virtual void loadNativeFromDB()
		{
		}

		protected static string IStoreAssetsToJSON(IStoreAssets storeAssets)
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualCurrency virtualCurrency in storeAssets.GetCurrencies())
			{
				jsonobject.Add(virtualCurrency.toJSONObject());
			}
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualCurrencyPack virtualCurrencyPack in storeAssets.GetCurrencyPacks())
			{
				jsonobject2.Add(virtualCurrencyPack.toJSONObject());
			}
			JSONObject jsonobject3 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject7 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualGood virtualGood in storeAssets.GetGoods())
			{
				if (virtualGood is SingleUseVG)
				{
					jsonobject3.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is EquippableVG)
				{
					jsonobject5.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is UpgradeVG)
				{
					jsonobject6.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is LifetimeVG)
				{
					jsonobject4.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is SingleUsePackVG)
				{
					jsonobject7.Add(virtualGood.toJSONObject());
				}
			}
			JSONObject jsonobject8 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject8.AddField("singleUse", jsonobject3);
			jsonobject8.AddField("lifetime", jsonobject4);
			jsonobject8.AddField("equippable", jsonobject5);
			jsonobject8.AddField("goodUpgrades", jsonobject6);
			jsonobject8.AddField("goodPacks", jsonobject7);
			JSONObject jsonobject9 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualCategory virtualCategory in storeAssets.GetCategories())
			{
				jsonobject9.Add(virtualCategory.toJSONObject());
			}
			JSONObject jsonobject10 = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject10.AddField("categories", jsonobject9);
			jsonobject10.AddField("currencies", jsonobject);
			jsonobject10.AddField("currencyPacks", jsonobject2);
			jsonobject10.AddField("goods", jsonobject8);
			return jsonobject10.print(false);
		}

		private static void initializeFromDB()
		{
			string key = StoreInfo.keyMetaStoreInfo();
			string value = KeyValueStorage.GetValue(key);
			if (string.IsNullOrEmpty(value))
			{
				SoomlaUtils.LogError("SOOMLA/UNITY StoreInfo", "store json is not in DB. Make sure you initialized SoomlaStore with your Store assets. The App will shut down now.");
				Application.Quit();
			}
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "the metadata-economy json (from DB) is " + value);
			JSONObject storeJSON = new JSONObject(value, -2, false, false);
			StoreInfo.fromJSONObject(storeJSON);
		}

		private static void fromJSONObject(JSONObject storeJSON)
		{
			StoreInfo.VirtualItems = new Dictionary<string, VirtualItem>();
			StoreInfo.PurchasableItems = new Dictionary<string, PurchasableVirtualItem>();
			StoreInfo.GoodsCategories = new Dictionary<string, VirtualCategory>();
			StoreInfo.GoodsUpgrades = new Dictionary<string, List<UpgradeVG>>();
			StoreInfo.CurrencyPacks = new List<VirtualCurrencyPack>();
			StoreInfo.Goods = new List<VirtualGood>();
			StoreInfo.Categories = new List<VirtualCategory>();
			StoreInfo.Currencies = new List<VirtualCurrency>();
			if (storeJSON.HasField("currencies"))
			{
				List<JSONObject> list = storeJSON["currencies"].list;
				foreach (JSONObject jsonVc in list)
				{
					VirtualCurrency item = new VirtualCurrency(jsonVc);
					StoreInfo.Currencies.Add(item);
				}
			}
			if (storeJSON.HasField("currencyPacks"))
			{
				List<JSONObject> list2 = storeJSON["currencyPacks"].list;
				foreach (JSONObject jsonItem in list2)
				{
					VirtualCurrencyPack item2 = new VirtualCurrencyPack(jsonItem);
					StoreInfo.CurrencyPacks.Add(item2);
				}
			}
			if (storeJSON.HasField("goods"))
			{
				JSONObject jsonobject = storeJSON["goods"];
				if (jsonobject.HasField("singleUse"))
				{
					List<JSONObject> list3 = jsonobject["singleUse"].list;
					foreach (JSONObject jsonVg in list3)
					{
						SingleUseVG item3 = new SingleUseVG(jsonVg);
						StoreInfo.Goods.Add(item3);
					}
				}
				if (jsonobject.HasField("lifetime"))
				{
					List<JSONObject> list4 = jsonobject["lifetime"].list;
					foreach (JSONObject jsonVg2 in list4)
					{
						LifetimeVG item4 = new LifetimeVG(jsonVg2);
						StoreInfo.Goods.Add(item4);
					}
				}
				if (jsonobject.HasField("equippable"))
				{
					List<JSONObject> list5 = jsonobject["equippable"].list;
					foreach (JSONObject jsonItem2 in list5)
					{
						EquippableVG item5 = new EquippableVG(jsonItem2);
						StoreInfo.Goods.Add(item5);
					}
				}
				if (jsonobject.HasField("goodPacks"))
				{
					List<JSONObject> list6 = jsonobject["goodPacks"].list;
					foreach (JSONObject jsonItem3 in list6)
					{
						SingleUsePackVG item6 = new SingleUsePackVG(jsonItem3);
						StoreInfo.Goods.Add(item6);
					}
				}
				if (jsonobject.HasField("goodUpgrades"))
				{
					List<JSONObject> list7 = jsonobject["goodUpgrades"].list;
					foreach (JSONObject jsonItem4 in list7)
					{
						UpgradeVG item7 = new UpgradeVG(jsonItem4);
						StoreInfo.Goods.Add(item7);
					}
				}
			}
			if (storeJSON.HasField("categories"))
			{
				List<JSONObject> list8 = storeJSON["categories"].list;
				foreach (JSONObject jsonItem5 in list8)
				{
					VirtualCategory item8 = new VirtualCategory(jsonItem5);
					StoreInfo.Categories.Add(item8);
				}
			}
			StoreInfo.updateAggregatedLists();
		}

		private static void updateAggregatedLists()
		{
			foreach (VirtualCurrency virtualCurrency in StoreInfo.Currencies)
			{
				StoreInfo.VirtualItems.AddOrUpdate(virtualCurrency.ItemId, virtualCurrency);
			}
			foreach (VirtualCurrencyPack virtualCurrencyPack in StoreInfo.CurrencyPacks)
			{
				StoreInfo.VirtualItems.AddOrUpdate(virtualCurrencyPack.ItemId, virtualCurrencyPack);
				PurchaseType purchaseType = virtualCurrencyPack.PurchaseType;
				if (purchaseType is PurchaseWithMarket)
				{
					StoreInfo.PurchasableItems.AddOrUpdate(((PurchaseWithMarket)purchaseType).MarketItem.ProductId, virtualCurrencyPack);
				}
			}
			foreach (VirtualGood virtualGood in StoreInfo.Goods)
			{
				StoreInfo.VirtualItems.AddOrUpdate(virtualGood.ItemId, virtualGood);
				if (virtualGood is UpgradeVG)
				{
					List<UpgradeVG> list;
					if (!StoreInfo.GoodsUpgrades.TryGetValue(((UpgradeVG)virtualGood).GoodItemId, out list))
					{
						list = new List<UpgradeVG>();
						StoreInfo.GoodsUpgrades.Add(((UpgradeVG)virtualGood).GoodItemId, list);
					}
					list.Add((UpgradeVG)virtualGood);
				}
				PurchaseType purchaseType2 = virtualGood.PurchaseType;
				if (purchaseType2 is PurchaseWithMarket)
				{
					StoreInfo.PurchasableItems.AddOrUpdate(((PurchaseWithMarket)purchaseType2).MarketItem.ProductId, virtualGood);
				}
			}
			foreach (VirtualCategory virtualCategory in StoreInfo.Categories)
			{
				foreach (string key in virtualCategory.GoodItemIds)
				{
					StoreInfo.GoodsCategories.AddOrUpdate(key, virtualCategory);
				}
			}
		}

		private static void replaceVirtualItem(VirtualItem virtualItem)
		{
			StoreInfo.VirtualItems.AddOrUpdate(virtualItem.ItemId, virtualItem);
			if (virtualItem is VirtualCurrency)
			{
				for (int i = 0; i < StoreInfo.Currencies.Count<VirtualCurrency>(); i++)
				{
					if (StoreInfo.Currencies[i].ItemId == virtualItem.ItemId)
					{
						StoreInfo.Currencies.RemoveAt(i);
						break;
					}
				}
				StoreInfo.Currencies.Add((VirtualCurrency)virtualItem);
			}
			if (virtualItem is VirtualCurrencyPack)
			{
				VirtualCurrencyPack virtualCurrencyPack = (VirtualCurrencyPack)virtualItem;
				if (virtualCurrencyPack.PurchaseType is PurchaseWithMarket)
				{
					StoreInfo.PurchasableItems.AddOrUpdate(((PurchaseWithMarket)virtualCurrencyPack.PurchaseType).MarketItem.ProductId, virtualCurrencyPack);
				}
				for (int j = 0; j < StoreInfo.CurrencyPacks.Count<VirtualCurrencyPack>(); j++)
				{
					if (StoreInfo.CurrencyPacks[j].ItemId == virtualCurrencyPack.ItemId)
					{
						StoreInfo.CurrencyPacks.RemoveAt(j);
						break;
					}
				}
				StoreInfo.CurrencyPacks.Add(virtualCurrencyPack);
			}
			if (virtualItem is VirtualGood)
			{
				VirtualGood virtualGood = (VirtualGood)virtualItem;
				if (virtualGood is UpgradeVG)
				{
					List<UpgradeVG> list;
					if (!StoreInfo.GoodsUpgrades.TryGetValue(((UpgradeVG)virtualGood).GoodItemId, out list))
					{
						list = new List<UpgradeVG>();
						StoreInfo.GoodsUpgrades.Add(((UpgradeVG)virtualGood).ItemId, list);
					}
					list.Add((UpgradeVG)virtualGood);
				}
				if (virtualGood.PurchaseType is PurchaseWithMarket)
				{
					StoreInfo.PurchasableItems.AddOrUpdate(((PurchaseWithMarket)virtualGood.PurchaseType).MarketItem.ProductId, virtualGood);
				}
				for (int k = 0; k < StoreInfo.Goods.Count<VirtualGood>(); k++)
				{
					if (StoreInfo.Goods[k].ItemId == virtualGood.ItemId)
					{
						StoreInfo.Goods.RemoveAt(k);
						break;
					}
				}
				StoreInfo.Goods.Add(virtualGood);
			}
		}

		private static JSONObject toJSONObject()
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualCurrency virtualCurrency in StoreInfo.Currencies)
			{
				jsonobject.Add(virtualCurrency.toJSONObject());
			}
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualCurrencyPack virtualCurrencyPack in StoreInfo.CurrencyPacks)
			{
				jsonobject2.Add(virtualCurrencyPack.toJSONObject());
			}
			JSONObject jsonobject3 = new JSONObject();
			JSONObject jsonobject4 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject5 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject6 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject7 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jsonobject8 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualGood virtualGood in StoreInfo.Goods)
			{
				if (virtualGood is SingleUseVG)
				{
					jsonobject4.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is UpgradeVG)
				{
					jsonobject8.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is EquippableVG)
				{
					jsonobject6.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is SingleUsePackVG)
				{
					jsonobject7.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is LifetimeVG)
				{
					jsonobject5.Add(virtualGood.toJSONObject());
				}
			}
			JSONObject jsonobject9 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualCategory virtualCategory in StoreInfo.Categories)
			{
				jsonobject9.Add(virtualCategory.toJSONObject());
			}
			JSONObject jsonobject10 = new JSONObject();
			jsonobject3.AddField("singleUse", jsonobject4);
			jsonobject3.AddField("lifetime", jsonobject5);
			jsonobject3.AddField("equippable", jsonobject6);
			jsonobject3.AddField("goodPacks", jsonobject7);
			jsonobject3.AddField("goodUpgrades", jsonobject8);
			jsonobject10.AddField("categories", jsonobject9);
			jsonobject10.AddField("currencies", jsonobject);
			jsonobject10.AddField("goods", jsonobject3);
			jsonobject10.AddField("currencyPacks", jsonobject2);
			return jsonobject10;
		}

		private static string keyMetaStoreInfo()
		{
			return "meta.storeinfo";
		}

		protected const string TAG = "SOOMLA/UNITY StoreInfo";

		private static StoreInfo _instance = null;

		public static Dictionary<string, VirtualItem> VirtualItems = new Dictionary<string, VirtualItem>();

		public static Dictionary<string, PurchasableVirtualItem> PurchasableItems = new Dictionary<string, PurchasableVirtualItem>();

		public static Dictionary<string, VirtualCategory> GoodsCategories = new Dictionary<string, VirtualCategory>();

		public static Dictionary<string, List<UpgradeVG>> GoodsUpgrades = new Dictionary<string, List<UpgradeVG>>();

		public static List<VirtualCurrency> Currencies = new List<VirtualCurrency>();

		public static List<VirtualCurrencyPack> CurrencyPacks = new List<VirtualCurrencyPack>();

		public static List<VirtualGood> Goods = new List<VirtualGood>();

		public static List<VirtualCategory> Categories = new List<VirtualCategory>();
	}
}
