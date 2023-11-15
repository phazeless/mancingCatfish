using System;
using System.Numerics;
using UnityEngine;

public class UseConsumableGoal : BaseGoal
{
	public override void Activate()
	{
		this.consumable.OnConsumed += this.Consumable_OnConsumed;
	}

	public override void Disable()
	{
		this.consumable.OnConsumed -= this.Consumable_OnConsumed;
	}

	public override BigInteger GetTargetValue()
	{
		return this.targetUseAmount;
	}

	private void Consumable_OnConsumed(BaseConsumable consumable, int amount)
	{
		base.GoalCounter = ++base.GoalCounter;
	}

	[SerializeField]
	private BaseConsumable consumable;

	[SerializeField]
	private int targetUseAmount;
}
