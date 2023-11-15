using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using FullInspector;
using UnityEngine;

public class Item : BaseScriptableObject
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Item, LevelChange, int, int> OnItemLevelUp;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Item, int, int, ResourceChangeReason> OnItemAmountChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Item, bool> OnItemUpgradeAvailabilityChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Item, ItemEquipState> OnItemEquipStateChanged;

	public string Id
	{
		get
		{
			return this.id;
		}
	}

	[InspectorDisabled]
	[ShowInInspector]
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
	public int CurrentItemAmount
	{
		get
		{
			return this.currentItemAmount;
		}
	}

	public bool IsMaxLevel
	{
		get
		{
			return this.CurrentLevel == this.maxLevel;
		}
	}

	public bool IsEquipped
	{
		get
		{
			return this.isEquipped;
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

	public string Title
	{
		get
		{
			return this.title;
		}
	}

	public string Description
	{
		get
		{
			return this.description;
		}
	}

	public Sprite Icon
	{
		get
		{
			return this.icon;
		}
	}

	public Color IconBgColor
	{
		get
		{
			return this.iconBgColor;
		}
	}

	public Rarity Rarity
	{
		get
		{
			return this.rarity;
		}
	}

	public bool IsHolidayItem
	{
		get
		{
			return this.isHolidayItem;
		}
	}

	public int TotalItemAmountRequiredForNextLevel
	{
		get
		{
			return (int)Mathf.Min(Mathf.Pow(2f, (float)this.CurrentLevel), 20f);
		}
	}

	public int TotalGemsRequiredForCurrentLevel
	{
		get
		{
			return this.GetGemsRequiredForLevel(this.CurrentLevel - 1);
		}
	}

	public int TotalGemsRequiredForNextLevel
	{
		get
		{
			return this.GetGemsRequiredForLevel(this.CurrentLevel);
		}
	}

	private int GetGemsRequiredForLevel(int level)
	{
		int num = (int)(this.rarity + 1);
		return (int)Mathf.Pow(2f, (float)(num + level));
	}

	public float ProgressUntilNextLevel
	{
		get
		{
			return (float)(this.CurrentItemAmount / this.TotalItemAmountRequiredForNextLevel);
		}
	}

	public bool HasEnoughItemAmountToLevelUp
	{
		get
		{
			return this.CurrentItemAmount >= this.TotalItemAmountRequiredForNextLevel;
		}
	}

	public bool HasEnoughGemsToLevelUp
	{
		get
		{
			return this.lastKnownResourceAmount >= (long)this.TotalGemsRequiredForNextLevel;
		}
	}

	public bool IsUnlocked
	{
		get
		{
			return this.CurrentLevel > 0;
		}
	}

	public IList<SkillBehaviour> SkillBehaviours
	{
		get
		{
			return this.skillBehaviours.AsReadOnly();
		}
	}

	public void ChangeItemAmount(int amount, ResourceChangeReason reason)
	{
		int arg = this.currentItemAmount;
		this.currentItemAmount += amount;
		if (this.currentItemAmount < 0)
		{
			this.currentItemAmount = 0;
		}
		if (this.OnItemAmountChanged != null)
		{
			this.OnItemAmountChanged(this, this.currentItemAmount, arg, reason);
		}
	}

	public void TryLevelUp(Action<Item, bool> levelUpCallback)
	{
		bool arg = false;
		if (this.HasEnoughItemAmountToLevelUp && this.HasEnoughGemsToLevelUp && !this.IsMaxLevel)
		{
			this.ChangeItemAmount(-this.TotalItemAmountRequiredForNextLevel, ResourceChangeReason.UpgradeItem);
			this.CurrentLevel++;
			if (this.OnItemLevelUp != null)
			{
				this.OnItemLevelUp(this, LevelChange.LevelUp, this.TotalItemAmountRequiredForNextLevel, this.TotalGemsRequiredForCurrentLevel);
			}
			arg = true;
		}
		if (levelUpCallback != null)
		{
			levelUpCallback(this, arg);
		}
	}

	public void NotifyEquipState(ItemEquipState state)
	{
		if (this.OnItemEquipStateChanged != null)
		{
			this.OnItemEquipStateChanged(this, state);
		}
	}

	public void Equip()
	{
		if (!this.isEquipped)
		{
			this.isEquipped = true;
			if (this.OnItemEquipStateChanged != null)
			{
				this.OnItemEquipStateChanged(this, ItemEquipState.Equipped);
			}
			for (int i = 0; i < this.customLogics.Count; i++)
			{
				this.customLogics[i].OnEquipped();
			}
		}
	}

	public void Unequip()
	{
		if (this.isEquipped)
		{
			this.isEquipped = false;
			if (this.OnItemEquipStateChanged != null)
			{
				this.OnItemEquipStateChanged(this, ItemEquipState.Unequipped);
			}
			for (int i = 0; i < this.customLogics.Count; i++)
			{
				this.customLogics[i].OnUnequipped();
			}
		}
	}

	public void OnResourceChanged(ResourceType resourceType, BigInteger newAmount)
	{
		if (resourceType == ResourceType.Gems)
		{
			this.lastKnownResourceAmount = newAmount;
			bool flag = this.HasEnoughGemsToLevelUp != this.wasUpgradeAvailable;
			if (flag)
			{
				this.wasUpgradeAvailable = this.HasEnoughGemsToLevelUp;
				if (this.OnItemUpgradeAvailabilityChanged != null)
				{
					this.OnItemUpgradeAvailabilityChanged(this, this.HasEnoughGemsToLevelUp);
				}
			}
		}
	}

	public void UpdateCustomLogics()
	{
		for (int i = 0; i < this.customLogics.Count; i++)
		{
			this.customLogics[i].Update(this);
		}
	}

	public void Save()
	{
		EncryptedPlayerPrefs.SetInt(this.GetKeyForCurrentLevel(), this.CurrentLevel, true);
		EncryptedPlayerPrefs.SetInt(this.GetKeyForCurrentAmount(), this.currentItemAmount, true);
		EncryptedPlayerPrefs.SetInt(this.GetKeyForIsEquipped(), (!this.isEquipped) ? 0 : 1, true);
		for (int i = 0; i < this.customLogics.Count; i++)
		{
			this.customLogics[i].Save();
		}
	}

	public void Load()
	{
		this.CurrentLevel = EncryptedPlayerPrefs.GetInt(this.GetKeyForCurrentLevel(), this.CurrentLevel);
		this.currentItemAmount = EncryptedPlayerPrefs.GetInt(this.GetKeyForCurrentAmount(), this.currentItemAmount);
		this.isEquipped = (EncryptedPlayerPrefs.GetInt(this.GetKeyForIsEquipped(), 0) == 1);
	}

	private string GetKeyForCurrentLevel()
	{
		return this.id + "_current_level";
	}

	private string GetKeyForCurrentAmount()
	{
		return this.id + "_current_amount";
	}

	private string GetKeyForIsEquipped()
	{
		return this.id + "_is_equipped";
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
	[InspectorDisabled]
	[ShowInInspector]
	private string id;

	[SerializeField]
	private int maxLevel = 1;

	[SerializeField]
	private Sprite icon;

	[SerializeField]
	private Color iconBgColor;

	[SerializeField]
	private Rarity rarity;

	[SerializeField]
	private string title;

	[SerializeField]
	private string description;

	[SerializeField]
	private List<SkillBehaviour> skillBehaviours = new List<SkillBehaviour>();

	[SerializeField]
	private bool isHolidayItem;

	[SerializeField]
	private List<BaseItemCustomLogic> customLogics = new List<BaseItemCustomLogic>();

	[NonSerialized]
	private int currentItemAmount;

	[NonSerialized]
	private int currentLevel;

	[NonSerialized]
	private int previousLevel;

	[NonSerialized]
	private BigInteger lastKnownResourceAmount = 0;

	[NonSerialized]
	private bool wasUpgradeAvailable;

	[NonSerialized]
	private bool isEquipped;
}
