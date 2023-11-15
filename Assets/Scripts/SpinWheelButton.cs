using System;
using TMPro;
using UnityEngine;

public class SpinWheelButton : MonoBehaviour
{
	private void Awake()
	{
		this.freeSpinSkill.OnSkillLevelUp += this.FreeSpinSkill_OnSkillLevelUp;
	}

	private void FreeSpinSkill_OnSkillLevelUp(Skill arg1, LevelChange arg2)
	{
		this.UpdateUI();
	}

	private void OnEnable()
	{
		this.freeSpinSkill.OnSkillLevelUp += this.FreeSpinSkill_OnSkillLevelUp;
		this.UpdateUI();
	}

	private void OnDisable()
	{
		this.freeSpinSkill.OnSkillLevelUp -= this.FreeSpinSkill_OnSkillLevelUp;
	}

	private void UpdateUI()
	{
		this.spinButtonLabel.text = ((this.freeSpinSkill.CurrentLevel <= 0) ? "Watch Ad to Spin" : "Free Spin");
	}

	[SerializeField]
	private Skill freeSpinSkill;

	[SerializeField]
	private TextMeshProUGUI spinButtonLabel;
}
