using System;

public class IGNConnectFacebook : InGameNotification
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
			return InGameNotification.IGN.ConnectFacebook;
		}
	}

	public override bool RemoveOnReset
	{
		get
		{
			return true;
		}
	}

	public override bool RemoveOnExit
	{
		get
		{
			return true;
		}
	}
}
