using System;
using UnityEngine;

public class GrantableGems : BaseGrantable
{
	public override int Amount
	{
		get
		{
			return this.gemAmount;
		}
	}

	public override void Grant(string contentIdForAnalytics, ResourceChangeReason resourceChangeReason)
	{
		ResourceChangeData gemChangeData = new ResourceChangeData(contentIdForAnalytics, null, this.gemAmount, ResourceType.Gems, ResourceChangeType.Earn, resourceChangeReason);
		ResourceManager.Instance.GiveGems(this.gemAmount, gemChangeData);
	}

	[SerializeField]
	private int gemAmount;
}
