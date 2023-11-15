using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyGiftHandler : MonoBehaviour
{
	private void Start()
	{
		this.dailyGiftSkill.OnSkillActivation += this.DailyGiftSkill_OnSkillActivation;
		this.dailyGiftSkill.OnSkillCooldownZero += this.DailyGiftSkill_OnSkillCooldownZero;
		this.UpdateUI();
	}

	private void DailyGiftSkill_OnSkillCooldownZero(Skill obj)
	{
		this.UpdateUI();
	}

	private void DailyGiftSkill_OnSkillActivation(Skill skill)
	{
		this.dailyGiftSkill.SetCurrentLevel(UnityEngine.Random.Range(3, 10), LevelChange.LevelUpFree);
		GameAnalyticsEvents.ResourceGemsIncreased(AnalyticsEvents.REType.DailyGift, AnalyticsEvents.RECategory.Gift, this.dailyGiftSkill.CurrentLevel);
		ResourceChangeData gemChangeData = new ResourceChangeData();
		GemGainVisual.Instance.GainGems(this.dailyGiftSkill.CurrentLevel, this.chestTransform.position, gemChangeData);
		this.UpdateUI();
	}

	private void UpdateUI()
	{
		this.collectButton.interactable = !this.dailyGiftSkill.IsOnCooldown;
		this.collectForFreeLabel.gameObject.SetActive(!this.dailyGiftSkill.IsOnCooldown);
		this.collectedAmountLabel.gameObject.SetActive(this.dailyGiftSkill.IsOnCooldown);
		this.expirationLabel.gameObject.SetActive(this.dailyGiftSkill.IsOnCooldown);
		this.collectedAmountLabel.SetVariableText(new string[]
		{
			this.dailyGiftSkill.CurrentLevel.ToString()
		});
		if (this.dailyGiftSkill.IsOnCooldown)
		{
			this.buttonOutline.color = this.inActiveOutlineColor;
			this.buttonBody.color = this.inActiveBodyColor;
			this.gemsInChest.SetActive(false);
		}
		else
		{
			this.buttonOutline.color = this.activeOutlineColor;
			this.buttonBody.color = this.activeBodyColor;
			this.gemsInChest.SetActive(true);
		}
		this.chestTween.isActive = !this.dailyGiftSkill.IsOnCooldown;
		if (ScreenManager.Instance.CurrentScreen == ScreenManager.Screen.Shop)
		{
			this.chestTween.UpdateTweenState();
		}
	}

	private void Update()
	{
		if (ScreenManager.Instance.CurrentScreen == ScreenManager.Screen.Shop && this.expirationLabel.gameObject.activeSelf)
		{
			this.expirationLabel.SetVariableText(new string[]
			{
				FHelper.FromSecondsToHoursMinutesSecondsFormat(this.dailyGiftSkill.GetTotalSecondsLeftOnCooldown())
			});
		}
	}

	[SerializeField]
	private Skill dailyGiftSkill;

	[SerializeField]
	private Button collectButton;

	[SerializeField]
	private TextMeshProUGUI collectForFreeLabel;

	[SerializeField]
	private TextMeshProUGUI collectedAmountLabel;

	[SerializeField]
	private TextMeshProUGUI expirationLabel;

	[SerializeField]
	private Transform chestTransform;

	[SerializeField]
	private GameObject gemsInChest;

	[SerializeField]
	private Image buttonOutline;

	[SerializeField]
	private Image buttonBody;

	[SerializeField]
	private DailyGiftChestTween chestTween;

	private Color activeOutlineColor = new Color(0.941f, 0.757f, 0.114f);

	private Color activeBodyColor = new Color(0.988f, 0.855f, 0.231f);

	private Color inActiveOutlineColor = new Color(0.7f, 0.7f, 0.7f);

	private Color inActiveBodyColor = new Color(0.8f, 0.8f, 0.8f);
}
