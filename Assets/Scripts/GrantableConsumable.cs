using System;
using UnityEngine;

public class GrantableConsumable : BaseGrantable
{
	public BaseConsumable Consumable
	{
		get
		{
			return this.consumable;
		}
	}

	public override int Amount
	{
		get
		{
			return this.amount;
		}
	}

	public override void Grant(string contentIdForAnalytics, ResourceChangeReason resourceChangeReason)
	{
		ConsumableManager.Instance.Grant(this.consumable, this.amount, resourceChangeReason, false);
	}

	public override Sprite Icon
	{
		get
		{
			return this.consumable.Icon;
		}
	}

	public override Color IconBg
	{
		get
		{
			return this.consumable.IconBg;
		}
	}

	[SerializeField]
	private BaseConsumable consumable;

	[SerializeField]
	private int amount;
}
