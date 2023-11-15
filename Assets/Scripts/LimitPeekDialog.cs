using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LimitPeekDialog : MonoBehaviour
{
	private void TweenDialog()
	{
		base.transform.localScale = Vector3.zero;
		base.transform.localEulerAngles = new Vector3(0f, 0f, 6f);
		base.transform.DORotate(Vector3.zero, 0.3f, RotateMode.Fast).SetEase(Ease.OutBack);
		base.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
		base.transform.DOMoveY(base.transform.position.y + 0.3f, 0.4f, false).SetEase(Ease.OutCirc);
	}

	public void Close()
	{
		base.transform.DOScale(0f, 0.15f).SetEase(Ease.InBack);
		base.transform.DORotate(new Vector3(0f, 0f, -6f), 0.15f, RotateMode.Fast).SetEase(Ease.InBack).OnComplete(delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		});
	}

	public void Setup(Skill skill)
	{
		this.currentSkill = skill;
		Vector3 position = this.edgeTransform.position;
		base.transform.position = new Vector3((0f - base.transform.position.x) * -0.4f, base.transform.position.y, base.transform.position.z);
		this.edgeTransform.position = position;
		this.TweenDialog();
		this.titleLabel.SetText(this.currentSkill.GetExtraInfo().TitleText);
		IList<SkillBehaviour> skillBehaviours = this.currentSkill.SkillBehaviours;
		foreach (SkillBehaviour skillBehaviour in skillBehaviours)
		{
			float valueAtLevel = skillBehaviour.GetValueAtLevel(this.currentSkill.NextLevel);
			string text = (valueAtLevel <= 0f) ? string.Empty : "+";
			float totalValueAtLevel = skillBehaviour.GetTotalValueAtLevel(this.currentSkill.CurrentLevel);
			string text2 = (totalValueAtLevel <= 0f) ? string.Empty : "+";
			string text3 = FHelper.FindBracketAndReplace(skillBehaviour.Description, new string[]
			{
				string.Concat(new object[]
				{
					"<b>",
					text,
					valueAtLevel,
					skillBehaviour.PostFixCharacter,
					"</b>"
				})
			});
			TextMeshProUGUI textMeshProUGUI = UnityEngine.Object.Instantiate<TextMeshProUGUI>(this.attributeLabel, this.attributeLabel.transform.parent);
			this.attributeLLabelist.Add(textMeshProUGUI);
			textMeshProUGUI.SetVariableText(new string[]
			{
				string.Empty,
				text3,
				string.Concat(new object[]
				{
					" (",
					text2,
					skillBehaviour.GetTotalValueAtLevel(this.currentSkill.CurrentLevel),
					skillBehaviour.PostFixCharacter,
					")"
				})
			});
		}
		UnityEngine.Object.Destroy(this.attributeLabel.gameObject);
	}

	private void UpdateUI()
	{
		for (int i = 0; i < this.attributeLLabelist.Count; i++)
		{
			IList<SkillBehaviour> skillBehaviours = this.currentSkill.SkillBehaviours;
			float valueAtLevel = skillBehaviours[i].GetValueAtLevel(this.currentSkill.NextLevel);
			string text = (valueAtLevel <= 0f) ? string.Empty : "+";
			float totalValueAtLevel = skillBehaviours[i].GetTotalValueAtLevel(this.currentSkill.CurrentLevel);
			string text2 = (totalValueAtLevel <= 0f) ? string.Empty : "+";
			string text3 = FHelper.FindBracketAndReplace(skillBehaviours[i].Description, new string[]
			{
				string.Concat(new object[]
				{
					"<b>",
					text,
					valueAtLevel,
					skillBehaviours[i].PostFixCharacter,
					"</b>"
				})
			});
			this.attributeLLabelist[i].SetVariableText(new string[]
			{
				string.Empty,
				text3,
				string.Concat(new object[]
				{
					" (",
					text2,
					skillBehaviours[i].GetTotalValueAtLevel(this.currentSkill.CurrentLevel),
					skillBehaviours[i].PostFixCharacter,
					")"
				})
			});
		}
	}

	private void TweenKiller()
	{
		base.transform.DOKill(false);
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	[SerializeField]
	[Header("References")]
	private TextMeshProUGUI titleLabel;

	[SerializeField]
	private TextMeshProUGUI attributeLabel;

	[SerializeField]
	private Transform edgeTransform;

	private Skill currentSkill;

	private List<TextMeshProUGUI> attributeLLabelist = new List<TextMeshProUGUI>();
}
