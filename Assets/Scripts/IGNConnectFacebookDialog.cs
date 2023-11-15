using System;
using UnityEngine;

public class IGNConnectFacebookDialog : InGameNotificationDialog<IGNConnectFacebook>
{
	protected override void OnAboutToOpen()
	{
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

	public void RemoveDialog()
	{
		this.inGameNotification.OverrideClearable = true;
		this.Close(true);
	}

	[SerializeField]
	private RectTransform dialogBackgroundHolder;
}
