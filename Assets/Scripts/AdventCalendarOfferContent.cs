using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class AdventCalendarOfferContent : HolidayOfferBehaviour
{
	private void Awake()
	{
		base.Init(this.holidayOfferModel, null);
		string text = this.minGemAmountInOffer + "-" + this.maxGemAmountInOffer;
		this.gemAmountLbl.SetVariableText(new string[]
		{
			text
		});
		string text2 = this.minItemAmountInOffer + "-" + this.maxItemAmountInOffer;
		this.itemAmountLbl.SetVariableText(new string[]
		{
			text2
		});
	}

	private void Start()
	{
		string productId = ResourceManager.Instance.GetProductId(this.holidayOfferModel.ItemId);
		this.costLbl.SetText(ResourceManager.Instance.GetMarketItemPriceAndCurrency(productId));
	}

	public override void OnBought()
	{
		int num = UnityEngine.Random.Range(this.minItemAmountInOffer, this.maxItemAmountInOffer + 1);
		int num2 = UnityEngine.Random.Range(this.minGemAmountInOffer, this.maxGemAmountInOffer + 1);
		int num3 = (int)Mathf.Ceil((float)num * 0.625f);
		int num4 = (int)Mathf.Round((float)num * 0.25f);
		int num5 = (int)Mathf.Round((float)num * 0.125f);
		base.transform.DOScale(0f, 0.7f).SetEase(Ease.InBack);
		DailyGiftContent dailyGiftContent = new DailyGiftContent();
		dailyGiftContent.Items.Add(this.PotentialItemsInOffer[0], num3);
		dailyGiftContent.Items.Add(this.PotentialItemsInOffer[1], num4);
		dailyGiftContent.Items.Add(this.PotentialItemsInOffer[2], num5);
		dailyGiftContent.Gems = num2;
		this.PotentialItemsInOffer[0].ChangeItemAmount(num3, ResourceChangeReason.ChristmasCalendarDailyReward);
		this.PotentialItemsInOffer[1].ChangeItemAmount(num4, ResourceChangeReason.ChristmasCalendarDailyReward);
		this.PotentialItemsInOffer[2].ChangeItemAmount(num5, ResourceChangeReason.ChristmasCalendarDailyReward);
		ResourceChangeData gemChangeData = new ResourceChangeData(base.Id, "OfferInsideAdventCalendar", num2, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.PurchaseSpecialOffer);
		ResourceManager.Instance.GiveGems(num2, gemChangeData);
		ResourceChangeData changeData = new ResourceChangeData(base.Id, "OfferInsideAdventCalendar", 0, ResourceType.CrownExp, ResourceChangeType.Earn, ResourceChangeReason.PurchaseSpecialOffer);
		CrownExpGranterManager.Instance.Grant(IAPPlacement.SpecialOffer, changeData);
		this.presentBehaviour.OpenOfferPresent(dailyGiftContent);
	}

	public void SetPreviewUI()
	{
		this.itemContainDescription.localScale = Vector3.zero;
		this.itemContainDescription.localEulerAngles = new Vector3(0f, 0f, 5f);
		this.itemContainDescription.DOScale(1f, 0.8f).SetEase(Ease.OutElastic);
		this.itemContainDescription.DORotate(Vector3.zero, 0.5f, RotateMode.Fast).SetEase(Ease.OutElastic);
		int min = (int)Mathf.Ceil((float)this.minItemAmountInOffer * 0.625f);
		int min2 = (int)Mathf.Ceil((float)this.minItemAmountInOffer * 0.25f);
		int min3 = (int)Mathf.Ceil((float)this.minItemAmountInOffer * 0.125f);
		int max = (int)Mathf.Ceil((float)this.maxItemAmountInOffer * 0.625f);
		int max2 = (int)Mathf.Ceil((float)this.maxItemAmountInOffer * 0.25f);
		int max3 = (int)Mathf.Ceil((float)this.maxItemAmountInOffer * 0.125f);
		foreach (Item item in this.PotentialItemsInOffer)
		{
			ItemPreview itemPreview = UnityEngine.Object.Instantiate<ItemPreview>(this.previewPrefab, this.itemContainDescription);
			if (item.Rarity == Rarity.Common)
			{
				itemPreview.SetPreview(item, min, max);
			}
			if (item.Rarity == Rarity.Rare)
			{
				itemPreview.SetPreview(item, min2, max2);
			}
			if (item.Rarity == Rarity.Epic)
			{
				itemPreview.SetPreview(item, min3, max3);
			}
		}
	}

	private void OnDestroy()
	{
		base.transform.DOKill(false);
		this.itemContainDescription.DOKill(false);
	}

	[SerializeField]
	private HolidayOffer holidayOfferModel;

	[SerializeField]
	private ChristmasPresentOpening presentBehaviour;

	[SerializeField]
	private TextMeshProUGUI costLbl;

	[SerializeField]
	private TextMeshProUGUI gemAmountLbl;

	[SerializeField]
	private TextMeshProUGUI itemAmountLbl;

	[SerializeField]
	private int minGemAmountInOffer;

	[SerializeField]
	private int maxGemAmountInOffer;

	[SerializeField]
	private List<Item> PotentialItemsInOffer = new List<Item>();

	[SerializeField]
	private int minItemAmountInOffer;

	[SerializeField]
	private int maxItemAmountInOffer;

	[SerializeField]
	private Transform itemContainDescription;

	[SerializeField]
	private ItemPreview previewPrefab;

	private List<Item> ItemsInOffer = new List<Item>();

	private const string contentName_adventCalendarOffer = "OfferInsideAdventCalendar";
}
