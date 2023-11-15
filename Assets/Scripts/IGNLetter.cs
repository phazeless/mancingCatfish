using System;

public class IGNLetter : InGameNotification
{
	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.Letter;
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

	public int CashAmount;
}
