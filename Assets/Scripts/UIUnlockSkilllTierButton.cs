using System;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUnlockSkilllTierButton : UIListItem<Skill>
{
	public override void OnUpdateUI(Skill updatedContent)
	{
		if (updatedContent == null)
		{
			return;
		}
		this.skill = updatedContent;
		this.reachedMaxTier = (this.skill.CurrentLevel + 1 > this.skill.MaxLevel);
		if (!this.reachedMaxTier)
		{
			this.title.SetVariableText(new string[]
			{
				(this.skill.CurrentLevel + 1).ToString()
			});
			this.cost.SetVariableText(new string[]
			{
				CashFormatter.SimpleToCashRepresentation(this.skill.CostForNextLevelUp, 3, false, false)
			});
		}
		else
		{
			this.title.SetText("Fishing Master");
			this.description.SetText("You've got the best boat available.");
			this.cost.SetText("More coming soon!");
		}
		this.upgradeButton.interactable = this.skill.IsAvailableForLevelUp;
	}

	private void Skill_OnSkillPriceChange(Skill skill, BigInteger newCost)
	{
		this.OnUpdateUI(skill);
	}

	private void OnSkillAvailableForLevelUpStatusChanged(Skill skill, bool isAvailableForLevelUp)
	{
		this.upgradeButton.interactable = skill.IsAvailableForLevelUp;
	}

	public void OnUnlockTierClick()
	{
		if (this.reachedMaxTier)
		{
			return;
		}
		this.skill.TryLevelUp();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		this.ShineTween();
		this.OnUpdateUI(this.skill);
	}

	public override void OnShouldRegisterListeners()
	{
		if (this.skill == null)
		{
			return;
		}
		this.skill.OnSkillAvailabilityChanged += this.OnSkillAvailableForLevelUpStatusChanged;
		this.skill.OnSkillCostChange += this.Skill_OnSkillPriceChange;
	}

	public override void OnShouldUnregisterListeners()
	{
		if (this.skill == null)
		{
			return;
		}
		this.skill.OnSkillAvailabilityChanged -= this.OnSkillAvailableForLevelUpStatusChanged;
		this.skill.OnSkillCostChange -= this.Skill_OnSkillPriceChange;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		this.TweenKiller();
	}

	private void ShineTween()
	{
		this.TweenKiller();
		this.shineEffect.DOAnchorPosX(657f, 0.5f, false).SetEase(Ease.Linear).SetDelay(2f).OnComplete(delegate
		{
			this.ShineTween();
		});
	}

	private void TweenKiller()
	{
		this.shineEffect.DOKill(false);
		this.shineEffect.anchoredPosition = new UnityEngine.Vector2(-656.7f, this.shineEffect.anchoredPosition.y);
	}

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private TextMeshProUGUI description;

	[SerializeField]
	private TextMeshProUGUI cost;

	[SerializeField]
	private Button upgradeButton;

	[SerializeField]
	private RectTransform shineEffect;

	private Skill skill;

	private bool reachedMaxTier;
}
