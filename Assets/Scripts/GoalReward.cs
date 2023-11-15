using System;
using FullSerializer;
using UnityEngine;

[fsObject]
public class GoalReward : fiScriptableObjectWithId
{
	public GrantableContent Content
	{
		get
		{
			return this.content;
		}
	}

	public bool HasContent
	{
		get
		{
			return this.Content != null;
		}
	}

	private string GetContentIdForAnalytics(BaseGoal goal)
	{
		return "rewardForGoal_" + goal.Id;
	}

	public void Collect(BaseGoal goal)
	{
		this.content.Grant(this.GetContentIdForAnalytics(goal), ResourceChangeReason.GoalReward);
	}

	[SerializeField]
	private GrantableContent content;
}
