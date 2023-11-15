using System;

public class IGNNewFish : InGameNotification
{
	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.NewFish;
		}
	}

	public override bool IsClearable
	{
		get
		{
			return true;
		}
	}

	public override bool RemoveOnReset
	{
		get
		{
			return false;
		}
	}

	public FishAttributes FishInfo;
}
