using System;

public class IGNSpecialOffer : InGameNotification
{
	public IGNSpecialOffer(SpecialOffer specialOffer)
	{
		this.SpecialOffer = specialOffer;
	}

	public SpecialOffer SpecialOffer { get; private set; }

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
			return InGameNotification.IGN.SpecialOffer;
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
