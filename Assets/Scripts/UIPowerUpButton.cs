using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPowerUpButton : MonoBehaviour
{
	private void Awake()
	{
		this.powerUpSkill.OnSkillLevelUp += this.PowerUpSkill_OnSkillLevelUp;
		base.gameObject.SetActive(false);
	}

	private void Start()
	{
		this.uiActivatePowerUpButton.onClick.AddListener(new UnityAction(this.OnClick));
		this.powerUpSkill.OnSkillActivation += this.PowerUpSkill_OnSkillActivation;
		if (this.powerUpSkill.IsOnCooldown)
		{
			this.uiCooldown.SetVariableText(new string[]
			{
				this.powerUpSkill.GetTotalSecondsLeftOnCooldown().ToString()
			});
			this.uiActivatePowerUpButton.interactable = false;
		}
		else
		{
			this.whiteBorder.color = Color.white;
			this.uiCooldown.SetVariableText(new string[]
			{
				string.Empty
			});
			this.TweenScript.SetOffCooldownAnimation();
		}
		this.startingFillHeight = this.cooldownFillImage.sizeDelta.y;
	}

	private void PowerUpSkill_OnSkillLevelUp(Skill skill, LevelChange arg2)
	{
		if (skill.CurrentLevel > 0)
		{
			base.gameObject.SetActive(true);
		}
	}

	private void OnClick()
	{
		this.powerUpSkill.Activate();
	}

	private void PowerUpSkill_OnSkillActivation(Skill skill)
	{
		this.uiActivatePowerUpButton.interactable = false;
		this.skillCooldown = (float)Mathf.CeilToInt(this.powerUpSkill.GetTotalSecondsLeftOnCooldown());
		this.whiteBorder.color = new Color(1f, 1f, 1f, 0.3f);
		this.activationEffect.gameObject.SetActive(true);
		this.activationEffect.transform.localScale = Vector2.one;
		this.activationEffect.color = Color.white;
		this.activationEffect.transform.DOScale(2f, 0.2f);
		this.activationEffect.DOFade(0f, 0.2f).SetEase(Ease.OutCirc).OnComplete(delegate
		{
			this.activationEffect.gameObject.SetActive(false);
		});
		this.TweenScript.SetOnCooldownAnimation();
	}

	private void Update()
	{
		if (this.powerUpSkill.IsOnCooldown)
		{
			int num = Mathf.CeilToInt(this.powerUpSkill.GetTotalSecondsLeftOnCooldown());
			if (this.lastSec != num)
			{
				this.uiCooldown.SetVariableText(new string[]
				{
					num.ToString()
				});
				this.lastSec = num;
			}
			this.cooldownFillImage.sizeDelta = new Vector2(this.cooldownFillImage.sizeDelta.x, (this.skillCooldown - this.powerUpSkill.GetTotalSecondsLeftOnCooldown()) / this.skillCooldown * this.startingFillHeight);
		}
		else if (!this.uiActivatePowerUpButton.interactable)
		{
			this.uiActivatePowerUpButton.interactable = true;
			this.uiCooldown.SetVariableText(new string[]
			{
				string.Empty
			});
			this.whiteBorder.color = Color.white;
			this.cooldownFillImage.sizeDelta = new Vector2(this.cooldownFillImage.sizeDelta.x, this.startingFillHeight);
			this.TweenScript.SetOffCooldownAnimation();
			base.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.5f, 10, 1f);
		}
	}

	private void UpdateCooldownText()
	{
	}

	private void OnEnable()
	{
	}

	[SerializeField]
	private Skill powerUpSkill;

	[SerializeField]
	private Button uiActivatePowerUpButton;

	[SerializeField]
	private TextMeshProUGUI uiCooldown;

	[SerializeField]
	private RectTransform cooldownFillImage;

	[SerializeField]
	private Image whiteBorder;

	[SerializeField]
	private Image activationEffect;

	[SerializeField]
	private PowerupAnimationBaseClass TweenScript;

	private int lastSec;

	private float startingFillHeight;

	private float skillCooldown;
}
