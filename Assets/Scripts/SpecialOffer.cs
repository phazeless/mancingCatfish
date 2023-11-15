using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FullInspector;
using UnityEngine;

[fiInspectorOnly]
public class SpecialOffer : ScriptableObject
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<SpecialOffer> OnSpecialOfferDurationEnd;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<SpecialOffer, SpecialOffer.Content> OnSpecialOfferClaimed;

	public SpecialOffer.SpecialOfferPriceTier PriceTier
	{
		get
		{
			return this.priceTier;
		}
	}

	public SpecialOffer.SpecialOfferVisualStyle VisualStyle
	{
		get
		{
			return this.visualStyle;
		}
	}

	public List<Skill> RandomCrewMembers
	{
		get
		{
			return this.randomBetweenCrewMembers;
		}
	}

	public string Id
	{
		get
		{
			return this.id;
		}
	}

	public int GemAmount
	{
		get
		{
			return this.gemAmount;
		}
	}

	public int CrownExpAmount
	{
		get
		{
			return CrownExpGranterManager.Instance.GetCrownExpAmountAtLocation(this.IAPPlacement);
		}
	}

	public int FreeSpinAmount
	{
		get
		{
			return this.freeSpins;
		}
	}

	public string OfferName
	{
		get
		{
			return this.offerName;
		}
	}

	public bool ContainsGems
	{
		get
		{
			return this.offerContents.Contains(SpecialOffer.SpecialOfferContent.Gems);
		}
	}

	public bool ContainsCrownExp
	{
		get
		{
			return this.CrownExpAmount > 0;
		}
	}

	public bool ContainsFreeSpins
	{
		get
		{
			return this.offerContents.Contains(SpecialOffer.SpecialOfferContent.FreeSpin);
		}
	}

	public bool ContainsHooks
	{
		get
		{
			return this.offerContents.Contains(SpecialOffer.SpecialOfferContent.Hooks);
		}
	}

	public bool ContainsCrewMembers
	{
		get
		{
			return this.offerContents.Contains(SpecialOffer.SpecialOfferContent.CrewMember);
		}
	}

	public bool ShowCrewMemberList
	{
		get
		{
			return this.ContainsCrewMembers && !this.randomBetweenAllCrewMembers;
		}
	}

	public bool HasRandomCrewMember
	{
		get
		{
			return this.randomBetweenCrewMembers.Count > 0;
		}
	}

	public bool HasId
	{
		get
		{
			return !string.IsNullOrEmpty(this.id);
		}
	}

	public float TotalSecondsLeftOnDuration
	{
		get
		{
			return Mathf.Max(0f, (float)((double)this.durationInSeconds - (DateTime.Now - this.lastActivated).TotalSeconds));
		}
	}

	public SpecialOffer.SpecialOfferShowType ShowType
	{
		get
		{
			return this.showType;
		}
	}

	public bool IsQuestShowType
	{
		get
		{
			return this.showType == SpecialOffer.SpecialOfferShowType.QuestCompletion;
		}
	}

	public bool IsDayShowType
	{
		get
		{
			return this.showType == SpecialOffer.SpecialOfferShowType.DayOfWeek;
		}
	}

	public DayOfWeek DayOfWeek
	{
		get
		{
			return this.showDay;
		}
	}

	public string ProductId
	{
		get
		{
			switch (this.priceTier)
			{
			case SpecialOffer.SpecialOfferPriceTier.Starter:
				return "se.ace.special_offer_starter.sku";
			case SpecialOffer.SpecialOfferPriceTier.SmallWeekend:
				return "se.ace.special_offer_1.sku";
			case SpecialOffer.SpecialOfferPriceTier.MediumWeekend:
				return "se.ace.special_offer_2.sku";
			case SpecialOffer.SpecialOfferPriceTier.UltimateWeekend:
				return "se.ace.special_offer_3.sku";
			default:
				return null;
			}
		}
	}

	public string ItemId
	{
		get
		{
			switch (this.priceTier)
			{
			case SpecialOffer.SpecialOfferPriceTier.Starter:
				return "se.ace.special_offer_starter";
			case SpecialOffer.SpecialOfferPriceTier.SmallWeekend:
				return "se.ace.special_offer_1";
			case SpecialOffer.SpecialOfferPriceTier.MediumWeekend:
				return "se.ace.special_offer_2";
			case SpecialOffer.SpecialOfferPriceTier.UltimateWeekend:
				return "se.ace.special_offer_3";
			default:
				return null;
			}
		}
	}

	public DateTime NextActivationDate
	{
		get
		{
			return this.nextActivationDate;
		}
	}

	public DateTime NextActivationEndDate
	{
		get
		{
			return this.nextActivationEndDate;
		}
	}

	public bool HasCompletedNeededQuest
	{
		get
		{
			return this.hasCompletedNeededQuest;
		}
		set
		{
			this.hasCompletedNeededQuest = value;
		}
	}

	public int CrewMemberCount
	{
		get
		{
			return Mathf.Min(this.randomBetweenCrewMembers.FindAll((Skill x) => x.CurrentLevel == 0 && !x.GetExtraInfo().IsFacebookCrew).Count, this.crewAmount);
		}
	}

	public IAPPlacement IAPPlacement
	{
		get
		{
			return FishStoreAssets.GetIAPPlacement(this.ItemId);
		}
	}

	public void Awake()
	{
		if (this.randomBetweenAllCrewMembers && SkillManager.Instance != null)
		{
			this.randomBetweenCrewMembers = new List<Skill>(SkillManager.Instance.CrewMembers);
		}
		if (this.IsQuestShowType)
		{
			this.showOnQuestComplete.OnQuestClaimed += this.ShowOnQuestComplete_OnQuestClaimed;
		}
	}

	private void ShowOnQuestComplete_OnQuestClaimed(Quest obj)
	{
		this.hasCompletedNeededQuest = true;
	}

	public void Activate()
	{
		this.hasNotifiedDurationEnd = false;
		if (this.IsQuestShowType)
		{
			this.lastActivated = DateTime.Now;
		}
		else if (this.IsDayShowType)
		{
			this.lastActivated = this.nextActivationDate;
		}
	}

	public void Claim()
	{
		this.lastActivated = DateTime.MinValue;
		SpecialOffer.Content content = new SpecialOffer.Content();
		content.GemAmount = this.gemAmount;
		content.FreeSpins = this.freeSpins;
		content.Hooks = this.hooks;
		List<Skill> list = this.randomBetweenCrewMembers.FindAll((Skill x) => x.CurrentLevel == 0 && !x.GetExtraInfo().IsFacebookCrew && !x.GetExtraInfo().IsOnlyAvailableThroughPurchase);
		if (this.HasRandomCrewMember)
		{
			if (list.Count < this.crewAmount)
			{
				List<Skill> source = this.randomBetweenCrewMembers.FindAll((Skill x) => x.CurrentLevel == 0 && !x.GetExtraInfo().IsFacebookCrew && x.GetExtraInfo().IsOnlyAvailableThroughPurchase);
				int count = this.crewAmount - list.Count;
				IEnumerable<Skill> collection = source.Take(count);
				list.AddRange(collection);
			}
			int count2 = list.Count;
			for (int i = 0; i < Mathf.Min(this.crewAmount, count2); i++)
			{
				Skill item = list[UnityEngine.Random.Range(0, list.Count)];
				list.Remove(item);
				content.CrewMembers.Add(item);
			}
		}
		if (this.OnSpecialOfferClaimed != null)
		{
			this.OnSpecialOfferClaimed(this, content);
		}
		if (this.OnSpecialOfferDurationEnd != null)
		{
			this.OnSpecialOfferDurationEnd(this);
		}
		EncryptedPlayerPrefs.DeleteKey(this.GetKeyLastActivated());
	}

	[InspectorOrder(0.0)]
	[InspectorButton]
	public void GenerateId()
	{
		this.id = Guid.NewGuid().ToString();
	}

	public void Update()
	{
		if (this.TotalSecondsLeftOnDuration <= 0f && !this.hasNotifiedDurationEnd)
		{
			this.hasNotifiedDurationEnd = true;
			if (this.OnSpecialOfferDurationEnd != null)
			{
				this.OnSpecialOfferDurationEnd(this);
			}
		}
	}

	public bool ShouldSpawn()
	{
		if (this.IsDayShowType)
		{
			DateTime now = DateTime.Now;
			bool flag = now >= this.nextActivationDate;
			bool flag2 = now < this.nextActivationEndDate;
			return flag && flag2;
		}
		return this.IsQuestShowType && this.hasCompletedNeededQuest;
	}

	public void Save()
	{
		EncryptedPlayerPrefs.SetString(this.GetKeyLastActivated(), this.lastActivated.Ticks.ToString(), true);
	}

	public SpecialOffer.SpecialOfferLoadState Load()
	{
		long num = long.Parse(EncryptedPlayerPrefs.GetString(this.GetKeyLastActivated(), "-1"));
		if (num < 0L)
		{
			return SpecialOffer.SpecialOfferLoadState.NotFound;
		}
		this.lastActivated = new DateTime(num);
		if (this.TotalSecondsLeftOnDuration > 0f)
		{
			return SpecialOffer.SpecialOfferLoadState.FoundAndActive;
		}
		return SpecialOffer.SpecialOfferLoadState.FoundAndExpired;
	}

	private string GetKeyLastActivated()
	{
		return this.id + "_lastActivated";
	}

	public bool ShouldBeActive
	{
		get
		{
			DateTime now = DateTime.Now;
			return now >= this.NextActivationDate && now < this.NextActivationEndDate;
		}
	}

	public void CalculateNextActivationAndEndDate(int offset, bool overrideNowWithRealNow = false)
	{
		DateTime start = DateTime.Now.AddDays((double)(offset * 7)).AddSeconds((double)((!overrideNowWithRealNow) ? (-(double)this.durationInSeconds) : 0));
		this.nextActivationDate = this.GetNextWeekday(start, this.showDay).Date.AddHours(-5.0);
		this.nextActivationEndDate = this.nextActivationDate.AddSeconds((double)this.durationInSeconds);
		if (!this.ShouldBeActive && this.nextActivationDate < DateTime.Now && !overrideNowWithRealNow)
		{
			this.CalculateNextActivationAndEndDate(offset, true);
		}
	}

	private DateTime GetNextWeekday(DateTime start, DayOfWeek day)
	{
		int num = (day - start.DayOfWeek + 7) % 7;
		return start.AddDays((double)num);
	}

	[ShowInInspector]
	[InspectorDisabled]
	[SerializeField]
	private string id;

	[SerializeField]
	private string offerName;

	[SerializeField]
	private SpecialOffer.SpecialOfferVisualStyle visualStyle;

	[SerializeField]
	private SpecialOffer.SpecialOfferPriceTier priceTier;

	[SerializeField]
	private SpecialOffer.SpecialOfferShowType showType = SpecialOffer.SpecialOfferShowType.DayOfWeek;

	[SerializeField]
	[InspectorShowIf("IsDayShowType")]
	private DayOfWeek showDay = DayOfWeek.Monday;

	[SerializeField]
	[InspectorShowIf("IsQuestShowType")]
	private Quest showOnQuestComplete;

	[SerializeField]
	private int durationInSeconds;

	[SerializeField]
	private List<SpecialOffer.SpecialOfferContent> offerContents = new List<SpecialOffer.SpecialOfferContent>();

	[SerializeField]
	[InspectorShowIf("ContainsGems")]
	private int gemAmount;

	[InspectorShowIf("ContainsFreeSpins")]
	[SerializeField]
	private int freeSpins;

	[InspectorShowIf("ContainsCrewMembers")]
	[SerializeField]
	private int crewAmount;

	[SerializeField]
	[InspectorShowIf("ContainsHooks")]
	private int hooks;

	[SerializeField]
	private bool randomBetweenAllCrewMembers;

	[InspectorShowIf("ShowCrewMemberList")]
	[SerializeField]
	private List<Skill> randomBetweenCrewMembers = new List<Skill>();

	private DateTime lastActivated = DateTime.MinValue;

	private bool hasNotifiedDurationEnd;

	private DateTime nextActivationDate = DateTime.MinValue;

	private DateTime nextActivationEndDate = DateTime.MinValue;

	[NonSerialized]
	private bool hasCompletedNeededQuest;

	public enum SpecialOfferLoadState
	{
		FoundAndActive,
		FoundAndExpired,
		NotFound
	}

	public enum SpecialOfferPriceTier
	{
		Starter,
		SmallWeekend,
		MediumWeekend,
		UltimateWeekend
	}

	public enum SpecialOfferContent
	{
		Gems,
		CrewMember,
		FreeSpin,
		Hooks
	}

	public enum SpecialOfferVisualStyle
	{
		Starter,
		Common,
		Rare,
		Epic
	}

	public enum SpecialOfferShowType
	{
		QuestCompletion,
		DayOfWeek
	}

	public class Content
	{
		public int GemAmount;

		public List<Skill> CrewMembers = new List<Skill>();

		public int FreeSpins;

		public int Hooks;
	}
}
