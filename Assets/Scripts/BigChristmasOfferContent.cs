using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BigChristmasOfferContent : HolidayOfferBehaviour
{
	private void Start()
	{
		string productId = ResourceManager.Instance.GetProductId(this.offer.ItemId);
		this.costLbl.SetText(ResourceManager.Instance.GetMarketItemPriceAndCurrency(productId));
		this.gemAmountLbl.SetVariableText(new string[]
		{
			this.gemAmountInOffer.ToString()
		});
		this.item1AmountLbl.SetText("x" + this.itemInOffer1Amount.ToString());
		this.item2AmountLbl.SetText("x" + this.itemInOffer2Amount.ToString());
		this.crewImage.sprite = this.crewMemberInOffer.GetExtraInfo().Icon;
		this.item1Image.sprite = this.itemInOffer1.Icon;
		this.item2Image.sprite = this.itemInOffer2.Icon;
	}

	public override void OnBought()
	{
		PurchaseCrewMemberHandler.Instance.GetCrewMember(this.crewMemberInOffer, ResourceChangeReason.PurchaseSpecialOffer, 0);
		this.itemInOffer1.ChangeItemAmount(this.itemInOffer1Amount, ResourceChangeReason.PurchaseSpecialOffer);
		this.itemInOffer2.ChangeItemAmount(this.itemInOffer2Amount, ResourceChangeReason.PurchaseSpecialOffer);
		ResourceChangeData gemChangeData = new ResourceChangeData(base.Id, "ChristmasOfferHollyPack", this.gemAmountInOffer, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.PurchaseSpecialOffer);
		ResourceManager.Instance.GiveGems(this.gemAmountInOffer, gemChangeData);
		ResourceChangeData changeData = new ResourceChangeData(base.Id, "ChristmasOfferHollyPack", 0, ResourceType.CrownExp, ResourceChangeType.Earn, ResourceChangeReason.PurchaseSpecialOffer);
		CrownExpGranterManager.Instance.Grant(IAPPlacement.SpecialOfferChristmasOffer, changeData);
	}

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
	protected Image crewImage;

	[SerializeField]
	protected Image item1Image;

	[SerializeField]
	protected Image item2Image;

	private const string contentName_christmasPackWithHolly = "ChristmasOfferHollyPack";
}
