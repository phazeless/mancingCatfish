using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillUpgradeList : UIListNormal
{
	public void Toggle()
	{
		base.gameObject.SetActive(!base.gameObject.activeInHierarchy);
	}

	private void Start()
	{
		this.coreSkillTierSkill = SkillManager.Instance.TierSkill;
		this.tierSkills = SkillManager.Instance.TierSkills;
		this.coreSkillTierSkill.SetVisualPrefab(this.prefabUnlockTierButton);
		this.coreSkillTierSkill.OnSkillLevelUp += this.OnTierUpgraded;
		for (int i = 0; i < SkillManager.Instance.TierSkills.Count; i++)
		{
			List<Skill> skills = SkillManager.Instance.TierSkills[i].Skills;
			foreach (Skill skill in skills)
			{
				skill.SetVisualPrefab(this.prefabSkillTierItem);
			}
		}
		this.UpdateList(0, this.coreSkillTierSkill.CurrentLevel);
	}

	private void OnTierUpgraded(Skill skill, LevelChange change)
	{
		if (skill.CurrentLevel == 0 || change == LevelChange.Initialization)
		{
			this.ClearList();
			this.UpdateList(0, this.coreSkillTierSkill.CurrentLevel);
		}
		else
		{
			base.Remove(this.coreSkillTierSkill);
			this.UpdateList(skill.CurrentLevel - 1, skill.CurrentLevel);
		}
	}

	private void UpdateList(int fromTierLevel, int currentSkillTierLevel)
	{
		for (int i = fromTierLevel; i < currentSkillTierLevel + 1; i++)
		{
			if (i < currentSkillTierLevel)
			{
				for (int j = 0; j < this.tierSkills[i].Skills.Count; j++)
				{
					this.AddSkillToList(this.tierSkills[i].Skills[j]);
				}
			}
			else
			{
				this.AddUnlockButton();
			}
		}
	}

	private void ClearList()
	{
		base.Clear();
	}

	private void AddUnlockButton()
	{
		base.AddItem(this.coreSkillTierSkill, true);
	}

	private void AddSkillToList(Skill skill)
	{
		base.AddItem(skill, true);
	}

	[SerializeField]
	private UIListItem prefabSkillTierItem;

	[SerializeField]
	private UIListItem prefabUnlockTierButton;

	private Skill coreSkillTierSkill;

	private IList<SkillManager.SkillsAtTierLevels> tierSkills = new List<SkillManager.SkillsAtTierLevels>();
}
