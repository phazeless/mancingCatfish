using System;
using ACE.IAPS;
using TMPro;
using UnityEngine;

public class UISpecialOfferChestItem : MonoBehaviour
{
	public void SetSpecialChestOfferItem(SpecialOfferChestItem specialOfferChestItem)
	{
		this.specialOfferChestItem = specialOfferChestItem;
	}

	public void Buy()
	{
		string chestIapSKU = this.specialOfferChestItem.IapSKU;
		if (ResourceManager.Instance.IsMarketItem(chestIapSKU))
		{
			UIIAPPendingBlocker.Instance.Show();
		}
		ResourceManager.Instance.Buy(chestIapSKU, delegate(PurchaseResult resp, string b)
		{
			if (resp == PurchaseResult.ItemPurchased)
			{
				if (ResourceManager.Instance.IsMarketItem(chestIapSKU))
				{
					UnityEngine.Debug.LogWarning("User just Purchased: " + chestIapSKU);
				}
				this.specialOfferChestItem.WasPurchased = true;
				UnityEngine.Object.Destroy(this.gameObject);
				ChestManager.Instance.OpenChest(this.specialOfferChestItem.ItemChest);
			}
			UIIAPPendingBlocker.Instance.Hide();
		});
	}

	private void Update()
	{
		if (this.specialOfferChestItem != null)
		{
			int secondsUntilExpiration = this.specialOfferChestItem.SecondsUntilExpiration;
			this.lblExpiresIn.SetVariableText(new string[]
			{
				FHelper.FromSecondsToHoursMinutesSecondsFormat((float)secondsUntilExpiration)
			});
			if (secondsUntilExpiration <= 0)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnEnable()
	{
		if (this.specialOfferChestItem != null)
		{
			this.lblCost.SetVariableText(new string[]
			{
				ResourceManager.Instance.GetMarketItemPriceAndCurrency("se.ace.special_offer_1")
			});
		}
	}

	[SerializeField]
	private TextMeshProUGUI lblExpiresIn;

	[SerializeField]
	private TextMeshProUGUI lblCost;

	private SpecialOfferChestItem specialOfferChestItem;
}
