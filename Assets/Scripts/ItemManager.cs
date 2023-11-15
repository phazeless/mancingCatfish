using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	public static ItemManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Item> OnItemUnlocked;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Item, ItemEquipState> OnItemEquipStateChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Item, LevelChange, int, int> OnItemLevelChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Item, int, int, ResourceChangeReason> OnItemAmountChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Item, bool> OnItemUpgradeAvailabilityChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<ISkillAttribute, float> OnItemAttributeValueChanged;

	public List<Item> UnlockedItems
	{
		get
		{
			return this.allItems.FindAll((Item x) => x.IsUnlocked);
		}
	}

	public List<Item> AllItems
	{
		get
		{
			return this.allItems;
		}
	}

	public List<Item> GetItemsWith(Rarity rarity, bool includeHolidayItems = false)
	{
		if (includeHolidayItems)
		{
			return this.allItems.FindAll((Item x) => x.Rarity == rarity);
		}
		return this.allItems.FindAll((Item x) => x.Rarity == rarity && !x.IsHolidayItem);
	}

	private void Awake()
	{
		ItemManager.Instance = this;
		for (int i = 0; i < this.allItems.Count; i++)
		{
			Item item = this.allItems[i];
			item.OnItemLevelUp += this.OnItemLevelUpCallback;
			item.OnItemUpgradeAvailabilityChanged += this.Item_OnItemUpgradeAvailabilityChanged;
			item.OnItemEquipStateChanged += this.OnItemEquipStateChangedCallback;
			item.OnItemAmountChanged += this.Item_OnItemAmountChanged;
			item.Load();
		}
	}

	private void Item_OnItemAmountChanged(Item item, int currentAmount, int oldAmount, ResourceChangeReason reason)
	{
		if (this.OnItemAmountChanged != null)
		{
			this.OnItemAmountChanged(item, currentAmount, oldAmount, reason);
		}
	}

	private void Start()
	{
		for (int i = 0; i < this.allItems.Count; i++)
		{
			Item item = this.allItems[i];
			if (item.IsEquipped)
			{
				item.NotifyEquipState(ItemEquipState.Equipped);
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < this.allItems.Count; i++)
		{
			Item item = this.allItems[i];
			if (item.IsEquipped)
			{
				item.UpdateCustomLogics();
			}
		}
	}

	private void OnItemEquipStateChangedCallback(Item item, ItemEquipState equipState)
	{
		this.RefreshItemSkillValues(item, equipState);
		if (this.OnItemEquipStateChanged != null)
		{
			this.OnItemEquipStateChanged(item, equipState);
		}
	}

	private void Item_OnItemUpgradeAvailabilityChanged(Item item, bool isAvailable)
	{
		if (this.OnItemUpgradeAvailabilityChanged != null)
		{
			this.OnItemUpgradeAvailabilityChanged(item, isAvailable);
		}
	}

	private void OnItemLevelUpCallback(Item item, LevelChange levelChange, int itemAmountSpent, int gemCost)
	{
		if (item.CurrentLevel == 1 && this.OnItemUnlocked != null)
		{
			this.OnItemUnlocked(item);
		}
		if (item.IsEquipped)
		{
			this.UpdateCachedValues(item, levelChange);
		}
		if (this.OnItemLevelChanged != null)
		{
			this.OnItemLevelChanged(item, levelChange, itemAmountSpent, gemCost);
		}
	}

	public void OnResourceChanged(ResourceType resourceType, BigInteger amount)
	{
		for (int i = 0; i < this.allItems.Count; i++)
		{
			this.allItems[i].OnResourceChanged(resourceType, amount);
		}
	}

	public void ClearAllItemSkillValues()
	{
		foreach (Item item in this.allItems)
		{
			this.ClearItemSkillValue(item);
		}
	}

	public void RefreshAllEquippedItemValues()
	{
		foreach (Item item in this.allItems)
		{
			if (item.IsEquipped)
			{
				this.RefreshItemSkillValues(item, ItemEquipState.Equipped);
			}
		}
	}

	private void ClearItemSkillValue(Item item)
	{
		foreach (SkillBehaviour skillBehaviour in item.SkillBehaviours)
		{
			StoredValue storedValue = ItemAndSkillValues.GetStoredValue(skillBehaviour);
			storedValue.ClearCalculations();
			storedValue.CalculateTotal();
		}
	}

	private void RefreshItemSkillValues(Item item, ItemEquipState equipState)
	{
		if (item.CurrentLevel == 0)
		{
			return;
		}
		foreach (SkillBehaviour skillBehaviour in item.SkillBehaviours)
		{
			StoredValue storedValue = ItemAndSkillValues.GetStoredValue(skillBehaviour);
			float total = storedValue.Total;
			if (skillBehaviour.ValueType == AttributeValueType.Base)
			{
				if (skillBehaviour.ChangeMethod == AttributeValueChangeMethod.NeverChange)
				{
					storedValue.Base = skillBehaviour.GetValueAtLevel(item.CurrentLevel);
				}
				else if (equipState == ItemEquipState.Equipped)
				{
					storedValue.Base += skillBehaviour.GetValueAtLevel(item.CurrentLevel);
				}
				else if (equipState == ItemEquipState.Unequipped)
				{
					storedValue.Base -= skillBehaviour.GetValueAtLevel(item.CurrentLevel);
				}
			}
			else if (skillBehaviour.ValueType == AttributeValueType.Calculation)
			{
				if (equipState == ItemEquipState.Equipped)
				{
					storedValue.AddCalculation(skillBehaviour.CalculationType, skillBehaviour.GetTotalValueAtLevel(item.CurrentLevel));
				}
				else if (equipState == ItemEquipState.Unequipped)
				{
					storedValue.AddCalculation(skillBehaviour.CalculationType, -skillBehaviour.GetTotalValueAtLevel(item.CurrentLevel));
				}
			}
			storedValue.CalculateTotal();
			if (this.OnItemAttributeValueChanged != null && total != storedValue.Total)
			{
				this.OnItemAttributeValueChanged(skillBehaviour.ChangeValue, storedValue.Total);
			}
		}
	}

	private void UpdateCachedValues(Item item, LevelChange levelChange)
	{
		bool flag = item.CurrentLevel == 1;
		foreach (SkillBehaviour skillBehaviour in item.SkillBehaviours)
		{
			StoredValue storedValue = ItemAndSkillValues.GetStoredValue(skillBehaviour);
			float total = storedValue.Total;
			if (levelChange == LevelChange.HardReset || levelChange == LevelChange.SoftReset)
			{
				storedValue.ClearCalculations();
			}
			int num = item.CurrentLevel - item.PreviousLevel;
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
						storedValue.Base = skillBehaviour.GetValueAtLevel(item.CurrentLevel);
					}
					else
					{
						storedValue.Base += skillBehaviour.GetValueAtLevel(item.CurrentLevel);
					}
				}
				else if (skillBehaviour.ValueType == AttributeValueType.Calculation)
				{
					if (flag && skillBehaviour.UseCustomInitialValue)
					{
						storedValue.Base = skillBehaviour.InitialValue;
					}
					storedValue.AddCalculation(skillBehaviour.CalculationType, skillBehaviour.GetValueAtLevel(item.CurrentLevel - i));
				}
			}
			storedValue.CalculateTotal();
			if (skillBehaviour.ChangeValue is Skills.CostReduction)
			{
				Skills.CostReduction costReduction = skillBehaviour.ChangeValue as Skills.CostReduction;
				Skill skill = costReduction.SetCostReduction(storedValue.Total);
			}
			if (this.OnItemAttributeValueChanged != null && total != storedValue.Total)
			{
				this.OnItemAttributeValueChanged(skillBehaviour.ChangeValue, storedValue.Total);
			}
		}
	}

	public bool HasEquippedMaxAmount
	{
		get
		{
			return this.maxEquippedItems == this.EquippedItemCount;
		}
	}

	public List<Item> EquippedItems
	{
		get
		{
			return this.allItems.FindAll((Item x) => x.IsEquipped);
		}
	}

	public int EquippedItemCount
	{
		get
		{
			return this.allItems.FindAll((Item x) => x.IsEquipped).Count;
		}
	}

	public void SaveCurrentStateOfItems()
	{
		for (int i = 0; i < this.allItems.Count; i++)
		{
			Item item = this.allItems[i];
			item.Save();
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			this.SaveCurrentStateOfItems();
		}
	}

	[SerializeField]
	private int maxEquippedItems = 4;

	[SerializeField]
	private List<Item> allItems = new List<Item>();
}
