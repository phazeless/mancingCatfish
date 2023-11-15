using System;
using System.Collections.Generic;
using UnityEngine;

public class CrownSkillLimitManager : MonoBehaviour
{
	public void Start()
	{
		SkilltreeSkill.OnPeekSkill += this.SkilltreeSkill_OnPeekSkill;
	}

	private void SkilltreeSkill_OnPeekSkill(Skill arg1, Transform arg2)
	{
		this.ClosePeekDialogs();
	}

	public void SetAndUpdateLimits(Skill crownLevelSkill, List<Skill> crownRewardSkills)
	{
		for (int i = 0; i < crownRewardSkills.Count; i++)
		{
			bool flag = i < this.CrownLevelLimitBehaviourList.Count;
			CrownLevelLimitBehaviour crownLevelLimitBehaviour;
			if (flag)
			{
				crownLevelLimitBehaviour = this.CrownLevelLimitBehaviourList[i];
			}
			else
			{
				crownLevelLimitBehaviour = UnityEngine.Object.Instantiate<CrownLevelLimitBehaviour>(this.limitPrefab, base.transform);
				this.CrownLevelLimitBehaviourList.Add(crownLevelLimitBehaviour);
				crownLevelLimitBehaviour.SetRewardBanner(i);
			}
			crownLevelLimitBehaviour.CrownSkillLimitManagerInstance = this;
			crownLevelLimitBehaviour.UpdateUi(crownLevelSkill, crownRewardSkills[i]);
		}
	}

	public void ClosePeekDialogs()
	{
		foreach (LimitPeekDialog limitPeekDialog in this.limitPeekDialogList)
		{
			limitPeekDialog.Close();
		}
		this.limitPeekDialogList.Clear();
		this.currentPeekSkill = null;
	}

	public void PeekLimit(Skill skill, Transform t)
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
		LimitPeekDialog limitPeekDialog = UnityEngine.Object.Instantiate<LimitPeekDialog>(this.limitPeekDialogPrefab, t);
		limitPeekDialog.transform.SetParent(t.parent.parent);
		limitPeekDialog.Setup(skill);
		this.limitPeekDialogList.Add(limitPeekDialog);
		this.cancelDialog.gameObject.SetActive(true);
	}

	private void OnDestroy()
	{
		SkilltreeSkill.OnPeekSkill -= this.SkilltreeSkill_OnPeekSkill;
	}

	[SerializeField]
	private CrownLevelLimitBehaviour limitPrefab;

	[SerializeField]
	private LimitPeekDialog limitPeekDialogPrefab;

	[SerializeField]
	private Transform cancelDialog;

	private List<CrownLevelLimitBehaviour> CrownLevelLimitBehaviourList = new List<CrownLevelLimitBehaviour>();

	private List<LimitPeekDialog> limitPeekDialogList = new List<LimitPeekDialog>();

	private Skill currentPeekSkill;
}
