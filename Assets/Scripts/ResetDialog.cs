using System;
using System.Numerics;
using TMPro;
using UnityEngine;

public class ResetDialog : UpgradeDialogTween
{
	private int ResetSkillsCost
	{
		get
		{
			return SkillTreeManager.Instance.ResetSkillsCost;
		}
	}

	public void ResetSkills()
	{
		if (ResourceManager.Instance.TakeResource(ResourceType.Gems, this.ResetSkillsCost) && this.hasSpentPoints)
		{
			this.hasSpentPoints = false;
			SkillTreeManager.Instance.ResetSkills();
			AudioManager.Instance.PlaySpecial(this.resetSound);
			this.UpdateUI();
		}
	}

	private void OnEnable()
	{
		ResourceManager.Instance.OnResourceChanged += this.Instance_OnResourceChanged;
	}

	private void OnDisable()
	{
		ResourceManager.Instance.OnResourceChanged -= this.Instance_OnResourceChanged;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ResourceManager.Instance.OnResourceChanged -= this.Instance_OnResourceChanged;
	}

	public override void Open()
	{
		base.Open();
		this.UpdateUI();
	}

	private void Instance_OnResourceChanged(ResourceType arg1, BigInteger arg2, BigInteger arg3)
	{
		if (arg1 == ResourceType.Gems)
		{
			this.UpdateUI();
		}
	}

	private void UpdateUI()
	{
		for (int i = 0; i < SkillTreeManager.Instance.SkillTreeSkills.Count; i++)
		{
			Skill skill = SkillTreeManager.Instance.SkillTreeSkills[i];
			if (skill.CurrentLevel > 0)
			{
				this.hasSpentPoints = true;
				break;
			}
		}
		this.button.interactable = (ResourceManager.Instance.GetResourceAmount(ResourceType.Gems) >= (long)this.ResetSkillsCost && this.hasSpentPoints);
		this.costLabel.SetVariableText(new string[]
		{
			this.ResetSkillsCost.ToString()
		});
	}

	[SerializeField]
	private UIScaleButton button;

	[SerializeField]
	private TextMeshProUGUI costLabel;

	[SerializeField]
	private AudioClip resetSound;

	private bool hasSpentPoints;
}
