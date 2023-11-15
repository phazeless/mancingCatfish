using System;
using ACE.Analytics;

public static class GameAnalyticsEvents
{
	public static void TutorialStarted(int part)
	{
		AnalyticsEvents.PostTutorialEvent(part, ProgressionStatus.Start);
	}

	public static void TutorialCompleted(int part)
	{
		AnalyticsEvents.PostTutorialEvent(part, ProgressionStatus.Complete);
	}

	public static void QuestStarted(int part)
	{
		AnalyticsEvents.PostQuestEvent(part, ProgressionStatus.Start);
	}

	public static void QuestCompleted(int part)
	{
		AnalyticsEvents.PostQuestEvent(part, ProgressionStatus.Complete);
	}

	public static void LevelStarted(int lvl)
	{
		AnalyticsEvents.PostLevelEvent(lvl, ProgressionStatus.Start, null);
	}

	public static void LevelCompleted(int lvl)
	{
		AnalyticsEvents.PostLevelEvent(lvl, ProgressionStatus.Complete, null);
	}

	public static void LevelFailed(int lvl)
	{
		AnalyticsEvents.PostLevelEvent(lvl, ProgressionStatus.Fail, null);
	}

	public static void LevelContinuedAd(int lvl)
	{
		AnalyticsEvents.PostLevelEvent(lvl, AnalyticsEvents.ContinueType.Ad);
	}

	public static void LevelContinuedGroove(int lvl)
	{
		AnalyticsEvents.PostLevelEvent(lvl, AnalyticsEvents.ContinueType.Groove);
	}

	public static void ResourceGemsIncreased(AnalyticsEvents.REType type, AnalyticsEvents.RECategory category, int amount)
	{
		AnalyticsEvents.PostResourceEvent(type, category, AnalyticsEvents.RECurrencyType.Gems, ResourceFlow.Increase, amount);
	}

	public static void ResourceGemsDecreased(AnalyticsEvents.REType type, AnalyticsEvents.RECategory category, int amount)
	{
		AnalyticsEvents.PostResourceEvent(type, category, AnalyticsEvents.RECurrencyType.Gems, ResourceFlow.Decrease, amount);
	}

	public static void ResourceFishingExperienceIncreased(AnalyticsEvents.REType type, AnalyticsEvents.RECategory category, int amount)
	{
		AnalyticsEvents.PostResourceEvent(type, category, AnalyticsEvents.RECurrencyType.FishingExperience, ResourceFlow.Increase, amount);
	}

	public static void GiftOpen()
	{
		AnalyticsEvents.PostGameEvent(AnalyticsEvents.GEType.GiftOpened);
	}

	public static void ReviewYes()
	{
		AnalyticsEvents.PostGameEvent(AnalyticsEvents.GEType.ReviewYes);
	}

	public static void ReviewNo()
	{
		AnalyticsEvents.PostGameEvent(AnalyticsEvents.GEType.ReviewNo);
	}

	public static void UnlockedProgressionWall(AnalyticsEvents.UnlockedWallType unlockType)
	{
		AnalyticsEvents.PostGameEvent(unlockType);
	}

	public static void FacebookConnect(AnalyticsEvents.CEType connectType)
	{
		AnalyticsEvents.PostSocialEvent(AnalyticsEvents.SEType.Connect, connectType);
	}

	public static void FacebookInvite()
	{
		AnalyticsEvents.PostSocialEvent(AnalyticsEvents.SEType.InviteFriends);
	}
}
