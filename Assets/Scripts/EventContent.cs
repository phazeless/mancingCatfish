using System;
using System.Collections.Generic;
using System.Diagnostics;
using FullInspector;
using UnityEngine;

public class EventContent : ScriptableObjectWithId
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<EventContent, int, int> GlobalOnEventTreasureUnlocked;

	public bool HasUnlockedBigReward
	{
		get
		{
			return this.hasUnlockedBigReward;
		}
	}

	public int SecondsLeftOnEvent
	{
		get
		{
			return Mathf.Max(0, (int)(this.Availability.GetExpireDate().ToUniversalTime() - TimeManager.Instance.RealNow).TotalSeconds);
		}
	}

	public bool IsWithinEventPeriod
	{
		get
		{
			return TimeManager.Instance.IsWithinPeriod(this.Availability);
		}
	}

	public bool HasCompletedRequiredQuest
	{
		get
		{
			return this.QuestNeededUntilActivated == null || QuestManager.Instance.IsClaimed(this.QuestNeededUntilActivated);
		}
	}

	public DateTime LastShownEventInfo
	{
		get
		{
			return this.lastShowedEventInfo;
		}
		set
		{
			this.lastShowedEventInfo = value;
		}
	}

	public void Grant(int gemsSpent)
	{
		for (int i = 0; i < this.BigRewardContents.Count; i++)
		{
			this.BigRewardContents[i].Grant(this.Name, ResourceChangeReason.UnlockEventTreasure);
		}
		this.hasUnlockedBigReward = true;
		int amount = ConsumableManager.Instance.GetAmount(this.ReductionConsumable);
		ConsumableManager.Instance.ConsumeAll(this.ReductionConsumable, ResourceChangeReason.UnlockEventTreasure);
		this.Save();
		if (EventContent.GlobalOnEventTreasureUnlocked != null)
		{
			EventContent.GlobalOnEventTreasureUnlocked(this, gemsSpent, amount);
		}
	}

	public EventContent Create()
	{
		EventContent eventContent = UnityEngine.Object.Instantiate<EventContent>(this);
		eventContent.Challenges.Clear();
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			EventChallenge item = this.Challenges[i].Create(eventContent);
			eventContent.Challenges.Add(item);
		}
		return eventContent;
	}

	public void Save()
	{
		EncryptedPlayerPrefs.SetInt(this.GetSaveKeyForHasReceivedEventStartingBonus(), (!this.hasReceivedEventStartingBonus) ? 0 : 1, true);
		EncryptedPlayerPrefs.SetInt(this.GetSaveKeyForHasUnlockedBigReward(), (!this.hasUnlockedBigReward) ? 0 : 1, true);
		EncryptedPlayerPrefs.SetString(this.GetSaveKeyForLastShowedEventInfo(), this.lastShowedEventInfo.Ticks.ToString(), true);
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			this.Challenges[i].Save();
		}
	}

	public void Load()
	{
		this.hasReceivedEventStartingBonus = (EncryptedPlayerPrefs.GetInt(this.GetSaveKeyForHasReceivedEventStartingBonus(), 0) == 1);
		this.hasUnlockedBigReward = (EncryptedPlayerPrefs.GetInt(this.GetSaveKeyForHasUnlockedBigReward(), 0) == 1);
		this.lastShowedEventInfo = new DateTime(long.Parse(EncryptedPlayerPrefs.GetString(this.GetSaveKeyForLastShowedEventInfo(), "0")));
		if (!this.hasReceivedEventStartingBonus)
		{
			this.StartingBonus.Grant(this.StartingBonus.name, ResourceChangeReason.EventStartingBonus);
			this.hasReceivedEventStartingBonus = true;
		}
	}

	public void Clear()
	{
		EncryptedPlayerPrefs.DeleteKey(this.GetSaveKeyForHasReceivedEventStartingBonus());
		EncryptedPlayerPrefs.DeleteKey(this.GetSaveKeyForHasUnlockedBigReward());
		EncryptedPlayerPrefs.DeleteKey(this.GetSaveKeyForLastShowedEventInfo());
		ConsumableManager.Instance.ConsumeAll(this.ReductionConsumable, ResourceChangeReason.EventEnded);
		EventContent eventContent = this.Create();
		for (int i = 0; i < eventContent.Challenges.Count; i++)
		{
			eventContent.Challenges[i].Clear(eventContent);
		}
	}

	private string GetSaveKeyForHasReceivedEventStartingBonus()
	{
		return "KEY_HAS_RECEIVED_EVENT_STARTING_BONUS" + base.Id;
	}

	private string GetSaveKeyForHasUnlockedBigReward()
	{
		return "KEY_HAS_UNLOCKED_BIG_REWARD_" + base.Id;
	}

	private string GetSaveKeyForLastShowedEventInfo()
	{
		return "KEY_LAST_SHOWED_EVENT_INFO_" + base.Id;
	}

	[InspectorHeader("Color Theme")]
	public Color ColorMain = Color.black;

	public Color ColorMainLighter = Color.black;

	public Color ColorMainBackground = Color.black;

	public Color ColorTextOnMainBackground = Color.black;

	[InspectorHeader("Other")]
	public string Name;

	public HolidayOfferAvailability Availability;

	public EventReductionTicket ReductionConsumable;

	public GrantableConsumable StartingBonus;

	public GameObject IGNIconPrefab;

	public GameObject RewardBasketPrefab;

	public IGNEventChallengesDialog IGNEventChallengesDialogPrefab;

	public UIEventChallenge UIEventChallengePrefab;

	public GameObject InfoContentPrefab;

	public string Title;

	public string SubTitle;

	public string Description;

	public int BigRewardInitialCost = 1000;

	public HolidayOffer SpecialOfferForEvent;

	public Quest QuestNeededUntilActivated;

	public List<BaseGrantable> BigRewardContents = new List<BaseGrantable>();

	public List<EventChallenge> Challenges = new List<EventChallenge>();

	[NonSerialized]
	private bool hasUnlockedBigReward;

	[NonSerialized]
	private bool hasReceivedEventStartingBonus;

	[NonSerialized]
	private DateTime lastShowedEventInfo = DateTime.Now;
}
