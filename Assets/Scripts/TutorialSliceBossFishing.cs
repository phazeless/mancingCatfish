using System;
using UnityEngine;

public class TutorialSliceBossFishing : TutorialSliceBase
{
	protected override void Setup()
	{
		base.Setup();
		CharacterConversationHandler.Instance.OnConversationCompleted += this.Instance_OnConversationCompleted;
		BossFishSpawner.Instance.onBossSpawned += this.Instance_BossSpawned;
	}

	private void Instance_BossSpawned()
	{
		this.RunAfterDelay(12f, delegate()
		{
			TutorialManager.Instance.SetGraphicRaycaster(true);
			if (!CharacterConversationHandler.Instance.isInConversation)
			{
				CharacterConversationHandler.Instance.TutorialBossFishing();
			}
			else
			{
				UnityEngine.Debug.LogWarning("From TutorialSliceBossFishing: Conversation is already active from other script");
				base.Exit(true);
			}
		});
	}

	private void Instance_OnConversationCompleted(int conversationID)
	{
		if (conversationID == 3)
		{
			base.Exit(true);
		}
	}

	protected override void Exited()
	{
		base.Exited();
		CharacterConversationHandler.Instance.OnConversationCompleted -= this.Instance_OnConversationCompleted;
		BossFishSpawner.Instance.onBossSpawned -= this.Instance_BossSpawned;
	}
}
