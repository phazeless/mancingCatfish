using System;

public class GrantableNothing : BaseGrantable
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
	}
}
