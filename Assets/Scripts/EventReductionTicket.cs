using System;
using FullInspector;
using UnityEngine;

public class EventReductionTicket : BaseConsumable
{
	public float PriceReduction
	{
		get
		{
			return this.priceReduction;
		}
	}

	public override int MaxAmount
	{
		get
		{
			return this.maxAmount;
		}
	}

	[SerializeField]
	private int maxAmount;

	[InspectorTooltip("Reduction decimal percent. 0.10 = 10%")]
	[SerializeField]
	private float priceReduction;
}
