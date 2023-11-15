using System;
using System.Collections.Generic;
using ACE.IAPS;
using TMPro;
using UnityEngine;

public class UISpecialOfferItem : MonoBehaviour
{
	public SpecialOffer SpecialOffer { get; private set; }

	public void SetSpecialOffer(SpecialOffer specialoffer)
	{
		this.SpecialOffer = specialoffer;
		if (this.crownExpGranter != null)
		{
			this.crownExpGranter.Location = GranterLocation.Create(specialoffer.IAPPlacement);
		}
		this.UpdateUI();
	}

	private void UpdateUI()
	{
		if (this.SpecialOffer == null)
		{
			return;
		}
		this.titleLabel.SetText(this.SpecialOffer.OfferName);
		int crewMemberCount = this.SpecialOffer.CrewMemberCount;
		this.crewAmountLabel.gameObject.SetActive(crewMemberCount > 0);
		this.gemAmountLabel.gameObject.SetActive(this.SpecialOffer.ContainsGems);
		this.crownAmountLabel.gameObject.SetActive(this.SpecialOffer.ContainsCrownExp);
		this.freeSpinAmountLabel.gameObject.SetActive(this.SpecialOffer.ContainsFreeSpins);
		this.crewAmountLabel.SetVariableText(new string[]
		{
			crewMemberCount.ToString()
		});
		this.gemAmountLabel.SetVariableText(new string[]
		{
			this.SpecialOffer.GemAmount.ToString()
		});
		this.crownAmountLabel.SetVariableText(new string[]
		{
			this.SpecialOffer.CrownExpAmount.ToString()
		});
		this.freeSpinAmountLabel.SetVariableText(new string[]
		{
			this.SpecialOffer.FreeSpinAmount.ToString()
		});
		this.costAmountLabel.SetText(ResourceManager.Instance.GetMarketItemPriceAndCurrency(this.SpecialOffer.ProductId));
		for (int i = 0; i < Mathf.Min(this.randomCrewMemberPortraits.Count, crewMemberCount); i++)
		{
			this.randomCrewMemberPortraits[i].SetActive(true);
		}
	}

	private void Update()
	{
		if (this.SpecialOffer != null)
		{
			this.expirationLabel.SetVariableText(new string[]
			{
				FHelper.FromSecondsToHoursMinutesSecondsFormat(this.SpecialOffer.TotalSecondsLeftOnDuration)
			});
		}
	}

	private void OnEnable()
	{
		this.UpdateUI();
	}

	public void Purchase()
	{
		UIIAPPendingBlocker.Instance.Show();
		ResourceManager.Instance.Buy(this.SpecialOffer.ItemId, delegate(PurchaseResult resp, string metaData)
		{
			UIIAPPendingBlocker.Instance.Hide();
			if (resp == PurchaseResult.ItemPurchased)
			{
				this.SpecialOffer.Claim();
				UnityEngine.Debug.LogWarning("User just Purchased: " + this.SpecialOffer.ItemId);
			}
			else
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					"Purchase canceled: ",
					resp,
					". MetaData: ",
					metaData
				}));
			}
		});
	}

	[SerializeField]
	private TextMeshProUGUI titleLabel;

	[SerializeField]
	private TextMeshProUGUI crewAmountLabel;

	[SerializeField]
	private TextMeshProUGUI gemAmountLabel;

	[SerializeField]
	private TextMeshProUGUI crownAmountLabel;

	[SerializeField]
	private TextMeshProUGUI freeSpinAmountLabel;

	[SerializeField]
	private TextMeshProUGUI costAmountLabel;

	[SerializeField]
	private TextMeshProUGUI expirationLabel;

	[SerializeField]
	private List<GameObject> randomCrewMemberPortraits;

	[SerializeField]
	private CrownExpGranter crownExpGranter;
}
