using System;
using UnityEngine;

public class GrantableCrownExp : BaseGrantable
{
	public override int Amount
	{
		get
		{
			return this.crownExpAmount;
		}
	}

	public override void Grant(string contentIdForAnalytics, ResourceChangeReason resourceChangeReason)
	{
		ResourceChangeData gemChangeData = new ResourceChangeData(contentIdForAnalytics, null, this.crownExpAmount, ResourceType.CrownExp, ResourceChangeType.Earn, resourceChangeReason);
		ResourceManager.Instance.GiveCrownExp(this.crownExpAmount, gemChangeData);
	}

	[SerializeField]
	private int crownExpAmount;
}
