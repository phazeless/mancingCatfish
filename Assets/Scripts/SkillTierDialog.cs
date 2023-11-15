using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillTierDialog : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Start()
	{
		this.coreSkillTierSkill = SkillManager.Instance.TierSkill;
		this.tierSkills = SkillManager.Instance.TierSkills;
		this.coreSkillTierSkill.OnSkillLevelUp += this.OnTierUpgraded;
		this.UpdateList(0, this.coreSkillTierSkill.CurrentLevel);
	}

	private void OnTierUpgraded(Skill skill, LevelChange change)
	{
		this.UpdateList(skill.CurrentLevel - 1, skill.CurrentLevel);
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
				this.AddUnlockButton(i + 1);
			}
		}
	}

	private void AddUnlockButton(int tierLevel)
	{
		UIUnlockSkilllTierButton uiunlockSkilllTierButton = UnityEngine.Object.Instantiate<UIUnlockSkilllTierButton>(this.prefabUnlockTierButton, this.contentView, false);
		uiunlockSkilllTierButton.OnUpdateUI(this.coreSkillTierSkill);
	}

	private void AddSkillToList(Skill skill)
	{
		UISkillTierItem uiskillTierItem = UnityEngine.Object.Instantiate<UISkillTierItem>(this.prefabSkillTierItem, this.contentView, false);
	}

	[SerializeField]
	private Transform contentView;

	[SerializeField]
	private UIUnlockSkilllTierButton prefabUnlockTierButton;

	[SerializeField]
	private UISkillTierItem prefabSkillTierItem;

	private Skill coreSkillTierSkill;

	private IList<SkillManager.SkillsAtTierLevels> tierSkills = new List<SkillManager.SkillsAtTierLevels>();
}
