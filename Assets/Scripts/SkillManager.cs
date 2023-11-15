using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using FullInspector;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class SkillManager : BaseBehavior
{
	public static SkillManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill> OnSkillUnlocked;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill, LevelChange> OnSkillLevelChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill, bool> OnSkillAvailabilityChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnSkillsReset;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<ISkillAttribute, float> OnSkillAttributeValueChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<SkillManager> OnSkillManagerInitialized;

	private Skill CrownLevelSkill
	{
		get
		{
			return SkillTreeManager.Instance.CrownLevelSkill;
		}
	}

	private List<Skill> SkillTreeSkills
	{
		get
		{
			return SkillTreeManager.Instance.SkillTreeSkills;
		}
	}

	private List<Skill> CrownRewardSkills
	{
		get
		{
			return SkillTreeManager.Instance.CrownRewardSkills;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		SkillManager.Instance = this;
		this.latestSkillCostResourceAmount.Add(0, 0);
		this.latestSkillCostResourceAmount.Add(1, 0);
		this.latestSkillCostResourceAmount.Add(2, 0);
		this.latestSkillCostResourceAmount.Add(3, 0);
		this.allSkills.Add(this.coreTierSkill);
		this.allSkills.Add(this.corePrestigeSkill);
		this.allSkills.Add(this.coreQuestSkill);
		this.allSkills.Add(this.coreUnlockCrewMemberSkill);
		this.allSkills.Add(this.coreDeepWaterSkill);
		this.allSkills.Add(this.collectStarsSkill);
		this.allSkills.Add(this.jackpotSkill);
		this.allSkills.Add(this.fishValueBonusSkill);
		this.allSkills.Add(this.CrownLevelSkill);
		this.allSkills.AddRange(this.baseSkills);
		this.allSkills.AddRange(this.crewMembers);
		this.allSkills.AddRange(this.otherSkills);
		this.allSkills.AddRange(this.SkillTreeSkills);
		this.allSkills.AddRange(this.CrownRewardSkills);
		this.corePrestigeSkill.OnSkillLevelUp += this.CorePrestigeSkill_OnSkillLevelUp;
		for (int i = 0; i < this.tierSkills.Count; i++)
		{
			for (int j = 0; j < this.tierSkills[i].Skills.Count; j++)
			{
				Skill skill = this.tierSkills[i].Skills[j];
				skill.Tier = i + 1;
				this.allSkills.Add(skill);
			}
		}
		foreach (Skill skill2 in this.allSkills)
		{
			skill2.OnSkillLevelUp += this.OnSkillLevelUpCallback;
			skill2.OnSkillAvailabilityChanged += this.OnSkillAvailabilityChangedCallback;
		}
		foreach (Skill skill3 in this.crewMembers)
		{
			skill3.OnSkillUnlocked += this.CrewMember_OnSkillUnlocked;
		}
		this.coreTierSkill.OnSkillLevelUp += this.CoreTierSkill_OnSkillLevelUp;
		this.itemManager.OnItemAttributeValueChanged += this.ItemManager_OnItemAttributeValueChanged;
		this.ValidateSkills();
	}

	private void ItemManager_OnItemAttributeValueChanged(ISkillAttribute attr, float val)
	{
		if (this.OnSkillAttributeValueChanged != null)
		{
			this.OnSkillAttributeValueChanged(attr, val);
		}
	}

	private void CrewMember_OnSkillUnlocked(Skill skill)
	{
		if (this.OnSkillUnlocked != null)
		{
			this.OnSkillUnlocked(skill);
		}
	}

	private void CorePrestigeSkill_OnSkillLevelUp(Skill arg1, LevelChange arg2)
	{
		foreach (Skill skill in this.crewMembers)
		{
			this.CanSkillBeLeveledUp(skill, true);
		}
	}

	private void CoreTierSkill_OnSkillLevelUp(Skill skillThatLeveledUp, LevelChange levelChange)
	{
		this.CanSkillBeLeveledUp(this.coreTierSkill, true);
		foreach (SkillManager.SkillsAtTierLevels skillsAtTierLevels in this.tierSkills)
		{
			foreach (Skill skill in skillsAtTierLevels.Skills)
			{
				this.CanSkillBeLeveledUp(skill, true);
			}
		}
	}

	private void Start()
	{
		this.InitBaseSkills();
		this.LoadSavedSkillLevels();
		this.CanSkillBeLeveledUp(this.coreTierSkill, true);
		this.CanSkillBeLeveledUp(this.coreDeepWaterSkill, true);
		this.CanSkillBeLeveledUp(this.corePrestigeSkill, true);
		this.CanSkillBeLeveledUp(this.coreQuestSkill, true);
		this.CanSkillBeLeveledUp(this.coreUnlockCrewMemberSkill, true);
		this.CanSkillBeLeveledUp(this.collectStarsSkill, true);
		this.CanSkillBeLeveledUp(this.jackpotSkill, true);
		this.CanSkillBeLeveledUp(this.fishValueBonusSkill, true);
		foreach (Skill skill in this.otherSkills)
		{
			this.CanSkillBeLeveledUp(skill, true);
		}
		this.hasHadTheChanceToLoadSkillsFromDisk = true;
		if (this.OnSkillManagerInitialized != null)
		{
			this.OnSkillManagerInitialized(this);
		}
	}

	private void Update()
	{
		for (int i = 0; i < this.allSkills.Count; i++)
		{
			Skill skill = this.allSkills[i];
			if (skill.IsActiveSkill || skill.GetExtraInfo().IsCrew)
			{
				skill.Update();
			}
		}
	}

	public IList<Skill> AllSkills
	{
		get
		{
			return this.allSkills.AsReadOnly();
		}
	}

	public IList<SkillManager.SkillsAtTierLevels> TierSkills
	{
		get
		{
			return this.tierSkills.AsReadOnly();
		}
	}

	public IList<Skill> CrewMembers
	{
		get
		{
			return this.crewMembers.AsReadOnly();
		}
	}

	public IList<Skill> OtherSkills
	{
		get
		{
			return this.otherSkills.AsReadOnly();
		}
	}

	public Skill TierSkill
	{
		get
		{
			return this.coreTierSkill;
		}
	}

	public Skill DeepWaterSkill
	{
		get
		{
			return this.coreDeepWaterSkill;
		}
	}

	public Skill PrestigeSkill
	{
		get
		{
			return this.corePrestigeSkill;
		}
	}

	public Skill QuestSkill
	{
		get
		{
			return this.coreQuestSkill;
		}
	}

	public Skill CollectStarsSkill
	{
		get
		{
			return this.collectStarsSkill;
		}
	}

	public Skill JackpotSkill
	{
		get
		{
			return this.jackpotSkill;
		}
	}

	public Skill FishValueBonusSkill
	{
		get
		{
			return this.fishValueBonusSkill;
		}
	}

	public Skill UnlockCrewMemberSkill
	{
		get
		{
			return this.coreUnlockCrewMemberSkill;
		}
	}

	public float GetCurrentTotalValueFor(Guid id)
	{
		return ItemAndSkillValues.GetCurrentTotalValueFor(id);
	}

	public float GetCurrentTotalValueFor<T>() where T : ISkillAttribute
	{
		return ItemAndSkillValues.GetCurrentTotalValueFor<T>();
	}

	public float GetCurrentTotalValueFor(ISkillAttribute changeValue)
	{
		return ItemAndSkillValues.GetCurrentTotalValueFor(changeValue);
	}

	public List<Skill> GetCrewMembersAvailableForPurchase()
	{
		long prestigeSkillLvl = this.PrestigeSkill.CurrentLevelAsLong;
		int deepWaterLvl = this.DeepWaterSkill.HighestLevel;
		return (from x in this.CrewMembers
		where x.CurrentLevel == 0 && prestigeSkillLvl >= (long)x.GetExtraInfo().RequiredFishingExperience && deepWaterLvl >= x.GetExtraInfo().RequiredDeepWaterLevel
		select x).ToList<Skill>();
	}

	public void ResetSkills()
	{
		foreach (SkillManager.SkillsAtTierLevels skillsAtTierLevels in this.tierSkills)
		{
			foreach (Skill skill in skillsAtTierLevels.Skills)
			{
				skill.SetCurrentLevel(0, LevelChange.HardReset);
			}
		}
		foreach (Skill skill2 in this.otherSkills)
		{
			if (skill2.GetExtraInfo().PurchaseWith == ResourceType.Cash && !skill2.GetExtraInfo().IgnoreReset)
			{
				skill2.SetCurrentLevel(0, LevelChange.HardReset);
			}
		}
		this.coreTierSkill.SetCurrentLevel(0, LevelChange.HardReset);
		this.coreDeepWaterSkill.SetCurrentLevel(0, LevelChange.HardReset);
		this.ClearAllCrewMemberSkillValues();
		this.itemManager.ClearAllItemSkillValues();
		this.RefreshAllCrewMembersSkillValues();
		this.itemManager.RefreshAllEquippedItemValues();
		this.RefreshSkillTreeSkills();
		if (this.OnSkillsReset != null)
		{
			this.OnSkillsReset();
		}
	}

	public void ResetStars()
	{
		this.collectStarsSkill.SetCurrentLevel(0, LevelChange.HardReset);
	}

	public void ResetWheelBonus()
	{
		this.fishValueBonusSkill.SetCurrentLevel(1, LevelChange.HardReset);
	}

	public void ResetCrewMembers()
	{
		foreach (Skill skill in this.crewMembers)
		{
			skill.SetCurrentLevel(0, LevelChange.HardReset);
		}
	}

	public void ResetPrestige()
	{
		this.corePrestigeSkill.SetCurrentLevel(0, LevelChange.HardReset);
	}

	public void FastForward(float seconds)
	{
		for (int i = 0; i < this.allSkills.Count; i++)
		{
			Skill skill = this.allSkills[i];
			if (skill.IsActiveSkill && !skill.IsBoost)
			{
				skill.FastForward(seconds);
			}
		}
	}

	public void OnSkillCostResourceChanged(ResourceType resType, BigInteger newAmount)
	{
		this.latestSkillCostResourceAmount[(int)resType] = newAmount;
		this.CanSkillBeLeveledUp(this.coreDeepWaterSkill, true);
		this.CanSkillBeLeveledUp(this.coreTierSkill, true);
		this.CanSkillBeLeveledUp(this.coreUnlockCrewMemberSkill, true);
		this.CanSkillBeLeveledUp(this.CrownLevelSkill, true);
		for (int i = 0; i < this.tierSkills.Count; i++)
		{
			for (int j = 0; j < this.tierSkills[i].Skills.Count; j++)
			{
				this.CanSkillBeLeveledUp(this.tierSkills[i].Skills[j], true);
			}
		}
		for (int k = 0; k < this.crewMembers.Count; k++)
		{
			this.CanSkillBeLeveledUp(this.crewMembers[k], true);
		}
		for (int l = 0; l < this.otherSkills.Count; l++)
		{
			this.CanSkillBeLeveledUp(this.otherSkills[l], true);
		}
		for (int m = 0; m < this.SkillTreeSkills.Count; m++)
		{
			this.CanSkillBeLeveledUp(this.SkillTreeSkills[m], true);
		}
		List<Skill> crownRewardSkills = this.CrownRewardSkills;
		for (int n = 0; n < crownRewardSkills.Count; n++)
		{
			this.CanSkillBeLeveledUp(crownRewardSkills[n], true);
		}
	}

	public bool CanSkillBeLeveledUp(Skill skill, bool notify = true)
	{
		bool isAvailableForLevelUp = skill.IsAvailableForLevelUp;
		bool flag = this.latestSkillCostResourceAmount[(int)skill.GetExtraInfo().PurchaseWith] >= skill.CostForNextLevelUp;
		bool flag2 = skill.CurrentLevel > 0;
		bool flag3 = this.corePrestigeSkill.CurrentLevelAsLong >= (long)skill.GetExtraInfo().RequiredFishingExperience || flag2;
		bool flag4 = this.coreDeepWaterSkill.HighestLevel >= skill.GetExtraInfo().RequiredDeepWaterLevel || flag2;
		bool flag5 = !skill.IsTierSkill || this.TierSkill.CurrentLevel >= skill.Tier;
		bool hasFulfilledDependency = skill.GetExtraInfo().HasFulfilledDependency;
		skill.IsAvailableForLevelUp = (flag && !skill.IsMaxLevel && flag3 && flag4 && flag5 && hasFulfilledDependency);
		skill.IsUnlocked = (flag3 && flag4);
		bool flag6 = isAvailableForLevelUp != skill.IsAvailableForLevelUp;
		if (notify && flag6 && this.OnSkillAvailabilityChanged != null)
		{
			skill.NotifySkillAvailabilityChanged(skill.IsAvailableForLevelUp);
		}
		return skill.IsAvailableForLevelUp;
	}

	private void OnSkillAvailabilityChangedCallback(Skill skill, bool hasEnoughResources)
	{
		if (this.OnSkillAvailabilityChanged != null)
		{
			this.OnSkillAvailabilityChanged(skill, hasEnoughResources);
		}
	}

	private void ClearCrewMembersCachedSkillValue(Skill crewMember)
	{
		foreach (SkillBehaviour skillBehaviour in crewMember.SkillBehaviours)
		{
			StoredValue storedValue = ItemAndSkillValues.GetStoredValue(skillBehaviour);
			storedValue.ClearCalculations();
			storedValue.CalculateTotal();
		}
	}

	public void RefreshCachedSkillValuesOfSkill(Skill skill, bool removeCurrentValues = false, bool notifyAttributesChanged = false)
	{
		if (skill.CurrentLevel == 0)
		{
			return;
		}
		foreach (SkillBehaviour skillBehaviour in skill.SkillBehaviours)
		{
			StoredValue storedValue = ItemAndSkillValues.GetStoredValue(skillBehaviour);
			if (skillBehaviour.ValueType == AttributeValueType.Base)
			{
				if (skillBehaviour.ChangeMethod == AttributeValueChangeMethod.NeverChange)
				{
					storedValue.Base = skillBehaviour.GetValueAtLevel(skill.CurrentLevel);
				}
				else if (!removeCurrentValues)
				{
					storedValue.Base += skillBehaviour.GetValueAtLevel(skill.CurrentLevel);
				}
				else
				{
					storedValue.Base -= skillBehaviour.GetValueAtLevel(skill.CurrentLevel);
				}
			}
			else if (skillBehaviour.ValueType == AttributeValueType.Calculation)
			{
				if (!removeCurrentValues)
				{
					storedValue.AddCalculation(skillBehaviour.CalculationType, skillBehaviour.GetTotalValueAtLevel(skill.CurrentLevel));
				}
				else
				{
					storedValue.AddCalculation(skillBehaviour.CalculationType, -skillBehaviour.GetTotalValueAtLevel(skill.CurrentLevel));
				}
			}
			storedValue.CalculateTotal();
			if (notifyAttributesChanged && this.OnSkillAttributeValueChanged != null)
			{
				this.OnSkillAttributeValueChanged(skillBehaviour.ChangeValue, storedValue.Total);
			}
		}
	}

	private void ClearAllCrewMemberSkillValues()
	{
		foreach (Skill crewMember in this.crewMembers)
		{
			this.ClearCrewMembersCachedSkillValue(crewMember);
		}
	}

	private void RefreshSkillTreeSkills()
	{
		foreach (Skill skill in this.CrownRewardSkills)
		{
			this.RefreshCachedSkillValuesOfSkill(skill, false, false);
		}
		foreach (Skill skill2 in this.SkillTreeSkills)
		{
			this.RefreshCachedSkillValuesOfSkill(skill2, false, false);
		}
	}

	private void RefreshAllCrewMembersSkillValues()
	{
		foreach (Skill skill in this.crewMembers)
		{
			this.RefreshCachedSkillValuesOfSkill(skill, false, false);
		}
	}

	public void ClearAllSkillsValues()
	{
		foreach (SkillManager.SkillsAtTierLevels skillsAtTierLevels in this.tierSkills)
		{
			foreach (Skill skill in skillsAtTierLevels.Skills)
			{
				foreach (SkillBehaviour skillBehaviour in skill.SkillBehaviours)
				{
					StoredValue storedValue = ItemAndSkillValues.GetStoredValue(skillBehaviour);
					storedValue.ClearCalculations();
					storedValue.CalculateTotal();
				}
			}
		}
	}

	public void RefreshAllSkillValues()
	{
		foreach (SkillManager.SkillsAtTierLevels skillsAtTierLevels in this.tierSkills)
		{
			foreach (Skill skill in skillsAtTierLevels.Skills)
			{
				this.OnSkillLevelUpCallback(skill, LevelChange.Initialization);
			}
		}
	}

	private void OnSkillLevelUpCallback(Skill skill, LevelChange change)
	{
		bool flag = skill.CurrentLevel == 1;
		foreach (SkillBehaviour skillBehaviour in skill.SkillBehaviours)
		{
			if (skillBehaviour != null)
			{
				StoredValue storedValue = ItemAndSkillValues.GetStoredValue(skillBehaviour);
				float total = storedValue.Total;
				if (change == LevelChange.HardReset || change == LevelChange.SoftReset)
				{
					storedValue.ClearCalculations();
				}
				int num = skill.CurrentLevel - skill.PreviousLevel;
				for (int i = 0; i < num; i++)
				{
					if (skillBehaviour.ValueType == AttributeValueType.Base)
					{
						if (flag)
						{
							storedValue.Base = skillBehaviour.InitialValue;
						}
						if (skillBehaviour.ChangeMethod == AttributeValueChangeMethod.NeverChange)
						{
							storedValue.Base = skillBehaviour.GetValueAtLevel(skill.CurrentLevel);
						}
						else
						{
							storedValue.Base += skillBehaviour.GetValueAtLevel(skill.CurrentLevel);
						}
					}
					else if (skillBehaviour.ValueType == AttributeValueType.Calculation)
					{
						if (flag && skillBehaviour.UseCustomInitialValue)
						{
							storedValue.Base = skillBehaviour.InitialValue;
						}
						storedValue.AddCalculation(skillBehaviour.CalculationType, skillBehaviour.GetValueAtLevel(skill.CurrentLevel - i));
					}
				}
				if (skill.name == "_TierSkill")
				{
				}
				storedValue.CalculateTotal();
				if (skillBehaviour.ChangeValue is Skills.CostReduction)
				{
					Skills.CostReduction costReduction = skillBehaviour.ChangeValue as Skills.CostReduction;
					Skill skill2 = costReduction.SetCostReduction(storedValue.Total);
					this.CanSkillBeLeveledUp(skill2, true);
				}
				if (this.OnSkillAttributeValueChanged != null && total != storedValue.Total)
				{
					this.OnSkillAttributeValueChanged(skillBehaviour.ChangeValue, storedValue.Total);
				}
			}
			else
			{
				UnityEngine.Debug.LogError("SkillManager.OnSkillLevelUpCallback, skillBehaviour is null!");
			}
		}
		if (this.OnSkillLevelChanged != null)
		{
			this.OnSkillLevelChanged(skill, change);
		}
	}

	private void InitBaseSkills()
	{
		foreach (Skill skill in this.baseSkills)
		{
			skill.SetCurrentLevel(1, LevelChange.Initialization);
		}
		this.fishValueBonusSkill.SetCurrentLevel(1, LevelChange.Initialization);
	}

	public void LoadSavedSkillLevels()
	{
		foreach (SkillManager.SkillsAtTierLevels skillsAtTierLevels in this.tierSkills)
		{
			foreach (Skill skill in skillsAtTierLevels.Skills)
			{
				this.LoadSkill(skill, 0);
			}
		}
		this.LoadSkill(this.coreTierSkill, 0);
		this.LoadSkill(this.coreDeepWaterSkill, 0);
		this.LoadSkill(this.corePrestigeSkill, 0);
		this.LoadSkill(this.coreQuestSkill, 0);
		this.LoadSkill(this.coreUnlockCrewMemberSkill, 0);
		this.LoadSkill(this.collectStarsSkill, 0);
		this.LoadSkill(this.jackpotSkill, 0);
		this.LoadSkill(this.fishValueBonusSkill, 1);
		this.LoadSkill(this.CrownLevelSkill, 0);
		foreach (Skill skill2 in this.crewMembers)
		{
			this.LoadSkill(skill2, 0);
		}
		foreach (Skill skill3 in this.otherSkills)
		{
			this.LoadSkill(skill3, 0);
		}
		foreach (Skill skill4 in this.SkillTreeSkills)
		{
			this.LoadSkill(skill4, 0);
		}
		foreach (Skill skill5 in this.CrownRewardSkills)
		{
			this.LoadSkill(skill5, 0);
		}
	}

	private void OnApplicationPause(bool didPause)
	{
		if (!didPause || !this.hasHadTheChanceToLoadSkillsFromDisk)
		{
			return;
		}
		if (!TournamentManager.Instance.IsInsideTournament)
		{
			this.SaveCurrentStateOfSkills(true);
		}
	}

	public void SaveCurrentStateOfSkills(bool saveToDisk = true)
	{
		this.SaveSkill(this.coreTierSkill);
		this.SaveSkill(this.coreDeepWaterSkill);
		this.SaveSkill(this.corePrestigeSkill);
		this.SaveSkill(this.coreQuestSkill);
		this.SaveSkill(this.coreUnlockCrewMemberSkill);
		this.SaveSkill(this.collectStarsSkill);
		this.SaveSkill(this.jackpotSkill);
		this.SaveSkill(this.fishValueBonusSkill);
		this.SaveSkill(this.CrownLevelSkill);
		foreach (Skill skill in this.crewMembers)
		{
			this.SaveSkill(skill);
		}
		foreach (SkillManager.SkillsAtTierLevels skillsAtTierLevels in this.tierSkills)
		{
			foreach (Skill skill2 in skillsAtTierLevels.Skills)
			{
				this.SaveSkill(skill2);
			}
		}
		foreach (Skill skill3 in this.otherSkills)
		{
			this.SaveSkill(skill3);
		}
		foreach (Skill skill4 in this.SkillTreeSkills)
		{
			this.SaveSkill(skill4);
		}
		foreach (Skill skill5 in this.CrownRewardSkills)
		{
			this.SaveSkill(skill5);
		}
		for (int i = 0; i < this.allSkills.Count; i++)
		{
			Skill skill6 = this.allSkills[i];
			if (skill6.IsActiveSkill || skill6.GetExtraInfo().IsCrew)
			{
				skill6.OnApplicationPause();
			}
		}
		if (saveToDisk)
		{
			EncryptedPlayerPrefs.Save();
		}
	}

	private void SaveSkill(Skill skill)
	{
		skill.SaveMiscData();
		EncryptedPlayerPrefs.SetInt(this.GetKeySkillLevel(skill, SkillManager.SkillSaveKeyType.Current), skill.CurrentLevel, true);
		EncryptedPlayerPrefs.SetInt(this.GetKeySkillLevel(skill, SkillManager.SkillSaveKeyType.Lifetime), skill.LifetimeLevel, true);
		EncryptedPlayerPrefs.SetInt(this.GetKeySkillLevel(skill, SkillManager.SkillSaveKeyType.Highest), skill.HighestLevel, true);
		EncryptedPlayerPrefs.SetString(this.GetKeySkillMetaData(skill), JsonConvert.SerializeObject(skill.MetaDataDict), true);
		if (skill.GetExtraInfo().CacheCurrentLevelAsBigInteger && skill.CurrentLevelAsLong > 0L)
		{
			EncryptedPlayerPrefs.SetString(this.GetKeySkillLevel(skill, SkillManager.SkillSaveKeyType.CurrentAsLong), skill.CurrentLevelAsLong.ToString(), true);
		}
	}

	public void LoadSkill(Skill skill, int ifNotFoundUseLevel = 0)
	{
		skill.LoadMiscData();
		skill.SetCurrentLevel(Mathf.Min(skill.MaxLevel, EncryptedPlayerPrefs.GetInt(this.GetKeySkillLevel(skill, SkillManager.SkillSaveKeyType.Current), ifNotFoundUseLevel)), LevelChange.Initialization);
		skill.SetLifetimeLevel(EncryptedPlayerPrefs.GetInt(this.GetKeySkillLevel(skill, SkillManager.SkillSaveKeyType.Lifetime), ifNotFoundUseLevel));
		skill.SetHighestLevel(EncryptedPlayerPrefs.GetInt(this.GetKeySkillLevel(skill, SkillManager.SkillSaveKeyType.Highest), ifNotFoundUseLevel));
		skill.MetaDataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(EncryptedPlayerPrefs.GetString(this.GetKeySkillMetaData(skill), "{}"));
		if (skill.GetExtraInfo().CacheCurrentLevelAsBigInteger)
		{
			string @string = EncryptedPlayerPrefs.GetString(this.GetKeySkillLevel(skill, SkillManager.SkillSaveKeyType.CurrentAsLong), skill.CurrentLevel.ToString());
			skill.SetCurrentLevelAsLong(long.Parse(@string), LevelChange.Initialization);
		}
	}

	private string GetKeySkillMetaData(Skill skill)
	{
		return skill.Id + "_metadata";
	}

	private string GetKeySkillLevel(Skill skill, SkillManager.SkillSaveKeyType type)
	{
		switch (type)
		{
		case SkillManager.SkillSaveKeyType.Current:
			return skill.Id + "_current_level";
		case SkillManager.SkillSaveKeyType.Lifetime:
			return skill.Id + "_lifetime_level";
		case SkillManager.SkillSaveKeyType.Highest:
			return skill.Id + "_highest_level";
		case SkillManager.SkillSaveKeyType.CurrentAsLong:
			return skill.Id + "_currentAsLong_level";
		default:
			return null;
		}
	}

	private void ValidateSkills()
	{
		if (this.tierSkills.Count < this.coreTierSkill.MaxLevel)
		{
			int num = this.coreTierSkill.MaxLevel - this.tierSkills.Count;
			for (int i = 0; i < num; i++)
			{
				this.tierSkills.Add(new SkillManager.SkillsAtTierLevels());
			}
			UnityEngine.Debug.LogWarning("The amount of Tiers is not the same as the Max Amount of Tiers (missing: " + num + " Tiers). Adding empty Tiers for now...");
		}
		HashSet<string> hashSet = new HashSet<string>();
		foreach (Skill skill in this.allSkills)
		{
			if (!hashSet.Add(skill.Id))
			{
				UnityEngine.Debug.LogWarning(string.Concat(new string[]
				{
					"The skill: '",
					skill.name,
					"' has the same Id(",
					skill.Id,
					") as another skill."
				}));
				skill.GenerateId(true);
			}
		}
	}

	[SerializeField]
	private ItemManager itemManager;

	[SerializeField]
	private Skill coreTierSkill;

	[SerializeField]
	private Skill coreDeepWaterSkill;

	[SerializeField]
	private Skill corePrestigeSkill;

	[SerializeField]
	private Skill coreQuestSkill;

	[SerializeField]
	private Skill coreUnlockCrewMemberSkill;

	[SerializeField]
	private Skill collectStarsSkill;

	[SerializeField]
	private Skill jackpotSkill;

	[SerializeField]
	private Skill fishValueBonusSkill;

	[SerializeField]
	private List<Skill> baseSkills = new List<Skill>();

	[SerializeField]
	private List<SkillManager.SkillsAtTierLevels> tierSkills = new List<SkillManager.SkillsAtTierLevels>();

	[SerializeField]
	private List<Skill> crewMembers = new List<Skill>();

	[SerializeField]
	private List<Skill> otherSkills = new List<Skill>();

	private List<Skill> allSkills = new List<Skill>();

	public Func<Skill, bool> HasEnoughResources;

	private Dictionary<int, BigInteger> latestSkillCostResourceAmount = new Dictionary<int, BigInteger>();

	private bool hasHadTheChanceToLoadSkillsFromDisk;

	private enum SkillSaveKeyType
	{
		Current,
		Lifetime,
		Highest,
		CurrentAsLong
	}

	[Serializable]
	public class SkillsAtTierLevels
	{
		[FullInspector.InspectorName("Skills for Tier")]
		public List<Skill> Skills = new List<Skill>();
	}
}
