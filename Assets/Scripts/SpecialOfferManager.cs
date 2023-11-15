using System;
using System.Collections.Generic;
using System.Diagnostics;
using FullInspector;
using UnityEngine;

[fiInspectorOnly]
public class SpecialOfferManager : MonoBehaviour
{
	public static SpecialOfferManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<SpecialOffer, bool> OnSpecialOfferAvailable;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<SpecialOffer> OnSpecialOfferDurationEnd;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<SpecialOffer, SpecialOffer.Content> OnSpecialOfferClaimed;

	public bool HasInitializedSpecialOffers { get; private set; }

	private void Awake()
	{
		SpecialOfferManager.Instance = this;
		AFKManager.Instance.OnUserLeaveCallback += this.Instance_OnUserLeaveCallback;
		this.weeklySpecialOffers = this.availableSpecialOffers.FindAll((SpecialOffer x) => x.IsDayShowType);
		this.currentWeeklyOfferIndex = EncryptedPlayerPrefs.GetInt(SpecialOfferManager.KEY_CURRENT_SPECIAL_OFFER_INDEX);
		string @string = EncryptedPlayerPrefs.GetString(SpecialOfferManager.KEY_ENDTIME_FOR_LAST_WEEKLY_SPECIAL_OFFER, "0");
		this.endTimeForLastWeeklySpecialOffer = new DateTime(long.Parse(@string));
		foreach (SpecialOffer specialOffer in this.availableSpecialOffers)
		{
			specialOffer.OnSpecialOfferDurationEnd += this.Offer_OnSpecialOfferDurationEnd;
			specialOffer.OnSpecialOfferClaimed += this.Offer_OnSpecialOfferClaimed;
			specialOffer.Awake();
		}
	}

	private void Start()
	{
		bool flag = false;
		this.LoadSpecialOffers();
		for (int i = 0; i < this.weeklySpecialOffers.Count; i++)
		{
			int offset = (this.weeklySpecialOffers.Count - (this.currentWeeklyOfferIndex - i)) % this.weeklySpecialOffers.Count;
			SpecialOffer specialOffer = this.weeklySpecialOffers[i];
			specialOffer.CalculateNextActivationAndEndDate(offset, false);
			if (this.endTimeForLastWeeklySpecialOffer >= specialOffer.NextActivationDate && this.endTimeForLastWeeklySpecialOffer <= specialOffer.NextActivationEndDate)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			for (int j = 0; j < this.weeklySpecialOffers.Count; j++)
			{
				int offset2 = (this.weeklySpecialOffers.Count - (this.currentWeeklyOfferIndex - j)) % this.weeklySpecialOffers.Count + 1;
				SpecialOffer specialOffer2 = this.weeklySpecialOffers[j];
				specialOffer2.CalculateNextActivationAndEndDate(offset2, false);
			}
		}
		this.HasInitializedSpecialOffers = true;
	}

	private void IncreaseShowTypDayCounter()
	{
		this.currentWeeklyOfferIndex++;
		this.currentWeeklyOfferIndex %= this.weeklySpecialOffers.Count;
	}

	private void Offer_OnSpecialOfferClaimed(SpecialOffer offer, SpecialOffer.Content content)
	{
		if (this.OnSpecialOfferClaimed != null)
		{
			this.OnSpecialOfferClaimed(offer, content);
		}
		GameAnalyticsEvents.ResourceGemsIncreased(AnalyticsEvents.REType.Special, AnalyticsEvents.RECategory.IAP, content.GemAmount);
		ResourceChangeData gemChangeData = new ResourceChangeData(offer.Id, offer.OfferName, content.GemAmount, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.PurchaseSpecialOffer);
		ResourceManager.Instance.GiveGems(content.GemAmount, gemChangeData);
		ResourceChangeData changeData = new ResourceChangeData(offer.Id, offer.OfferName, 0, ResourceType.CrownExp, ResourceChangeType.Earn, ResourceChangeReason.PurchaseSpecialOffer);
		CrownExpGranterManager.Instance.Grant(offer.IAPPlacement, changeData);
		if (content.FreeSpins > 0)
		{
			ResourceManager.Instance.GiveFreeSpin();
		}
		foreach (Skill crewMember in content.CrewMembers)
		{
			PurchaseCrewMemberHandler.Instance.GetCrewMember(crewMember, ResourceChangeReason.PurchaseSpecialOffer, 0);
		}
	}

	private void Offer_OnSpecialOfferDurationEnd(SpecialOffer specialOffer)
	{
		this.activeSpecialOffers.Remove(specialOffer);
		if (this.OnSpecialOfferDurationEnd != null)
		{
			this.OnSpecialOfferDurationEnd(specialOffer);
		}
		if (this.ignSpecialOffer != null)
		{
			this.ignSpecialOffer.SetExpiration(0f);
		}
		if (specialOffer.IsDayShowType)
		{
			this.IncreaseShowTypDayCounter();
			this.endTimeForLastWeeklySpecialOffer = specialOffer.NextActivationEndDate;
		}
	}

	private void LoadSpecialOffers()
	{
		foreach (SpecialOffer specialOffer in this.availableSpecialOffers)
		{
			SpecialOffer.SpecialOfferLoadState specialOfferLoadState = specialOffer.Load();
			if (specialOfferLoadState == SpecialOffer.SpecialOfferLoadState.FoundAndActive)
			{
				this.Create(specialOffer, false);
			}
			else if (specialOfferLoadState == SpecialOffer.SpecialOfferLoadState.FoundAndExpired)
			{
				specialOffer.Update();
			}
		}
	}

	private void Instance_OnUserLeaveCallback(DateTime arg1, bool fromApplicationQuit)
	{
		if (fromApplicationQuit)
		{
			return;
		}
		foreach (SpecialOffer specialOffer in this.activeSpecialOffers)
		{
			specialOffer.Save();
		}
		EncryptedPlayerPrefs.SetInt(SpecialOfferManager.KEY_CURRENT_SPECIAL_OFFER_INDEX, this.currentWeeklyOfferIndex, true);
		EncryptedPlayerPrefs.SetString(SpecialOfferManager.KEY_ENDTIME_FOR_LAST_WEEKLY_SPECIAL_OFFER, this.endTimeForLastWeeklySpecialOffer.Ticks.ToString(), true);
	}

	public void Create(SpecialOffer offer, bool shouldActivate = true)
	{
		if (!this.IsSpecialOfferActive(offer))
		{
			this.activeSpecialOffers.Add(offer);
			if (shouldActivate)
			{
				offer.Activate();
				this.ignSpecialOffer = InGameNotificationManager.Instance.Create<IGNSpecialOffer>(new IGNSpecialOffer(offer));
			}
			if (this.OnSpecialOfferAvailable != null)
			{
				this.OnSpecialOfferAvailable(offer, shouldActivate);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("Trying to add Special Offer (" + offer.OfferName + ") that is already active!");
		}
	}

	public bool IsSpecialOfferActive(SpecialOffer specialOffer)
	{
		return this.activeSpecialOffers.Contains(specialOffer);
	}

	public bool IsAnySpecialOfferActive
	{
		get
		{
			return this.activeSpecialOffers.Count > 0;
		}
	}

	public DateTime WhenIsNextSpecialOffer(SpecialOffer.SpecialOfferShowType showType)
	{
		return this.GetNextWeeklySpecialOffer().NextActivationDate;
	}

	private void Update()
	{
		for (int i = 0; i < this.activeSpecialOffers.Count; i++)
		{
			this.activeSpecialOffers[i].Update();
		}
		for (int j = 0; j < this.availableSpecialOffers.Count; j++)
		{
			SpecialOffer offer = this.availableSpecialOffers[j];
			if (this.ShouldCreateOffer(offer))
			{
				this.Create(offer, true);
			}
		}
	}

	private bool ShouldCreateOffer(SpecialOffer offer)
	{
		if (this.IsSpecialOfferActive(offer) || TournamentManager.Instance.IsInsideTournament)
		{
			return false;
		}
		if (offer.IsDayShowType)
		{
			DateTime now = DateTime.Now;
			bool flag = now >= offer.NextActivationDate;
			bool flag2 = now < offer.NextActivationEndDate;
			bool flag3 = this.GetCurrentWeeklySpecialOfferIfAny() == offer;
			return flag && flag2 && flag3;
		}
		if (offer.IsQuestShowType)
		{
			bool hasCompletedNeededQuest = offer.HasCompletedNeededQuest;
			offer.HasCompletedNeededQuest = false;
			return hasCompletedNeededQuest;
		}
		return false;
	}

	private SpecialOffer GetCurrentWeeklySpecialOfferIfAny()
	{
		return this.weeklySpecialOffers[this.currentWeeklyOfferIndex % this.weeklySpecialOffers.Count];
	}

	private SpecialOffer GetNextWeeklySpecialOffer()
	{
		SpecialOffer currentWeeklySpecialOfferIfAny = this.GetCurrentWeeklySpecialOfferIfAny();
		if (this.IsSpecialOfferActive(currentWeeklySpecialOfferIfAny))
		{
			return this.weeklySpecialOffers[(this.currentWeeklyOfferIndex + 1) % this.weeklySpecialOffers.Count];
		}
		return currentWeeklySpecialOfferIfAny;
	}

	private static readonly string KEY_CURRENT_SPECIAL_OFFER_INDEX = "KEY_CURRENT_SPECIAL_OFFER_INDEX";

	private static readonly string KEY_ENDTIME_FOR_LAST_WEEKLY_SPECIAL_OFFER = "KEY_ENDTIME_FOR_LAST_WEEKLY_SPECIAL_OFFER";

	[SerializeField]
	private List<SpecialOffer> availableSpecialOffers = new List<SpecialOffer>();

	[ShowInInspector]
	private List<SpecialOffer> activeSpecialOffers = new List<SpecialOffer>();

	private int currentWeeklyOfferIndex;

	private DateTime endTimeForLastWeeklySpecialOffer = DateTime.MinValue;

	private List<SpecialOffer> weeklySpecialOffers = new List<SpecialOffer>();

	private IGNSpecialOffer ignSpecialOffer;
}
