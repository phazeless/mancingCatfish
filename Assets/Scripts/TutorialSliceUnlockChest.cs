using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSliceUnlockChest : TutorialSliceBase
{
	private void Start()
	{
		CharacterConversationHandler.Instance.OnConversationCompleted += this.Instance_OnConversationCompleted;
		OpenChestVisualManager.Instance.OnChestOpeningFinished += this.On_OnChestOpeningFinished;
		ChestManager.Instance.OnChestOpened += this.Instance_OnChestOpened;
		DailyGiftManager.Instance.OnDailyCatchCollectedAndShown += this.Instance_OnDailyCatchCollectedAndShown;
	}

	private void Instance_OnDailyCatchCollectedAndShown(DailyGiftContent content)
	{
		if ((content.Item != null || content.Items.Count > 0) && !this.hasStarted && !this.isOpeningChest)
		{
			this.hasStarted = true;
			this.EnterItemTutorial();
		}
	}

	private void Instance_OnConversationCompleted(int conversationID)
	{
		if (conversationID == 5)
		{
			Transform transform = DialogInteractionHandler.Instance.transform.Find("UpgradeDialogHolder");
			if (transform != null)
			{
				transform.GetComponent<TabSwapBehaviour>().SetTab(2);
				transform.GetComponent<UpgradeDialogTween>().Open();
			}
			else
			{
				UnityEngine.Debug.Log("UpgradeDialogHolder not found by TutorialSliceUnlockChest");
			}
			base.Exit(true);
		}
	}

	private void Instance_OnChestOpened(ItemChest arg1, List<Item> arg2)
	{
		this.isOpeningChest = true;
	}

	private void On_OnChestOpeningFinished()
	{
		this.isOpeningChest = false;
		this.EnterItemTutorial();
	}

	private void EnterItemTutorial()
	{
		TutorialManager.Instance.SetGraphicRaycaster(true);
		if (!CharacterConversationHandler.Instance.isInConversation)
		{
			CharacterConversationHandler.Instance.TutorialItemBoxtOpened();
		}
		else
		{
			UnityEngine.Debug.LogWarning("From TutorialSliceUnlockChest: Conversation is already active from other script");
			base.Exit(true);
		}
		this.Enter();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		CharacterConversationHandler.Instance.OnConversationCompleted -= this.Instance_OnConversationCompleted;
		OpenChestVisualManager.Instance.OnChestOpeningFinished -= this.On_OnChestOpeningFinished;
		ChestManager.Instance.OnChestOpened -= this.Instance_OnChestOpened;
		DailyGiftManager.Instance.OnDailyCatchCollectedAndShown -= this.Instance_OnDailyCatchCollectedAndShown;
	}

	private bool hasdelayed;

	private bool isOpeningChest;

	private bool hasStarted;
}
