using System;
using UnityEngine;

public class FireworkConsumable : BaseConsumable
{
	public override int MaxAmount
	{
		get
		{
			return this.maxAmount;
		}
	}

	[SerializeField]
	private int maxAmount = 999;
}
