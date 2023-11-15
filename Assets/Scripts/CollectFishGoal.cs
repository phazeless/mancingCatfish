using System;
using System.Numerics;
using FullInspector;
using UnityEngine;

public class CollectFishGoal : BaseGoal
{
	public override void Activate()
	{
		BaseCatcher.OnFishCollected += this.SwipeCatcher_OnFishCollected;
	}

	public override void Disable()
	{
		BaseCatcher.OnFishCollected -= this.SwipeCatcher_OnFishCollected;
	}

	private void SwipeCatcher_OnFishCollected(FishBehaviour fish)
	{
		if (this.fishCountType == CollectFishGoal.FishCountType.All)
		{
			base.GoalCounter = ++base.GoalCounter;
		}
		else if (this.fishCountType == CollectFishGoal.FishCountType.Specific && this.fishTypeToCount == fish.FishInfo.FishType)
		{
			base.GoalCounter = ++base.GoalCounter;
		}
	}

	public override BigInteger GetTargetValue()
	{
		return this.targetFishAmount;
	}

	private bool IsCountingForAllFishes
	{
		get
		{
			return this.fishCountType == CollectFishGoal.FishCountType.All;
		}
	}

	[SerializeField]
	private CollectFishGoal.FishCountType fishCountType;

	[SerializeField]
	[InspectorHideIf("IsCountingForAllFishes")]
	private FishBehaviour.FishType fishTypeToCount;

	[SerializeField]
	private int targetFishAmount;

	public enum FishCountType
	{
		All,
		Specific
	}
}
