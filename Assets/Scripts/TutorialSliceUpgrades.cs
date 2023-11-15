using System;
using System.Numerics;
using UnityEngine;

public class TutorialSliceUpgrades : TutorialSliceBase
{
	private void Start()
	{
		ResourceManager.Instance.OnResourceChanged += this.Instance_OnResourceChanged;
	}

	private void Instance_OnResourceChanged(ResourceType type, BigInteger addedAmount, BigInteger totalAmount)
	{
		if (totalAmount >= 15L && !this.isActivated && this.isWaitingToShow && ScreenManager.Instance.CurrentScreen == ScreenManager.Screen.Main)
		{
			TutorialManager.Instance.SetGraphicRaycaster(true);
			if (this.upgradedskill.CurrentLevel > 0)
			{
				base.Exit(true);
				return;
			}
			this.isActivated = true;
			this.Enter();
			this.RunAfterDelay(0.5f, delegate()
			{
				this.hasdelayed = true;
			});
		}
	}

	public void ClickNext()
	{
		if (this.hasdelayed)
		{
			base.Exit(true);
		}
	}

	protected override void Setup()
	{
		base.Setup();
		if (TutorialSliceSwiping.Instance != null)
		{
			TutorialSliceSwiping.Instance.OnCompleted += this.Instance_OnConversationCompleted;
		}
		else
		{
			this.isWaitingToShow = true;
		}
	}

	private void Instance_OnConversationCompleted()
	{
		this.isWaitingToShow = true;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ResourceManager.Instance.OnResourceChanged -= this.Instance_OnResourceChanged;
		if (TutorialSliceSwiping.Instance != null)
		{
			TutorialSliceSwiping.Instance.OnCompleted -= this.Instance_OnConversationCompleted;
		}
	}

	private bool hasdelayed;

	private bool isActivated;

	private bool isWaitingToShow;

	[SerializeField]
	private Skill upgradedskill;
}
