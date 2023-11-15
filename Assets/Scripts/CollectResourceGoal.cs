using System;
using System.Numerics;
using UnityEngine;

public class CollectResourceGoal : BaseGoal
{
	public override void Activate()
	{
		ResourceManager.Instance.OnResourceChanged += this.Instance_OnResourceChanged;
	}

	private void Instance_OnResourceChanged(ResourceType resType, BigInteger amountAdded, BigInteger totalCurrentAmount)
	{
		if (resType == this.resourceTypeToCount)
		{
			base.GoalCounter += amountAdded;
		}
	}

	public override void Disable()
	{
		ResourceManager.Instance.OnResourceChanged -= this.Instance_OnResourceChanged;
	}

	public override BigInteger GetTargetValue()
	{
		BigInteger? bigInteger = this.cachedValue;
		if (bigInteger == null)
		{
			this.cachedValue = new BigInteger?(BigInteger.Parse(this.targetResourceAmount.stringValue));
		}
		BigInteger? bigInteger2 = this.cachedValue;
		return bigInteger2.Value;
	}

	[SerializeField]
	private ResourceType resourceTypeToCount;

	[SerializeField]
	private BigIntWrapper targetResourceAmount = new BigIntWrapper();

	private BigInteger? cachedValue;
}
