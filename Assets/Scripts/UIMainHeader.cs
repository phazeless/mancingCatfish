using System;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class UIMainHeader : MonoBehaviour
{
	private void Start()
	{
		SkillManager.Instance.OnSkillLevelChanged += this.Instance_OnSkillLevelChanged;
		ResourceManager.Instance.OnResourceChanged += this.ResourceManager_OnResourceChanged;
		this.deepWaterItems = new List<UIDeepWaterItem>(this.dwLvlIconsHolder.GetComponentsInChildren<UIDeepWaterItem>());
		this.Instance_OnSkillLevelChanged(SkillManager.Instance.DeepWaterSkill, LevelChange.Initialization);
	}

	private void ResourceManager_OnResourceChanged(ResourceType resourceType, BigInteger amount, BigInteger totalAmount)
	{
		if (resourceType == ResourceType.Cash)
		{
			this.dwLvlProgressMeter.SetCurrent(totalAmount);
		}
	}

	private void Instance_OnSkillLevelChanged(Skill skill, LevelChange levelChange)
	{
		if (skill == SkillManager.Instance.DeepWaterSkill)
		{
			this.dwLvlProgressMeter.SetMax(skill.CostForNextLevelUp);
		}
	}

	[SerializeField]
	private TextMeshProUGUI uiCash;

	[SerializeField]
	private UIMeterBigInteger dwLvlProgressMeter;

	[SerializeField]
	private Transform dwLvlIconsHolder;

	private List<UIDeepWaterItem> deepWaterItems = new List<UIDeepWaterItem>();
}
