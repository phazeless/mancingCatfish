using System;

public class IGNHolidayOffer : InGameNotification
{
	public IGNHolidayOffer(HolidayOffer offer)
	{
		this.Offer = offer;
		this.ignType = offer.IGNToUse;
	}

	public HolidayOffer Offer { get; private set; }

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
			return this.ignType;
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

	private InGameNotification.IGN ignType = InGameNotification.IGN.HolidayOffer;
}
