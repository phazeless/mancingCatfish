using System;
using TMPro;
using UnityEngine;

public class IGNInfoDialog : InGameNotificationDialog<IGNInfo>
{
	protected override void OnAboutToOpen()
	{
		this.UpdateUI(this.inGameNotification.Info.MainTitle, this.inGameNotification.Info.SubTitle, this.inGameNotification.Info.Content, "Okay");
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

	public void UpdateUI(string title, string header, string info, string button)
	{
		this.titleLabel.text = title;
		this.headerLabel.text = header;
		this.infoLabel.text = info;
		this.buttonLabel.text = button;
	}

	[SerializeField]
	private RectTransform dialogBackgroundHolder;

	[SerializeField]
	private TextMeshProUGUI titleLabel;

	[SerializeField]
	private TextMeshProUGUI headerLabel;

	[SerializeField]
	private TextMeshProUGUI infoLabel;

	[SerializeField]
	private TextMeshProUGUI buttonLabel;
}
