using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUpgradeButtonBehaviour : MonoBehaviour
{
	private void Awake()
	{
		this.skillmanager = SkillManager.Instance;
		this.skillmanager.OnSkillAvailabilityChanged += this.Skillmanager_OnSkillAvailabilityChanged;
		this.DeactivateButton();
	}

	private void Start()
	{
		this.Skillmanager_OnSkillAvailabilityChanged(this.skillmanager.TierSkill, this.skillmanager.TierSkill.IsAvailableForLevelUp);
	}

	private void Skillmanager_OnSkillAvailabilityChanged(Skill skill, bool avaliability)
	{
		SkillManager instance = SkillManager.Instance;
		if (skill.IsActiveSkill || instance.OtherSkills.Contains(skill) || skill == instance.QuestSkill || skill == instance.DeepWaterSkill || skill == instance.PrestigeSkill || skill == SkillManager.Instance.CollectStarsSkill || skill.GetExtraInfo().PurchaseWith == ResourceType.Gems || skill.GetExtraInfo().PurchaseWith == ResourceType.SkillPoints || skill.GetExtraInfo().PurchaseWith == ResourceType.CrownExp)
		{
			return;
		}
		if (avaliability)
		{
			if (!this.upgradableSkillsList.Contains(skill))
			{
				this.upgradableSkillsList.Add(skill);
			}
		}
		else
		{
			this.skillThatJustGotRemoved = skill;
			this.upgradableSkillsList.Remove(skill);
			skill.OnSkillLevelUp -= this.Skill_OnSkillLevelUp;
		}
		this.SetNumberOfSkillsToLevelUp();
		if (this.upgradableSkillsList.Count > 0)
		{
			this.SetUpgradableSkill();
			if (!this.isActivated)
			{
				this.ActivateButton();
			}
		}
		else
		{
			this.DeactivateButton();
		}
	}

	private void Skill_OnSkillLevelUp(Skill skill, LevelChange arg2)
	{
		bool flag = skill == this.skillThatJustGotRemoved;
		bool flag2 = this.upgradableSkillsList.Count > 0 && this.upgradableSkillsList[0] != this.skillThatJustGotRemoved;
		if (flag && flag2)
		{
			return;
		}
		this.levelLabel.SetVariableText(new string[]
		{
			skill.CurrentLevel.ToString()
		});
	}

	private void SetUpgradableSkill()
	{
		this.upgradableSkillsList[0].OnSkillLevelUp += this.Skill_OnSkillLevelUp;
		this.skillPartImage.sprite = this.upgradableSkillsList[0].GetExtraInfo().Icon;
		this.levelLabel.SetVariableText(new string[]
		{
			this.upgradableSkillsList[0].CurrentLevel.ToString()
		});
	}

	private void ActivateButton()
	{
		this.animator.ResetTrigger("Disable");
		this.animator.SetTrigger("Activate");
		this.isActivated = true;
	}

	private void DeactivateButton()
	{
		this.skillPartImage.sprite = null;
		this.animator.ResetTrigger("Activate");
		this.animator.SetTrigger("Disable");
		this.isActivated = false;
	}

	public void OnMenuPartPressed()
	{
		base.transform.DOKill(true);
		base.transform.localScale = Vector3.one * 0.8f;
		base.transform.DOScale(1f, 0.6f).SetEase(Ease.OutElastic);
	}

	public void OnSkillPartPressed()
	{
		if (this.upgradableSkillsList.Count > 0)
		{
			this.SpawnUpgradeInfoFeedback();
			this.upgradableSkillsList[0].TryLevelUp();
		}
		if (this.isActivated)
		{
			this.animator.SetTrigger("SkillClick");
		}
	}

	private void SetNumberOfSkillsToLevelUp()
	{
		if (this.upgradableSkillsList.Count > 0)
		{
			this.skillsToLevelUpHolder.DOKill(true);
			this.skillsToLevelUpLabel.SetVariableText(new string[]
			{
				this.upgradableSkillsList.Count.ToString()
			});
			this.skillsToLevelUpHolder.localScale = Vector2.one;
			this.skillsToLevelUpHolder.DOPunchScale(new Vector2(0.5f, 0.5f), 0.5f, 10, 1f);
		}
		else
		{
			this.skillsToLevelUpHolder.DOKill(true);
			this.skillsToLevelUpHolder.DOScale(0f, 0.2f);
		}
	}

	private void SpawnUpgradeInfoFeedback()
	{
		TextMeshProUGUI upgradeInfoFeedbackInstance = UnityEngine.Object.Instantiate<TextMeshProUGUI>(this.upgradeInfoFeedbackLabel, base.transform.parent, false);
		upgradeInfoFeedbackInstance.transform.position = this.upgradeInfoFeedbackLabelPositioner.position;
		upgradeInfoFeedbackInstance.fontSize = 45f;
		SkillBehaviour skillBehaviour = this.upgradableSkillsList[0].SkillBehaviours[0];
		string text = FHelper.FindBracketAndReplace(skillBehaviour.Description, new string[]
		{
			string.Concat(new object[]
			{
				"<b>+",
				skillBehaviour.GetTotalValueAtLevel(1),
				skillBehaviour.PostFixCharacter,
				" </b>"
			})
		});
		upgradeInfoFeedbackInstance.text = text;
		upgradeInfoFeedbackInstance.transform.DOPunchScale(new Vector2(0.2f, 0.1f), 0.6f, 10, 1f);
		upgradeInfoFeedbackInstance.transform.DOMove(new Vector3(upgradeInfoFeedbackInstance.transform.position.x, upgradeInfoFeedbackInstance.transform.position.y + 0.5f, upgradeInfoFeedbackInstance.transform.position.z), 0.8f, false).OnComplete(delegate
		{
			upgradeInfoFeedbackInstance.transform.DOKill(false);
			upgradeInfoFeedbackInstance.DOKill(false);
			UnityEngine.Object.Destroy(upgradeInfoFeedbackInstance.transform.gameObject);
		});
		upgradeInfoFeedbackInstance.DOFade(0f, 0.8f).SetEase(Ease.InCirc);
	}

	private List<Skill> upgradableSkillsList = new List<Skill>();

	private SkillManager skillmanager;

	[SerializeField]
	private Image skillPartImage;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TextMeshProUGUI levelLabel;

	[SerializeField]
	private Transform skillsToLevelUpHolder;

	[SerializeField]
	private TextMeshProUGUI skillsToLevelUpLabel;

	[SerializeField]
	private TextMeshProUGUI upgradeInfoFeedbackLabel;

	[SerializeField]
	private Transform upgradeInfoFeedbackLabelPositioner;

	private Skill skillThatJustGotRemoved;

	private bool isActivated;
}
