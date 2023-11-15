using System;

public class IGNReview : InGameNotification
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
			return InGameNotification.IGN.Review;
		}
	}

	public override bool RemoveOnReset
	{
		get
		{
			return false;
		}
	}
}
