using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using FullInspector;
using UnityEngine;

[Serializable]
public class Skill : BaseScriptableObject, IListItemContent
{
	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill> OnSkillActivation;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill> OnSkillCooldownZero;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill> OnSkillDurationEnd;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill> OnSkillUnlocked;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill, LevelChange> OnSkillLevelUp;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill, bool> OnSkillAvailabilityChanged;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill, BigInteger> OnSkillCostChange;



	public UIListItem GetPrefab()
	{
		return this.visualPrefab;
	}

	public void SetVisualPrefab(UIListItem prefab)
	{
		this.visualPrefab = prefab;
	}

	public string Id
	{
		get
		{
			return this.id;
		}
	}

	[ShowInInspector]
	[InspectorDisabled]
	public int CurrentLevel
	{
		get
		{
			return this.currentLevel;
		}
		private set
		{
			this.previousLevel = this.currentLevel;
			this.currentLevel = value;
		}
	}

	[InspectorDisabled]
	[ShowInInspector]
	public int LifetimeLevel
	{
		get
		{
			return this.lifetimeLevel;
		}
		private set
		{
			this.lifetimeLevel = value;
		}
	}

	[InspectorDisabled]
	[ShowInInspector]
	public int HighestLevel
	{
		get
		{
			return this.highestLevel;
		}
		private set
		{
			this.highestLevel = value;
		}
	}

	public long CurrentLevelAsLong
	{
		get
		{
			return this.currentLevelAsLong;
		}
		private set
		{
			this.currentLevelAsLong = value;
		}
	}

	public int PreviousLevel
	{
		get
		{
			return this.previousLevel;
		}
	}

	public int NextLevel
	{
		get
		{
			return this.CurrentLevel + 1;
		}
	}

	public int MaxLevel
	{
		get
		{
			return this.maxLevel;
		}
	}

	public bool IsMaxLevel
	{
		get
		{
			return this.CurrentLevel >= this.maxLevel;
		}
	}

	public bool IsOnCooldown
	{
		get
		{
			return this.cooldown > 0 && this.GetTotalSecondsLeftOnCooldown() > 0f;
		}
	}

	public bool IsActivated
	{
		get
		{
			return this.GetTotalSecondsLeftOnDuration() > 0f;
		}
	}

	public int Duration
	{
		get
		{
			return this.duration;
		}
	}

	public int Cooldown
	{
		get
		{
			return this.cooldown;
		}
	}

	[HideInInspector]
	public bool IsUnlocked { get; set; }

	public BigInteger CostForCurrentLevelUp
	{
		get
		{
			return this.GetCostForLevelUp(this.CurrentLevel - 1);
		}
	}

	public BigInteger CostForNextLevelUp
	{
		get
		{
			return this.GetCostForLevelUp(this.CurrentLevel);
		}
	}

	public bool IsAvailableForLevelUp
	{
		get
		{
			return this.isAvailableForLevelUp;
		}
		set
		{
			this.isAvailableForLevelUp = value;
		}
	}

	public Skills.ActiveSkill ActiveSkill
	{
		get
		{
			return this.activeSkill;
		}
	}

	public IList<SkillBehaviour> SkillBehaviours
	{
		get
		{
			return this.skillBehaviours.AsReadOnly();
		}
	}

	[NotSerialized]
	[HideInInspector]
	public int Tier { get; set; }

	public bool IsTierSkill
	{
		get
		{
			return this.Tier > 0;
		}
	}

	public DateTime LastActivated
	{
		get
		{
			return this.lastActivated;
		}
	}

	public Dictionary<string, object> MetaDataDict
	{
		get
		{
			return this.metaDataDict;
		}
		set
		{
			this.metaDataDict = value;
		}
	}

	public float PriceReductionInPercent
	{
		get
		{
			return this.priceReducationInPercent;
		}
	}

	public void NotifySkillAvailabilityChanged(bool available)
	{
		if (this.OnSkillAvailabilityChanged != null)
		{
			this.OnSkillAvailabilityChanged(this, available);
		}
	}

	public void LoadMiscData()
	{
		this.lastActivated = new DateTime(long.Parse(EncryptedPlayerPrefs.GetString(this.GetKeyForLastActivationTime(), "0")));
		this.hasNotifiedCooldownZero = (EncryptedPlayerPrefs.GetInt(this.GetKeyForHasNotifiedCooldownZero(), 0) != 0);
		this.hasNotifiedSkillUnlocked = (EncryptedPlayerPrefs.GetInt(this.GetKeyForHasNotifiedIsUnlocked(), 0) != 0);
	}

	public void SaveMiscData()
	{
		if (this.IsActiveSkill || this.skillExtraInfo.IsCrew)
		{
			EncryptedPlayerPrefs.SetString(this.GetKeyForLastActivationTime(), this.lastActivated.Ticks.ToString(), true);
			EncryptedPlayerPrefs.SetInt(this.GetKeyForHasNotifiedCooldownZero(), (!this.hasNotifiedCooldownZero) ? 0 : 1, true);
			EncryptedPlayerPrefs.SetInt(this.GetKeyForHasNotifiedIsUnlocked(), (!this.hasNotifiedSkillUnlocked) ? 0 : 1, true);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.skillExtraInfo.HasLevelUpDependency)
		{
			this.skillExtraInfo.DependancyLevelUp.OnSkillLevelUp -= this.LevelUpDependency_OnSkillLevelUp;
			this.skillExtraInfo.DependancyLevelUp.OnSkillLevelUp += this.LevelUpDependency_OnSkillLevelUp;
		}
	}

	public void OnApplicationPause()
	{
	}

	public void Update()
	{
		if (!this.hasNotifiedCooldownZero && !this.IsOnCooldown && (this.CurrentLevel > 0 || this.IsBoost))
		{
			if (this.OnSkillCooldownZero != null)
			{
				this.OnSkillCooldownZero(this);
			}
			this.hasNotifiedCooldownZero = true;
		}
		if (!this.hasNotifiedDurationEnd && !this.IsActivated)
		{
			if (this.OnSkillDurationEnd != null)
			{
				this.OnSkillDurationEnd(this);
			}
			this.hasNotifiedDurationEnd = true;
		}
		if (!this.hasNotifiedSkillUnlocked && this.IsUnlocked && this.skillExtraInfo.IsCrew)
		{
			if (this.OnSkillUnlocked != null)
			{
				this.OnSkillUnlocked(this);
			}
			this.hasNotifiedSkillUnlocked = true;
		}
	}

	public void Activate()
	{
		if (this.GetTotalSecondsLeftOnCooldown() == 0f)
		{
			this.hasNotifiedCooldownZero = false;
			this.hasNotifiedDurationEnd = false;
			this.lastActivated = DateTime.Now;
			if (this.OnSkillActivation != null)
			{
				this.OnSkillActivation(this);
			}
		}
	}

	public void ExpireNow()
	{
		this.lastActivated = DateTime.Now.AddSeconds((double)(-(double)this.duration));
		this.hasNotifiedDurationEnd = false;
	}

	public float GetTotalSecondsLeftOnCooldown()
	{
		return Mathf.Max(0f, (float)((double)this.cooldown - (DateTime.Now - this.lastActivated).TotalSeconds));
	}

	public float GetTotalSecondsLeftOnDuration()
	{
		return Mathf.Max(0f, (float)((double)this.duration - (DateTime.Now - this.lastActivated).TotalSeconds));
	}

	public void FastForward(float seconds)
	{
		if (this.GetExtraInfo().IsCrew)
		{
			return;
		}
		try
		{
			this.lastActivated = this.lastActivated.AddSeconds((double)(-(double)seconds));
		}
		catch (ArgumentOutOfRangeException ex)
		{
		}
	}

	public BigInteger GetCostForLevelUp(int level)
	{
		if (level < 0)
		{
			return 0;
		}
		BigInteger bigInteger = -1;
		if (this.skillExtraInfo.CostMethod == Skill.SkillExtraInfo.CostMethodType.ByFormula1)
		{
			bigInteger = this.skillExtraInfo.InitialUpgradeCost.Value + this.skillExtraInfo.CostPerLevel.Value * (BigInteger)Mathf.Pow((float)level, (float)this.skillExtraInfo.ExponentialLevelCostModifier);
		}
		else if (this.skillExtraInfo.CostMethod == Skill.SkillExtraInfo.CostMethodType.ByLevel)
		{
			if (level >= this.skillExtraInfo.CostPerLevels.Count)
			{
				bigInteger = this.skillExtraInfo.CostPerLevels[this.skillExtraInfo.CostPerLevels.Count - 1].Value;
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"(",
					base.name,
					")Trying to get cost for level: '",
					level,
					"' when there only exists cost for levels up to: '",
					this.skillExtraInfo.CostPerLevels.Count - 1,
					"'"
				}));
			}
			else
			{
				bigInteger = this.skillExtraInfo.CostPerLevels[level].Value;
			}
		}
		return bigInteger.MultiplyFloat((100f - this.priceReducationInPercent) / 100f);
	}

	private bool LevelUp(LevelChange levelChange, bool forced)
	{
		if (this.CurrentLevel + 1 > this.maxLevel)
		{
			return false;
		}
		if (this.IsAvailableForLevelUp || forced)
		{
			this.CurrentLevel++;
			this.LifetimeLevel++;
			if (this.CurrentLevel > this.highestLevel)
			{
				this.HighestLevel = this.CurrentLevel;
			}
			if (this.OnSkillLevelUp != null)
			{
				this.OnSkillLevelUp(this, levelChange);
			}
			return true;
		}
		return false;
	}

	public bool LevelUpForFree()
	{
		return this.LevelUp(LevelChange.LevelUpFree, true);
	}

	public bool TryLevelUp()
	{
		return this.LevelUp(LevelChange.LevelUp, false);
	}

	public void SetExtraInfo(Skill.SkillExtraInfo skillExtraInfo)
	{
		this.skillExtraInfo = skillExtraInfo;
	}

	public Skill.SkillExtraInfo GetExtraInfo()
	{
		return this.skillExtraInfo;
	}

	public void AddMetaData(string key, object metaData)
	{
		if (this.metaDataDict.ContainsKey(key))
		{
			this.metaDataDict[key] = metaData;
		}
		else
		{
			this.metaDataDict.Add(key, metaData);
		}
	}

	public T GetMetaData<T>(string key)
	{
		object value = null;
		if (this.metaDataDict.TryGetValue(key, out value))
		{
			return (T)((object)Convert.ChangeType(value, typeof(T)));
		}
		return default(T);
	}

	public void SetLifetimeLevel(int lifetimeLevel)
	{
		this.LifetimeLevel = lifetimeLevel;
	}

	public void SetHighestLevel(int highestLevel)
	{
		this.HighestLevel = highestLevel;
	}

	public void SetCurrentLevel(int level, LevelChange levelChange)
	{
		this.CurrentLevel = level;
		if (this.OnSkillLevelUp != null)
		{
			this.OnSkillLevelUp(this, levelChange);
		}
	}

	public void SetCurrentLevelAsLong(long level, LevelChange levelChange)
	{
		this.CurrentLevelAsLong = level;
		if (this.OnSkillLevelUp != null)
		{
			this.OnSkillLevelUp(this, levelChange);
		}
	}

	public void AddCostReduction(float reductionInPercent)
	{
		float num = this.priceReducationInPercent + reductionInPercent;
		num = ((num >= 0f) ? num : 0f);
		this.SetCostReduction(num);
	}

	public void SetCostReduction(float priceReducationInPercent)
	{
		bool flag = this.priceReducationInPercent != priceReducationInPercent;
		if (flag)
		{
			this.priceReducationInPercent = priceReducationInPercent;
			if (this.OnSkillCostChange != null)
			{
				this.OnSkillCostChange(this, this.CostForNextLevelUp);
			}
		}
	}

	private string GetKeyForHasNotifiedIsUnlocked()
	{
		return "key_" + this.id + "_hasnotifiedisunlocked";
	}

	private string GetKeyForHasNotifiedCooldownZero()
	{
		return "key_" + this.id + "_hasnotifiedcooldownzero";
	}

	private string GetKeyForLastActivationTime()
	{
		return "key_" + this.id + "_lastactivationtime";
	}

	private void LevelUpDependency_OnSkillLevelUp(Skill dependencySkill, LevelChange levelChange)
	{
		if (this.skillExtraInfo.IsPartOfSkillTree)
		{
			return;
		}
		if (this.CurrentLevel > 0 && (levelChange == LevelChange.LevelUp || levelChange == LevelChange.LevelUpFree) && (this.skillExtraInfo.DependancyLevelUpType == Skill.SkillExtraInfo.LevelType.Current || this.skillExtraInfo.DependancyLevelUpType == Skill.SkillExtraInfo.LevelType.Lifetime))
		{
			int num = (this.skillExtraInfo.DependancyLevelUpType != Skill.SkillExtraInfo.LevelType.Current) ? dependencySkill.LifetimeLevel : dependencySkill.CurrentLevel;
			int num2 = num / this.skillExtraInfo.DependancyLevelUpPerXthLevel;
			int num3 = num2 - this.CurrentLevel;
			for (int i = 0; i < num3; i++)
			{
				this.LevelUp(LevelChange.LevelUpFree, true);
			}
		}
	}

	public void SetHasNotifiedSkillUnlocked(bool state)
	{
		this.hasNotifiedSkillUnlocked = state;
	}

	public void ResetCooldown()
	{
		this.lastActivated = DateTime.Now;
	}

	public bool IsActiveSkill
	{
		get
		{
			return this.skillType == Skill.SkillType.Active;
		}
	}

	public bool IsBoost
	{
		get
		{
			return this.activeSkill != null && this.activeSkill is Skills.Boost;
		}
	}

	public bool IsPowerUp
	{
		get
		{
			return this.activeSkill != null && this.activeSkill is Skills.PowerUp;
		}
	}

	public void GenerateId(bool overrideIdEvenIfNotNull = false)
	{
		if (string.IsNullOrEmpty(this.id) || overrideIdEvenIfNotNull)
		{
			this.id = Guid.NewGuid().ToString();
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
	}

	[SerializeField]
	[ShowInInspector]
	[InspectorDisabled]
	private string id;

	[SerializeField]
	private Skill.SkillType skillType;

	[InspectorShowIf("IsActiveSkill")]
	[SerializeField]
	private Skills.ActiveSkill activeSkill;

	[FullInspector.InspectorName("Duration (in seconds)")]
	[InspectorShowIf("IsActiveSkill")]
	[SerializeField]
	private int duration;

	[InspectorShowIf("IsActiveSkill")]
	[FullInspector.InspectorName("Cooldown (in seconds)")]
	[SerializeField]
	private int cooldown;

	private DateTime lastActivated = DateTime.MinValue;

	[NonSerialized]
	private bool hasNotifiedCooldownZero;

	[NonSerialized]
	private bool hasNotifiedDurationEnd;

	[NonSerialized]
	private bool hasNotifiedSkillUnlocked;

	[NonSerialized]
	private int currentLevel;

	[NonSerialized]
	private int lifetimeLevel;

	[NonSerialized]
	private int highestLevel;

	[NonSerialized]
	private int previousLevel;

	[NonSerialized]
	private long currentLevelAsLong;

	[SerializeField]
	private int maxLevel;

	[SerializeField]
	private Skill.SkillExtraInfo skillExtraInfo = new Skill.SkillExtraInfo();

	[SerializeField]
	private List<SkillBehaviour> skillBehaviours = new List<SkillBehaviour>();

	[NonSerialized]
	private bool isAvailableForLevelUp;

	[NonSerialized]
	private float priceReducationInPercent;

	[NonSerialized]
	private UIListItem visualPrefab;

	[NonSerialized]
	private Dictionary<string, object> metaDataDict = new Dictionary<string, object>();

	public enum SkillType
	{
		Passive,
		Active
	}

	[Serializable]
	public class SkillExtraInfo
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnSkillIconChanged;

		public bool HasLevelUpDependency
		{
			get
			{
				return this.DependancyLevelUp != null;
			}
		}

		private bool IsByFormula1
		{
			get
			{
				return this.CostMethod == Skill.SkillExtraInfo.CostMethodType.ByFormula1;
			}
		}

		private bool IsByLevel
		{
			get
			{
				return this.CostMethod == Skill.SkillExtraInfo.CostMethodType.ByLevel;
			}
		}

		public bool HasFulfilledDependency
		{
			get
			{
				return !this.IsPartOfSkillTree || !this.HasLevelUpDependency || this.DependancyLevelUp.CurrentLevel >= this.DependancyLevelUpPerXthLevel;
			}
		}

		public bool IsCrew;

		public bool IsPartOfSkillTree;

		public bool IgnoreReset;

		public bool IgnoreIGN;

		public bool IsFacebookCrew;

		public bool IsOnlyAvailableThroughPurchase;

		public bool CacheCurrentLevelAsBigInteger;

		public Sprite Icon;

		public Color IconBgColor = Color.white;

		public string TitleText = "No title has been assigned...";

		public string DescriptionText = "No description has been assigned...";

		public string MiscText = "No misc-text has been assigned...";

		public ResourceType PurchaseWith;

		public Skill.SkillExtraInfo.CostMethodType CostMethod;

		[InspectorShowIf("IsByFormula1")]
		public BigIntWrapper InitialUpgradeCost = new BigIntWrapper();

		[InspectorShowIf("IsByFormula1")]
		public BigIntWrapper CostPerLevel = new BigIntWrapper();

		[InspectorShowIf("IsByFormula1")]
		public int ExponentialLevelCostModifier;

		[InspectorShowIf("IsByLevel")]
		public List<BigIntWrapper> CostPerLevels = new List<BigIntWrapper>();

		public Skill DependancyLevelUp;

		[InspectorShowIf("HasLevelUpDependency")]
		public Skill.SkillExtraInfo.LevelType DependancyLevelUpType;

		[InspectorShowIf("HasLevelUpDependency")]
		public int DependancyLevelUpPerXthLevel = 1;

		public Skills.BaseSkillAttribute SkillSpecifics = new Skills.None();

		public int RequiredFishingExperience;

		public int RequiredDeepWaterLevel;

		public enum CostMethodType
		{
			ByFormula1,
			ByLevel
		}

		public enum LevelType
		{
			Current,
			Lifetime
		}
	}
}
