using System;
using System.Collections.Generic;
using ACE.IAPS;
using Soomla;
using Soomla.Store;
using UnityEngine;

public class SoomlaStoreProvider : BaseStoreProvider
{
	public string GetMarketItemPriceAndCurrency(string productId)
	{
		Soomla.Store.PurchasableVirtualItem purchasableItemWithProductId = StoreInfo.GetPurchasableItemWithProductId(productId);
		if (purchasableItemWithProductId != null && purchasableItemWithProductId.PurchaseType is Soomla.Store.PurchaseWithMarket)
		{
			return (purchasableItemWithProductId.PurchaseType as Soomla.Store.PurchaseWithMarket).MarketItem.MarketPriceAndCurrency;
		}
		return "???";
	}

	public string GetProductId(string itemId)
	{
		Dictionary<string, Soomla.Store.PurchasableVirtualItem> purchasableItems = StoreInfo.PurchasableItems;
		foreach (KeyValuePair<string, Soomla.Store.PurchasableVirtualItem> keyValuePair in purchasableItems)
		{
			if (keyValuePair.Value != null && keyValuePair.Value.ItemId == itemId)
			{
				Soomla.Store.PurchasableVirtualItem value = keyValuePair.Value;
				if (value.PurchaseType is Soomla.Store.PurchaseWithMarket)
				{
					return (value.PurchaseType as Soomla.Store.PurchaseWithMarket).MarketItem.ProductId;
				}
			}
		}
		return null;
	}

	public override void Initialize(IStoreAssetsConvertable storeAssets, Action callback)
	{
		base.OnInitilizeFinished = (Action)Delegate.Combine(base.OnInitilizeFinished, callback);
		StoreEvents.OnSoomlaStoreInitialized = new StoreEvents.Action(this.Soomla_OnSoomlaStoreInitialized);
		StoreEvents.OnCurrencyBalanceChanged = new Action<Soomla.Store.VirtualCurrency, int, int>(this.Soomla_OnCurrencyBalanceChanged);
		StoreEvents.OnGoodBalanceChanged = new Action<Soomla.Store.VirtualGood, int, int>(this.Soomla_OnGoodBalanceChanged);
		StoreEvents.OnMarketPurchase = new Action<Soomla.Store.PurchasableVirtualItem, string, Dictionary<string, string>>(this.Soomla_OnMarketPurchase);
		StoreEvents.OnItemPurchased = new Action<Soomla.Store.PurchasableVirtualItem, string>(this.Soomla_OnItemPurchased);
		StoreEvents.OnMarketPurchaseCancelled = new Action<Soomla.Store.PurchasableVirtualItem>(this.Soomla_OnItemPurchaseFailed);
		SoomlaStore.Initialize(storeAssets.ConvertTo<SoomlaStoreAssetsFormat>());
	}

	public override void Buy(string itemId, Action<PurchaseResult, string> callback)
	{
		try
		{
			if (base.IsPurchaseInProgress)
			{
				UnityEngine.Debug.LogWarning("A purchase is already in progress. Make sure to call Buy(...) once the other purchase has finished.");
				callback(PurchaseResult.AnotherPurchaseInProgress, itemId);
			}
			else if (this.IsItemAlreadyOwned(itemId))
			{
				UnityEngine.Debug.LogWarning("The item you are trying to purchase is already owned.");
				callback(PurchaseResult.ItemAlreadyOwned, itemId);
			}
			else if (!StoreInventory.CanAfford(itemId))
			{
				UnityEngine.Debug.LogWarning("There is insufficient funds to purchase the requested item");
				callback(PurchaseResult.InsufficientFunds, itemId);
			}
			else
			{
				base.IsPurchaseInProgress = true;
				base.OnPurchaseFinished = callback;
				StoreInventory.BuyItem(itemId);
			}
		}
		catch (VirtualItemNotFoundException ex)
		{
			callback(PurchaseResult.ItemNotFound, itemId);
		}
	}

	public override void GiveItem(string itemId, int amount)
	{
		StoreInventory.GiveItem(itemId, amount);
	}

	public override void TakeItem(string itemId, int amount)
	{
		StoreInventory.TakeItem(itemId, amount);
	}

	public override int GetItemBalance(string itemId)
	{
		return StoreInventory.GetItemBalance(itemId);
	}

	public override bool CanAfford(string itemId)
	{
		return StoreInventory.CanAfford(itemId);
	}

	public override void RestoreTransactions()
	{
		SoomlaStore.RestoreTransactions();
	}

	private bool IsItemAlreadyOwned(string itemId)
	{
		return StoreInfo.GetItemByItemId(itemId) is Soomla.Store.LifetimeVG && StoreInventory.GetItemBalance(itemId) > 0;
	}

	private void Soomla_OnSoomlaStoreInitialized()
	{
		if (base.OnInitilizeFinished != null)
		{
			base.OnInitilizeFinished();
		}
	}

	private void Soomla_OnMarketPurchase(Soomla.Store.PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
	{
		base.IsPurchaseInProgress = false;
		if (base.OnPurchaseFinished != null)
		{
			base.OnPurchaseFinished(PurchaseResult.ItemPurchased, pvi.ItemId);
		}
	}

	private void Soomla_OnItemPurchased(Soomla.Store.PurchasableVirtualItem pvi, string payload)
	{
		base.IsPurchaseInProgress = false;
		if (base.OnPurchaseFinished != null)
		{
			base.OnPurchaseFinished(PurchaseResult.ItemPurchased, pvi.ItemId);
		}
	}

	private void Soomla_OnCurrencyBalanceChanged(Soomla.Store.VirtualCurrency currency, int balance, int amountAdded)
	{
		if (base.OnCurrencyBalanceChanged != null)
		{
			base.OnCurrencyBalanceChanged(currency.ItemId, balance, amountAdded);
		}
	}

	private void Soomla_OnGoodBalanceChanged(Soomla.Store.VirtualGood good, int balance, int amountAdded)
	{
		if (base.OnGoodBalanceChanged != null)
		{
			base.OnGoodBalanceChanged(good.ItemId, balance, amountAdded);
		}
	}

	private void Soomla_OnItemPurchaseFailed(Soomla.Store.PurchasableVirtualItem pvi)
	{
		base.IsPurchaseInProgress = false;
		if (base.OnPurchaseFinished != null)
		{
			base.OnPurchaseFinished(PurchaseResult.FailedForUnknownReason, pvi.ItemId);
		}
	}

	public override void Clear()
	{
		KeyValueStorage.Purge();
	}
}
