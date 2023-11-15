using System;

public class AlwaysAvailability : HolidayOfferAvailability
{
	public override DateTime GetAvailableDate()
	{
		DateTime? dateTime = this.cachedAvailableDate;
		if (dateTime == null)
		{
			this.cachedAvailableDate = new DateTime?(new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0));
		}
		DateTime? dateTime2 = this.cachedAvailableDate;
		return dateTime2.Value;
	}

	public override DateTime GetExpireDate()
	{
		DateTime? dateTime = this.cachedExpireDate;
		if (dateTime == null)
		{
			this.cachedExpireDate = new DateTime?(new DateTime(DateTime.Now.Year, 12, DateTime.DaysInMonth(DateTime.Now.Year, 12), 0, 0, 0));
		}
		DateTime? dateTime2 = this.cachedExpireDate;
		return dateTime2.Value;
	}

	private DateTime? cachedAvailableDate;

	private DateTime? cachedExpireDate;
}
