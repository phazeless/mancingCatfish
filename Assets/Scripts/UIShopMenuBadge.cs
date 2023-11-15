using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIShopMenuBadge : MonoBehaviour
{
	private bool SomethingNewHasHappened
	{
		get
		{
			return this.somethingNewHasHappened;
		}
		set
		{
			this.somethingNewHasHappened = value;
			this.UpdateUI();
		}
	}

	private void Awake()
	{
		SpecialOfferManager.Instance.OnSpecialOfferAvailable += this.Instance_OnSpecialOfferAvailable;
	}

	private void Start()
	{
		this.freeSpinSkill.OnSkillLevelUp += this.FreeSpinSkill_OnSkillLevelUp;
		ScreenManager.Instance.OnScreenTransitionStarted += this.Instance_OnScreenTransitionStarted;
		this.SomethingNewHasHappened = SpecialOfferManager.Instance.IsAnySpecialOfferActive;
		this.UpdateUI();
	}

	private void Instance_OnSpecialOfferAvailable(SpecialOffer specialOffer, bool shouldActivate)
	{
		if (shouldActivate)
		{
			this.SomethingNewHasHappened = true;
		}
	}

	private void FreeSpinSkill_OnSkillLevelUp(Skill skill, LevelChange levelUp)
	{
		if (skill.CurrentLevel > 0)
		{
			this.SomethingNewHasHappened = true;
		}
	}

	private void Instance_OnScreenTransitionStarted(ScreenManager.Screen toScreen, ScreenManager.Screen fromScreen)
	{
		if (toScreen == ScreenManager.Screen.Shop)
		{
			this.SomethingNewHasHappened = false;
		}
		else if (toScreen == ScreenManager.Screen.Main && fromScreen == ScreenManager.Screen.Shop)
		{
			this.SomethingNewHasHappened = false;
		}
	}

	private void UpdateUI()
	{
		if (this.somethingNewHasHappened)
		{
			this.badgeNumbericon.SetActive(true);
			this.TweenKiller();
			this.badgeNumbericon.transform.DOPunchScale(Vector2.one * 1.5f, 0.5f, 10, 1f);
			this.badgeNumbericon.transform.parent.DOPunchScale(Vector2.one * 0.05f, 0.5f, 7, 0.7f);
			this.iconImage.color = this.activeColor;
			this.badgeNumbericon.transform.parent.GetComponent<Image>().color = Color.white;
		}
		else
		{
			this.TweenKiller();
			this.badgeNumbericon.SetActive(false);
			this.iconImage.color = this.inActiveDarkColor;
			this.badgeNumbericon.transform.parent.GetComponent<Image>().color = this.inActiveBrightColor;
		}
	}

	private void TweenKiller()
	{
		this.badgeNumbericon.transform.DOKill(true);
		this.badgeNumbericon.transform.parent.DOKill(true);
	}

	[SerializeField]
	private Skill freeSpinSkill;

	[SerializeField]
	private GameObject badgeNumbericon;

	[SerializeField]
	private Image iconImage;

	[SerializeField]
	private Color activeColor;

	private Color inActiveDarkColor = new Color(0f, 0f, 0f, 0.294f);

	private Color inActiveBrightColor = new Color(1f, 1f, 1f, 0.294f);

	private bool somethingNewHasHappened;
}
