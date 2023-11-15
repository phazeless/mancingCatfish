using System;
using ACE.Analytics;
using UnityEngine;

public static class AnalyticsEvents
{
	public static void PostTutorialEvent(ProgressionStatus status)
	{
		ProgressionEvent evnt = new ProgressionEvent("Tutorial", status);
		Analytics.PostEvent(evnt);
	}

	public static void PostTutorialEvent(int step, ProgressionStatus status)
	{
		ProgressionEvent evnt = new ProgressionEvent("Tutorial:Step" + step, status);
		Analytics.PostEvent(evnt);
	}

	public static void PostQuestEvent(ProgressionStatus status)
	{
		ProgressionEvent evnt = new ProgressionEvent("Quest", status);
		Analytics.PostEvent(evnt);
	}

	public static void PostQuestEvent(int questNr, ProgressionStatus status)
	{
		ProgressionEvent evnt = new ProgressionEvent("Quest:questNr" + questNr, status);
		Analytics.PostEvent(evnt);
	}

	public static void PostLevelEvent(int level, ProgressionStatus status, int? score = null)
	{
		ProgressionEvent evnt = new ProgressionEvent("Level" + level, status);
		Analytics.PostEvent(evnt);
	}

	public static void PostLevelEvent(int level, AnalyticsEvents.ContinueType continueType)
	{
		ProgressionEvent evnt = new ProgressionEvent(string.Concat(new object[]
		{
			"Level",
			level,
			":Continue:",
			continueType
		}), ProgressionStatus.Start);
		Analytics.PostEvent(evnt);
	}

	public static void PostResourceEvent(AnalyticsEvents.REType type)
	{
		string eventID = "Resource:" + type;
		IEvent evnt = new CustomEvent(eventID);
		Analytics.PostEvent(evnt);
	}

	public static void PostResourceEvent(AnalyticsEvents.REType type, AnalyticsEvents.RECategory category, AnalyticsEvents.RECurrencyType currencyType, ResourceFlow flow, int amount)
	{
		if (amount == 0)
		{
			return;
		}
		string id = type.ToString();
		string category2 = category.ToString();
		string currency = currencyType.ToString();
		IEvent evnt = new ResourceEvent(id, category2, flow, Mathf.Abs(amount), currency);
		Analytics.PostEvent(evnt);
	}

	public static void PostPlayerEvent(AnalyticsEvents.PEType type)
	{
		CustomEvent evnt = new CustomEvent("Player:" + type);
		Analytics.PostEvent(evnt);
	}

	public static void PostGameEvent(AnalyticsEvents.GEType type)
	{
		CustomEvent evnt = new CustomEvent("Game:" + type);
		Analytics.PostEvent(evnt);
	}

	public static void PostGameEvent(AnalyticsEvents.UnlockedWallType progressType)
	{
		CustomEvent evnt = new CustomEvent(string.Concat(new object[]
		{
			"Game:",
			AnalyticsEvents.GEType.UnlockedProgression,
			":",
			progressType
		}));
		Analytics.PostEvent(evnt);
	}

	public static void PostSocialEvent(AnalyticsEvents.SEType type)
	{
		CustomEvent evnt = new CustomEvent("Social:" + type);
		Analytics.PostEvent(evnt);
	}

	public static void PostSocialEvent(AnalyticsEvents.SEType type, AnalyticsEvents.CEType connectType)
	{
		CustomEvent evnt = new CustomEvent(string.Concat(new object[]
		{
			"Social:",
			type,
			":",
			connectType
		}));
		Analytics.PostEvent(evnt);
	}

	public enum ContinueType
	{
		Ad,
		Groove
	}

	public enum REType
	{
		UnlockCrew,
		UpgradeCrew,
		Goldfish,
		Rushtime,
		SpawnEpic,
		Chest,
		DailyGift,
		GemPack,
		Special,
		WheelGem,
		Jackpot,
		Quest
	}

	public enum RECategory
	{
		Gift,
		IAP,
		Gameplay,
		Boost,
		Wheel,
		Ad
	}

	public enum RECurrencyType
	{
		Gems,
		FreeSpins,
		Crew,
		FishingExperience
	}

	public enum PEType
	{
		LevelUp
	}

	public enum GEType
	{
		GiftOpened,
		ReviewYes,
		ReviewNo,
		UnlockedProgression
	}

	public enum UnlockedWallType
	{
		Social,
		IAP
	}

	public enum SEType
	{
		Connect,
		InviteFriends
	}

	public enum CEType
	{
		HighscorePopup,
		HighscoreLabel,
		Paywall
	}
}
