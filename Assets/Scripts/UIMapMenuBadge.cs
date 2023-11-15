using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIMapMenuBadge : MonoBehaviour
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

	private void Start()
	{
		this.deepWaterSkill.OnSkillAvailabilityChanged += this.Skill_OnSkillAvailabilityChanged;
		ScreenManager.Instance.OnScreenTransitionStarted += this.Instance_OnScreenTransitionStarted;
		this.SomethingNewHasHappened = this.deepWaterSkill.IsAvailableForLevelUp;
		this.UpdateUI();
	}

	private void Instance_OnScreenTransitionStarted(ScreenManager.Screen toScreen, ScreenManager.Screen fromScreen)
	{
		if (toScreen == ScreenManager.Screen.Map)
		{
			this.SomethingNewHasHappened = false;
		}
		else if (toScreen == ScreenManager.Screen.Main && fromScreen == ScreenManager.Screen.Map)
		{
			this.SomethingNewHasHappened = false;
		}
	}

	private void Skill_OnSkillAvailabilityChanged(Skill skill, bool available)
	{
		this.SomethingNewHasHappened = true;
	}

	private void UpdateUI()
	{
		if (this.somethingNewHasHappened)
		{
			this.badgeNumbericon.SetActive(true);
			this.TweenKiller();
			this.badgeNumbericon.transform.DOPunchScale(Vector2.one * 1.5f, 0.5f, 10, 1f);
			this.badgeNumbericon.transform.parent.DOPunchScale(Vector2.one * 0.05f, 0.5f, 5, 0.5f);
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
	private Skill deepWaterSkill;

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
