using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IGNRandomRewardDialog : InGameNotificationDialog<IGNFishValueBonus>
{
	protected override void OnOpened()
	{
		this.icon.interactable = false;
		this.someExampleButtons.SetActive(true);
	}

	protected override void OnAboutToReturn()
	{
		this.icon.interactable = true;
		this.someExampleButtons.SetActive(false);
	}

	protected override void OnRemovedFromList()
	{
	}

	protected override void OnAboutToOpen()
	{
	}

	protected override void OnReturned()
	{
	}

	private void Update()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
	}

	[SerializeField]
	private Button icon;

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private GameObject someExampleButtons;
}
