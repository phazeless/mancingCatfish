using System;
using TMPro;
using UnityEngine;

public class IGNWheelBonus : InGameNotificationDialog<IGNFishValueBonus>
{
	public void GoToShop()
	{
		this.Close(false);
		ShopScreen.Instance.GoToScreen(delegate
		{
			WheelBehaviour.Instance.WatchAdAndSpinWheel(AdPlacement.SpinWheelDialog);
		});
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void OnAboutToOpen()
	{
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
		this.customSize = new Vector2?(this.dialogBackgroundHolder.rect.size);
		this.UpdateUI();
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

	private void Update()
	{
		if (!this.inGameNotification.IsReady)
		{
			return;
		}
		if (this.inGameNotification.HasChanged)
		{
			this.UpdateUI();
			this.inGameNotification.HasChanged = false;
		}
		if (base.IsOpen)
		{
			this.timerLabel.SetVariableText(new string[]
			{
				FHelper.FromSecondsToHoursMinutesSecondsFormat(this.inGameNotification.TotalSecondsLeftOnDuration)
			});
		}
	}

	private void UpdateUI()
	{
		if (this.inGameNotification.IsReady)
		{
			this.bonusLabelDialog.SetVariableText(new string[]
			{
				this.inGameNotification.CurrentBonus.ToString()
			});
			this.bonusLabelIcon.SetVariableText(new string[]
			{
				this.inGameNotification.CurrentBonus.ToString()
			});
			if (this.inGameNotification.CurrentBonus == 4)
			{
				this.quadrupleParticles.SetActive(true);
			}
		}
	}

	[SerializeField]
	private TextMeshProUGUI bonusLabelIcon;

	[SerializeField]
	private TextMeshProUGUI bonusLabelDialog;

	[SerializeField]
	private TextMeshProUGUI timerLabel;

	[SerializeField]
	private RectTransform dialogBackgroundHolder;

	[SerializeField]
	private GameObject quadrupleParticles;
}
