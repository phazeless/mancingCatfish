using System;
using System.Diagnostics;
using FullInspector;
using UnityEngine;

[fiInspectorOnly]
public class Quest : ScriptableObject
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Quest> OnQuestProgress;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Quest> OnQuestCompleted;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Quest> OnQuestClaimed;

	public void SetBonusGems(int bonusGems)
	{
		this.BonusGems = bonusGems;
	}

	public int BonusGems { get; private set; }

	public Quest.QuestGoalType GoalType
	{
		get
		{
			return this.goalType;
		}
	}

	public int Goal
	{
		get
		{
			return this.goal;
		}
	}

	public string Title
	{
		get
		{
			return this.title;
		}
	}

	public string QuestDescription
	{
		get
		{
			return this.questDescription.Replace("{goal}", this.Goal.ToString());
		}
	}

	private bool IsLifetime
	{
		get
		{
			return this.goalType == Quest.QuestGoalType.Lifetime;
		}
	}

	public int Progress
	{
		get
		{
			if (!this.skill.GetExtraInfo().CacheCurrentLevelAsBigInteger)
			{
				return (this.goalType != Quest.QuestGoalType.Current) ? ((!this.restartAfterCompletion) ? this.skill.LifetimeLevel : (this.skill.LifetimeLevel - this.cachedPreviousActivationLevel)) : this.skill.CurrentLevel;
			}
			return (this.skill.CurrentLevelAsLong < 2147483647L) ? ((int)this.skill.CurrentLevelAsLong) : int.MaxValue;
		}
	}

	public int GemReward
	{
		get
		{
			return this.gemReward + this.BonusGems;
		}
	}

	public bool IsCompleted
	{
		get
		{
			return this.Progress >= this.Goal;
		}
	}

	public void Claim()
	{
		if (this.OnQuestClaimed != null)
		{
			this.OnQuestClaimed(this);
		}
	}

	public void Activate(bool isStartup = false)
	{
		this.skill.OnSkillLevelUp += this.Listener_OnSkillLevelUp;
		if (isStartup)
		{
			this.cachedPreviousActivationLevel = this.skill.GetMetaData<int>("KEY_PREVIOUS_ACTIVATION_LEVEL");
		}
		else
		{
			this.cachedPreviousActivationLevel = this.skill.LifetimeLevel;
			this.skill.AddMetaData("KEY_PREVIOUS_ACTIVATION_LEVEL", this.skill.LifetimeLevel);
		}
	}

	private void Listener_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		if (this.Progress < this.goal)
		{
			if (this.OnQuestProgress != null)
			{
				this.OnQuestProgress(this);
			}
		}
		else
		{
			skill.OnSkillLevelUp -= this.Listener_OnSkillLevelUp;
			if (this.OnQuestCompleted != null)
			{
				this.OnQuestCompleted(this);
			}
		}
	}

	private const string KEY_PREVIOUS_ACTIVATION_LEVEL = "KEY_PREVIOUS_ACTIVATION_LEVEL";

	[SerializeField]
	private string title;

	[SerializeField]
	private string questDescription;

	[SerializeField]
	private Skill skill;

	[SerializeField]
	private int goal;

	[SerializeField]
	private int gemReward;

	[SerializeField]
	private Quest.QuestGoalType goalType;

	[SerializeField]
	[InspectorShowIf("IsLifetime")]
	private bool restartAfterCompletion;

	private int cachedPreviousActivationLevel;

	public enum QuestGoalType
	{
		Current,
		Lifetime
	}
}
