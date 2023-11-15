using System;
using UnityEngine;

public class MonthAvailability : HolidayOfferAvailability
{
	public override DateTime GetAvailableDate()
	{
		DateTime? dateTime = this.cachedAvailableDate;
		if (dateTime == null)
		{
			this.cachedAvailableDate = new DateTime?(new DateTime(DateTime.Now.Year, this.monthOfYear, 1, 0, 0, 0));
		}
		DateTime? dateTime2 = this.cachedAvailableDate;
		return dateTime2.Value;
	}

	public override DateTime GetExpireDate()
	{
		DateTime? dateTime = this.cachedExpireDate;
		if (dateTime == null)
		{
			this.cachedExpireDate = new DateTime?(new DateTime(DateTime.Now.Year, this.monthOfYear, DateTime.DaysInMonth(DateTime.Now.Year, this.monthOfYear), 23, 59, 59));
		}
		DateTime? dateTime2 = this.cachedExpireDate;
		return dateTime2.Value;
	}

	[SerializeField]
	private int monthOfYear = 1;

	private DateTime? cachedAvailableDate;

	private DateTime? cachedExpireDate;
}
