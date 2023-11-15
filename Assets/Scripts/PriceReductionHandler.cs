using System;
using System.Collections.Generic;
using UnityEngine;

public class PriceReductionHandler : MonoBehaviour
{
	private void Awake()
	{
		this.ticketItem.OnItemEquipStateChanged += this.TicketItem_OnItemEquipStateChanged;
		this.ticketItem.OnItemLevelUp += this.TicketItem_OnItemLevelUp;
	}

	private void TicketItem_OnItemLevelUp(Item item, LevelChange levelChange, int itemAmountSpent, int gemCost)
	{
		this.UpdateValues();
	}

	private void TicketItem_OnItemEquipStateChanged(Item item, ItemEquipState state)
	{
		this.UpdateValues();
	}

	private void UpdateValues()
	{
		float currentTotalValueFor = ItemAndSkillValues.GetCurrentTotalValueFor<Skills.TierSkillsPriceReduction>();
		IList<SkillManager.SkillsAtTierLevels> tierSkills = SkillManager.Instance.TierSkills;
		foreach (SkillManager.SkillsAtTierLevels skillsAtTierLevels in tierSkills)
		{
			foreach (Skill skill in skillsAtTierLevels.Skills)
			{
				skill.SetCostReduction(currentTotalValueFor);
				SkillManager.Instance.CanSkillBeLeveledUp(skill, true);
			}
		}
	}

	[SerializeField]
	private Item ticketItem;
}
