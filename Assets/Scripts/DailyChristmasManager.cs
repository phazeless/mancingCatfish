using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DailyChristmasManager : MonoBehaviour
{
	public static DailyChristmasManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<DailyGiftContent> OnChristmasGiftCollectedAndShown;

	public DateTime Now
	{
		get
		{
			return DateTime.Now.AddDays((double)this.addedDays);
		}
	}

	public bool HasCreatedIGN
	{
		get
		{
			return this.ign != null;
		}
	}

	public DateTime LastCollected
	{
		get
		{
			return this.lastCollectedGiftAt;
		}
	}

	public float GetSecondsUntilDailyGiftAvailable(bool useNextDayIfAvailableToday)
	{
		if (this.IsGiftAvailable && !useNextDayIfAvailableToday)
		{
			return 0f;
		}
		DateTime now = this.Now;
		DateTime date = now.AddDays(1.0).Date;
		return (float)(date - now).TotalSeconds;
	}

	public bool IsGiftAvailable
	{
		get
		{
			return (this.allowCheating || this.IsLocalTimeWithinReasonableDiffFromRealTime) && this.IsChristmasPeriod && (this.lastCollectedGiftAt.Year < 2018 || (this.Now.Day != this.lastCollectedGiftAt.Day && this.Now > this.lastCollectedGiftAt));
		}
	}

	public bool IsLocalTimeWithinReasonableDiffFromRealTime
	{
		get
		{
			DateTime d = this.Now.ToUniversalTime();
			TimeSpan timeSpan = d - this.actualTime;
			return timeSpan.TotalHours > -13.0 && timeSpan.TotalHours <= 13.0;
		}
	}

	private bool IsChristmasPeriod
	{
		get
		{
			DateTime now = this.Now;
			return now.Month == 12 && now.Day <= 25;
		}
	}

	private void Awake()
	{
		DailyChristmasManager.Instance = this;
		long ticks = long.Parse(EncryptedPlayerPrefs.GetString("KEY_LAST_COLLECTED_CHRISTMAS_GIFT", "0"));
		this.lastCollectedGiftAt = new DateTime(ticks, DateTimeKind.Local);
	}

	private void Start()
	{
		InGameNotificationManager.Instance.OnInGameNotificationRemoved += this.Instance_OnInGameNotificationRemoved;
		this.RefreshServerTime();
	}

	private void RefreshServerTime()
	{
		FHelper.GetTime(true, delegate(bool succeed, DateTime realTime)
		{
			if (succeed)
			{
				this.actualTime = realTime.ToUniversalTime();
			}
		});
	}

	private void Instance_OnInGameNotificationRemoved(InGameNotification removedIgn)
	{
		if (removedIgn == this.ign)
		{
			if (this.OnChristmasGiftCollectedAndShown != null && this.currentContent != null)
			{
				this.OnChristmasGiftCollectedAndShown(this.currentContent);
			}
			this.ign = null;
			this.ResetContent();
		}
	}

	private void CreateIGNIfAvailable()
	{
		if (this.IsGiftAvailable && QuestManager.Instance.CurrentQuest != this.firstQuest && TournamentManager.Instance != null && !TournamentManager.Instance.IsInsideTournament)
		{
			this.ign = new IGNChristmasGift();
			this.ign.SetExpiration(this.GetSecondsUntilDailyGiftAvailable(true));
			InGameNotificationManager.Instance.Create<IGNChristmasGift>(this.ign);
			if (this.OnChristmasGiftAvailable != null)
			{
				this.OnChristmasGiftAvailable(this.Now.Day);
			}
		}
	}

	public DailyGiftContentPossibilities GetDailyGiftContentPossibilitiesForStreak(int streak)
	{
		return this.dailyRewards.GetDailyGiftPossibilitiesForStreak(streak);
	}

	public DailyGiftContent OpenAndCollect()
	{
		this.Open();
		return this.Collect();
	}

	private void Open()
	{
		this.lastCollectedGiftAt = this.Now;
		this.currentContent = this.dailyRewards.GetDailyGiftContent(this.Now.Day);
	}

	private DailyGiftContent Collect()
	{
		DailyGiftContent result = this.currentContent;
		if (this.currentContent != null)
		{
			if (this.currentContent.Chest != null)
			{
				ChestManager.Instance.CreateReceivedChest(this.currentContent.Chest);
			}
			if (this.currentContent.CrewMember != null)
			{
				PurchaseCrewMemberHandler.Instance.GetCrewMember(this.currentContent.CrewMember, ResourceChangeReason.ChristmasCalendarDailyReward, 0);
			}
			if (this.currentContent.Fish != null)
			{
				FishBook.Instance.TryAddToBook(this.currentContent.Fish);
			}
			if (this.currentContent.Item != null)
			{
				this.currentContent.Item.ChangeItemAmount(1, ResourceChangeReason.ChristmasCalendarDailyReward);
			}
			if (this.currentContent.Items != null)
			{
				foreach (KeyValuePair<Item, int> keyValuePair in this.currentContent.Items)
				{
					keyValuePair.Key.ChangeItemAmount(keyValuePair.Value, ResourceChangeReason.ChristmasCalendarDailyReward);
				}
			}
			if (this.currentContent.Gems > 0)
			{
				ResourceChangeData gemChangeData = new ResourceChangeData("contentId_christmasCalendarDailyReward", null, this.currentContent.Gems, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.ChristmasCalendarDailyReward);
				ResourceManager.Instance.GiveGems(this.currentContent.Gems, gemChangeData);
			}
			if (this.currentContent.FreeSpins > 0)
			{
				ResourceManager.Instance.GiveFreeSpin();
			}
			if (this.currentContent.FishingExp > 0)
			{
				this.giftFishingExpSkill.SetCurrentLevel(this.giftFishingExpSkill.CurrentLevel + this.currentContent.FishingExp, LevelChange.LevelUp);
			}
			this.ign.OverrideClearable = true;
		}
		this.RefreshServerTime();
		return result;
	}

	private void ResetContent()
	{
		this.currentContent = null;
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			EncryptedPlayerPrefs.SetString("KEY_LAST_COLLECTED_CHRISTMAS_GIFT", this.lastCollectedGiftAt.Ticks.ToString(), true);
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.A))
		{
			this.addedDays++;
			if (this.ign != null)
			{
				this.ign.SetExpiration(0f);
			}
		}
		else if (UnityEngine.Input.GetKeyUp(KeyCode.B))
		{
			this.addedDays--;
			if (this.ign != null)
			{
				this.ign.SetExpiration(0f);
			}
		}
		if (!this.HasCreatedIGN)
		{
			this.CreateIGNIfAvailable();
		}
	}

	private const string KEY_LAST_COLLECTED_CHRISTMAS_GIFT = "KEY_LAST_COLLECTED_CHRISTMAS_GIFT";

	private const float SECONDS_UNTIL_DAILY_GIFT_RESET = 86400f;

	private const string contentId_christmasCalendarDailyReward = "contentId_christmasCalendarDailyReward";

	[SerializeField]
	private bool allowCheating;

	[SerializeField]
	private Skill giftFishingExpSkill;

	[SerializeField]
	private DailyGiftRewards dailyRewards;

	[SerializeField]
	private Quest firstQuest;

	private DateTime lastCollectedGiftAt = DateTime.MinValue;

	private DailyGiftContent currentContent;

	public Action<int> OnChristmasGiftAvailable;

	private IGNChristmasGift ign;

	private int addedDays;

	private DateTime actualTime = DateTime.MinValue;
}
