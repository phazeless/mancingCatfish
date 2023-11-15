using System;
using System.Diagnostics;
using UnityEngine;

public class TutorialSliceCollectFishingExperience : TutorialSliceBase
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnCompleted;

	private void DelayedEnter()
	{
		CharacterConversationHandler.Instance.TutorialCollectFishingExperiance();
		TutorialManager.Instance.SetGraphicRaycaster(true);
	}

	protected override void Entered()
	{
	}

	protected override void Setup()
	{
		base.Setup();
		if (QuestManager.Instance != null)
		{
			QuestManager.Instance.OnQuestClaimed += this.Instance_OnQuestClaimed;
		}
		if (CharacterConversationHandler.Instance != null)
		{
			CharacterConversationHandler.Instance.OnConversationCompleted += this.Instance_OnConversationCompleted;
		}
	}

	private void Instance_OnConversationCompleted(int conversationID)
	{
		if (conversationID == 4)
		{
			ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Map);
			this.RunAfterDelay(0.5f, delegate()
			{
				this.Enter();
			});
		}
	}

	public void ClickedOnFishingExperienceButton()
	{
		FishingExperienceHolder.Instance.ToggleInfo();
		base.Exit(true);
	}

	private void Instance_OnQuestClaimed(Skill questSkill, Quest claimedQuest, Quest nextQuest)
	{
		if (nextQuest == this.prestiegeQuest)
		{
			int num = this.bonusPrestiegeGain;
			int num2 = this.bonusPrestiegeGain - (int)FishingExperienceHolder.Instance.TotalFishingExp;
			if (FishingExperienceHolder.Instance != null)
			{
				num = ((num2 <= 0) ? 0 : num2);
			}
			this.bonusFishingExperience.SetCurrentLevel(this.bonusFishingExperience.CurrentLevel + num, LevelChange.LevelUpFree);
			this.DelayedEnter();
		}
	}

	protected override void Exited()
	{
		if (this.OnCompleted != null)
		{
			this.OnCompleted();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		QuestManager.Instance.OnQuestClaimed -= this.Instance_OnQuestClaimed;
		CharacterConversationHandler.Instance.OnConversationCompleted -= this.Instance_OnConversationCompleted;
	}

	[SerializeField]
	private Skill bonusFishingExperience;

	[SerializeField]
	private Quest prestiegeQuest;

	[SerializeField]
	private int bonusPrestiegeGain = 100;
}
