using System;
using DG.Tweening;
using UnityEngine;

public class TutorialSliceBGItemGift : TutorialSliceBase
{
	private void Start()
	{
	}

	private void DwProgressskill_OnSkillLevelUp(Skill skill, LevelChange arg2)
	{
		if (skill.CurrentLevel != this.atWhatLevelToTrigger)
		{
			return;
		}
		this.bgInstance = UnityEngine.Object.Instantiate<BGMovementHandler>(this.bGprefab);
		this.bgInstance.transform.position = new Vector3(-2.6f, -20f, 0f);
		this.bgInstance.ActivateEffects();
		this.bgInstance.transform.DOMoveY(0.7f, 3f, false).SetEase(Ease.OutCirc);
		this.dwProgressskill.OnSkillLevelUp -= this.DwProgressskill_OnSkillLevelUp;
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tutorial);
		CameraMovement.Instance.BgIntroductionZoomStart();
		this.RunAfterDelay(1.2f, delegate()
		{
			TutorialManager.Instance.SetGraphicRaycaster(true);
			if (!CharacterConversationHandler.Instance.isInConversation)
			{
				CharacterConversationHandler.Instance.TutorialBadGuyEnterItemGift();
			}
			else
			{
				UnityEngine.Debug.LogWarning("From TutorialSliceBGItemGift: Conversation is already active from other script");
				base.Exit(true);
			}
		});
	}

	protected override void Setup()
	{
		base.Setup();
		this.dwProgressskill.OnSkillLevelUp += this.DwProgressskill_OnSkillLevelUp;
		CharacterConversationHandler.Instance.OnConversationCompleted += this.Instance_OnConversationCompleted;
	}

	private void Instance_OnConversationCompleted(int conversationID)
	{
		if (conversationID == 6)
		{
			this.ConversationFinished();
			CameraMovement.Instance.BgIntroductionZoomEnd();
			ChestManager.Instance.RecieveChest();
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
		this.dwProgressskill.OnSkillLevelUp -= this.DwProgressskill_OnSkillLevelUp;
		CharacterConversationHandler.Instance.OnConversationCompleted -= this.Instance_OnConversationCompleted;
	}

	[SerializeField]
	private Skill dwProgressskill;

	[SerializeField]
	private int atWhatLevelToTrigger = 2;

	[SerializeField]
	private BGMovementHandler bGprefab;

	private BGMovementHandler bgInstance;
}
