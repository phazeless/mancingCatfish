using System;
using UnityEngine;

public class TutorialSliceItemLimiter : TutorialSliceBase
{
	private void DwProgressskill_OnSkillLevelUp(Skill skill, LevelChange arg2)
	{
		if (skill.CurrentLevel < this.atWhatLevelToTrigger)
		{
			return;
		}
		this.dwProgressskill.OnSkillLevelUp -= this.DwProgressskill_OnSkillLevelUp;
		HarborBoxHolderReferencer.Instance.gameObject.SetActive(true);
		base.Exit(true);
	}

	protected override void Setup()
	{
		base.Setup();
		this.dwProgressskill.OnSkillLevelUp += this.DwProgressskill_OnSkillLevelUp;
		HarborBoxHolderReferencer.Instance.gameObject.SetActive(false);
	}

	protected override void Exited()
	{
		base.Exited();
		this.dwProgressskill.OnSkillLevelUp -= this.DwProgressskill_OnSkillLevelUp;
	}

	[SerializeField]
	private Skill dwProgressskill;

	[SerializeField]
	private int atWhatLevelToTrigger = 2;
}
