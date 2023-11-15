using System;
using UnityEngine;

public class TutorialSliceRoyalIntro : TutorialSliceBase
{
	private void Start()
	{
		CharacterConversationHandler.Instance.OnConversationCompleted += this.Instance_OnConversationCompleted;
		TutorialSliceRoyalIntro.Instance = this;
	}

	public void OpenedCrownDialog()
	{
		if (!this.hasStarted)
		{
			this.EnterTutorial();
		}
		this.hasStarted = true;
	}

	private void Instance_OnConversationCompleted(int conversationID)
	{
		if (conversationID == 7 || conversationID == 8)
		{
			if (!this.isAlreadyLeveledUp)
			{
				int amount = (int)SkillTreeManager.Instance.CrownLevelSkill.CostForNextLevelUp - (int)ResourceManager.Instance.GetResourceAmount(ResourceType.CrownExp);
				string text = ResourceChangeReason.TutorialCrownExpIntro.ToString();
				ResourceChangeData gemChangeData = new ResourceChangeData(text, text, amount, ResourceType.CrownExp, ResourceChangeType.Earn, ResourceChangeReason.TutorialCrownExpIntro);
				ResourceManager.Instance.GiveCrownExp(amount, gemChangeData);
			}
			this.Enter();
			this.RunAfterDelay(0.3f, delegate()
			{
				this.hasWaited = true;
			});
		}
	}

	private void Update()
	{
		if (this.hasWaited && Input.GetMouseButtonDown(0) && !this.isExitingNow)
		{
			this.isExitingNow = true;
			base.Exit(true);
		}
	}

	private void EnterTutorial()
	{
		TutorialManager.Instance.SetGraphicRaycaster(true);
		if (CharacterConversationHandler.Instance.isInConversation)
		{
			UnityEngine.Debug.LogWarning("From TutorialSliceRoyalIntro: Conversation is already active from other script");
			base.Exit(true);
		}
		else
		{
			if (SkillTreeManager.Instance.CrownLevelSkill.CurrentLevel == 0)
			{
				this.isAlreadyLeveledUp = false;
			}
			else
			{
				this.isAlreadyLeveledUp = true;
			}
			if (this.isAlreadyLeveledUp)
			{
				CharacterConversationHandler.Instance.TutorialRoyalGuyIntro();
			}
			else
			{
				CharacterConversationHandler.Instance.TutorialRoyalGuyIntroGive();
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		CharacterConversationHandler.Instance.OnConversationCompleted -= this.Instance_OnConversationCompleted;
	}

	private bool hasStarted;

	public static TutorialSliceRoyalIntro Instance;

	private bool isAlreadyLeveledUp;

	private bool hasWaited;

	private bool isExitingNow;
}
