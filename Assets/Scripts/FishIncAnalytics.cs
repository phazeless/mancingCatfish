using System;
using ACE.IAPS;
using UnityEngine;

public class FishIncAnalytics : MonoBehaviour
{
	private void Start()
	{
		this.unlockCrewMember.OnSkillLevelUp += this.UnlockCrewMember_OnSkillLevelUp;
		this.rushTime.OnSkillActivation += this.RushTime_OnSkillActivation;
		this.goldFish.OnSkillActivation += this.GoldFish_OnSkillActivation;
		this.spawnBoss.OnSkillActivation += this.SpawnBoss_OnSkillActivation;
		this.dWProgression.OnSkillLevelUp += this.DWProgression_OnSkillLevelUp;
		this.quests.OnSkillLevelUp += this.Quests_OnSkillLevelUp;
		foreach (Skill skill in SkillManager.Instance.CrewMembers)
		{
			skill.OnSkillLevelUp += this.Item_OnSkillLevelUp;
		}
		if (this.dWProgression.CurrentLevel == 0)
		{
			GameAnalyticsEvents.LevelStarted(0);
		}
		if (this.quests.CurrentLevel == 0)
		{
			GameAnalyticsEvents.QuestStarted(0);
		}
		StoreManager storeManager = ResourceManager.StoreManager;
		storeManager.OnGoodBalanceChanged = (Action<string, int, int>)Delegate.Combine(storeManager.OnGoodBalanceChanged, new Action<string, int, int>(this.OnGoodBalanceChanged));
		this.boatTierSkill.OnSkillLevelUp += this.BoatTierSkill_OnSkillLevelUp;
	}

	private void OnGoodBalanceChanged(string itemId, int b, int c)
	{
		if (itemId == "se.ace.gem_pack_1")
		{
			GameAnalyticsEvents.ResourceGemsIncreased(AnalyticsEvents.REType.GemPack, AnalyticsEvents.RECategory.IAP, 40);
		}
		else if (itemId == "se.ace.gem_pack_2")
		{
			GameAnalyticsEvents.ResourceGemsIncreased(AnalyticsEvents.REType.GemPack, AnalyticsEvents.RECategory.IAP, 300);
		}
		else if (itemId == "se.ace.gem_pack_3")
		{
			GameAnalyticsEvents.ResourceGemsIncreased(AnalyticsEvents.REType.GemPack, AnalyticsEvents.RECategory.IAP, 800);
		}
	}

	private void Quests_OnSkillLevelUp(Skill arg1, LevelChange arg2)
	{
		GameAnalyticsEvents.QuestCompleted(arg1.PreviousLevel);
		GameAnalyticsEvents.QuestStarted(arg1.CurrentLevel);
	}

	private void DWProgression_OnSkillLevelUp(Skill arg1, LevelChange arg2)
	{
		GameAnalyticsEvents.LevelCompleted(arg1.PreviousLevel);
		GameAnalyticsEvents.LevelStarted(arg1.CurrentLevel);
	}

	private void SpawnBoss_OnSkillActivation(Skill obj)
	{
		GameAnalyticsEvents.ResourceGemsDecreased(AnalyticsEvents.REType.SpawnEpic, AnalyticsEvents.RECategory.Boost, 5);
	}

	private void GoldFish_OnSkillActivation(Skill obj)
	{
		GameAnalyticsEvents.ResourceGemsDecreased(AnalyticsEvents.REType.Goldfish, AnalyticsEvents.RECategory.Boost, 5);
	}

	private void RushTime_OnSkillActivation(Skill obj)
	{
		GameAnalyticsEvents.ResourceGemsDecreased(AnalyticsEvents.REType.Rushtime, AnalyticsEvents.RECategory.Boost, 5);
	}

	private void Item_OnSkillLevelUp(Skill arg1, LevelChange arg2)
	{
		if (arg2 != LevelChange.LevelUpFree)
		{
			if (arg2 == LevelChange.LevelUp)
			{
				GameAnalyticsEvents.ResourceGemsDecreased(AnalyticsEvents.REType.UpgradeCrew, AnalyticsEvents.RECategory.Gameplay, (int)arg1.CostForCurrentLevelUp);
			}
		}
	}

	private void UnlockCrewMember_OnSkillLevelUp(Skill arg1, LevelChange arg2)
	{
		if (arg2 == LevelChange.LevelUp)
		{
			GameAnalyticsEvents.ResourceGemsDecreased(AnalyticsEvents.REType.UnlockCrew, AnalyticsEvents.RECategory.Gameplay, (int)arg1.CostForCurrentLevelUp);
			LionAnalytics.IncreaseAndTrackCrewMemberAmountReached(arg1.CurrentLevel);
		}
	}

	private void BoatTierSkill_OnSkillLevelUp(Skill arg1, LevelChange arg2)
	{
		if (arg2 == LevelChange.LevelUp)
		{
			LionAnalytics.IncreaseAndTrackBoatLevelReached(arg1.CurrentLevel);
		}
	}

	[SerializeField]
	private Skill unlockCrewMember;

	[SerializeField]
	private Skill rushTime;

	[SerializeField]
	private Skill goldFish;

	[SerializeField]
	private Skill spawnBoss;

	[SerializeField]
	private Skill dWProgression;

	[SerializeField]
	private Skill quests;

	[SerializeField]
	private Skill boatTierSkill;
}
