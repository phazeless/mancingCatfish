using System;

public class IGNTournament : InGameNotification
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
			return InGameNotification.IGN.Tournament;
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
