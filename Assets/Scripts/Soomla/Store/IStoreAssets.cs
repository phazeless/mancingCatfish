using System;

namespace Soomla.Store
{
	public interface IStoreAssets
	{
		int GetVersion();

		VirtualCurrency[] GetCurrencies();

		VirtualGood[] GetGoods();

		VirtualCurrencyPack[] GetCurrencyPacks();

		VirtualCategory[] GetCategories();
	}
}
