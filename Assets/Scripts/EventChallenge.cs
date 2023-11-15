using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;

public class EventChallenge : ScriptableObjectWithId
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<EventChallenge> GlobalOnChallengeCompleted;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<EventChallenge> OnChallengeCompleted;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<EventChallenge, BaseGoal> OnGoalProgress;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<EventChallenge, BaseGoal> OnGoalCompleted;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<EventChallenge, BaseGoal> OnGoalClaimed;

	public EventContent ParentEvent
	{
		get
		{
			return this.parentEvent;
		}
	}

	public bool UseBigIntegerCashFormatting
	{
		get
		{
			return this.useBigIntegerCashFormatting;
		}
	}

	public int CurrentGoalIndex
	{
		get
		{
			return this.currentGoalIndex;
		}
	}

	public bool IsCompleted
	{
		get
		{
			return this.isCompleted;
		}
	}

	public BaseGoal CurrentGoal
	{
		get
		{
			return this.goals[this.CurrentGoalIndex];
		}
	}

	public List<BaseGoal> Goals
	{
		get
		{
			return this.goals;
		}
	}

	public int MaxGoals
	{
		get
		{
			return this.goals.Count;
		}
	}

	public BigInteger TotalRequiredTargetValue
	{
		get
		{
			BigInteger bigInteger = BigInteger.Zero;
			for (int i = 0; i <= this.CurrentGoalIndex; i++)
			{
				bigInteger += this.goals[i].GetTargetValue();
			}
			return bigInteger;
		}
	}

	public BigInteger TotalProgress
	{
		get
		{
			BigInteger bigInteger = BigInteger.Zero;
			for (int i = 0; i <= this.CurrentGoalIndex; i++)
			{
				bigInteger += this.goals[i].GetCurrentValue();
			}
			return bigInteger;
		}
	}

	public string CurrentGoalDescription
	{
		get
		{
			return this.CurrentGoal.Description;
		}
	}

	public EventChallenge Create(EventContent parentEvent)
	{
		EventChallenge eventChallenge = UnityEngine.Object.Instantiate<EventChallenge>(this);
		eventChallenge.parentEvent = parentEvent;
		bool flag = false;
		eventChallenge.goals.Clear();
		for (int i = 0; i < this.goals.Count; i++)
		{
			BaseGoal baseGoal = this.goals[i].Create(eventChallenge);
			baseGoal.OnProgressChanged += eventChallenge.Goal_OnProgressChanged;
			baseGoal.OnCompleted += eventChallenge.Goal_OnCompleted;
			baseGoal.OnClaimed += eventChallenge.Goal_OnClaimed;
			eventChallenge.goals.Add(baseGoal);
			if (baseGoal.IsClaimed && !flag)
			{
				eventChallenge.IncreaseGoalIndex(this.goals.Count);
			}
			else
			{
				flag = true;
			}
		}
		eventChallenge.ActivateNext();
		return eventChallenge;
	}

	private void Goal_OnCompleted(BaseGoal obj)
	{
		if (this.OnGoalCompleted != null)
		{
			this.OnGoalCompleted(this, obj);
		}
	}

	private void Goal_OnClaimed(BaseGoal obj)
	{
		this.currentGoalIndex = 0;
		for (int i = 0; i < this.goals.Count; i++)
		{
			BaseGoal baseGoal = this.goals[i];
			if (!baseGoal.IsClaimed)
			{
				break;
			}
			this.IncreaseGoalIndex(this.goals.Count);
		}
		this.ActivateNext();
		if (this.OnGoalClaimed != null)
		{
			this.OnGoalClaimed(this, obj);
		}
		if (this.IsCompleted)
		{
			if (this.OnChallengeCompleted != null)
			{
				this.OnChallengeCompleted(this);
			}
			if (EventChallenge.GlobalOnChallengeCompleted != null)
			{
				EventChallenge.GlobalOnChallengeCompleted(this);
			}
		}
	}

	private void Goal_OnProgressChanged(BaseGoal obj)
	{
		if (this.OnGoalProgress != null)
		{
			this.OnGoalProgress(this, obj);
		}
	}

	protected virtual void IncreaseGoalIndex(int totalGoals)
	{
		this.currentGoalIndex++;
		if (this.currentGoalIndex >= totalGoals)
		{
			this.isCompleted = true;
			this.currentGoalIndex = Math.Max(0, totalGoals - 1);
		}
	}

	private void ActivateNext()
	{
		if (!this.IsCompleted && !this.CurrentGoal.IsCompleted)
		{
			this.CurrentGoal.Activate();
		}
	}

	public virtual void Save()
	{
		for (int i = 0; i < this.goals.Count; i++)
		{
			this.goals[i].Save();
		}
	}

	public void Clear(EventContent parentEvent)
	{
		for (int i = 0; i < this.goals.Count; i++)
		{
			this.goals[i].Clear(this);
		}
	}

	[SerializeField]
	private bool useBigIntegerCashFormatting;

	[SerializeField]
	private List<BaseGoal> goals = new List<BaseGoal>();

	[NonSerialized]
	private int currentGoalIndex;

	[NonSerialized]
	private bool isCompleted;

	[NonSerialized]
	private EventContent parentEvent;
}
