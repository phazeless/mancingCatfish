using System;
using UnityEngine;

public class IGNGemChest : InGameNotification
{
	public override bool IsClearable
	{
		get
		{
			return false;
		}
	}

	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.GemChest;
		}
	}

	[HideInInspector]
	public int GemAmount { get; private set; }

	[HideInInspector]
	public int CrownAmount { get; private set; }

	public override bool RemoveOnReset
	{
		get
		{
			return false;
		}
	}

	public override void OnCreated()
	{
		this.GemAmount = UnityEngine.Random.Range(this.MinRandomGemAmount, this.MaxRandomGemAmount);
		this.CrownAmount = UnityEngine.Random.Range(this.MinRandomCrownAmount, this.MaxRandomCrownAmount);
	}

	public int MinRandomGemAmount = 10;

	public int MaxRandomGemAmount = 27;

	public int MinRandomCrownAmount = 1;

	public int MaxRandomCrownAmount = 3;
}
