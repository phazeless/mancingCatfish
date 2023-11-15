using System;
using DG.Tweening;
using UnityEngine;

public class TutorialSliceBG : TutorialSliceBase
{
	private void Start()
	{
	}

	private void N1QuestComplete_OnQuestClaimed(Quest obj)
	{
		this.bgInstance = UnityEngine.Object.Instantiate<BGMovementHandler>(this.bGprefab);
		this.bgInstance.transform.position = new Vector3(-2.6f, -20f, 0f);
		this.bgInstance.ActivateEffects();
		this.bgInstance.transform.DOMoveY(0.7f, 3f, false).SetEase(Ease.OutCirc);
		this.n1QuestComplete.OnQuestClaimed -= this.N1QuestComplete_OnQuestClaimed;
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		TutorialManager.Instance.SetGraphicRaycaster(true);
		CameraMovement.Instance.BgIntroductionZoomStart();
		this.RunAfterDelay(1.2f, delegate()
		{
			if (!CharacterConversationHandler.Instance.isInConversation)
			{
				CharacterConversationHandler.Instance.TutorialBadGuyEnter();
			}
			else
			{
				UnityEngine.Debug.LogWarning("From TutorialSliceBG: Conversation is already active from other script");
				base.Exit(true);
			}
		});
	}

	protected override void Setup()
	{
		base.Setup();
		this.n1QuestComplete.OnQuestClaimed += this.N1QuestComplete_OnQuestClaimed;
		CharacterConversationHandler.Instance.OnConversationCompleted += this.Instance_OnConversationCompleted;
	}

	private void Instance_OnConversationCompleted(int conversationID)
	{
		if (conversationID == 1)
		{
			this.ConversationFinished();
			CameraMovement.Instance.BgIntroductionZoomEnd();
			CharacterConversationHandler.Instance.TutorialBadGuyExit();
		}
		else if (conversationID == 2)
		{
			base.Exit(true);
		}
	}

	private void ConversationFinished()
	{
		this.bgInstance.ActivateEffects();
		this.bgInstance.transform.DOMoveY(20f, 3f, false).SetEase(Ease.InCirc).OnComplete(delegate
		{
			UnityEngine.Object.Destroy(this.bgInstance.gameObject);
		});
	}

	protected override void Exited()
	{
		base.Exited();
		CharacterConversationHandler.Instance.OnConversationCompleted -= this.Instance_OnConversationCompleted;
		this.n1QuestComplete.OnQuestClaimed -= this.N1QuestComplete_OnQuestClaimed;
	}

	[SerializeField]
	private Quest n1QuestComplete;

	[SerializeField]
	private BGMovementHandler bGprefab;

	private BGMovementHandler bgInstance;
}
