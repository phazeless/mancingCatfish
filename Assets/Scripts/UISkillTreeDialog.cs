using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UISkillTreeDialog : UpgradeDialogTween
{
	protected override void Start()
	{
		base.Start();
		this.UpdateSkillpointLabel();
		ResourceManager.Instance.OnResourceChanged += this.Instance_OnResourceChanged;
		this.crownLevelSkill.OnSkillLevelUp += this.CrownLevelSkill_OnSkillLevelUp;
		this.UpdateUI();
		SkilltreeSkill.OnPeekSkill += this.SkilltreeSkill_OnPeekSkill;
	}

	private void SkilltreeSkill_OnPeekSkill(Skill skill, Transform t)
	{
		if (this.currentPeekSkill == skill)
		{
			this.ClosePeekDialogs();
			return;
		}
		if (this.currentPeekSkill != null)
		{
			this.ClosePeekDialogs();
		}
		this.currentPeekSkill = skill;
		SkillTreeSkillUpgradeDialog skillTreeSkillUpgradeDialog = UnityEngine.Object.Instantiate<SkillTreeSkillUpgradeDialog>(this.skillTreeSkillUpgradeDialogPrefab, t);
		skillTreeSkillUpgradeDialog.transform.SetParent(this.PeekDialogPositionReference);
		skillTreeSkillUpgradeDialog.Setup(skill);
		this.skillTreeSkillUpgradeDialogList.Add(skillTreeSkillUpgradeDialog);
		this.cancelDialog.gameObject.SetActive(true);
	}

	public void ClosePeekDialogs()
	{
		foreach (SkillTreeSkillUpgradeDialog skillTreeSkillUpgradeDialog in this.skillTreeSkillUpgradeDialogList)
		{
			skillTreeSkillUpgradeDialog.Close();
		}
		this.skillTreeSkillUpgradeDialogList.Clear();
		this.cancelDialog.gameObject.SetActive(false);
		this.currentPeekSkill = null;
	}

	public override void Open()
	{
		base.Open();
		this.UpdateUI();
		if (TutorialSliceRoyalIntro.Instance != null)
		{
			TutorialSliceRoyalIntro.Instance.OpenedCrownDialog();
		}
	}

	public void DelayedOpen()
	{
		this.RunAfterDelay(1f, delegate()
		{
			this.Open();
		});
	}

	private void CrownLevelSkill_OnSkillLevelUp(Skill arg1, LevelChange arg2)
	{
		this.UpdateUI();
	}

	private void UpdateSkillpointLabel()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.skillPointLabel.SetVariableText(new string[]
		{
			ResourceManager.Instance.GetResourceAmount(ResourceType.SkillPoints).ToString()
		});
		this.TweenKiller();
		this.skillPointLabel.transform.DOPunchScale(new UnityEngine.Vector3(0.2f, 0.2f, 0.2f), 0.5f, 10, 1f);
	}

	private void Instance_OnResourceChanged(ResourceType res, BigInteger arg2, BigInteger arg3)
	{
		if (res == ResourceType.SkillPoints)
		{
			this.UpdateSkillpointLabel();
		}
		else if (res == ResourceType.CrownExp)
		{
			this.UpdateUI();
		}
	}

	private void UpdateUI()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.crownLevelLabel.SetVariableText(new string[]
		{
			this.crownLevelSkill.CurrentLevel.ToString()
		});
		int num = (int)ResourceManager.Instance.GetResourceAmount(ResourceType.CrownExp);
		int num2 = (int)this.crownLevelSkill.CostForNextLevelUp;
		this.crownLevelExpLabel.SetVariableText(new string[]
		{
			num.ToString(),
			num2.ToString()
		});
		this.UpdateSkillpointLabel();
		this.crownExpMeter.SetMax((float)num2);
		this.crownExpMeter.SetCurrent((float)num);
	}

	private void TweenKiller()
	{
		this.skillPointLabel.transform.DOKill(true);
	}

	public override void Close(bool destroyOnFinish = false)
	{
		base.Close(destroyOnFinish);
		this.crownSkillLimitManager.ClosePeekDialogs();
		this.ClosePeekDialogs();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ResourceManager.Instance.OnResourceChanged -= this.Instance_OnResourceChanged;
		SkilltreeSkill.OnPeekSkill -= this.SkilltreeSkill_OnPeekSkill;
		this.TweenKiller();
	}

	[SerializeField]
	private Skill crownLevelSkill;

	[SerializeField]
	[Header("References")]
	private UIMeter crownExpMeter;

	[SerializeField]
	private TextMeshProUGUI skillPointLabel;

	[SerializeField]
	private TextMeshProUGUI crownLevelLabel;

	[SerializeField]
	private TextMeshProUGUI crownLevelExpLabel;

	[SerializeField]
	private Transform cancelDialog;

	[SerializeField]
	private Transform PeekDialogPositionReference;

	[SerializeField]
	private SkillTreeSkillUpgradeDialog skillTreeSkillUpgradeDialogPrefab;

	[SerializeField]
	private CrownSkillLimitManager crownSkillLimitManager;

	private List<SkillTreeSkillUpgradeDialog> skillTreeSkillUpgradeDialogList = new List<SkillTreeSkillUpgradeDialog>();

	private Skill currentPeekSkill;
}
