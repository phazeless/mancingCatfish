using System;
using System.Collections.Generic;
using ACE.IAPS;

public class FishStoreAssets : BaseStoreAssetsConvertable
{
	public static bool IsGemCurrencyPack(string itemId)
	{
		return FishStoreAssets.GemTitleFromPacks.ContainsKey(itemId);
	}

	public static bool IsSpecialOffer(string itemId)
	{
		return FishStoreAssets.SpecialOfferTitles.ContainsKey(itemId);
	}

	public static bool IsBoostInShop(string itemId)
	{
		return FishStoreAssets.BoostTitles.ContainsKey(itemId);
	}

	public static int GetGemAmountFromPack(string itemId)
	{
		if (FishStoreAssets.GemAmountsFromPacks.ContainsKey(itemId))
		{
			return FishStoreAssets.GemAmountsFromPacks[itemId];
		}
		return 0;
	}

	public static string GetGemTitleFromPack(string itemId)
	{
		if (FishStoreAssets.GemTitleFromPacks.ContainsKey(itemId))
		{
			return FishStoreAssets.GemTitleFromPacks[itemId];
		}
		return "Unknown";
	}

	public static string GetSpecialOfferTitleFromPack(string itemId)
	{
		if (FishStoreAssets.SpecialOfferTitles.ContainsKey(itemId))
		{
			return FishStoreAssets.SpecialOfferTitles[itemId];
		}
		return "Unknown";
	}

	public static string GetBoostTitle(string itemId)
	{
		if (FishStoreAssets.BoostTitles.ContainsKey(itemId))
		{
			return FishStoreAssets.BoostTitles[itemId];
		}
		return "Unknown";
	}

	public static IAPPlacement GetIAPPlacement(string itemId)
	{
		if (FishStoreAssets.IAPPlacements.ContainsKey(itemId))
		{
			return FishStoreAssets.IAPPlacements[itemId];
		}
		return IAPPlacement.None;
	}

	public override VirtualCurrency[] GetCurrencies()
	{
		return new VirtualCurrency[]
		{
			new VirtualCurrency("se.ace.gem", "Gem Currency", "The Gem Currency used in the game")
		};
	}

	public override VirtualCurrencyPack[] GetCurrencyPacks()
	{
		return this.currencyPacks;
	}

	public override VirtualGood[] GetGoods()
	{
		return new VirtualGood[]
		{
			new SingleUseVG("se.ace.boost_goldfish", "Goldfish Boost", "A boost", new PurchaseWithVirtualItem("se.ace.gem", 5)),
			new SingleUseVG("se.ace.boost_time_0", "Time Boost", "A boost", new PurchaseWithVirtualItem("se.ace.gem", 5)),
			new SingleUseVG("se.ace.boost_boss_fish", "Epic Boost", "A boost", new PurchaseWithVirtualItem("se.ace.gem", 5)),
			new SingleUseVG("se.ace.special_offer_starter", "Special Offer Starter", "A special offer", new PurchaseWithMarket("se.ace.special_offer_starter.sku")),
			new SingleUseVG("se.ace.special_offer_1", "Special Offer 1", "A special offer", new PurchaseWithMarket("se.ace.special_offer_1.sku")),
			new SingleUseVG("se.ace.special_offer_2", "Special Offer 2", "A special offer", new PurchaseWithMarket("se.ace.special_offer_2.sku")),
			new SingleUseVG("se.ace.special_offer_3", "Special Offer 3", "A special offer", new PurchaseWithMarket("se.ace.special_offer_3.sku")),
			new SingleUseVG("se.ace.special_offer_chest_1", "Special Offer Chest 1", "A special offer", new PurchaseWithMarket("se.ace.special_offer_1")),
			new SingleUseVG("se.ace.unlock_crew_member", "Unlock Crew Member", "Unlock Crew Member", new PurchaseWithVirtualItem("se.ace.gem", 0)),
			new SingleUseVG("se.ace.holiday_offer_1", "Holiday Offer 1", "A special offer", new PurchaseWithMarket("se.ace.holiday_offer_1")),
			new SingleUseVG("se.ace.holiday_offer_2", "Holiday Offer 2", "A special offer", new PurchaseWithMarket("se.ace.holiday_offer_2")),
			new SingleUseVG("se.ace.holiday_offer_3", "Holiday Offer 3", "A special offer", new PurchaseWithMarket("se.ace.holiday_offer_3")),
			new SingleUseVG("se.ace.firework_pack_gems", "Firework Pack", "A pack of Fireworks", new PurchaseWithVirtualItem("se.ace.gem", 100))
		};
	}

	public override int GetVersion()
	{
		return 3;
	}

	public const string CURRENCY_ITEM_ID = "se.ace.gem";

	public const string BOOST_GOLDFISH_ITEM_ID = "se.ace.boost_goldfish";

	public const string BOOST_TIME_1_ITEM_ID = "se.ace.boost_time_0";

	public const string BOOST_SPAWNEPIC_ITEMID = "se.ace.boost_boss_fish";

	public const string UNLOCK_CREW_MEMBER = "se.ace.unlock_crew_member";

	public const string CURRENCY_PACK_1_ITEM_ID = "se.ace.gem_pack_1";

	public const string CURRENCY_PACK_2_ITEM_ID = "se.ace.gem_pack_2";

	public const string CURRENCY_PACK_3_ITEM_ID = "se.ace.gem_pack_3";

	public const string CURRENCY_PACK_4_ITEM_ID = "se.ace.gem_pack_4";

	public const string SPECIAL_OFFER_STARTER_ITEM_ID = "se.ace.special_offer_starter";

	public const string SPECIAL_OFFER_1_ITEM_ID = "se.ace.special_offer_1";

	public const string SPECIAL_OFFER_2_ITEM_ID = "se.ace.special_offer_2";

	public const string SPECIAL_OFFER_3_ITEM_ID = "se.ace.special_offer_3";

	public const string SPECIAL_OFFER_CHEST_1_ITEM_ID = "se.ace.special_offer_chest_1";

	public const string HOLIDAY_OFFER_1_ITEM_ID = "se.ace.holiday_offer_1";

	public const string HOLIDAY_OFFER_2_ITEM_ID = "se.ace.holiday_offer_2";

	public const string HOLIDAY_OFFER_3_ITEM_ID = "se.ace.holiday_offer_3";

	public const string FIREWORK_PACK_GEMS_ITEM_ID = "se.ace.firework_pack_gems";

	public const int COST_OF_BOOSTS = 5;

	public const int COST_BOOST_GOLDFISH = 5;

	public const int COST_BOOST_TIME_1 = 5;

	public const int COST_BOOST_SPAWNEPIC = 5;

	public const int AMOUNT_GEM_PACK_1 = 40;

	public const int AMOUNT_GEM_PACK_2 = 300;

	public const int AMOUNT_GEM_PACK_3 = 800;

	public const int AMOUNT_GEM_PACK_4 = 3000;

	private static readonly Dictionary<string, IAPPlacement> IAPPlacements = new Dictionary<string, IAPPlacement>
	{
		{
			"se.ace.gem_pack_1",
			IAPPlacement.GemPack1
		},
		{
			"se.ace.gem_pack_2",
			IAPPlacement.GemPack2
		},
		{
			"se.ace.gem_pack_3",
			IAPPlacement.GemPack3
		},
		{
			"se.ace.gem_pack_4",
			IAPPlacement.GemPackWhale
		},
		{
			"se.ace.special_offer_starter",
			IAPPlacement.SpecialOfferStarter
		},
		{
			"se.ace.special_offer_1",
			IAPPlacement.SpecialOfferSmall
		},
		{
			"se.ace.special_offer_2",
			IAPPlacement.SpecialOfferAwesome
		},
		{
			"se.ace.special_offer_3",
			IAPPlacement.SpecialOfferUltimate
		}
	};

	private static readonly Dictionary<string, int> GemAmountsFromPacks = new Dictionary<string, int>
	{
		{
			"se.ace.gem_pack_1",
			40
		},
		{
			"se.ace.gem_pack_2",
			300
		},
		{
			"se.ace.gem_pack_3",
			800
		},
		{
			"se.ace.gem_pack_4",
			3000
		}
	};

	private static readonly Dictionary<string, string> GemTitleFromPacks = new Dictionary<string, string>
	{
		{
			"se.ace.gem_pack_1",
			"Handful"
		},
		{
			"se.ace.gem_pack_2",
			"Big Pile"
		},
		{
			"se.ace.gem_pack_3",
			"Shipment"
		},
		{
			"se.ace.gem_pack_4",
			"Gem Whale"
		}
	};

	private static readonly Dictionary<string, string> BoostTitles = new Dictionary<string, string>
	{
		{
			"se.ace.boost_goldfish",
			"Gold Fishing"
		},
		{
			"se.ace.boost_time_0",
			"Rush Time"
		},
		{
			"se.ace.boost_boss_fish",
			"Epic Action"
		}
	};

	private static readonly Dictionary<string, string> SpecialOfferTitles = new Dictionary<string, string>
	{
		{
			"se.ace.special_offer_starter",
			"Starter Pack"
		},
		{
			"se.ace.special_offer_1",
			"Special Weekend"
		},
		{
			"se.ace.special_offer_2",
			"Awesome Weekend"
		},
		{
			"se.ace.special_offer_3",
			"Ultra Weekend"
		},
		{
			"se.ace.special_offer_chest_1",
			"Legendary Chest"
		},
		{
			"se.ace.holiday_offer_1",
			"Holiday Offer 1"
		},
		{
			"se.ace.holiday_offer_2",
			"Holiday Offer 2"
		},
		{
			"se.ace.holiday_offer_3",
			"Holiday Offer 3"
		}
	};

	public const string CURRENCY_PACK_1_PRODUCT_ID = "se.ace.gem_pack_1.sku";

	public const string CURRENCY_PACK_2_PRODUCT_ID = "se.ace.gem_pack_2.sku";

	public const string CURRENCY_PACK_3_PRODUCT_ID = "se.ace.gem_pack_3.sku";

	public const string CURRENCY_PACK_4_PRODUCT_ID = "se.ace.gem_pack_4.sku";

	public const string SPECIAL_OFFER_STARTER_PRODUCT_ID = "se.ace.special_offer_starter.sku";

	public const string SPECIAL_OFFER_1_PRODUCT_ID = "se.ace.special_offer_1.sku";

	public const string SPECIAL_OFFER_2_PRODUCT_ID = "se.ace.special_offer_2.sku";

	public const string SPECIAL_OFFER_3_PRODUCT_ID = "se.ace.special_offer_3.sku";

	public const string SPECIAL_OFFER_CHEST_1_PRODUCT_ID = "se.ace.special_offer_1";

	public const string HOLIDAY_OFFER_1_PRODUCT_ID = "se.ace.holiday_offer_1";

	public const string HOLIDAY_OFFER_2_PRODUCT_ID = "se.ace.holiday_offer_2";

	public const string HOLIDAY_OFFER_3_PRODUCT_ID = "se.ace.holiday_offer_3";

	private VirtualCurrencyPack[] currencyPacks = new VirtualCurrencyPack[]
	{
		new VirtualCurrencyPack("se.ace.gem_pack_1", "Gem Pack 1", "A pack of Gems", 40, "se.ace.gem", new PurchaseWithMarket("se.ace.gem_pack_1.sku")),
		new VirtualCurrencyPack("se.ace.gem_pack_2", "Gem Pack 2", "A pack of Gems", 300, "se.ace.gem", new PurchaseWithMarket("se.ace.gem_pack_2.sku")),
		new VirtualCurrencyPack("se.ace.gem_pack_3", "Gem Pack 3", "A pack of Gems", 800, "se.ace.gem", new PurchaseWithMarket("se.ace.gem_pack_3.sku")),
		new VirtualCurrencyPack("se.ace.gem_pack_4", "Gem Pack 4", "A pack of Gems", 3000, "se.ace.gem", new PurchaseWithMarket("se.ace.gem_pack_4.sku"))
	};
}
