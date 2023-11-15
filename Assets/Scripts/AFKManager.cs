using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using ACE.Notifications;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class AFKManager : BaseAFKManager
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BigInteger, bool> OnAFKCashCollected;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<UIAfkFishingDialog> OnAfkDialogClosed;

	public static AFKManager Instance { get; private set; }

	protected void Awake()
	{
		AFKManager.Instance = this;
		this.afkDialog.OnClose += this.OnAFKDialogClose;
		this.pushManager = new PushManager(new PushwooshPushProvider(base.GetComponent<PushwooshPushProviderSettings>()));
		PushManager pushManager = this.pushManager;
		pushManager.OnPushNotificationsReceived = (Action<string>)Delegate.Combine(pushManager.OnPushNotificationsReceived, new Action<string>(this.OnPushNotificationReceived));
		this.pushManager.Initialize();
	}

	protected override void Start()
	{
		base.Start();
		FishAdManager.Instance.OnAdAvailable += this.Instance_OnAdAvailable;
	}

	private void Instance_OnAdAvailable()
	{
	}

	private void OnAFKDialogClose()
	{
		this.CollectAFKCash();
		AudioManager.Instance.Cacthing();
		if (this.OnAfkDialogClosed != null)
		{
			this.OnAfkDialogClosed(this.afkDialog);
		}
	}

	protected override void OnUserReturn(bool fromAppRestart, DateTime time, float afkTimeInSeconds)
	{
		base.OnUserReturn(fromAppRestart, time, afkTimeInSeconds);
		if (this.afkDialog.IsOpen && (double)afkTimeInSeconds >= this.afkThresholdInSeconds)
		{
			this.RunAfterDelay(1f, delegate()
			{
				this.afkDialog.Close(false);
				this.HandleSomeAfkCashThings();
			});
		}
		else
		{
			this.HandleSomeAfkCashThings();
		}
	}

	private void HandleSomeAfkCashThings()
	{
		this.afkTotalGPM = this.CurrentGPM;
		if (this.afkGeneratedCash == 0L)
		{
			float currentTotalValueFor = SkillManager.Instance.GetCurrentTotalValueFor<Skills.AfkCollectedValueIncrease>();
			this.afkGeneratedCash = this.afkTotalGPM.MultiplyFloat((float)this.CappedAFKTimeInSeconds / 60f * currentTotalValueFor);
			if (this.HasReachedAFKThreshold)
			{
				this.RunAfterDelay(2f, delegate()
				{
					if (!TournamentManager.Instance.IsInsideTournament)
					{
						this.ShowAFKFishingDialog();
					}
					else
					{
						this.afkGeneratedCash = 0;
					}
				});
			}
			else
			{
				this.CollectAFKCash();
			}
		}
		else
		{
			float currentTotalValueFor2 = SkillManager.Instance.GetCurrentTotalValueFor<Skills.AfkCollectedValueIncrease>();
			BigInteger bigInteger = this.afkTotalGPM.MultiplyFloat((float)this.CappedAFKTimeInSeconds / 60f * currentTotalValueFor2);
			this.CollectAFKCash(ref bigInteger);
		}
		this.pushManager.ClearLocalNotifications();
		this.HandlePressedNotification();
	}

	protected override void OnUserLeave(DateTime time, bool fromApplicationQuit)
	{
		base.OnUserLeave(time, fromApplicationQuit);
		if (!fromApplicationQuit)
		{
			if (this.afkSkillUnlocked.CurrentLevel > 0)
			{
				this.pushManager.ScheduleLocalNotification("Ahoy! Your boat is full! Get in here so we can keep fishing! \ud83d\udc1f\ud83d\udc1f", DateTime.Now.AddSeconds((double)this.MaxAllowedAfkTimeInSeconds), AFKManager.NOTIFICATION_FULLCAPACITY);
			}
			if (SkillManager.Instance.FishValueBonusSkill.IsActivated)
			{
				this.pushManager.ScheduleLocalNotification("Your Fishing Luck has expired. \ud83c\udf40 It's time to spin the wheel!", DateTime.Now.AddSeconds((double)SkillManager.Instance.FishValueBonusSkill.GetTotalSecondsLeftOnDuration()), AFKManager.NOTIFICATION_WHEELBONUS);
			}
			int num = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.ClaimLobsterAfterHours>();
			if (this.crewCatchLobster.CurrentLevel > 0 && this.crewCatchLobster.IsOnCooldown)
			{
				this.pushManager.ScheduleLocalNotification("Miss " + this.crewCatchLobster.GetExtraInfo().TitleText + " caught something. Go check it out! \ud83e\udd90", DateTime.Now.AddSeconds((double)this.crewCatchLobster.GetTotalSecondsLeftOnCooldown()), AFKManager.NOTIFICATION_LOBSTER);
			}
			if (TournamentManager.Instance.HasTournament && TournamentManager.Instance.HasUserJoinedCurrentTournament && !TournamentManager.Instance.CurrentTournament.HasEnded)
			{
				this.pushManager.ScheduleLocalNotification("The tournament has ended. Check to see if you've won!", TournamentManager.Instance.CurrentTournament.EndTime.AddMinutes(12.0).ToLocalTime(), AFKManager.NOTIFICATION_TOURNAMENTENDED);
			}
			if (SpecialOfferManager.Instance.HasInitializedSpecialOffers)
			{
				DateTime date = SpecialOfferManager.Instance.WhenIsNextSpecialOffer(SpecialOffer.SpecialOfferShowType.DayOfWeek);
				this.pushManager.ScheduleLocalNotification("There's a Special Offer for you in the Shop. Check it out! \ud83d\udc48\ud83d\ude09", date, AFKManager.NOTIFICATION_SPECIALOFFER);
			}
			if (ChestManager.Instance != null)
			{
				List<DateTime> unlockedTimesForRecievedChests = ChestManager.Instance.GetUnlockedTimesForRecievedChests();
				if (unlockedTimesForRecievedChests.Count > 0)
				{
					unlockedTimesForRecievedChests.Sort();
					DateTime dateTime = unlockedTimesForRecievedChests[unlockedTimesForRecievedChests.Count - 1];
					if (dateTime > DateTime.Now)
					{
						this.pushManager.ScheduleLocalNotification("All your Boxes are unlocked! ✨", dateTime, AFKManager.NOTIFICATION_UNLOCKEDCHEST);
					}
				}
			}
			if (!SpecialChestOfferManager.Instance.HasSpecialChestOffer && SkillManager.Instance.DeepWaterSkill.LifetimeLevel >= SpecialChestOfferManager.Instance.DwLvlThreshold)
			{
				this.pushManager.ScheduleLocalNotification("Killer deal on a Legendary Box. Grab it, before it's gone! ✨", SpecialChestOfferManager.Instance.DayOfMonthToCreateOffer, AFKManager.NOTIFICATION_SPECIALOFFER);
			}
			bool isGiftAvailable = DailyGiftManager.Instance.IsGiftAvailable;
			if (isGiftAvailable)
			{
				this.pushManager.ScheduleLocalNotification("Oops! You forgot your daily catch \ud83c\udfa3", DateTime.Now.AddSeconds(70.0), AFKManager.NOTIFICATION_DAILYGIFT);
			}
			else
			{
				DateTime date2 = DailyGiftManager.Instance.LastCollected.AddHours(24.0);
				this.pushManager.ScheduleLocalNotification("Your daily catch is available! \ud83c\udfa3", date2, AFKManager.NOTIFICATION_DAILYGIFT);
			}
			if (this.valentineOffer != null && !HolidayOfferManager.Instance.IsBought(this.valentineOffer))
			{
				DateTime dateTime2 = new DateTime(DateTime.Now.Year, 2, 14, 18, 0, 0);
				if (DateTime.Now < dateTime2)
				{
					this.pushManager.ScheduleLocalNotification("Happy Valentine's Day! Don't forget to check out the Valentine's deal in the shop!", dateTime2, AFKManager.NOTIFICATION_SPECIALOFFER);
				}
			}
			if (this.fourthJulyEvent != null && TimeManager.Instance.IsWithinPeriodLocal(this.fourthJulyEvent.Availability))
			{
				bool flag = this.fourthJulyEvent.SpecialOfferForEvent != null && !HolidayOfferManager.Instance.IsBought(this.fourthJulyEvent.SpecialOfferForEvent);
				DateTime dateTime3 = new DateTime(DateTime.Now.Year, 7, 4, 12, 0, 0);
				if (DateTime.Now < dateTime3)
				{
					if (flag)
					{
						this.pushManager.ScheduleLocalNotification("Happy 4th of July! Don't miss out on the Super Special deal during the event! \ud83c\udf81", dateTime3, AFKManager.NOTIFICATION_SPECIALOFFER);
					}
					else
					{
						this.pushManager.ScheduleLocalNotification("Happy 4th of July! Hope you're having a great time!", dateTime3, AFKManager.NOTIFICATION_SPECIALOFFER);
					}
				}
				DateTime expireDate = this.fourthJulyEvent.Availability.GetExpireDate();
				DateTime t = expireDate.AddDays(-1.0);
				DateTime t2 = expireDate.AddDays(-2.0);
				DateTime t3 = expireDate.AddDays(-3.0);
				if (DateTime.Now < t3)
				{
					DateTime date3 = new DateTime(t3.Year, t3.Month, t3.Day, 12, 0, 0);
					this.pushManager.ScheduleLocalNotification("Peeew! \ud83d\udca5 Was that a Firework? Don't forget to use yours too!", date3, AFKManager.NOTIFICATION_SPECIALOFFER);
				}
				if (DateTime.Now < t2)
				{
					DateTime date4 = new DateTime(t2.Year, t2.Month, t2.Day, 12, 0, 0);
					this.pushManager.ScheduleLocalNotification("Slacking are we? Go finish your Independence Day Challenges!", date4, AFKManager.NOTIFICATION_SPECIALOFFER);
				}
				if (DateTime.Now < t)
				{
					DateTime date5 = new DateTime(t.Year, t.Month, t.Day, 9, 0, 0);
					this.pushManager.ScheduleLocalNotification("Hurry!! ⏰ The event is almost over. Finish your challenges and claim your rewards!!", date5, AFKManager.NOTIFICATION_SPECIALOFFER);
				}
			}
		}
	}

	private void ShowAFKFishingDialog()
	{
		if (this.afkGeneratedCash <= 0L && this.afkSkillUnlocked.CurrentLevel <= 0)
		{
			return;
		}
		if (this.afkDialog.IsOpen)
		{
			this.CollectAFKCash();
		}
		else
		{
			this.afkDialog.Open();
			this.afkDialog.SetUpDialog(this.afkGeneratedCash, this.lastAFKTimeInSeconds, SkillManager.Instance.GetCurrentTotalValueFor<Skills.CapacityAfkHours>());
		}
	}

	private void CollectAFKCash()
	{
		this.CollectAFKCash(ref this.afkGeneratedCash);
	}

	private void CollectAFKCash(ref BigInteger value)
	{
		if (this.OnAFKCashCollected == null)
		{
			throw new InvalidOperationException("OnAFKCashCollected is missing a listener. At the very least ResourceManager should listen to the event.");
		}
		if (this.OnAFKCashCollected != null)
		{
			BigInteger arg = (!this.ShouldDoubleUp) ? value : (value * this.DoubleUpMultiplier);
			this.OnAFKCashCollected(arg, this.ShouldDoubleUp);
			value = 0;
		}
	}

	private void HandlePressedNotification()
	{
		if (!(this.lastPressedNotification == AFKManager.NOTIFICATION_DAILYGIFT))
		{
			if (!(this.lastPressedNotification == AFKManager.NOTIFICATION_LOBSTER))
			{
				if (this.lastPressedNotification == AFKManager.NOTIFICATION_WHEELBONUS)
				{
					if (TournamentManager.Instance != null && !TournamentManager.Instance.IsInsideTournament)
					{
						ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Shop);
					}
				}
				else if (this.lastPressedNotification == AFKManager.NOTIFICATION_SPECIALOFFER)
				{
					bool flag = SpecialOfferManager.Instance != null && SpecialOfferManager.Instance.IsAnySpecialOfferActive;
					bool flag2 = HolidayOfferManager.Instance != null && HolidayOfferManager.Instance.IsAnyOffersActive();
					if ((flag || flag2) && TournamentManager.Instance != null && !TournamentManager.Instance.IsInsideTournament)
					{
						ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Shop);
					}
				}
			}
		}
		this.lastPressedNotification = null;
	}

	private void OnPushNotificationReceived(string payload)
	{
		Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
		if (payload != null && dictionary.ContainsKey("u"))
		{
			payload = (string)dictionary["u"];
		}
		this.lastPressedNotification = payload;
		this.HandlePressedNotification();
	}

	public void RegisterGPMGenerator(IHasGPM hasGPM)
	{
		this.hasGPMObjects.Add(hasGPM);
	}

	public void UnregisterGPMGenerator(IHasGPM hasGPM)
	{
		this.hasGPMObjects.Remove(hasGPM);
	}

	public BigInteger CurrentGPM
	{
		get
		{
			BigInteger bigInteger = 0;
			foreach (IHasGPM hasGPM in this.hasGPMObjects)
			{
				bigInteger += hasGPM.GetGPM();
			}
			return bigInteger;
		}
	}

	public int DoubleUpMultiplier
	{
		get
		{
			return this.afkDialog.GetIdleDoubleUpMultiplier();
		}
	}

	public bool ShouldDoubleUp
	{
		get
		{
			return this.afkDialog.didDoubleUp;
		}
	}

	public bool HasReachedAFKThreshold
	{
		get
		{
			return (double)this.CappedAFKTimeInSeconds > this.afkThresholdInSeconds;
		}
	}

	public int FullAFKTimeInSeconds
	{
		get
		{
			return (int)this.lastAFKTimeInSeconds;
		}
	}

	public float MaxAllowedAfkTimeInSeconds
	{
		get
		{
			return SkillManager.Instance.GetCurrentTotalValueFor<Skills.CapacityAfkHours>() * 60f * 60f;
		}
	}

	public int CappedAFKTimeInSeconds
	{
		get
		{
			return (int)Mathf.Min(this.MaxAllowedAfkTimeInSeconds, (float)this.FullAFKTimeInSeconds);
		}
	}

	[SerializeField]
	private ScrollRect harborScrollRect;

	[SerializeField]
	private Skill afkSkillUnlocked;

	[SerializeField]
	private UIAfkFishingDialog afkDialog;

	[SerializeField]
	private double afkThresholdInSeconds;

	[SerializeField]
	private Skill crewCatchLobster;

	[SerializeField]
	private Skill dailyGift;

	[SerializeField]
	private HolidayOffer valentineOffer;

	[SerializeField]
	private EventContent fourthJulyEvent;

	private BigInteger afkGeneratedCash = 0;

	private BigInteger afkTotalGPM = 0;

	private List<IHasGPM> hasGPMObjects = new List<IHasGPM>();

	private PushManager pushManager;

	private static readonly string NOTIFICATION_FULLCAPACITY = "FullCapacity";

	private static readonly string NOTIFICATION_WHEELBONUS = "WheelBonus";

	private static readonly string NOTIFICATION_DAILYGIFT = "DailyGift";

	private static readonly string NOTIFICATION_SPECIALOFFER = "SpecialOffer";

	private static readonly string NOTIFICATION_LOBSTER = "Lobster";

	private static readonly string NOTIFICATION_TOURNAMENTENDED = "TournamentEnded";

	private static readonly string NOTIFICATION_UNLOCKEDCHEST = "UnlockedChest";

	private string lastPressedNotification;
}
