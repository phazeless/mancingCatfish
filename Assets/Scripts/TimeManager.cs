using System;
using System.Diagnostics;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	public static TimeManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<bool, DateTime> OnInitializedWithInternetTime;

	public bool IsInitializedWithInternetTime { get; private set; }

	public DateTime RealNow
	{
		get
		{
			if (this.useLocalTime)
			{
				return DateTime.Now.ToUniversalTime();
			}
			return this.initialRealNow.AddSeconds((double)Time.realtimeSinceStartup);
		}
	}

	public void OverrideUseLocalTime()
	{
		this.useLocalTime = true;
	}

	public bool IsLocalTimeWithinReasonableDiffFromRealTime()
	{
		DateTime d = DateTime.Now.ToUniversalTime();
		TimeSpan timeSpan = d - this.RealNow;
		return this.allowCheat || (timeSpan.TotalHours > -13.0 && timeSpan.TotalHours <= 13.0);
	}

	private void Awake()
	{
		TimeManager.Instance = this;
		this.UpdateRealNow(null);
	}

	public void UpdateRealNow(Action<DateTime> onUpdatedRealNow = null)
	{
		FHelper.GetTime(true, delegate(bool success, DateTime currentTime)
		{
			this.initialRealNow = currentTime;
			if (!this.IsInitializedWithInternetTime)
			{
				this.IsInitializedWithInternetTime = success;
				if (this.OnInitializedWithInternetTime != null)
				{
					this.OnInitializedWithInternetTime(success, (!this.useLocalTime) ? currentTime : DateTime.Now);
				}
			}
			if (success && onUpdatedRealNow != null)
			{
				onUpdatedRealNow((!this.useLocalTime) ? currentTime : DateTime.Now);
			}
		});
	}

	public bool IsWithinPeriod(HolidayOfferAvailability availability)
	{
		if (this.IsLocalTimeWithinReasonableDiffFromRealTime())
		{
			DateTime realNow = this.RealNow;
			return realNow > availability.GetAvailableDate().ToUniversalTime() && realNow < availability.GetExpireDate().ToUniversalTime();
		}
		return false;
	}

	public bool IsWithinPeriodLocal(HolidayOfferAvailability availability)
	{
		DateTime now = DateTime.Now;
		return now > availability.GetAvailableDate() && now < availability.GetExpireDate();
	}

	[SerializeField]
	private bool allowCheat;

	[SerializeField]
	private bool useLocalTime;

	private DateTime initialRealNow = DateTime.Now;
}
