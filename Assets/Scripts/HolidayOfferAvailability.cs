using System;
using UnityEngine;

public abstract class HolidayOfferAvailability : MonoBehaviour
{
	public abstract DateTime GetAvailableDate();

	public abstract DateTime GetExpireDate();

	public virtual bool IsAvailableAtThisTime
	{
		get
		{
			return TimeManager.Instance.RealNow >= this.GetAvailableDate() && TimeManager.Instance.RealNow < this.GetExpireDate();
		}
	}
}
