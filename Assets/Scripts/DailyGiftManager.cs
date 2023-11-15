using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class DailyGiftManager : MonoBehaviour
{
	public static DailyGiftManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<DailyGiftContent> OnDailyCatchCollectedAndShown;

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

	public int CurrentDayStreak
	{
		get
		{
			return this.currentDayStreak;
		}
	}

	public DateTime LastCollected
	{
		get
		{
			return this.lastCollectedDailyGiftAt;
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
			return (this.allowCheating || this.IsLocalTimeWithinReasonableDiffFromRealTime) && (this.lastCollectedDailyGiftAt.Year < 2018 || (this.Now.Day != this.lastCollectedDailyGiftAt.Day && this.Now > this.lastCollectedDailyGiftAt));
		}
	}

	public bool IsWithinStreak
	{
		get
		{
			return this.currentDayStreak == 0 || this.lastCollectedDailyGiftAt.Day == this.Now.AddDays(-1.0).Day;
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

	public void SetStreak(int streak)
	{
		this.currentDayStreak = streak;
	}

	private void Awake()
	{
		DailyGiftManager.Instance = this;
		this.Load();
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
			if (this.OnDailyCatchCollectedAndShown != null && this.currentDailyGiftContent != null)
			{
				this.OnDailyCatchCollectedAndShown(this.currentDailyGiftContent);
			}
			this.ign = null;
			this.ResetContent();
		}
	}

	private void CreateIGNIfAvailable()
	{
		if (this.IsGiftAvailable && QuestManager.Instance.CurrentQuest != this.firstQuest && TournamentManager.Instance != null && !TournamentManager.Instance.IsInsideTournament)
		{
			this.ign = new IGNDailyGift();
			this.ign.SetExpiration(this.GetSecondsUntilDailyGiftAvailable(true));
			InGameNotificationManager.Instance.Create<IGNDailyGift>(this.ign);
			if (this.OnDailyGiftAvailable != null)
			{
				this.OnDailyGiftAvailable(this.IsWithinStreak, this.Now.Day);
			}
		}
	}

	public List<int> GetBigRewardsDays()
	{
		int item = 1;
		int item2 = 1;
		int item3 = 1;
		this.GetComingCatch(0, 1, 3, out item);
		this.GetComingCatch(1, 1, 3, out item2);
		this.GetComingCatch(2, 1, 3, out item3);
		return new List<int>
		{
			item,
			item2,
			item3
		};
	}

	public DailyGiftContentPossibilities GetDailyGiftContentPossibilitiesForStreak(int streak)
	{
		return this.dailyGiftRewards.GetDailyGiftPossibilitiesForStreak(streak);
	}

	public void RestartStreak()
	{
		this.currentDayStreak = 0;
	}

	public void ContinueStreak()
	{
		this.lastCollectedDailyGiftAt = this.Now.AddDays(-1.0);
	}

	private DailyGiftContentPossibilities GetComingCatch(int catchIndex, int fromIndex, int toIndex, out int requiredStreak)
	{
		int num = toIndex - fromIndex;
		List<int> list = (from x in this.dailyGiftRewards.AvailableStreaks
		orderby x
		select x).ToList<int>();
		list = list.GetRange(fromIndex, toIndex);
		int num2 = list[num];
		int num3 = this.currentDayStreak / num2;
		int num4 = (catchIndex != 0) ? ((catchIndex != 2) ? (num / 2) : num) : 0;
		requiredStreak = (num3 * num2 / list[catchIndex] + 1) * list[catchIndex];
		return this.dailyGiftRewards.GetDailyGiftPossibilitiesForStreak(requiredStreak);
	}

	public DailyGiftContent OpenAndCollect()
	{
		this.Open();
		return this.Collect();
	}

	private void Open()
	{
		this.currentDayStreak++;
		this.lastCollectedDailyGiftAt = this.Now;
		this.currentDailyGiftContent = this.dailyGiftRewards.GetDailyGiftContent(this.currentDayStreak);
	}

	private DailyGiftContent Collect()
	{
		DailyGiftContent result = this.currentDailyGiftContent;
		if (this.currentDailyGiftContent != null)
		{
			if (this.currentDailyGiftContent.Chest != null)
			{
				ChestManager.Instance.CreateReceivedChest(this.currentDailyGiftContent.Chest);
			}
			if (this.currentDailyGiftContent.CrewMember != null)
			{
				PurchaseCrewMemberHandler.Instance.GetCrewMember(this.currentDailyGiftContent.CrewMember, ResourceChangeReason.DailyCatch, 0);
			}
			if (this.currentDailyGiftContent.Fish != null)
			{
				FishBook.Instance.TryAddToBook(this.currentDailyGiftContent.Fish);
			}
			if (this.currentDailyGiftContent.Item != null)
			{
				this.currentDailyGiftContent.Item.ChangeItemAmount(1, ResourceChangeReason.DailyCatch);
			}
			if (this.currentDailyGiftContent.Items != null)
			{
				if (this.currentDayStreak <= 1)
				{
					this.currentDailyGiftContent.Items.Clear();
				}
				foreach (KeyValuePair<Item, int> keyValuePair in this.currentDailyGiftContent.Items)
				{
					keyValuePair.Key.ChangeItemAmount(keyValuePair.Value, ResourceChangeReason.DailyCatch);
				}
			}
			if (this.currentDailyGiftContent.Gems > 0)
			{
				ResourceChangeData gemChangeData = new ResourceChangeData("contentId_dailyCatchReward", null, this.currentDailyGiftContent.Gems, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.DailyCatch);
				ResourceManager.Instance.GiveGems(this.currentDailyGiftContent.Gems, gemChangeData);
			}
			if (this.currentDailyGiftContent.FreeSpins > 0)
			{
				ResourceManager.Instance.GiveFreeSpin();
			}
			if (this.currentDailyGiftContent.FishingExp > 0)
			{
				this.dailyCatchFishingExpSkill.SetCurrentLevel(this.dailyCatchFishingExpSkill.CurrentLevel + this.currentDailyGiftContent.FishingExp, LevelChange.LevelUp);
			}
			if (this.currentDailyGiftContent.CrownExp > 0)
			{
				ResourceChangeData gemChangeData2 = new ResourceChangeData("contentId_dailyCatchReward", null, this.currentDailyGiftContent.CrownExp, ResourceType.CrownExp, ResourceChangeType.Earn, ResourceChangeReason.DailyCatch);
				ResourceManager.Instance.GiveCrownExp(this.currentDailyGiftContent.CrownExp, gemChangeData2);
			}
			if (this.currentDailyGiftContent.Grantable != null)
			{
				this.currentDailyGiftContent.Grantable.Grant("contentId_dailyCatchReward", ResourceChangeReason.DailyCatch);
			}
			this.ign.OverrideClearable = true;
		}
		this.RefreshServerTime();
		return result;
	}

	private void ResetContent()
	{
		this.currentDailyGiftContent = null;
	}

	public void Load()
	{
		this.currentDayStreak = EncryptedPlayerPrefs.GetInt("KEY_CURRENT_DAY_STREAK", this.currentDayStreak);
		long ticks = long.Parse(EncryptedPlayerPrefs.GetString("KEY_LAST_COLLECTED_DAILY_GIFT_AT", "0"));
		this.lastCollectedDailyGiftAt = new DateTime(ticks, DateTimeKind.Local);
	}

	public void Save()
	{
		EncryptedPlayerPrefs.SetInt("KEY_CURRENT_DAY_STREAK", this.currentDayStreak, true);
		EncryptedPlayerPrefs.SetString("KEY_LAST_COLLECTED_DAILY_GIFT_AT", this.lastCollectedDailyGiftAt.Ticks.ToString(), true);
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			this.Save();
		}
	}

	private void Update()
	{
		if (!this.HasCreatedIGN)
		{
			this.CreateIGNIfAvailable();
		}
	}

	private const string KEY_CURRENT_DAY_STREAK = "KEY_CURRENT_DAY_STREAK";

	private const string KEY_LAST_COLLECTED_DAILY_GIFT_AT = "KEY_LAST_COLLECTED_DAILY_GIFT_AT";

	private const float SECONDS_UNTIL_DAILY_GIFT_RESET = 86400f;

	private const string contentId_dailyCatchReward = "contentId_dailyCatchReward";

	[SerializeField]
	private bool allowCheating;

	[SerializeField]
	private Skill dailyCatchFishingExpSkill;

	[SerializeField]
	private DailyGiftRewards dailyGiftRewards;

	[SerializeField]
	private Quest firstQuest;

	private int currentDayStreak;

	private DateTime lastCollectedDailyGiftAt = DateTime.MinValue;

	private DailyGiftContent currentDailyGiftContent;

	public Action<bool, int> OnDailyGiftAvailable;

	private IGNDailyGift ign;

	private int addedDays;

	private DateTime actualTime = DateTime.MinValue;
}
