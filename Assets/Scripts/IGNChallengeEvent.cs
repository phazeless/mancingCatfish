using System;

public class IGNChallengeEvent : InGameNotification
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
			return this.ignType;
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

	private InGameNotification.IGN ignType = InGameNotification.IGN.ChallengeEvent;
}
