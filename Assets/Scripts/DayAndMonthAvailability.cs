using System;
using UnityEngine;

public class DayAndMonthAvailability : HolidayOfferAvailability
{
	public override DateTime GetAvailableDate()
	{
		DateTime? dateTime = this.cachedAvailableDate;
		if (dateTime == null)
		{
			this.cachedAvailableDate = new DateTime?(new DateTime(DateTime.Now.Year, this.monthOfYear, this.dayOfMonth, 0, 0, 0));
			DateTime? dateTime2 = this.cachedAvailableDate;
			bool flag = this.GetExpireDate(dateTime2.Value).Year > DateTime.Now.Year;
			if (flag)
			{
				this.cachedAvailableDate = new DateTime?(new DateTime(DateTime.Now.Year - 1, this.monthOfYear, this.dayOfMonth, 0, 0, 0));
			}
		}
		DateTime? dateTime3 = this.cachedAvailableDate;
		return dateTime3.Value;
	}

	public override DateTime GetExpireDate()
	{
		return this.GetExpireDate(this.GetAvailableDate());
	}

	private DateTime GetExpireDate(DateTime availableDate)
	{
		return availableDate.AddSeconds((double)this.durationInSeconds);
	}

	[SerializeField]
	private int dayOfMonth;

	[SerializeField]
	private int monthOfYear;

	[SerializeField]
	private float durationInSeconds;

	private DateTime? cachedAvailableDate;
}
