using System;

public class TutorialSliceUnlockCrew : TutorialSliceBase
{
	private void Start()
	{
		SkillManager.Instance.OnSkillLevelChanged += this.Instance_OnSkillLevelChanged;
		InGameNotificationManager.Instance.OnInGameNotificationOpened += this.Instance_OnInGameNotificationOpened;
	}

	private void Instance_OnSkillLevelChanged(Skill arg1, LevelChange arg2)
	{
		if (arg1.GetExtraInfo().IsCrew && arg1.CurrentLevel > 0)
		{
			if (!this.hasOpenedIgn)
			{
				TutorialManager.Instance.SetGraphicRaycaster(true);
			}
			base.Exit(true);
		}
	}

	private void Instance_OnInGameNotificationOpened(InGameNotification obj)
	{
		if (obj is IGNNewCrew)
		{
			IGNNewCrew ignnewCrew = obj as IGNNewCrew;
			if (ignnewCrew.IsNewCrew)
			{
				int num = (int)ResourceManager.Instance.GetResourceAmount(ResourceType.Gems);
				int num2 = (int)SkillManager.Instance.UnlockCrewMemberSkill.CostForNextLevelUp;
				int num3 = num2 - num;
				ResourceChangeData gemChangeData = new ResourceChangeData("contentId_crewTutorial", "Tutorial First Crew", num3, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.TutorialFirstCrew);
				ResourceManager.Instance.GiveGems(num3, gemChangeData);
				TutorialManager.Instance.SetGraphicRaycaster(true);
				this.hasOpenedIgn = true;
				this.Enter();
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		SkillManager.Instance.OnSkillLevelChanged -= this.Instance_OnSkillLevelChanged;
		InGameNotificationManager.Instance.OnInGameNotificationOpened -= this.Instance_OnInGameNotificationOpened;
	}

	private bool hasdelayed;

	private const string contentId_crewTutorial = "contentId_crewTutorial";

	private const string contentName_crewTutorial = "Tutorial First Crew";

	private bool hasOpenedIgn;
}
