using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FullInspector;
using UnityEngine;

[fiInspectorOnly]
public class QuestManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Quest> OnQuestProgress;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Quest> OnQuestCompleted;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Skill, Quest, Quest> OnQuestClaimed;

	public static QuestManager Instance { get; private set; }

	[ShowInInspector]
	public Quest CurrentQuest
	{
		get
		{
			if (this.questSkill.CurrentLevel < this.quests.Count)
			{
				return this.quests[this.questSkill.CurrentLevel];
			}
			return null;
		}
	}

	public int QuestCount
	{
		get
		{
			return this.questSkill.CurrentLevel;
		}
	}

	public bool IsClaimed(Quest quest)
	{
		return this.questSkill.CurrentLevel > this.quests.IndexOf(quest);
	}

	private void Awake()
	{
		QuestManager.Instance = this;
	}

	private void Start()
	{
		BaseCatcher.OnFishCollected += this.BaseCatcher_OnFishCollected;
		SkillManager.Instance.OnSkillLevelChanged += this.Instance_OnSkillLevelChanged;
		WheelBehaviour.OnWheelSpun += this.WheelBehaviour_OnWheelSpun;
		for (int i = 0; i < this.quests.Count; i++)
		{
			Quest quest = this.quests[i];
			quest.OnQuestProgress += this.Quest_OnQuestProgress;
			quest.OnQuestCompleted += this.Quest_OnQuestCompleted;
			quest.OnQuestClaimed += this.Quest_OnQuestClaimed;
			if (this.questSkill.CurrentLevel == i)
			{
				quest.Activate(true);
			}
		}
		this.LoadQuests();
		LionAnalytics.TrackQuestReached(this.questSkill.CurrentLevel);
		LionAnalytics.BidOptimization_CompletedTutorial(this.questSkill.CurrentLevel);
		LionAnalytics.BidOptimization_AchievedLevel(this.questSkill.CurrentLevel);
	}

	public void LoadQuests()
	{
		InGameNotification inGameNotification = InGameNotificationManager.Instance.GetActiveNotifications(InGameNotification.IGN.Quest).FirstOrDefault<InGameNotification>();
		if (inGameNotification == null)
		{
			IGNQuest ignquest = new IGNQuest();
			ignquest.Quest = this.CurrentQuest;
			InGameNotificationManager.Instance.Create<IGNQuest>(ignquest);
		}
		else
		{
			((IGNQuest)inGameNotification).Quest = this.CurrentQuest;
		}
	}

	private void WheelBehaviour_OnWheelSpun()
	{
		this.wheelSpinCounterSkill.TryLevelUp();
	}

	private void Quest_OnQuestClaimed(Quest quest)
	{
		GameAnalyticsEvents.ResourceGemsIncreased(AnalyticsEvents.REType.Quest, AnalyticsEvents.RECategory.Gameplay, quest.GemReward);
		ResourceChangeData gemChangeData = new ResourceChangeData(this.getQuestContentId(this.questSkill.CurrentLevel), this.getQuestContentName(this.questSkill.CurrentLevel), quest.GemReward, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.QuestReward);
		GemGainVisual.Instance.GainGems(quest.GemReward, new Vector2(0f, 2.5f), gemChangeData);
		this.questSkill.TryLevelUp();
		Quest currentQuest = this.CurrentQuest;
		if (this.OnQuestClaimed != null)
		{
			this.OnQuestClaimed(this.questSkill, quest, currentQuest);
		}
		if (currentQuest != null)
		{
			currentQuest.Activate(false);
			IGNQuest ignquest = new IGNQuest();
			ignquest.Quest = currentQuest;
			InGameNotificationManager.Instance.Create<IGNQuest>(ignquest);
		}
		LionAnalytics.TrackQuestReached(this.questSkill.CurrentLevel);
		LionAnalytics.BidOptimization_CompletedTutorial(this.questSkill.CurrentLevel);
		LionAnalytics.BidOptimization_AchievedLevel(this.questSkill.CurrentLevel);
	}

	private void Quest_OnQuestProgress(Quest quest)
	{
		if (this.OnQuestProgress != null)
		{
			this.OnQuestProgress(quest);
		}
	}

	private void Quest_OnQuestCompleted(Quest quest)
	{
		if (this.OnQuestCompleted != null)
		{
			this.OnQuestCompleted(quest);
		}
	}

	private void Instance_OnSkillLevelChanged(Skill skill, LevelChange levelChange)
	{
		if (skill.IsTierSkill && levelChange == LevelChange.LevelUp && this.totalTierSkillsUpgradeCount != null)
		{
			this.totalTierSkillsUpgradeCount.TryLevelUp();
		}
		bool flag = SkillManager.Instance.CrewMembers.Contains(skill);
		if (flag)
		{
			if (skill.CurrentLevel == 1)
			{
				this.totalCrewMemberCounts.TryLevelUp();
			}
			if (skill.CurrentLevel > this.crewMemberLevel.CurrentLevel)
			{
				this.crewMemberLevel.TryLevelUp();
			}
		}
	}

	private void Skill_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		if (this.totalTierSkillsUpgradeCount != null)
		{
			this.totalTierSkillsUpgradeCount.TryLevelUp();
		}
	}

	private void BaseCatcher_OnFishCollected(FishBehaviour obj)
	{
		if (this.fishCollectedCount != null)
		{
			this.fishCollectedCount.TryLevelUp();
		}
	}

	private string getQuestContentId(int currentLevel)
	{
		return "contentId_quest_" + currentLevel;
	}

	private string getQuestContentName(int currentLevel)
	{
		return "Quest " + currentLevel;
	}

	[SerializeField]
	private Skill questSkill;

	[SerializeField]
	private List<Quest> quests = new List<Quest>();

	[SerializeField]
	private Skill fishCollectedCount;

	[SerializeField]
	private Skill totalTierSkillsUpgradeCount;

	[SerializeField]
	private Skill totalCrewMemberCounts;

	[SerializeField]
	private Skill crewMemberLevel;

	[SerializeField]
	private Skill wheelSpinCounterSkill;
}
