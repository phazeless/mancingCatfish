using System;
using System.Collections.Generic;
using UnityEngine;

public class GrantableComposite : BaseGrantable
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
		for (int i = 0; i < this.grantables.Count; i++)
		{
			this.grantables[i].Grant(contentIdForAnalytics, resourceChangeReason);
		}
	}

	[SerializeField]
	private List<BaseGrantable> grantables = new List<BaseGrantable>();
}
