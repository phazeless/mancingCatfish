using System;

public class IGNInfo : InGameNotification
{
	public override bool IsClearable
	{
		get
		{
			return true;
		}
	}

	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.Info;
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

	public InfoHandler.InfoModel Info { get; set; }
}
