using System;

public class IGNDailyGift : InGameNotification
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
			return InGameNotification.IGN.DailyGift;
		}
	}

	public override bool RemoveOnReset
	{
		get
		{
			return false;
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
