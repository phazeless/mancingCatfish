using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class LionAnalytics : MonoBehaviour
{
	public static LAnalytics LAnalytics
	{
		get
		{
			return LionAnalytics.a;
		}
	}

	private static bool IsInitialized
	{
		get
		{
			return LionAnalytics.a != null;
		}
	}

	private void Awake()
	{
		UnityEngine.Debug.Log("Lion: Application.version: " + Application.version);
		this.Load();
	}

	private void OnDestroy()
	{
		this.UnregisterListeners();
	}

	private void Start()
	{
		
		
	}

	private void Update()
	{
		
	}

	private void Load()
	{
		
	}

	private void Save()
	{
		EncryptedPlayerPrefs.SetInt("KEY_TRACK_BOAT_LEVEL_REACHED", LionAnalytics.boatLevelReached, true);
		EncryptedPlayerPrefs.SetInt("KEY_TRACK_CREW_MEMBER_AMOUNT_REACHED", LionAnalytics.crewMemberAmountReached, true);
		EncryptedPlayerPrefs.SetInt("KEY_TRACK_STARS_AMOUNT_REACHED", LionAnalytics.starsAmountReached, true);
		EncryptedPlayerPrefs.SetInt("KEY_TRACK_LEVEL_REACHED", LionAnalytics.levelReached, true);
		EncryptedPlayerPrefs.SetInt("KEY_TRACK_TIME_PLAYED_MINUTES", LionAnalytics.timePlayedInMinutes, true);
		EncryptedPlayerPrefs.SetInt("KEY_TRACK_QUEST_REACHED", LionAnalytics.questNrReached, true);
		EncryptedPlayerPrefs.SetInt("KEY_TRACK_SESSION_COUNT", LionAnalytics.sessionCount, true);
		EncryptedPlayerPrefs.SetInt("KEY_TRACK_TOTAL_ADS_WATCHED", LionAnalytics.totalAdsWatched, true);
		EncryptedPlayerPrefs.SetString("KEY_TRACK_LAST_APP_VERSION", LionAnalytics.lastAppVersion, true);
		EncryptedPlayerPrefs.SetInt("KEY_TRACK_TIME_PLAYED_SECONDS", (int)LionAnalytics.timePlayedInSeconds, true);
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause)
		{
			LionAnalytics.leaveAppAtTime = DateTime.Now;
			this.Save();
		}
		else
		{
			double totalSeconds = (DateTime.Now - LionAnalytics.leaveAppAtTime).TotalSeconds;
			if (totalSeconds >= 60.0)
			{
				LionAnalytics.IncreaseAndTrackSessions();
			}
		}
	}

	public static void Rev_TrackPurchase(ILAnalyticsReceiptData receiptData)
	{
		if (!LionAnalytics.IsInitialized)
		{
			return;
		}
		LionAnalytics.a.LogPurchase(receiptData);
	}

	public static void Ad_TrackRewardedVideoRequest(AdPlacement placement)
	{
		
	}

	public static void Ad_TrackRewardedVideoViewSuccess(AdPlacement placement)
	{
		
	}

	public static void Ad_TrackRewardedVideoViewFailed(AdPlacement placement)
	{
		
	}

	public static void Ad_TrackRewardedVideoClick(AdPlacement placement)
	{
		
	}

	public static void TrackAppVersion()
	{
	
	}

	public static void TrackExperimentGroup()
	{
		
	}

	public static void TrackEvent(string eventName, Dictionary<string, object> parameters)
	{
		
	}

	public static void IncreaseAndTrackBoatLevelReached(int currentLevelReached)
	{
		
	}

	public static void IncreaseAndTrackCrewMemberAmountReached(int currentAmountReached)
	{
		
	}

	public static void TrackStarsAmountReached(int currentStarsAmountReached)
	{
		
	}

	public static void TrackQuestReached(int currentQuestNr)
	{
		
	}

	public static void IncreaseAndTrackTimePlayed()
	{
		
	}

	public static void IncreaseAndTrackSessions()
	{
		
	}

	public static void IncreaseAndTrackAdWatched()
	{
		
	}

	public static void BidOptimization_CompletedTutorial(int currentQuestNr)
	{
		if (!LionAnalytics.IsInitialized)
		{
			return;
		}
		if (!LionAnalytics.hasSentEvent_CompletedTutorial && currentQuestNr >= 1)
		{
			string eventName = "fb_mobile_tutorial_completion";
			string eventName2 = "af_tutorial_completion";
			LionAnalytics.a.LogEvent<FacebookLAnalytics>(eventName, new Dictionary<string, object>());
			LionAnalytics.hasSentEvent_CompletedTutorial = true;
			EncryptedPlayerPrefs.SetInt("KEY_HAS_SENT_QUEST_GOAL_FIRST_REACHED", 1, true);
		}
	}

	public static void BidOptimization_AchievedLevel(int currentQuestNr)
	{
		if (!LionAnalytics.hasSentEvent_AchievedLevel && currentQuestNr >= 4)
		{
			string eventName = "fb_mobile_level_achieved";
			string eventName2 = "af_level_achieved";
			LionAnalytics.a.LogEvent<FacebookLAnalytics>(eventName, new Dictionary<string, object>());
			LionAnalytics.hasSentEvent_AchievedLevel = true;
			EncryptedPlayerPrefs.SetInt("KEY_HAS_SENT_QUEST_GOAL_SECOND_REACHED", 1, true);
		}
	}

	private static void IfMatch(List<int> numbers, int matchNumber, Action callback)
	{
		if (numbers != null && numbers.Count > 0)
		{
			foreach (int num in numbers)
			{
				if (num == matchNumber)
				{
					if (callback != null)
					{
						callback();
					}
					break;
				}
			}
		}
		else if (callback != null)
		{
			callback();
		}
	}

	private void RegisterListeners()
	{
		try
		{
			SkillManager.Instance.DeepWaterSkill.OnSkillLevelUp += this.TrackDeepWaterLocationChanged;
			SkillManager.Instance.TierSkill.OnSkillLevelUp += this.TrackUpgradeBoat;
			ChestManager.Instance.OnChestOpened += this.Instance_OnChestOpened;
			IGNGemChestDialog.OnGemChestCollected += this.TrackGemChestCollected;
			IGNPackageDialog.OnPackageCollected += this.TrackPackageCollected;
			ItemManager.Instance.OnItemAmountChanged += this.Item_OnItemAmountChanged;
			ItemManager.Instance.OnItemLevelChanged += this.Item_OnItemLevelUp;
			ItemManager.Instance.OnItemEquipStateChanged += this.Item_OnItemEquipStateChanged;
			PurchaseCrewMemberHandler.Instance.OnCrewUnlocked += this.Instance_OnCrewUnlocked;
			FishingExperienceHolder.Instance.OnCollectedFishingExp += this.Instance_OnCollectedFishingExp;
			TournamentManager.Instance.OnJoinTournament += this.Instance_OnJoinTournament;
			TournamentManager.Instance.OnLeftTournament += this.Instance_OnLeftTournament;
			TournamentManager.Instance.OnFinishedTournament += this.Instance_OnFinishedTournament;
			SettingsManager.Instance.OnSettingsChanged += this.Instance_OnSettingsChanged;
			FacebookHandler.Instance.OnFacebookConnected += this.Instance_OnFacebookConnected;
			SkillTreeManager.Instance.OnCrownLevelIncreased += this.Instance_OnCrownLevelIncreased;
			SkillTreeManager.Instance.OnSkillTreeReset += this.Instance_OnSkillTreeReset;
			DailyGiftManager.Instance.OnDailyCatchCollectedAndShown += this.Instance_OnDailyCatchCollectedAndShown;
			AFKManager.Instance.OnAFKCashCollected += this.TrackIncreasedReturnBonus;
			WheelBehaviour.OnWheelSpun += this.TrackSpinTheWheel;
			WheelBehaviour.OnWheelSpinFinished += this.TrackSpinTheWheelFinished;
			FishBook.Instance.OnNewFishCaught += this.TrackNewFishCaught;
			QuestManager.Instance.OnQuestClaimed += this.TrackQuestReached;
			SkillManager.Instance.OnSkillLevelChanged += this.Instance_OnSkillLevelChanged;
			TutorialManager.Instance.OnEnterTutorial += this.Instance_OnEnterTutorial;
			TutorialManager.Instance.OnExitedTutorial += this.Instance_OnExitedTutorial;
			EventContent.GlobalOnEventTreasureUnlocked += this.EventContent_GlobalOnEventRewardGranted;
			EventChallenge.GlobalOnChallengeCompleted += this.EventChallenge_GlobalOnChallengeCompleted;
			BaseGoal.GlobalOnGoalClaimed += this.BaseGoal_GlobalOnGoalClaimed;
			FireworkFishingManager.Instance.Firework.OnGranted += this.Firework_OnGranted;
			FireworkFishingManager.Instance.Firework.OnConsumed += this.Firework_OnConsumed;
			HolidayOfferManager.Instance.OnHolidayOfferBought += this.Instance_OnHolidayOfferBought;
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Failed to register some listener: " + ex.Message);
		}
	}

	private void EventContent_GlobalOnEventRewardGranted(EventContent eventContent, int gemsSpent, int ticketsCollected)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("eventId", eventContent.Id);
		dictionary.Add("eventName", eventContent.name);
		dictionary.Add("gemsSpent", gemsSpent);
		dictionary.Add("ticketsCollected", ticketsCollected);
		LionAnalytics.a.LogEvent("eventTreasureUnlocked", dictionary);
	}

	private void Instance_OnHolidayOfferBought(HolidayOffer offer)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("offerBoughtItemId", offer.ItemId);
		dictionary.Add("offerBoughtName", offer.name);
		LionAnalytics.a.LogEvent("offerBought", dictionary);
	}

	private void Firework_OnConsumed(BaseConsumable consumable, int amount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("fireworkAmountConsumed", amount);
		LionAnalytics.a.LogEvent("fireworkConsumed", dictionary);
	}

	private void Firework_OnGranted(BaseConsumable consumable, int amount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("fireworkAmountGranted", amount);
		LionAnalytics.a.LogEvent("fireworkGranted", dictionary);
	}

	private void EventChallenge_GlobalOnChallengeCompleted(EventChallenge eventChallenge)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("challengeId", eventChallenge.Id);
		dictionary.Add("challengeName", eventChallenge.name);
		LionAnalytics.a.LogEvent("eventChallengeCompleted", dictionary);
	}

	private void BaseGoal_GlobalOnGoalClaimed(BaseGoal goal)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("challengeId", goal.ParentChallenge.Id);
		dictionary.Add("challengeName", goal.ParentChallenge.name);
		dictionary.Add("challengeGoalId", goal.Id);
		dictionary.Add("challengeGoalName", goal.name);
		dictionary.Add("challengeGoalRewardAmount", goal.GoalReward.Amount);
		dictionary.Add("challengeGoalRewardType", goal.GoalReward.Title);
		LionAnalytics.a.LogEvent("eventChallengeGoalClaimed", dictionary);
	}

	private void StartExtended()
	{
		if (this.HasCompletedFTUE("KEY_FTUE_JOIN_TOURNAMENT") && !this.HasCompletedFTUE("KEY_FTUE_START_GAME_AFTER_TOURNAMENT"))
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("ftueStepName", "ftueStartGameAfterTournament");
			dictionary.Add("ftueStepNumber", 9);
			LionAnalytics.a.LogEvent("ftueStep", dictionary);
			this.CompleteFTUE("KEY_FTUE_START_GAME_AFTER_TOURNAMENT");
		}
	}

	private void Instance_OnExitedTutorial(TutorialSliceBase slice)
	{
		if (slice is TutorialSliceBG)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("ftueStepName", "ftueEnterNameTutorialFinish");
			dictionary.Add("ftueStepNumber", 4);
			LionAnalytics.a.LogEvent("ftueStep", dictionary);
		}
	}

	private void UnregisterListeners()
	{
		SkillManager.Instance.DeepWaterSkill.OnSkillLevelUp -= this.TrackDeepWaterLocationChanged;
		SkillManager.Instance.TierSkill.OnSkillLevelUp -= this.TrackUpgradeBoat;
		ChestManager.Instance.OnChestOpened -= this.Instance_OnChestOpened;
		IGNGemChestDialog.OnGemChestCollected -= this.TrackGemChestCollected;
		IGNPackageDialog.OnPackageCollected -= this.TrackPackageCollected;
		ItemManager.Instance.OnItemAmountChanged -= this.Item_OnItemAmountChanged;
		ItemManager.Instance.OnItemLevelChanged -= this.Item_OnItemLevelUp;
		ItemManager.Instance.OnItemEquipStateChanged -= this.Item_OnItemEquipStateChanged;
		PurchaseCrewMemberHandler.Instance.OnCrewUnlocked -= this.Instance_OnCrewUnlocked;
		FishingExperienceHolder.Instance.OnCollectedFishingExp -= this.Instance_OnCollectedFishingExp;
		TournamentManager.Instance.OnJoinTournament -= this.Instance_OnJoinTournament;
		TournamentManager.Instance.OnLeftTournament -= this.Instance_OnLeftTournament;
		TournamentManager.Instance.OnFinishedTournament -= this.Instance_OnFinishedTournament;
		SettingsManager.Instance.OnSettingsChanged -= this.Instance_OnSettingsChanged;
		FacebookHandler.Instance.OnFacebookConnected -= this.Instance_OnFacebookConnected;
		SkillTreeManager.Instance.OnCrownLevelIncreased -= this.Instance_OnCrownLevelIncreased;
		SkillTreeManager.Instance.OnSkillTreeReset -= this.Instance_OnSkillTreeReset;
		DailyGiftManager.Instance.OnDailyCatchCollectedAndShown -= this.Instance_OnDailyCatchCollectedAndShown;
		AFKManager.Instance.OnAFKCashCollected -= this.TrackIncreasedReturnBonus;
		WheelBehaviour.OnWheelSpun -= this.TrackSpinTheWheel;
		WheelBehaviour.OnWheelSpinFinished -= this.TrackSpinTheWheelFinished;
		FishBook.Instance.OnNewFishCaught -= this.TrackNewFishCaught;
		QuestManager.Instance.OnQuestClaimed -= this.TrackQuestReached;
		SkillManager.Instance.OnSkillLevelChanged -= this.Instance_OnSkillLevelChanged;
		TutorialManager.Instance.OnEnterTutorial -= this.Instance_OnEnterTutorial;
		TutorialManager.Instance.OnExitedTutorial -= this.Instance_OnExitedTutorial;
		EventContent.GlobalOnEventTreasureUnlocked -= this.EventContent_GlobalOnEventRewardGranted;
		EventChallenge.GlobalOnChallengeCompleted -= this.EventChallenge_GlobalOnChallengeCompleted;
		BaseGoal.GlobalOnGoalClaimed -= this.BaseGoal_GlobalOnGoalClaimed;
		FireworkFishingManager.Instance.Firework.OnGranted -= this.Firework_OnGranted;
		FireworkFishingManager.Instance.Firework.OnConsumed -= this.Firework_OnConsumed;
		HolidayOfferManager.Instance.OnHolidayOfferBought -= this.Instance_OnHolidayOfferBought;
	}

	private void Instance_OnEnterTutorial(TutorialSliceBase slice)
	{
		if (slice is TutorialSliceUpgrades)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("ftueStepName", "ftueBoatSkillTutorialStart");
			dictionary.Add("ftueStepNumber", 0);
			LionAnalytics.a.LogEvent("ftueStep", dictionary);
		}
		else if (slice is TutorialSliceBG)
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("ftueStepName", "ftueEnterNameTutorialStart");
			dictionary2.Add("ftueStepNumber", 3);
			LionAnalytics.a.LogEvent("ftueStep", dictionary2);
		}
	}

	private void Instance_OnSkillLevelChanged(Skill skill, LevelChange levelChange)
	{
		string b = "0dd9ca34-01a8-4349-a08f-017f97f21f12";
		if (levelChange == LevelChange.LevelUp && skill.Id == b && skill.CurrentLevel == 1 && !this.HasCompletedFTUE("KEY_FTUE_BOAT_SKILL_UPGRADE"))
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("ftueStepName", "ftueBoatSkillTutorialFinish");
			dictionary.Add("ftueStepNumber", 1);
			LionAnalytics.a.LogEvent("ftueStep", dictionary);
			this.CompleteFTUE("KEY_FTUE_BOAT_SKILL_UPGRADE");
		}
	}

	private void Instance_OnDailyCatchCollectedAndShown(DailyGiftContent content)
	{
		this.TrackDailyCatchCollected(DailyGiftManager.Instance.CurrentDayStreak);
	}

	private void Instance_OnSkillTreeReset(int skillPointsReturned)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("skillTreePointsReturned", skillPointsReturned);
		LionAnalytics.a.LogEvent("skillTreeReset", dictionary);
	}

	private void Instance_OnCrownLevelIncreased(int currentCrownLevel)
	{
		Dictionary<string, object> eventValues = new Dictionary<string, object>();
		LionAnalytics.a.LogEvent("crownLevelIncreased", eventValues);
	}

	private void Instance_OnFacebookConnected()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("socialPlatform", "Facebook");
		LionAnalytics.a.LogEvent("socialConnect", dictionary);
	}

	private void Instance_OnSettingsChanged(SettingsType settings, string changeValue)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("settingsType", settings.ToString());
		dictionary.Add("settingsChangeValue", changeValue);
		LionAnalytics.a.LogEvent("settingsChanged", dictionary);
	}

	private void Instance_OnFinishedTournament(string tournamentId, int rank, string score)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("tournamentId", tournamentId);
		dictionary.Add("tournamentRank", rank);
		dictionary.Add("tournamentScore", score);
		LionAnalytics.a.LogEvent("tournamentFinished", dictionary);
	}

	private void Instance_OnLeftTournament(string tournamentId, int rank, string score)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("tournamentId", tournamentId);
		dictionary.Add("tournamentRank", rank);
		dictionary.Add("tournamentScore", score);
		LionAnalytics.a.LogEvent("tournamentLeft", dictionary);
	}

	private void Instance_OnJoinTournament(string tournamentId)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("tournamentId", tournamentId);
		LionAnalytics.a.LogEvent("tournamentJoined", dictionary);
		if (!this.HasCompletedFTUE("KEY_FTUE_JOIN_TOURNAMENT"))
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("ftueStepName", "ftueJoinTournament");
			dictionary2.Add("ftueStepNumber", 8);
			LionAnalytics.a.LogEvent("ftueStep", dictionary2);
			this.CompleteFTUE("KEY_FTUE_JOIN_TOURNAMENT");
		}
	}

	private void Instance_OnCollectedFishingExp(long collectedFishingExp, long oldFishingExpAmount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("collectedFishingExpAmount", collectedFishingExp);
		LionAnalytics.a.LogEvent("fishingExpCollected", dictionary);
		if (!this.HasCompletedFTUE("KEY_FTUE_COLLECT_FISH_EXP"))
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("ftueStepName", "ftueCollectFishExp");
			dictionary2.Add("ftueStepNumber", 7);
			LionAnalytics.a.LogEvent("ftueStep", dictionary2);
			this.CompleteFTUE("KEY_FTUE_COLLECT_FISH_EXP");
		}
	}

	private void Item_OnItemAmountChanged(Item item, int currentAmount, int oldAmount, ResourceChangeReason reason)
	{
		bool flag = currentAmount > oldAmount;
		if (flag)
		{
			int amount = currentAmount - oldAmount;
			this.TrackItemReceived(item, amount, reason.ToString());
		}
	}

	private void Item_OnItemEquipStateChanged(Item item, ItemEquipState state)
	{
		if (state == ItemEquipState.Equipped)
		{
			this.TrackItemEquipped(item);
		}
		else
		{
			this.TrackItemUnequipped(item);
		}
	}

	private void Item_OnItemLevelUp(Item item, LevelChange levelChange, int itemAmountSpent, int gemCost)
	{
		if (levelChange == LevelChange.LevelUp)
		{
			this.TrackItemUpgrade(item, itemAmountSpent, gemCost);
		}
	}

	private void Instance_OnCrewUnlocked(Skill crewUnlocked, ResourceChangeReason source, int gemsSpent)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("crewName", crewUnlocked.GetExtraInfo().TitleText);
		dictionary.Add("evSource", source.ToString());
		dictionary.Add("premiumCurrencySpent", gemsSpent);
		LionAnalytics.a.LogEvent("crewUnlocked", dictionary);
	}

	private void Instance_OnChestOpened(ItemChest itemChest, List<Item> items)
	{
		this.TrackItemBoxCollected(itemChest);
		Dictionary<Item, int> dictionary = items.DistinctAndCount<Item>();
	}

	public void TrackDeepWaterLocationChanged(Skill deepWaterSkill, LevelChange levelChange)
	{
		BigInteger left = -1;
		if (levelChange == LevelChange.LevelUp)
		{
			left = deepWaterSkill.GetCostForLevelUp(deepWaterSkill.CurrentLevel - 1);
		}
		else if (levelChange == LevelChange.HardReset)
		{
			left = 0;
		}
		if (left >= 0L)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("dwLocationLevel", deepWaterSkill.CurrentLevel);
			dictionary.Add("grindCurrencySpent", left.ToString());
			LionAnalytics.a.LogEvent("deepWaterLocationChanged", dictionary);
			if (deepWaterSkill.CurrentLevel == 1 && !this.HasCompletedFTUE("KEY_FTUE_GOTO_DEEPER_WATER"))
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2.Add("ftueStepName", "ftueGoToDeeperWater");
				dictionary2.Add("ftueStepNumber", 6);
				LionAnalytics.a.LogEvent("ftueStep", dictionary2);
				this.CompleteFTUE("KEY_FTUE_GOTO_DEEPER_WATER");
			}
		}
	}

	public void TrackItemBoxCollected(ItemChest itemChest)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("itemBoxName", itemChest.ChestName);
		LionAnalytics.a.LogEvent("itemBoxCollected", dictionary);
	}

	public void TrackItemReceived(Item item, int amount, string receivedItemFromSource)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("itemId", item.Id);
		dictionary.Add("itemName", item.Title);
		dictionary.Add("itemAmountReceived", amount);
		dictionary.Add("evSource", receivedItemFromSource);
		LionAnalytics.a.LogEvent("itemReceived", dictionary);
	}

	public void TrackGemChestCollected()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("gemChestName", "standard");
		LionAnalytics.a.LogEvent("gemChestCollected", dictionary);
	}

	public void TrackPackageCollected()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("packageName", "standard");
		LionAnalytics.a.LogEvent("packageCollected", dictionary);
	}

	public void TrackItemUpgrade(Item item, int itemAmountUsed, int gemCost)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("itemId", item.Id);
		dictionary.Add("itemName", item.Title);
		dictionary.Add("itemAmountUsed", itemAmountUsed);
		dictionary.Add("premiumCurrencySpent", gemCost);
		dictionary.Add("itemLevel", item.CurrentLevel);
		LionAnalytics.a.LogEvent("itemUpgrade", dictionary);
	}

	public void TrackItemEquipped(Item item)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("itemId", item.Id);
		dictionary.Add("itemName", item.Title);
		dictionary.Add("itemLevel", item.CurrentLevel);
		LionAnalytics.a.LogEvent("itemEquipped", dictionary);
	}

	public void TrackItemUnequipped(Item item)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("itemId", item.Id);
		dictionary.Add("itemName", item.Title);
		dictionary.Add("itemLevel", item.CurrentLevel);
		LionAnalytics.a.LogEvent("itemUnequipped", dictionary);
	}

	public void TrackDailyCatchCollected(int streak)
	{
		Dictionary<string, object> eventValues = new Dictionary<string, object>();
		LionAnalytics.a.LogEvent("dailyCatchCollected", eventValues);
	}

	public void TrackIncreasedReturnBonus(BigInteger amountCollected, bool didUseDoubleUp)
	{
		if (didUseDoubleUp)
		{
			Dictionary<string, object> eventValues = new Dictionary<string, object>();
			LionAnalytics.a.LogEvent("increasedReturnBonus", eventValues);
		}
	}

	public void TrackSpinTheWheel()
	{
		Dictionary<string, object> eventValues = new Dictionary<string, object>();
		LionAnalytics.a.LogEvent("spinTheWheel", eventValues);
	}

	public void TrackSpinTheWheelFinished(bool wasJackpot, int gemsFromJackpot)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("wasWheelJackpot", wasJackpot);
		dictionary.Add("gemsFromWheelJackpot", gemsFromJackpot);
		LionAnalytics.a.LogEvent("spinTheWheelFinished", dictionary);
	}

	public void TrackNewFishCaught(FishAttributes fish)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("fishId", fish.FishType.ToString());
		dictionary.Add("fishName", fish.Name);
		dictionary.Add("fishStarWorth", fish.Stars);
		LionAnalytics.a.LogEvent("newFighCaught", dictionary);
	}

	public void TrackUpgradeBoat(Skill boatSkill, LevelChange levelChange)
	{
		if (levelChange == LevelChange.LevelUp)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("boatLevel", boatSkill.CurrentLevel);
			LionAnalytics.a.LogEvent("upgradeBoat", dictionary);
		}
	}

	public void TrackQuestReached(Skill questSkill, Quest completed, Quest next)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("questId", questSkill.CurrentLevel);
		LionAnalytics.a.LogEvent("questReached", dictionary);
		if (questSkill.CurrentLevel == 1)
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("ftueStepName", "ftueFirstQuestFinish");
			dictionary2.Add("ftueStepNumber", 2);
			LionAnalytics.a.LogEvent("ftueStep", dictionary2);
		}
		else if (questSkill.CurrentLevel == 2)
		{
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3.Add("ftueStepName", "ftueSecondQuestFinish");
			dictionary3.Add("ftueStepNumber", 5);
			LionAnalytics.a.LogEvent("ftueStep", dictionary3);
		}
	}

	private void CompleteFTUE(string key)
	{
		PlayerPrefs.SetInt(key, 1);
	}

	private bool HasCompletedFTUE(string key)
	{
		return PlayerPrefs.GetInt(key, 0) == 1;
	}

	private KeyValuePair<string, object> GetCashBalanceForAnalytics()
	{
		return new KeyValuePair<string, object>("grindCurrencyBalance", ResourceManager.Instance.GetResourceAmount(ResourceType.Cash).ToString());
	}

	private KeyValuePair<string, object> GetGemBalanceForAnalytics()
	{
		return new KeyValuePair<string, object>("premiumCurrencyBalance", (int)ResourceManager.Instance.GetResourceAmount(ResourceType.Gems));
	}

	private KeyValuePair<string, object> GetSkillPointBalanceForAnalytics()
	{
		return new KeyValuePair<string, object>("skillPointsBalance", (int)ResourceManager.Instance.GetResourceAmount(ResourceType.SkillPoints));
	}

	private KeyValuePair<string, object> GetCrownLevelBalanceForAnalytics()
	{
		return new KeyValuePair<string, object>("crownLevelBalance", (int)ResourceManager.Instance.GetResourceAmount(ResourceType.CrownExp));
	}

	private KeyValuePair<string, object> GetStarsBalanceForAnalytics()
	{
		return new KeyValuePair<string, object>("starsBalance", SkillManager.Instance.CollectStarsSkill.CurrentLevel);
	}

	private KeyValuePair<string, object> GetFishingExpBalanceForAnalytics()
	{
		return new KeyValuePair<string, object>("fishingExpBalance", SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong);
	}

	private KeyValuePair<string, object> GetDailyCatchStreakForAnalytics()
	{
		return new KeyValuePair<string, object>("dailyCatchStreakBalance", DailyGiftManager.Instance.CurrentDayStreak);
	}

	
	public const string paramName_experimentGroup = "experimentGroup";

	private const string KEY_TRACK_BOAT_LEVEL_REACHED = "KEY_TRACK_BOAT_LEVEL_REACHED";

	private const string KEY_TRACK_CREW_MEMBER_AMOUNT_REACHED = "KEY_TRACK_CREW_MEMBER_AMOUNT_REACHED";

	private const string KEY_TRACK_STARS_AMOUNT_REACHED = "KEY_TRACK_STARS_AMOUNT_REACHED";

	private const string KEY_TRACK_LEVEL_REACHED = "KEY_TRACK_LEVEL_REACHED";

	private const string KEY_TRACK_TIME_PLAYED_MINUTES = "KEY_TRACK_TIME_PLAYED_MINUTES";

	private const string KEY_TRACK_QUEST_REACHED = "KEY_TRACK_QUEST_REACHED";

	private const string KEY_TRACK_SESSION_COUNT = "KEY_TRACK_SESSION_COUNT";

	private const string KEY_TRACK_TOTAL_ADS_WATCHED = "KEY_TRACK_TOTAL_ADS_WATCHED";

	private const string KEY_TRACK_LAST_APP_VERSION = "KEY_TRACK_LAST_APP_VERSION";

	private const string KEY_TRACK_TIME_PLAYED_SECONDS = "KEY_TRACK_TIME_PLAYED_SECONDS";

	private const int NEW_SESSION_THRESHOLD = 60;

	private static LAnalytics a = null;

	private static int boatLevelReached = 0;

	private static int crewMemberAmountReached = 0;

	private static int starsAmountReached = 0;

	private static int levelReached = 0;

	private static int timePlayedInMinutes = 0;

	private static int questNrReached = 0;

	private static int sessionCount = 0;

	private static float timePlayedInSeconds = 0f;

	private static int totalAdsWatched = 0;

	private static string lastAppVersion = string.Empty;

	private static string lastExperimentGroup = string.Empty;

	private static float retrySendEventTimer = 0f;

	private static DateTime leaveAppAtTime = DateTime.Now;

	private const string KEY_HAS_SENT_QUEST_GOAL_FIRST_REACHED = "KEY_HAS_SENT_QUEST_GOAL_FIRST_REACHED";

	private const string KEY_HAS_SENT_QUEST_GOAL_SECOND_REACHED = "KEY_HAS_SENT_QUEST_GOAL_SECOND_REACHED";

	private const int QUEST_GOAL_FIRST = 1;

	private const int QUEST_GOAL_SECOND = 4;

	private static bool hasSentEvent_AchievedLevel = false;

	private static bool hasSentEvent_CompletedTutorial = false;

	private const string KEY_FTUE_BOAT_SKILL_UPGRADE = "KEY_FTUE_BOAT_SKILL_UPGRADE";

	private const string KEY_FTUE_GOTO_DEEPER_WATER = "KEY_FTUE_GOTO_DEEPER_WATER";

	private const string KEY_FTUE_COLLECT_FISH_EXP = "KEY_FTUE_COLLECT_FISH_EXP";

	private const string KEY_FTUE_JOIN_TOURNAMENT = "KEY_FTUE_JOIN_TOURNAMENT";

	private const string KEY_FTUE_START_GAME_AFTER_TOURNAMENT = "KEY_FTUE_START_GAME_AFTER_TOURNAMENT";

	public const string PARAM_GRIND_CURRENCY_BALANCE = "grindCurrencyBalance";

	public const string PARAM_PREMIUM_CURRENCY_BALANCE = "premiumCurrencyBalance";

	public const string PARAM_SKILL_POINT_BALANCE = "skillPointsBalance";

	public const string PARAM_CROWN_LEVEL_BALANCE = "crownLevelBalance";

	public const string PARAM_STARS_BALANCE = "starsBalance";

	public const string PARAM_FISHING_EXP_BALANCE = "fishingExpBalance";

	public const string PARAM_DAILY_CATCH_STREAK_BALANCE = "dailyCatchStreakBalance";

	public const string PARAM_EXPERIMENT_GROUP = "experimentGroup";

	public const string PARAM_GRIND_CURRENCY_SPENT = "grindCurrencySpent";

	public const string PARAM_PREMIUM_CURRENCY_SPENT = "premiumCurrencySpent";

	public const string PARAM_EV_SOURCE = "evSource";

	public const string PARAM_DW_LOCATION_LEVEL = "dwLocationLevel";

	public const string PARAM_ITEM_BOX_NAME = "itemBoxName";

	public const string PARAM_GEM_CHEST_NAME = "gemChestName";

	public const string PARAM_PACKAGE_NAME = "packageName";

	public const string PARAM_ITEM_ID = "itemId";

	public const string PARAM_ITEM_NAME = "itemName";

	public const string PARAM_CREW_NAME = "crewName";

	public const string PARAM_ITEM_LEVEL = "itemLevel";

	public const string PARAM_ITEM_AMOUNT_RECEIVED = "itemAmountReceived";

	public const string PARAM_ITEM_AMOUNT_USED = "itemAmountUsed";

	public const string PARAM_COLLECTED_FISHING_EXP_AMOUNT = "collectedFishingExpAmount";

	public const string PARAM_TOURNAMENT_ID = "tournamentId";

	public const string PARAM_TOURNAMENT_RANK = "tournamentRank";

	public const string PARAM_TOURNAMENT_SCORE = "tournamentScore";

	public const string PARAM_SETTINGS_TYPE = "settingsType";

	public const string PARAM_SETTINGS_CHANGE_VALUE = "settingsChangeValue";

	public const string PARAM_SOCIAL_PLATFORM = "socialPlatform";

	public const string PARAM_AD_PLACEMENT = "placement";

	public const string PARAM_RESOURCE_ID = "resourceId";

	public const string PARAM_RESOURCE_AMOUNT = "resourceAmount";

	public const string PARAM_EXTRA_INFO = "extraInfo";

	public const string PARAM_SKILL_TREE_POINTS_RETURNED = "skillTreePointsReturned";

	public const string PARAM_FISH_ID = "fishId";

	public const string PARAM_FISH_NAME = "fishName";

	public const string PARAM_FISH_STAR_WORTH = "fishStarWorth";

	public const string PARAM_BOAT_LEVEL = "boatLevel";

	public const string PARAM_QUEST_ID = "questId";

	public const string PARAM_WAS_WHEEL_JACKPOT = "wasWheelJackpot";

	public const string PARAM_GEMS_FROM_WHEEL_JACKPOT = "gemsFromWheelJackpot";

	public const string PARAM_FTUE_NAME = "ftueStepName";

	public const string PARAM_FTUE_NUMBER = "ftueStepNumber";

	public const string PARAM_EVENT_ID = "eventId";

	public const string PARAM_EVENT_NAME = "eventName";

	public const string PARAM_CHALLENGE_ID = "challengeId";

	public const string PARAM_CHALLENGE_NAME = "challengeName";

	public const string PARAM_CHALLENGE_GOAL_ID = "challengeGoalId";

	public const string PARAM_CHALLENGE_GOAL_NAME = "challengeGoalName";

	public const string PARAM_CHALLENGE_GOAL_REWARD_AMOUNT = "challengeGoalRewardAmount";

	public const string PARAM_CHALLENGE_GOAL_REWARD_TYPE = "challengeGoalRewardType";

	public const string PARAM_OFFER_BOUGHT_ITEM_ID = "offerBoughtItemId";

	public const string PARAM_OFFER_BOUGHT_NAME = "offerBoughtName";
}
