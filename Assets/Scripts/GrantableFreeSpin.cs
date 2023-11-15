using System;

public class GrantableFreeSpin : BaseGrantable
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
		ResourceManager.Instance.GiveFreeSpin();
	}
}
