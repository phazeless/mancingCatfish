using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class IGNHolidayOfferDialog : InGameNotificationDialog<IGNHolidayOffer>
{
	protected override void OnAboutToOpen()
	{
		if (this.instantiatedoffer == null)
		{
			this.instantiatedoffer = UnityEngine.Object.Instantiate<HolidayOfferBehaviour>(this.inGameNotification.Offer.BehaviourToInstantiate, this.parentForInstantiatedOffer);
			this.instantiatedoffer.Init(this.inGameNotification.Offer, new Action<HolidayOffer>(this.OnBought));
			this.instantiatedoffer.transform.localScale = new Vector3(0f, 0.5f, 1f);
			this.instantiatedoffer.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.3f).OnComplete(delegate
			{
				this.instantiatedoffer.transform.DOLocalMoveY(this.instantiatedoffer.transform.localPosition.y + 15f, 2f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
			});
			this.instantiatedoffer.transform.GetChild(0).gameObject.SetActive(false);
			AudioManager.Instance.OneShooter(this.SpecialSound, 1f);
		}
		if (this.expiresIn != null)
		{
			this.expiresIn.SetVariableText(new string[]
			{
				FHelper.FromSecondsToDaysHoursMinutesSecondsFormatMaxTwo(this.inGameNotification.Offer.SecondsUntilExpiration)
			});
		}
	}

	private void OnBought(HolidayOffer offer)
	{
		HolidayOfferManager.Instance.MarkOfferAsBought(offer);
		this.Close(true);
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
	}

	protected override void OnOpened()
	{
	}

	protected override void OnRemovedFromList()
	{
		if (this.instantiatedoffer != null)
		{
			this.instantiatedoffer.transform.DOKill(false);
		}
	}

	protected override void OnReturned()
	{
	}

	[SerializeField]
	private Transform parentForInstantiatedOffer;

	[SerializeField]
	private AudioClip SpecialSound;

	[SerializeField]
	private TextMeshProUGUI expiresIn;

	private HolidayOfferBehaviour instantiatedoffer;
}
