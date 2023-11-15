using System;
using UnityEngine;

public class TutorialSliceFreeGift : TutorialSliceBase
{
	private void Start()
	{
	}

	private void DwProgressskill_OnSkillLevelUp(Skill skill, LevelChange arg2)
	{
		if (skill.CurrentLevel < this.atWhatLevelToTrigger)
		{
			return;
		}
		TutorialManager.Instance.SetGraphicRaycaster(true);
		this.dwProgressskill.OnSkillLevelUp -= this.DwProgressskill_OnSkillLevelUp;
		this.RunAfterDelay(3f, delegate()
		{
			ScreenManager.Instance.GoToScreen(0);
			ChestManager.Instance.OpenChest(this.chestToRecieve);
			base.Exit(true);
		});
	}

	protected override void Setup()
	{
		base.Setup();
		this.dwProgressskill.OnSkillLevelUp += this.DwProgressskill_OnSkillLevelUp;
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

	[SerializeField]
	private ItemChest chestToRecieve;
}
