using System;
using UnityEngine;

public class IGNSpecialOfferDialog : InGameNotificationDialog<IGNSpecialOffer>
{
	protected override void OnAboutToOpen()
	{
		this.uiSpecialOfferItem.SetSpecialOffer(this.inGameNotification.SpecialOffer);
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
		this.customSize = new Vector2?(this.dialogBackgroundHolder.rect.size);
	}

	protected override void OnOpened()
	{
	}

	protected override void OnRemovedFromList()
	{
	}

	protected override void OnReturned()
	{
	}

	[SerializeField]
	private UISpecialOfferItem uiSpecialOfferItem;

	[SerializeField]
	private RectTransform dialogBackgroundHolder;
}
