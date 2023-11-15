using System;
using UnityEngine;

public class GrantableItemChest : BaseGrantable
{
	public override int Amount
	{
		get
		{
			return 1;
		}
	}

	public override void Grant(string contentIdForAnalytics, ResourceChangeReason resourceChangeReason)
	{
		ChestManager.Instance.CreateReceivedChest(this.chest);
	}

	[SerializeField]
	private ItemChest chest;
}
