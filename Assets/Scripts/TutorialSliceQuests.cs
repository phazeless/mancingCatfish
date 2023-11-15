using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSliceQuests : TutorialSliceBase
{
	private void DelayedEnter()
	{
		this.RunAfterDelay(0.5f, delegate()
		{
			this.hasdelayed = true;
		});
		if (InGameNotificationManager.Instance != null)
		{
			List<UIInGameNotificationItem> uiinGameNotificationItem = InGameNotificationManager.Instance.GetUIInGameNotificationItem(InGameNotification.IGN.Quest);
			if (uiinGameNotificationItem.Count > 0)
			{
				base.MaskCircle.transform.position = uiinGameNotificationItem[0].transform.position;
			}
		}
		this.Enter();
	}

	public void ClickNext()
	{
		if (this.hasdelayed)
		{
			base.Exit(true);
			if (InGameNotificationManager.Instance != null)
			{
				InGameNotificationManager.Instance.OpenFirstOccurrenceOfIGN<IGNQuest>();
			}
		}
	}

	protected override void Setup()
	{
		base.Setup();
		if (this.questToListenTo == QuestManager.Instance.CurrentQuest)
		{
			this.questToListenTo.OnQuestCompleted += this.QuestToListenTo_OnQuestCompleted;
		}
		else
		{
			base.Exit(true);
		}
	}

	private void QuestToListenTo_OnQuestCompleted(Quest quest)
	{
		bool flag = TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialSliceCompleted(this.tutorialNeededToContinue.Id);
		bool flag2 = QuestManager.Instance != null && this.questToListenTo == QuestManager.Instance.CurrentQuest;
		bool flag3 = ScreenManager.Instance != null && ScreenManager.Instance.CurrentScreen == ScreenManager.Screen.Main;
		List<UIInGameNotificationItem> uiinGameNotificationItem = InGameNotificationManager.Instance.GetUIInGameNotificationItem(InGameNotification.IGN.Quest);
		bool flag4 = uiinGameNotificationItem != null && uiinGameNotificationItem.Count > 0 && uiinGameNotificationItem[0].Dialog.IsOpen;
		if (flag2 && this.questToListenTo.IsCompleted && flag3 && flag)
		{
			TutorialManager.Instance.SetGraphicRaycaster(true);
			if (!flag4)
			{
				this.questToListenTo.OnQuestCompleted -= this.QuestToListenTo_OnQuestCompleted;
				this.DelayedEnter();
			}
			else
			{
				base.Exit(true);
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.questToListenTo.OnQuestCompleted -= this.QuestToListenTo_OnQuestCompleted;
	}

	private bool hasdelayed;

	[SerializeField]
	private Quest questToListenTo;

	[SerializeField]
	private TutorialSliceBase tutorialNeededToContinue;
}
