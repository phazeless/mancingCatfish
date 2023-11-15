using System;
using ACE.IAPS;
using FullInspector;
using UnityEngine;

public abstract class HolidayOfferBehaviour : MonoBehaviour
{
	public void Init(HolidayOffer offer, Action<HolidayOffer> onOfferBought)
	{
		this.offer = offer;
		this.onOfferBought = onOfferBought;
		this.isMarketOffer = (ResourceManager.Instance.GetProductId(offer.ItemId) != null);
	}

	public string Id
	{
		get
		{
			return this.id;
		}
	}

	public HolidayOffer Offer
	{
		get
		{
			return this.offer;
		}
	}

	public virtual void DisableShopSpecificUI()
	{
		Transform child = base.transform.GetChild(0);
		if (child != null)
		{
			child.gameObject.SetActive(false);
		}
	}

	public virtual void Buy()
	{
		if (this.isMarketOffer)
		{
			UIIAPPendingBlocker.Instance.Show();
			ResourceManager.StoreManager.Buy(this.offer.ItemId, delegate(PurchaseResult response, string msg)
			{
				UIIAPPendingBlocker.Instance.Hide();
				if (response == PurchaseResult.ItemPurchased)
				{
					this.OnBought();
					if (this.onOfferBought != null)
					{
						this.onOfferBought(this.offer);
					}
					CloudOnceManager.Instance.SaveDataToCache();
				}
			});
		}
		else
		{
			ResourceManager.Instance.BuyWithGems(this.offer, delegate(PurchaseResult result, string msg)
			{
				if (result == PurchaseResult.ItemPurchased)
				{
					this.OnBought();
					if (this.onOfferBought != null)
					{
						this.onOfferBought(this.offer);
					}
					CloudOnceManager.Instance.SaveDataToCache();
				}
				else
				{
					this.OnBuyWithGemsFailed();
				}
			});
		}
	}

	public abstract void OnBought();

	protected virtual void OnBuyWithGemsFailed()
	{
	}

	[ShowInInspector]
	[InspectorDisabled]
	[SerializeField]
	private string id;

	protected HolidayOffer offer;

	protected Action<HolidayOffer> onOfferBought;

	protected bool isMarketOffer = true;
}
