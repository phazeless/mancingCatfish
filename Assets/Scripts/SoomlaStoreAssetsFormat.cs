using System;
using ACE.IAPS;
using Soomla.Store;

public class SoomlaStoreAssetsFormat : HasInternalStoreAssets, Soomla.Store.IStoreAssets
{
	public SoomlaStoreAssetsFormat(ACE.IAPS.IStoreAssets storeAssetsToBeConverted) : base(storeAssetsToBeConverted)
	{
	}

	public VirtualCategory[] GetCategories()
	{
		return new VirtualCategory[0];
	}

	public Soomla.Store.VirtualCurrency[] GetCurrencies()
	{
		ACE.IAPS.VirtualCurrency[] currencies = base.StoreAssets.GetCurrencies();
		int num = currencies.Length;
		Soomla.Store.VirtualCurrency[] array = new Soomla.Store.VirtualCurrency[num];
		for (int i = 0; i < num; i++)
		{
			ACE.IAPS.VirtualCurrency virtualCurrency = currencies[i];
			array[i] = new Soomla.Store.VirtualCurrency(virtualCurrency.Name, virtualCurrency.Description, virtualCurrency.ItemId);
		}
		return array;
	}

	public Soomla.Store.VirtualCurrencyPack[] GetCurrencyPacks()
	{
		ACE.IAPS.VirtualCurrencyPack[] currencyPacks = base.StoreAssets.GetCurrencyPacks();
		int num = currencyPacks.Length;
		Soomla.Store.VirtualCurrencyPack[] array = new Soomla.Store.VirtualCurrencyPack[num];
		for (int i = 0; i < num; i++)
		{
			ACE.IAPS.VirtualCurrencyPack virtualCurrencyPack = currencyPacks[i];
			array[i] = new Soomla.Store.VirtualCurrencyPack(virtualCurrencyPack.Name, virtualCurrencyPack.Description, virtualCurrencyPack.ItemId, virtualCurrencyPack.CurrencyAmount, virtualCurrencyPack.CurrencyItemId, this.ConvertPurchaseType(virtualCurrencyPack.PurchaseType));
		}
		return array;
	}

	public Soomla.Store.VirtualGood[] GetGoods()
	{
		ACE.IAPS.VirtualGood[] goods = base.StoreAssets.GetGoods();
		int num = goods.Length;
		Soomla.Store.VirtualGood[] array = new Soomla.Store.VirtualGood[num];
		for (int i = 0; i < num; i++)
		{
			ACE.IAPS.VirtualGood virtualGood = goods[i];
			Soomla.Store.VirtualGood virtualGood2;
			if (virtualGood is ACE.IAPS.SingleUseVG)
			{
				virtualGood2 = new Soomla.Store.SingleUseVG(virtualGood.Name, virtualGood.Description, virtualGood.ItemId, this.ConvertPurchaseType(virtualGood.PurchaseType));
			}
			else
			{
				if (!(virtualGood is ACE.IAPS.LifetimeVG))
				{
					throw new InvalidOperationException("The VirtualItem of type: '" + virtualGood.GetType() + "' is not supported yet. Make sure to create a ACE.IAPS-version of that type before using an VirtualItem of this type.");
				}
				virtualGood2 = new Soomla.Store.LifetimeVG(virtualGood.Name, virtualGood.Description, virtualGood.ItemId, this.ConvertPurchaseType(virtualGood.PurchaseType));
			}
			array[i] = virtualGood2;
		}
		return array;
	}

	public int GetVersion()
	{
		return base.StoreAssets.GetVersion();
	}

	private Soomla.Store.PurchaseType ConvertPurchaseType(ACE.IAPS.PurchaseType purchaseType)
	{
		Soomla.Store.PurchaseType result = null;
		if (purchaseType is ACE.IAPS.PurchaseWithVirtualItem)
		{
			result = new Soomla.Store.PurchaseWithVirtualItem(purchaseType.TargetItemOrProductId, purchaseType.Amount);
		}
		else if (purchaseType is ACE.IAPS.PurchaseWithMarket)
		{
			result = new Soomla.Store.PurchaseWithMarket(purchaseType.TargetItemOrProductId, (double)purchaseType.Amount);
		}
		return result;
	}
}
