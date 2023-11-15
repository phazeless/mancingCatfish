using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class HolidayOfferManager : MonoBehaviour
{
	public static HolidayOfferManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<HolidayOffer> OnHolidayOfferBought;

	private void Awake()
	{
		HolidayOfferManager.Instance = this;
		this.Load();
	}

	private void Start()
	{
		this.CheckForChanges(true);
	}

	private void Update()
	{
		this.CheckForChanges(false);
	}

	private void CheckForChanges(bool forceOpenOfferIGN = false)
	{
		for (int i = 0; i < this.offers.Count; i++)
		{
			HolidayOffer holidayOffer = this.offers[i];
			if (holidayOffer.IsHandledByManager)
			{
				if (this.IsVisuallyActive(holidayOffer))
				{
					if ((!holidayOffer.IsAvailableAtThisTime || this.IsBought(holidayOffer)) && this.IsStarted(holidayOffer))
					{
						this.ExpireOffer(holidayOffer);
					}
				}
				else if (holidayOffer.IsAvailableAtThisTime)
				{
					if (!this.IsBought(holidayOffer))
					{
						if (!this.IsStarted(holidayOffer))
						{
							this.startedOffers.Add(holidayOffer.Id);
							if (holidayOffer.AutoCreateIGN)
							{
								this.CreateOfferIGN(holidayOffer, forceOpenOfferIGN);
							}
						}
						this.CreateOfferInShop(holidayOffer);
					}
				}
				else
				{
					this.boughtOffers.Remove(holidayOffer.Id);
				}
			}
		}
	}

	public void CreateOfferInShop(HolidayOffer offer)
	{
		HolidayOfferBehaviour holidayOfferBehaviour = UnityEngine.Object.Instantiate<HolidayOfferBehaviour>(offer.BehaviourToInstantiate, this.positionForUiToInstantiate.parent);
		int siblingIndex = this.positionForUiToInstantiate.GetSiblingIndex() - 1;
		holidayOfferBehaviour.transform.SetSiblingIndex(siblingIndex);
		holidayOfferBehaviour.Init(offer, new Action<HolidayOffer>(this.MarkOfferAsBought));
		this.instantiatedOffers.Add(offer, holidayOfferBehaviour);
	}

	public void CreateOfferIGN(HolidayOffer offer, bool forceOpenOfferIGN = false)
	{
		this.ignOffer = new IGNHolidayOffer(offer);
		InGameNotificationManager.Instance.Create<IGNHolidayOffer>(this.ignOffer);
	}

	public bool IsAnyOffersActive()
	{
		return this.startedOffers.Count > 0;
	}

	public bool IsStarted(HolidayOffer offer)
	{
		return this.startedOffers.Contains(offer.Id);
	}

	public bool IsVisuallyActive(HolidayOffer offer)
	{
		return this.instantiatedOffers.ContainsKey(offer);
	}

	public bool IsBought(HolidayOffer offer)
	{
		return this.boughtOffers.Contains(offer.Id);
	}

	public void ExpireOffer(HolidayOffer offer)
	{
		HolidayOfferBehaviour holidayOfferBehaviour = this.instantiatedOffers[offer];
		this.instantiatedOffers.Remove(offer);
		UnityEngine.Object.Destroy(holidayOfferBehaviour.gameObject);
		this.startedOffers.Remove(offer.Id);
		if (this.ignOffer != null)
		{
			this.ignOffer.SetExpiration(0f);
			this.ignOffer = null;
		}
	}

	public void MarkOfferAsBought(HolidayOffer offer)
	{
		if (!offer.AllowMultiplePurchase)
		{
			this.boughtOffers.Add(offer.Id);
		}
		if (this.OnHolidayOfferBought != null)
		{
			this.OnHolidayOfferBought(offer);
		}
	}

	public List<DateTime> GetIncomingOffers()
	{
		DateTime now = DateTime.Now;
		return (from x in this.offers.FindAll((HolidayOffer x) => now < x.Availability.GetAvailableDate())
		select x.Availability.GetAvailableDate()).ToList<DateTime>();
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause)
		{
			this.Save();
		}
	}

	private void Save()
	{
		EncryptedPlayerPrefs.SetString("KEY_BOUGHT_OFFERS", JsonConvert.SerializeObject(this.boughtOffers), true);
		EncryptedPlayerPrefs.SetString("KEY_STARTED_OFFERS", JsonConvert.SerializeObject(this.startedOffers), true);
		EncryptedPlayerPrefs.SetString("KEY_KEEP_TRACK_OF_24_H_PASSING", this.keepTrackOf24hPassing.Ticks.ToString(), true);
	}

	private void Load()
	{
		string @string = EncryptedPlayerPrefs.GetString("KEY_BOUGHT_OFFERS", null);
		string string2 = EncryptedPlayerPrefs.GetString("KEY_STARTED_OFFERS", null);
		if (!string.IsNullOrEmpty(@string))
		{
			this.boughtOffers = JsonConvert.DeserializeObject<HashSet<string>>(@string);
		}
		if (!string.IsNullOrEmpty(string2))
		{
			this.startedOffers = JsonConvert.DeserializeObject<HashSet<string>>(string2);
		}
		if (this.boughtOffers == null)
		{
			this.boughtOffers = new HashSet<string>();
		}
		if (this.startedOffers == null)
		{
			this.startedOffers = new HashSet<string>();
		}
		this.keepTrackOf24hPassing = new DateTime(long.Parse(EncryptedPlayerPrefs.GetString("KEY_KEEP_TRACK_OF_24_H_PASSING", "0")));
	}

	private const string KEY_KEEP_TRACK_OF_24_H_PASSING = "KEY_KEEP_TRACK_OF_24_H_PASSING";

	private const string KEY_BOUGHT_OFFERS = "KEY_BOUGHT_OFFERS";

	private const string KEY_STARTED_OFFERS = "KEY_STARTED_OFFERS";

	[SerializeField]
	private Transform positionForUiToInstantiate;

	[SerializeField]
	private List<HolidayOffer> offers = new List<HolidayOffer>();

	private Dictionary<HolidayOffer, HolidayOfferBehaviour> instantiatedOffers = new Dictionary<HolidayOffer, HolidayOfferBehaviour>();

	private HashSet<string> startedOffers = new HashSet<string>();

	private HashSet<string> boughtOffers = new HashSet<string>();

	private IGNHolidayOffer ignOffer;

	private DateTime keepTrackOf24hPassing = DateTime.Now;
}
