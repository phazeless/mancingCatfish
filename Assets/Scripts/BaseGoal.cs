using System;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;

public abstract class BaseGoal : ScriptableObjectWithId
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<BaseGoal> GlobalOnGoalClaimed;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BaseGoal> OnProgressChanged;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BaseGoal> OnCompleted;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BaseGoal> OnClaimed;

	public EventChallenge ParentChallenge
	{
		get
		{
			return this.parentChallenge;
		}
	}

	public string Description
	{
		get
		{
			return this.description;
		}
	}

	public BaseGrantable GoalReward
	{
		get
		{
			return this.goalReward;
		}
	}

	protected BigInteger GoalCounter
	{
		get
		{
			return this.goalCounter;
		}
		set
		{
			this.NotifyProgress(value);
		}
	}

	protected virtual void NotifyProgress(BigInteger newVal)
	{
		BigInteger right = this.goalCounter;
		bool isCompleted = this.IsCompleted;
		this.goalCounter = BigInteger.Min(newVal, this.GetTargetValue());
		if (this.goalCounter != right && this.OnProgressChanged != null)
		{
			this.OnProgressChanged(this);
		}
		if (!isCompleted && this.IsCompleted && this.OnCompleted != null)
		{
			this.Disable();
			this.OnCompleted(this);
		}
	}

	public bool IsCompleted
	{
		get
		{
			return this.GetCurrentValue() >= this.GetTargetValue();
		}
	}

	public bool IsClaimed { get; private set; }

	public abstract void Activate();

	public abstract void Disable();

	public abstract BigInteger GetTargetValue();

	public virtual BaseGoal Create(EventChallenge parentChallenge)
	{
		BaseGoal baseGoal = UnityEngine.Object.Instantiate<BaseGoal>(this);
		baseGoal.parentChallenge = parentChallenge;
		baseGoal.Load();
		baseGoal.NotifyProgress(baseGoal.goalCounter);
		return baseGoal;
	}

	public virtual BigInteger GetCurrentValue()
	{
		return this.GoalCounter;
	}

	public virtual void CollectReward()
	{
		if (this.IsCompleted)
		{
			this.goalReward.Grant(this.GetAnalyticsContentIdForReward(), ResourceChangeReason.GoalReward);
			this.IsClaimed = true;
			this.Save();
			if (this.OnClaimed != null)
			{
				this.OnClaimed(this);
			}
			if (BaseGoal.GlobalOnGoalClaimed != null)
			{
				BaseGoal.GlobalOnGoalClaimed(this);
			}
		}
	}

	public virtual void Save()
	{
		EncryptedPlayerPrefs.SetInt(this.GetIsClaimedStorageId(this.ParentChallenge), (!this.IsClaimed) ? 0 : 1, true);
		EncryptedPlayerPrefs.SetString(this.GetGoalCounterStorageId(this.ParentChallenge), this.GoalCounter.ToString(), true);
	}

	public virtual void Load()
	{
		this.IsClaimed = (EncryptedPlayerPrefs.GetInt(this.GetIsClaimedStorageId(this.ParentChallenge), 0) == 1);
        BigInteger temp = BigInteger.Parse(EncryptedPlayerPrefs.GetString(this.GetGoalCounterStorageId(this.ParentChallenge), this.goalCounter.ToString()));

        this.goalCounter = new BigInteger((int)temp);
		if (this.IsClaimed && !this.IsCompleted)
		{
			this.goalCounter = this.GetTargetValue();
		}
	}

	public virtual void Clear(EventChallenge parentChallenge)
	{
		EncryptedPlayerPrefs.DeleteKey(this.GetGoalCounterStorageId(parentChallenge));
		EncryptedPlayerPrefs.DeleteKey(this.GetIsClaimedStorageId(parentChallenge));
	}

	private string GetGoalCounterStorageId(EventChallenge _parentChallenge)
	{
		return "GoalCounter_" + this.GetParentCombinedId(_parentChallenge);
	}

	private string GetIsClaimedStorageId(EventChallenge _parentChallenge)
	{
		return "IsClaimed_" + this.GetParentCombinedId(_parentChallenge);
	}

	private string GetAnalyticsContentIdForReward()
	{
		return "rewardForGoal_" + base.Id;
	}

	private string GetParentCombinedId(EventChallenge _parentChallenge)
	{
		return string.Concat(new string[]
		{
			_parentChallenge.ParentEvent.Id,
			"_",
			_parentChallenge.Id,
			"_",
			base.Id
		});
	}

	[SerializeField]
	private string description;

	[SerializeField]
	private BaseGrantable goalReward;

	[NonSerialized]
	private BigInteger goalCounter = 0;

	[NonSerialized]
	private EventChallenge parentChallenge;
}
