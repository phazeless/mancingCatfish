using System;
using UnityEngine;

public class GrantableItem : BaseGrantable
{
	public Item Item
	{
		get
		{
			return this.item;
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
		this.item.ChangeItemAmount(this.amount, resourceChangeReason);
	}

	[SerializeField]
	private Item item;

	[SerializeField]
	private int amount;
}
