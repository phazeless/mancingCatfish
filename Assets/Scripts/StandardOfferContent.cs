using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandardOfferContent : HolidayOfferBehaviour
{
	private void Start()
	{
		string productId = ResourceManager.Instance.GetProductId(this.offer.ItemId);
		if (productId != null && this.costLbl != null)
		{
			this.costLbl.SetText(ResourceManager.Instance.GetMarketItemPriceAndCurrency(productId));
		}
		if (this.gemAmountLbl != null)
		{
			this.gemAmountLbl.SetVariableText(new string[]
			{
				this.gemAmountInOffer.ToString()
			});
		}
		if (this.item1AmountLbl != null)
		{
			this.item1AmountLbl.SetText("x" + this.itemInOffer1Amount.ToString());
		}
		if (this.item2AmountLbl != null)
		{
			this.item2AmountLbl.SetText("x" + this.itemInOffer2Amount.ToString());
		}
		if (this.crewImage != null)
		{
			this.crewImage.sprite = this.crewMemberInOffer.GetExtraInfo().Icon;
		}
		if (this.item1Image != null)
		{
			this.item1Image.sprite = this.itemInOffer1.Icon;
		}
		if (this.item2Image != null)
		{
			this.item2Image.sprite = this.itemInOffer2.Icon;
		}
		if (this.consumableAmountLbl != null && this.consumableInOffer != null)
		{
			this.consumableAmountLbl.SetVariableText(new string[]
			{
				this.consumableInOffer.Amount.ToString()
			});
		}
	}

	public override void OnBought()
	{
		ResourceChangeData gemChangeData = new ResourceChangeData(base.Id, this.iapPlacement.ToString(), this.gemAmountInOffer, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.PurchaseSpecialOffer);
		ResourceManager.Instance.GiveGems(this.gemAmountInOffer, gemChangeData);
		ResourceChangeData changeData = new ResourceChangeData(base.Id, this.iapPlacement.ToString(), 0, ResourceType.CrownExp, ResourceChangeType.Earn, ResourceChangeReason.PurchaseSpecialOffer);
		CrownExpGranterManager.Instance.Grant(this.iapPlacement, changeData);
		if (this.crewMemberInOffer != null)
		{
			PurchaseCrewMemberHandler.Instance.GetCrewMember(this.crewMemberInOffer, ResourceChangeReason.PurchaseSpecialOffer, 0);
		}
		if (this.itemInOffer1 != null)
		{
			this.itemInOffer1.ChangeItemAmount(this.itemInOffer1Amount, ResourceChangeReason.PurchaseSpecialOffer);
		}
		if (this.itemInOffer2 != null)
		{
			this.itemInOffer2.ChangeItemAmount(this.itemInOffer2Amount, ResourceChangeReason.PurchaseSpecialOffer);
		}
		if (this.consumableInOffer != null)
		{
			this.consumableInOffer.Grant(base.Id, ResourceChangeReason.PurchaseSpecialOffer);
		}
	}

	private void Update()
	{
		if (this.expireLbl != null)
		{
			this.expireLbl.SetVariableText(new string[]
			{
				FHelper.FromSecondsToDaysHoursMinutesSecondsFormatMaxTwo(this.offer.SecondsUntilExpiration)
			});
		}
	}

	[SerializeField]
	protected IAPPlacement iapPlacement = IAPPlacement.None;

	[SerializeField]
	private Skill crewMemberInOffer;

	[SerializeField]
	private Item itemInOffer1;

	[SerializeField]
	private int itemInOffer1Amount;

	[SerializeField]
	private Item itemInOffer2;

	[SerializeField]
	private int itemInOffer2Amount;

	[SerializeField]
	private int gemAmountInOffer;

	[SerializeField]
	private GrantableConsumable consumableInOffer;

	[SerializeField]
	protected TextMeshProUGUI expireLbl;

	[SerializeField]
	protected TextMeshProUGUI costLbl;

	[SerializeField]
	protected TextMeshProUGUI gemAmountLbl;

	[SerializeField]
	protected TextMeshProUGUI item1AmountLbl;

	[SerializeField]
	protected TextMeshProUGUI item2AmountLbl;

	[SerializeField]
	protected TextMeshProUGUI consumableAmountLbl;

	[SerializeField]
	protected Image crewImage;

	[SerializeField]
	protected Image item1Image;

	[SerializeField]
	protected Image item2Image;
}
