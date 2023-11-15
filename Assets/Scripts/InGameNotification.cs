using System;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public abstract class InGameNotification
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnNotificationExpired;

	public InGameNotification SetExpiration(float secondsFromNow)
	{
		this.expiresAt = DateTime.Now.AddSeconds((double)secondsFromNow);
		this.hasExpiration = true;
		return this;
	}

	[HideInInspector]
	public bool OverrideClearable { get; set; }

	public abstract bool IsClearable { get; }

	public abstract InGameNotification.IGN Type { get; }

	public abstract bool RemoveOnReset { get; }

	public virtual bool RemoveOnExit
	{
		get
		{
			return false;
		}
	}

	public object Clone()
	{
		return base.MemberwiseClone();
	}

	public virtual DateTime Expiration
	{
		get
		{
			return this.expiresAt;
		}
	}

	public float ExpiresInSeconds
	{
		get
		{
			return (float)(this.Expiration - DateTime.Now).TotalSeconds;
		}
	}

	public bool HasExpiration
	{
		get
		{
			return this.hasExpiration;
		}
	}

	public bool HasExpired
	{
		get
		{
			return this.HasExpiration && DateTime.Now >= this.Expiration;
		}
	}

	public void NotifyExpired()
	{
		this.hasExpiration = true;
		this.expiresAt = DateTime.Now;
		if (this.OnNotificationExpired != null)
		{
			this.OnNotificationExpired();
		}
	}

	public virtual void OnCreated()
	{
	}

	private DateTime expiresAt = default(DateTime);

	private bool hasExpiration;

	public enum IGN
	{
		FishValueBonus,
		Package,
		Letter,
		Quest,
		Challenge,
		Ad,
		Gift,
		NewFish,
		Lobster,
		GemChest,
		Review,
		NewCrew,
		SpecialOffer,
		ConnectFacebook,
		Info,
		Tournament,
		ChristmasGift,
		ItemChest,
		Crab,
		DailyGift,
		HolidayOffer,
		ValentineOffer,
		EasterOffer,
		EasterEvent,
		ChallengeEvent,
		FourthJulyOffer
	}
}
