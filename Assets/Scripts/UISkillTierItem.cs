using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillTierItem : UIListItem<Skill>
{
	protected override void Awake()
	{
		base.Awake();
		this.prestigeSkill = SkillManager.Instance.PrestigeSkill;
		this.originalBgColor = this.background.color;
	}

	private void OnSkillAvailableForLevelUpStatusChanged(Skill skill, bool isAvailableForLevelUp)
	{
		this.OnUpdateUI(skill);
	}

	private void OnSkillLevelUp(Skill skill, LevelChange change)
	{
		this.OnUpdateUI(skill);
	}

	public void OnLevelUpClick()
	{
		if (this.skill.TryLevelUp())
		{
			AudioManager.Instance.MenuClick();
		}
	}

	public override void OnUpdateUI(Skill updatedContent)
	{
		if (updatedContent != null)
		{
			this.skill = updatedContent;
			this.SetupBackground(this.skill);
			this.SetupIcon(this.skill);
			this.SetupTitleText(this.skill);
			this.SetupAttributeValues(this.skill);
			this.SetupSkillLevelText(this.skill);
			this.SetupSkillProgressMeter(this.skill);
			this.SetupSkillCostButton(this.skill);
			if (base.gameObject.activeInHierarchy)
			{
				this.TweenKiller();
				this.TweenEffect();
			}
		}
	}

	private bool HasEnoughPrestigeOrDWLvl(Skill skill)
	{
		bool flag = SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong >= (long)skill.GetExtraInfo().RequiredFishingExperience;
		bool flag2 = SkillManager.Instance.DeepWaterSkill.HighestLevel >= skill.GetExtraInfo().RequiredDeepWaterLevel;
		return this.HasUnlocked(skill) || (flag && flag2) || skill.GetExtraInfo().IsOnlyAvailableThroughPurchase;
	}

	private bool HasUnlocked(Skill skill)
	{
		return skill.IsTierSkill || skill.CurrentLevel > 0;
	}

	private void SetupBackground(Skill updatedContent)
	{
		if (this.HasEnoughPrestigeOrDWLvl(updatedContent))
		{
			this.background.color = this.originalBgColor;
		}
		else
		{
			this.background.color = new Color(0.875f, 0.851f, 0.875f, 1f);
		}
	}

	private void TweenEffect()
	{
		this.holder.DOPunchScale(new UnityEngine.Vector3(0.1f, 0.1f, 0.1f), 0.5f, 10, 1f);
	}

	private void TweenKiller()
	{
		if (this.holder != null)
		{
			this.holder.DOKill(true);
		}
	}

	private void SetupSkillCostButton(Skill updatedContent)
	{
		if (updatedContent.GetExtraInfo().HasLevelUpDependency)
		{
			this.upgradeButton.interactable = false;
			this.cost.SetText("N/A");
			return;
		}
		if (this.HasEnoughPrestigeOrDWLvl(updatedContent))
		{
			this.upgradeButton.gameObject.SetActive(true);
			string text = string.Empty;
			if (updatedContent.GetExtraInfo().PurchaseWith == ResourceType.Cash)
			{
				text = CashFormatter.SimpleToCashRepresentation(updatedContent.CostForNextLevelUp, 1, false, false);
			}
			else
			{
				text = updatedContent.CostForNextLevelUp.ToString();
			}
			if (this.skill.IsMaxLevel)
			{
				this.cost.SetText("MAX");
				this.upgradeButton.interactable = updatedContent.IsAvailableForLevelUp;
				if (this.lockImage != null)
				{
					this.lockImage.SetActive(false);
				}
			}
			else if (this.HasUnlocked(updatedContent))
			{
				if (updatedContent.GetExtraInfo().IsCrew)
				{
					this.upgradeButton.interactable = (updatedContent.IsAvailableForLevelUp && !TournamentManager.Instance.IsInsideTournament);
				}
				else
				{
					this.upgradeButton.interactable = updatedContent.IsAvailableForLevelUp;
				}
				this.cost.SetVariableText(new string[]
				{
					text
				});
				if (this.lockImage != null)
				{
					this.lockImage.SetActive(false);
				}
			}
			else
			{
				this.upgradeButton.interactable = false;
				this.cost.SetText(string.Empty);
				if (this.lockImage != null)
				{
					this.lockImage.SetActive(true);
				}
			}
		}
		else
		{
			this.upgradeButton.gameObject.SetActive(false);
		}
	}

	private void SetupSkillInfoButton(Skill updatedContent)
	{
		if (this.HasEnoughPrestigeOrDWLvl(updatedContent))
		{
			this.infoButton.gameObject.SetActive(true);
		}
		else
		{
			this.infoButton.gameObject.SetActive(false);
		}
	}

	private void SetupSkillProgressMeter(Skill updatedContent)
	{
		if (this.HasEnoughPrestigeOrDWLvl(updatedContent))
		{
			this.progressMeter.gameObject.SetActive(true);
			this.progressMeter.SetMax((float)updatedContent.MaxLevel);
			this.progressMeter.SetCurrent((float)updatedContent.CurrentLevel);
			if (this.HasUnlocked(updatedContent))
			{
				this.progressMeter.gameObject.SetActive(true);
			}
			else
			{
				this.progressMeter.gameObject.SetActive(false);
			}
		}
		else
		{
			this.progressMeter.gameObject.SetActive(false);
		}
	}

	private void SetupSkillLevelText(Skill updatedContent)
	{
		if (this.HasEnoughPrestigeOrDWLvl(updatedContent))
		{
			this.skillLevel.gameObject.SetActive(true);
			this.skillLevel.SetVariableText(new string[]
			{
				updatedContent.CurrentLevel.ToString()
			});
		}
		else
		{
			this.skillLevel.gameObject.SetActive(false);
		}
	}

	private void SetupAttributeValues(Skill updatedContent)
	{
		foreach (TextMeshProUGUI textMeshProUGUI in this.valueAndAttribute)
		{
			textMeshProUGUI.transform.parent.gameObject.SetActive(false);
		}
		if (this.HasEnoughPrestigeOrDWLvl(updatedContent))
		{
			for (int i = 0; i < updatedContent.SkillBehaviours.Count; i++)
			{
				SkillBehaviour skillBehaviour = updatedContent.SkillBehaviours[i];
				this.valueAndAttribute[i].transform.parent.gameObject.SetActive(true);
				float valueAtLevel = skillBehaviour.GetValueAtLevel(updatedContent.NextLevel);
				string text = (valueAtLevel <= 0f) ? string.Empty : "+";
				float totalValueAtLevel = skillBehaviour.GetTotalValueAtLevel(updatedContent.CurrentLevel);
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
				this.valueAndAttribute[i].SetVariableText(new string[]
				{
					string.Empty,
					text3,
					string.Concat(new object[]
					{
						" (",
						text2,
						skillBehaviour.GetTotalValueAtLevel(updatedContent.CurrentLevel),
						skillBehaviour.PostFixCharacter,
						")"
					})
				});
			}
		}
		else
		{
			this.valueAndAttribute[0].transform.parent.gameObject.SetActive(true);
			this.valueAndAttribute[0].SetText("Requires:");
			bool flag = updatedContent.GetExtraInfo().RequiredFishingExperience > 0;
			bool flag2 = updatedContent.GetExtraInfo().RequiredDeepWaterLevel > 0;
			if (flag)
			{
				this.valueAndAttribute[1].transform.parent.gameObject.SetActive(true);
				this.valueAndAttribute[1].SetText("<color=#EB5C0BFF>" + updatedContent.GetExtraInfo().RequiredFishingExperience + "</color> Fishing Experience to unlock.");
			}
			if (flag2)
			{
				this.valueAndAttribute[2].transform.parent.gameObject.SetActive(true);
				this.valueAndAttribute[2].SetText("Deep Water Level <color=#EB5C0BFF>" + updatedContent.GetExtraInfo().RequiredDeepWaterLevel + "</color> to unlock");
			}
		}
	}

	private void SetupTitleText(Skill updatedContent)
	{
		if (this.HasEnoughPrestigeOrDWLvl(updatedContent))
		{
			this.title.text = updatedContent.GetExtraInfo().TitleText;
		}
		else
		{
			this.title.text = "??????";
		}
	}

	private void SetupIcon(Skill updatedContent)
	{
		if (this.HasEnoughPrestigeOrDWLvl(updatedContent))
		{
			this.icon.sprite = updatedContent.GetExtraInfo().Icon;
			if (this.HasUnlocked(updatedContent))
			{
				this.icon.material = null;
				this.icon.color = Color.white;
			}
			else
			{
				this.icon.material = this.greyScaleMaterial;
			}
		}
	}

	private void Skill_OnSkillPriceChange(Skill skill, BigInteger newCost)
	{
		this.OnUpdateUI(skill);
	}

	private void RegisterListeners(Skill skill)
	{
		if (skill == null)
		{
			return;
		}
		skill.OnSkillLevelUp += this.OnSkillLevelUp;
		skill.OnSkillAvailabilityChanged += this.OnSkillAvailableForLevelUpStatusChanged;
		skill.OnSkillCostChange += this.Skill_OnSkillPriceChange;
	}

	private void UnregisterListeners(Skill skill)
	{
		if (skill == null)
		{
			return;
		}
		skill.OnSkillLevelUp -= this.OnSkillLevelUp;
		skill.OnSkillAvailabilityChanged -= this.OnSkillAvailableForLevelUpStatusChanged;
		skill.OnSkillCostChange -= this.Skill_OnSkillPriceChange;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.skill != null)
		{
			this.OnUpdateUI(this.skill);
		}
	}

	public override void OnShouldRegisterListeners()
	{
		this.RegisterListeners(this.skill);
	}

	public override void OnShouldUnregisterListeners()
	{
		this.UnregisterListeners(this.skill);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.TweenKiller();
	}

	private Skill skill;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private List<TextMeshProUGUI> valueAndAttribute = new List<TextMeshProUGUI>();

	[SerializeField]
	private TextMeshProUGUI skillLevel;

	[SerializeField]
	private UIMeter progressMeter;

	[SerializeField]
	private TextMeshProUGUI cost;

	[SerializeField]
	private Button upgradeButton;

	[SerializeField]
	private GameObject infoButton;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Material greyScaleMaterial;

	[SerializeField]
	private GameObject lockImage;

	[SerializeField]
	private Transform holder;

	[SerializeField]
	private Image[] progressDots;

	private Skill prestigeSkill;

	private Color originalBgColor = Color.white;
}
