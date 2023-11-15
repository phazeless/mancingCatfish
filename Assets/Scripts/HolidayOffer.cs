using System;
using UnityEngine;

public class HolidayOffer : MonoBehaviour
{
	public string Id
	{
		get
		{
			return this.behaviourToInstantiate.Id;
		}
	}

	public string ItemId
	{
		get
		{
			return this.itemId;
		}
	}

	public int GemCost
	{
		get
		{
			return this.gemCost;
		}
	}

	public virtual InGameNotification.IGN IGNToUse
	{
		get
		{
			return this.ignToUse;
		}
	}

	public HolidayOfferAvailability Availability
	{
		get
		{
			return this.offerAvailability;
		}
	}

	public HolidayOfferBehaviour BehaviourToInstantiate
	{
		get
		{
			return this.behaviourToInstantiate;
		}
	}

	public bool IsHandledByManager
	{
		get
		{
			return this.isHandledByManager;
		}
	}

	public bool AllowMultiplePurchase
	{
		get
		{
			return this.allowMultiplePurchase;
		}
	}

	public bool AutoCreateIGN
	{
		get
		{
			return this.autoCreateIGN;
		}
	}

	public virtual bool IsAvailableAtThisTime
	{
		get
		{
			return this.offerAvailability.IsAvailableAtThisTime;
		}
	}

	public virtual float SecondsUntilExpiration
	{
		get
		{
			return Mathf.Max(0f, (float)(this.offerAvailability.GetExpireDate() - DateTime.Now).TotalSeconds);
		}
	}

	public virtual string TimeUntilExpiration
	{
		get
		{
			return FHelper.FromSecondsToHoursMinutesSecondsFormat(this.SecondsUntilExpiration);
		}
	}

	[SerializeField]
	protected string itemId;

	[SerializeField]
	protected int gemCost;

	[SerializeField]
	protected HolidayOfferAvailability offerAvailability;

	[SerializeField]
	protected HolidayOfferBehaviour behaviourToInstantiate;

	[SerializeField]
	protected bool isHandledByManager = true;

	[SerializeField]
	protected bool allowMultiplePurchase;

	[SerializeField]
	protected bool autoCreateIGN = true;

	[SerializeField]
	protected InGameNotification.IGN ignToUse = InGameNotification.IGN.HolidayOffer;
}
