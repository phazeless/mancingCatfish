using System;
using TMPro;
using UnityEngine;

public class FireworkForGemsOfferContent : HolidayOfferBehaviour
{
	private void Start()
	{
		this.gemCostAmountLbl.SetVariableText(new string[]
		{
			this.offer.GemCost.ToString()
		});
		this.fireworkAmountLbl.SetVariableText(new string[]
		{
			this.fireworkAmount.ToString()
		});
	}

	public override void OnBought()
	{
		ConsumableManager.Instance.Grant(this.fireworkConsumableToGrant, this.fireworkAmount, ResourceChangeReason.PurchaseFireworkPack, true);
		this.rocketItemPurchaseTween.Open(this.fireworkAmount);
	}

	protected override void OnBuyWithGemsFailed()
	{
		ShopScreen.Instance.AnimateToGemPack();
	}

	[SerializeField]
	private FireworkConsumable fireworkConsumableToGrant;

	[SerializeField]
	private TextMeshProUGUI fireworkAmountLbl;

	[SerializeField]
	private TextMeshProUGUI gemCostAmountLbl;

	[SerializeField]
	private RocketItemPurchaseConfirmTween rocketItemPurchaseTween;

	[SerializeField]
	private int fireworkAmount;
}
