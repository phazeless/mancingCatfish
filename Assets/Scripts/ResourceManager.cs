using System;
using System.Diagnostics;
using System.Numerics;
using ACE.IAPS;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
	public static ResourceManager Instance { get; private set; }

	public static StoreManager StoreManager
	{
		get
		{
			return ResourceManager.storeManager;
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<ResourceType, BigInteger, BigInteger> OnResourceChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<PurchaseResult, string, ResourceChangeReason> OnPurchaseResponse;

	public bool IsInitialized { get; private set; }

	private void Awake()
	{
		ResourceManager.Instance = this;
		SkillManager.Instance.OnSkillLevelChanged += this.OnSkillLevelChanged;
		ItemManager.Instance.OnItemLevelChanged += this.Instance_OnItemLevelChanged;
		AFKManager.Instance.OnAFKCashCollected += this.OnAFKCashCollected;
		this.LoadSavedCash(false);
		StoreManager storeManager = ResourceManager.storeManager;
		storeManager.OnCurrencyBalanceChanged = (Action<string, int, int>)Delegate.Combine(storeManager.OnCurrencyBalanceChanged, new Action<string, int, int>(delegate(string itemId, int balance, int amountAdded)
		{
			if (this.IsGemItemId(itemId))
			{
				this.NotifyResourceChanged(ResourceType.Gems, amountAdded, balance);
			}
		}));
		this.storeAssets = new FishStoreAssets();
		ResourceManager.storeManager.Initialize(this.storeAssets, delegate
		{
			this.IsInitialized = true;
			ResourceChangeData gemChangeData = new ResourceChangeData("contentId_onInitialize_0", "On Initialize For Testing 0", this.startingGems, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.ForTesting);
			this.GiveGems(this.startingGems, gemChangeData);
			this.NotifyResourceChanged(ResourceType.Gems, 0, this.GetResourceAmount(ResourceType.Gems));
		});
		this.NotifyResourceChanged(ResourceType.Cash, 0, this.GetResourceAmount(ResourceType.Cash));
	}

	private void JackpotSkill_OnSkillActivation(Skill skill)
	{
	}

	private void Start()
	{
		BaseCatcher.OnFishCollected += this.OnFishCollected;
		if (!this.hasReceivedStartingResources)
		{
			this.freeSpinSkill.SetCurrentLevel(1, LevelChange.Initialization);
			this.hasReceivedStartingResources = true;
		}
	}

	private void OnAFKCashCollected(BigInteger amount, bool didDoubleUp)
	{
		this.GiveResource(ResourceType.Cash, amount);
	}

	private bool HasEnoughCash(Skill skill)
	{
		return this.availableCash >= skill.CostForCurrentLevelUp;
	}

	private void OnFishCollected(FishBehaviour fish)
	{
		this.GiveResource(ResourceType.Cash, fish.GetValue(false));
	}

	private void Instance_OnItemLevelChanged(Item item, LevelChange change, int itemAmountSpent, int gemCost)
	{
		if (change == LevelChange.LevelUp)
		{
			ResourceChangeData changeData = new ResourceChangeData(item.Id, item.Title, item.TotalGemsRequiredForCurrentLevel, ResourceType.Gems, ResourceChangeType.Spend, ResourceChangeReason.UpgradeItem);
			this.TakeGems(item.TotalGemsRequiredForCurrentLevel, changeData);
		}
	}

	private void OnSkillLevelChanged(Skill skill, LevelChange change)
	{
		if (change == LevelChange.LevelUp)
		{
			ResourceType purchaseWith = skill.GetExtraInfo().PurchaseWith;
			if (purchaseWith == ResourceType.Gems)
			{
				int amount = (int)skill.CostForCurrentLevelUp;
				if (skill.GetExtraInfo().IsCrew)
				{
					ResourceChangeData changeData = new ResourceChangeData(skill.Id, skill.GetExtraInfo().TitleText, amount, purchaseWith, ResourceChangeType.Spend, ResourceChangeReason.UpgradeCrew);
					this.TakeGems(amount, changeData);
				}
				else
				{
					ResourceChangeData changeData2 = new ResourceChangeData();
					if (this.IsUnlockCrewMemberSkill(skill.Id))
					{
						changeData2 = new ResourceChangeData(skill.Id, skill.GetExtraInfo().TitleText, amount, purchaseWith, ResourceChangeType.Spend, ResourceChangeReason.UnlockRandomCrew);
					}
					this.TakeGems(amount, changeData2);
				}
			}
			else if (purchaseWith == ResourceType.SkillPoints)
			{
				int amount2 = (int)skill.CostForCurrentLevelUp;
				ResourceChangeData changeData3 = new ResourceChangeData(skill.Id, skill.GetExtraInfo().TitleText, amount2, purchaseWith, ResourceChangeType.Spend, ResourceChangeReason.UpgradeSkillTreeSkill);
				this.TakeSkillPoints(amount2, changeData3);
			}
			else if (purchaseWith == ResourceType.CrownExp)
			{
				int amount3 = (int)skill.CostForCurrentLevelUp;
				ResourceChangeData changeData4 = new ResourceChangeData(skill.Id, skill.GetExtraInfo().TitleText, amount3, purchaseWith, ResourceChangeType.Spend, ResourceChangeReason.CrownLevelIncreased);
				this.TakeCrownExp(amount3, changeData4);
			}
			else
			{
				BigInteger costForCurrentLevelUp = skill.CostForCurrentLevelUp;
				this.TakeResource(skill.GetExtraInfo().PurchaseWith, costForCurrentLevelUp);
			}
		}
		else if (change == LevelChange.HardReset && skill.GetExtraInfo().PurchaseWith == ResourceType.Cash)
		{
			this.TakeResource(ResourceType.Cash, this.availableCash);
		}
	}

	public string GetProductId(string itemId)
	{
		return ResourceManager.soomlaStoreProvider.GetProductId(itemId);
	}

	public string GetMarketItemPriceAndCurrency(string productId)
	{
		return ResourceManager.soomlaStoreProvider.GetMarketItemPriceAndCurrency(productId);
	}

	public void GiveFreeSpin()
	{
		this.freeSpinSkill.SetCurrentLevel(1, LevelChange.Initialization);
	}

	public void GiveGems(BigInteger amount, ResourceChangeData gemChangeData)
	{
		this.GiveResource(ResourceType.Gems, amount);
		this.TrackEvent(gemChangeData);
	}

	public void GiveSkillPoints(int amount, ResourceChangeData gemChangeData)
	{
		this.GiveResource(ResourceType.SkillPoints, amount);
		this.TrackEvent(gemChangeData);
	}

	public void GiveCrownExp(int amount, ResourceChangeData gemChangeData)
	{
		this.GiveResource(ResourceType.CrownExp, amount);
		this.TrackEvent(gemChangeData);
	}

	public void GiveResource(ResourceType resourceType, BigInteger amount)
	{
		if (amount < 0L)
		{
			return;
		}
		if (resourceType == ResourceType.Cash)
		{
			this.availableCash += amount;
		}
		else if (resourceType == ResourceType.Gems)
		{
			ResourceManager.storeManager.GiveItem("se.ace.gem", (int)amount);
		}
		else if (resourceType == ResourceType.SkillPoints)
		{
			this.availableSkillPoints += (int)amount;
		}
		else if (resourceType == ResourceType.CrownExp)
		{
			this.availableCrownExp += (int)amount;
		}
		if (amount > 0L)
		{
			this.NotifyResourceChanged(resourceType, amount, this.GetResourceAmount(resourceType));
		}
	}

	public bool TakeGems(int amount, ResourceChangeData changeData)
	{
		bool flag = this.TakeResource(ResourceType.Gems, amount);
		if (flag)
		{
			this.TrackEvent(changeData);
		}
		return flag;
	}

	public bool TakeSkillPoints(int amount, ResourceChangeData changeData)
	{
		bool flag = this.TakeResource(ResourceType.SkillPoints, amount);
		if (flag)
		{
			this.TrackEvent(changeData);
		}
		return flag;
	}

	public bool TakeCrownExp(int amount, ResourceChangeData changeData)
	{
		bool flag = this.TakeResource(ResourceType.CrownExp, amount);
		if (flag)
		{
			this.TrackEvent(changeData);
		}
		return flag;
	}

	public bool TakeResource(ResourceType resourceType, BigInteger amount)
	{
		if (amount <= 0L)
		{
			return true;
		}
		BigInteger bigInteger = this.GetResourceAmount(resourceType) - amount;
		if (bigInteger < 0L)
		{
			return false;
		}
		if (resourceType == ResourceType.Cash)
		{
			this.availableCash = bigInteger;
		}
		else if (resourceType == ResourceType.Gems)
		{
			ResourceManager.storeManager.TakeItem("se.ace.gem", (int)amount);
		}
		else if (resourceType == ResourceType.SkillPoints)
		{
			this.availableSkillPoints = (int)bigInteger;
		}
		else if (resourceType == ResourceType.CrownExp)
		{
			this.availableCrownExp = (int)bigInteger;
		}
		this.NotifyResourceChanged(resourceType, amount, this.GetResourceAmount(resourceType));
		return true;
	}

	public BigInteger GetResourceAmount(ResourceType resourceType)
	{
		if (resourceType == ResourceType.Cash)
		{
			return this.availableCash;
		}
		if (resourceType == ResourceType.Gems)
		{
			return ResourceManager.storeManager.GetItemBalance("se.ace.gem");
		}
		if (resourceType == ResourceType.SkillPoints)
		{
			return this.availableSkillPoints;
		}
		if (resourceType == ResourceType.CrownExp)
		{
			return this.availableCrownExp;
		}
		return 0;
	}

	public int GetFreeSpinAmount()
	{
		return this.freeSpinSkill.CurrentLevel;
	}

	public bool SuggestGemPackIfNotEnoughGems(int gemCostForPurchase)
	{
		string text = this.DetermineGemPackNeededForGemPurchase(gemCostForPurchase);
		if (text != null)
		{
			UIIAPPendingBlocker.Instance.Show();
			ResourceManager.Instance.Buy(text, delegate(PurchaseResult resp, string metaData)
			{
				UIIAPPendingBlocker.Instance.Hide();
			});
			return true;
		}
		return false;
	}

	public string DetermineGemPackNeededForGemPurchase(int gemCostForPurchase)
	{
		BigInteger left = gemCostForPurchase - this.GetResourceAmount(ResourceType.Gems);
		if (left > 800L)
		{
			return "se.ace.gem_pack_4";
		}
		if (left > 300L)
		{
			return "se.ace.gem_pack_3";
		}
		if (left > 40L)
		{
			return "se.ace.gem_pack_2";
		}
		if (left > 0L)
		{
			return "se.ace.gem_pack_1";
		}
		return null;
	}

	private bool IsGemItemId(string itemId)
	{
		return itemId == "se.ace.gem";
	}

	private bool IsUnlockCrewMemberSkill(string skillId)
	{
		return "c6b7d419-0885-420b-bf9f-4ed0dda7c56d" == skillId;
	}

	public bool IsMarketItem(string itemId)
	{
		return itemId == "se.ace.gem_pack_1" || itemId == "se.ace.gem_pack_2" || itemId == "se.ace.gem_pack_3" || itemId == "se.ace.gem_pack_4" || itemId == "se.ace.special_offer_starter" || itemId == "se.ace.special_offer_1" || itemId == "se.ace.special_offer_2" || itemId == "se.ace.special_offer_3" || itemId == "se.ace.holiday_offer_1" || itemId == "se.ace.holiday_offer_2" || itemId == "se.ace.holiday_offer_3";
	}

	public void BuyWithGems(HolidayOffer offer, Action<PurchaseResult, string> callback)
	{
		string itemId = offer.ItemId;
		int gemCost = offer.GemCost;
		ResourceChangeReason resourceChangeReason = this.GetResourceChangeReason(itemId);
		string contentNameForItemId = this.GetContentNameForItemId(resourceChangeReason, itemId);
		ResourceChangeData changeData = new ResourceChangeData(itemId, contentNameForItemId, gemCost, ResourceType.Gems, ResourceChangeType.Spend, resourceChangeReason);
		if (this.TakeGems(gemCost, changeData))
		{
			if (callback != null)
			{
				callback(PurchaseResult.ItemPurchased, null);
			}
		}
		else if (callback != null)
		{
			callback(PurchaseResult.InsufficientFunds, null);
		}
	}

	public void Buy(string itemId, Action<PurchaseResult, string> callback)
	{
		ResourceManager.storeManager.Buy(itemId, delegate(PurchaseResult response, string b)
		{
			if (callback != null)
			{
				callback(response, b);
			}
			if (this.OnPurchaseResponse != null)
			{
				this.OnPurchaseResponse(response, itemId, this.GetResourceChangeReason(itemId));
			}
			if (response == PurchaseResult.ItemPurchased)
			{
				ResourceChangeReason resourceChangeReason = this.GetResourceChangeReason(itemId);
				string contentNameForItemId = this.GetContentNameForItemId(resourceChangeReason, itemId);
				if (resourceChangeReason == ResourceChangeReason.PurchaseGemPack)
				{
					int gemAmountFromPack = FishStoreAssets.GetGemAmountFromPack(itemId);
					ResourceChangeData changeData = new ResourceChangeData(itemId, contentNameForItemId, gemAmountFromPack, ResourceType.Gems, ResourceChangeType.Earn, resourceChangeReason);
					this.TrackEvent(changeData);
					ResourceChangeData changeData2 = new ResourceChangeData(itemId, contentNameForItemId, 0, ResourceType.CrownExp, ResourceChangeType.Earn, resourceChangeReason);
					CrownExpGranterManager.Instance.Grant(FishStoreAssets.GetIAPPlacement(itemId), changeData2);
				}
				else if (resourceChangeReason == ResourceChangeReason.PurchaseBoostInShop)
				{
					ResourceChangeData changeData3 = new ResourceChangeData(itemId, contentNameForItemId, 5, ResourceType.Gems, ResourceChangeType.Spend, resourceChangeReason);
					this.TrackEvent(changeData3);
				}
				if (this.IsMarketItem(itemId))
				{
					CloudOnceManager.Instance.SaveDataToCache();
				}
			}
		});
	}

	private string GetContentNameForItemId(ResourceChangeReason reason, string itemId)
	{
		if (reason == ResourceChangeReason.PurchaseGemPack)
		{
			return FishStoreAssets.GetGemTitleFromPack(itemId);
		}
		if (reason == ResourceChangeReason.PurchaseSpecialOffer)
		{
			return FishStoreAssets.GetSpecialOfferTitleFromPack(itemId);
		}
		if (reason == ResourceChangeReason.PurchaseBoostInShop)
		{
			return FishStoreAssets.GetBoostTitle(itemId);
		}
		return "Unknown";
	}

	private ResourceChangeReason GetResourceChangeReason(string itemId)
	{
		if (FishStoreAssets.IsGemCurrencyPack(itemId))
		{
			return ResourceChangeReason.PurchaseGemPack;
		}
		if (FishStoreAssets.IsSpecialOffer(itemId))
		{
			return ResourceChangeReason.PurchaseSpecialOffer;
		}
		if (FishStoreAssets.IsBoostInShop(itemId))
		{
			return ResourceChangeReason.PurchaseBoostInShop;
		}
		return ResourceChangeReason.Unknown;
	}

	private void NotifyResourceChanged(ResourceType resourceType, BigInteger amountAdded, BigInteger totalAmount)
	{
		if (resourceType == ResourceType.Cash)
		{
			string str = null;
			string text = null;
			CashFormatter.GetMainCashFormat(totalAmount, 3, out str, out text);
			this.cash.text = "$" + str;
			this.cashPostfix.text = text;
		}
		else if (resourceType == ResourceType.Gems)
		{
			this.gems.SetVariableText(new string[]
			{
				((int)totalAmount).ToString()
			});
		}
		SkillManager.Instance.OnSkillCostResourceChanged(resourceType, totalAmount);
		ItemManager.Instance.OnResourceChanged(resourceType, totalAmount);
		if (this.OnResourceChanged != null)
		{
			this.OnResourceChanged(resourceType, amountAdded, totalAmount);
		}
	}

	private void TrackEvent(ResourceChangeData changeData)
	{
		LionAnalytics.TrackEvent(changeData.GetAnalyticsEventName(), changeData.GetAnalyticsDictionary());
		LionAnalytics.TrackEvent(changeData.GetNormalizedAnalyticsEventName(), changeData.GetNormalizedAnalyticsDictionary());
	}

	private void OnDestroy()
	{
		this.OnResourceChanged = null;
	}

	public void LoadSavedCash(bool isFromTournament = false)
	{
		this.GiveResource(ResourceType.Cash, BigInteger.Parse(EncryptedPlayerPrefs.GetString("KEY_CASH_VALUE", "0")));
		if (!isFromTournament)
		{
			this.GiveResource(ResourceType.SkillPoints, EncryptedPlayerPrefs.GetInt("KEY_SKILLPOINT_VALUE", 0));
			this.GiveResource(ResourceType.CrownExp, EncryptedPlayerPrefs.GetInt("KEY_CROWNEXP_VALUE", 0));
			this.hasReceivedStartingResources = (EncryptedPlayerPrefs.GetInt("KEY_HAS_RECEIVED_STARTING", 0) != 0);
		}
	}

	public void SaveGems()
	{
		ResourceChangeData gemChangeData = new ResourceChangeData("contentId_onInitialize_1", "On Initialize For Testing 1", this.startingGems, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.ForTesting);
		this.GiveGems(0, gemChangeData);
	}

	public void SaveCash()
	{
		EncryptedPlayerPrefs.SetString("KEY_CASH_VALUE", this.availableCash.ToString(), true);
		EncryptedPlayerPrefs.SetInt("KEY_SKILLPOINT_VALUE", this.availableSkillPoints, true);
		EncryptedPlayerPrefs.SetInt("KEY_CROWNEXP_VALUE", this.availableCrownExp, true);
		EncryptedPlayerPrefs.SetInt("KEY_HAS_RECEIVED_STARTING", (!this.hasReceivedStartingResources) ? 0 : 1, true);
	}

	private void OnApplicationPause(bool didPause)
	{
		if (!didPause || TournamentManager.Instance.IsInsideTournament)
		{
			return;
		}
		this.SaveCash();
	}

	private void Update()
	{
		if (FHelper.HasSecondsPassed(1f, ref this.passiveGainCashTimer, true))
		{
			BigInteger totalValueIncrease = ValueModifier.GetTotalValueIncrease();
			int value = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.PassiveCashGain>();
			this.GiveResource(ResourceType.Cash, totalValueIncrease * value);
			this.passiveGainCashTimer = 0f;
		}
	}

	public const string ITEM_BOOST_GOLD_FISH = "se.ace.boost_goldfish";

	public const string ITEM_BOOST_TIME_0 = "se.ace.boost_time_0";

	public const string ITEM_BOOST_SPAWN_BOSS_FISH = "se.ace.boost_boss_fish";

	private const string KEY_GEM_VALUE = "KEY_GEM_VALUE";

	public const string KEY_CASH_VALUE = "KEY_CASH_VALUE";

	private const string KEY_SKILLPOINT_VALUE = "KEY_SKILLPOINT_VALUE";

	private const string KEY_CROWNEXP_VALUE = "KEY_CROWNEXP_VALUE";

	private const string KEY_HAS_RECEIVED_STARTING = "KEY_HAS_RECEIVED_STARTING";

	private const string contentId_onInitialize = "contentId_onInitialize";

	private const string contentName_onInitialize = "On Initialize For Testing";

	[SerializeField]
	private Skill freeSpinSkill;

	[SerializeField]
	private TextMeshProUGUI cash;

	[SerializeField]
	private TextMeshProUGUI cashPostfix;

	[SerializeField]
	private TextMeshProUGUI gems;

	[SerializeField]
	private BigIntWrapper startingCash = new BigIntWrapper();

	[SerializeField]
	private int startingGems;

	[SerializeField]
	private int startingFreeSpins;

	[SerializeField]
	private Skill totalCrewMemberSkill;

	[SerializeField]
	public int startingCrewMemberUnlockPrice = 9;

	private bool hasReceivedStartingResources;

	private BigInteger availableCash = default(BigInteger);

	private int availableSkillPoints;

	private int availableCrownExp;

	private FishStoreAssets storeAssets;

	private static SoomlaStoreProvider soomlaStoreProvider = new SoomlaStoreProvider();

	private static StoreManager storeManager = new StoreManager(ResourceManager.soomlaStoreProvider);

	private float passiveGainCashTimer;
}
