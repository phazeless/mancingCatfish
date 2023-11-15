using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IGNItemChestDialog : InGameNotificationDialog<IGNItemChest>
{
	protected IGNItemChestDialog()
	{
	}

	protected override void OnAboutToOpen()
	{
		this.UpdateUI();
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
		this.customSize = new Vector2?(this.dialogBackgroundHolder.rect.size);
		RecievedChest chest = this.inGameNotification.Chest;
		ItemChest chestById = ChestManager.Instance.GetChestById(chest.ChestId);
		if (chestById != null)
		{
			this.icon.sprite = chestById.Icon;
		}
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

	public void OpenChest()
	{
		RecievedChest chest = this.inGameNotification.Chest;
		ItemChest chestById = ChestManager.Instance.GetChestById(chest.ChestId);
		int elapsedSecondsSinceReceived = chest.GetElapsedSecondsSinceReceived();
		int costToUnlock = chestById.GetCostToUnlock((float)elapsedSecondsSinceReceived);
		ResourceChangeData changeData = new ResourceChangeData(chestById.Id, chestById.ChestName, costToUnlock, ResourceType.Gems, ResourceChangeType.Spend, ResourceChangeReason.OpenItemChestEarly);
		if (ResourceManager.Instance.TakeGems(costToUnlock, changeData))
		{
			ChestManager.Instance.OpenChest(this.inGameNotification.Chest);
			this.inGameNotification.OverrideClearable = true;
			this.Close(true);
		}
	}

	private void UpdateUI()
	{
		RecievedChest chest = this.inGameNotification.Chest;
		ItemChest chestById = ChestManager.Instance.GetChestById(chest.ChestId);
		this.boxes[chestById.Tier].SetActive(true);
		this.countLabel.SetVariableText(new string[]
		{
			chestById.MinItemAmount.ToString(),
			chestById.MaxItemAmount.ToString()
		});
		if (chestById != null)
		{
			int elapsedSecondsSinceReceived = chest.GetElapsedSecondsSinceReceived();
			int costToUnlock = chestById.GetCostToUnlock((float)elapsedSecondsSinceReceived);
			int secondsUntilUnlocked = chestById.GetSecondsUntilUnlocked(elapsedSecondsSinceReceived);
			this.unlockButtonLabel.SetVariableText(new string[]
			{
				costToUnlock.ToString()
			});
			this.titleLabel.SetText(chestById.ChestName);
			this.unlockTimerLabel.SetVariableText(new string[]
			{
				FHelper.FromSecondsToHoursMinutesSecondsFormat((float)secondsUntilUnlocked)
			});
			this.unlockButton.interactable = (ResourceManager.Instance.GetResourceAmount(ResourceType.Gems) >= (long)costToUnlock);
		}
	}

	private void CheckIfUnlockForFree()
	{
		RecievedChest chest = this.inGameNotification.Chest;
		ItemChest chestById = ChestManager.Instance.GetChestById(chest.ChestId);
		if (chestById != null)
		{
			int elapsedSecondsSinceReceived = chest.GetElapsedSecondsSinceReceived();
			int secondsUntilUnlocked = chestById.GetSecondsUntilUnlocked(elapsedSecondsSinceReceived);
			if (secondsUntilUnlocked <= 0)
			{
				this.isUnlockableForFree = true;
				this.ignBackground.color = new Color(0.976f, 0.698f, 0.2f);
			}
		}
	}

	private void Update()
	{
		if (this.inGameNotification != null && this.inGameNotification.Chest != null)
		{
			if (base.IsOpen)
			{
				this.UpdateUI();
			}
			if (!this.isUnlockableForFree)
			{
				this.CheckIfUnlockForFree();
			}
		}
	}

	[SerializeField]
	private RectTransform dialogBackgroundHolder;

	[SerializeField]
	private Button unlockButton;

	[SerializeField]
	private TextMeshProUGUI unlockButtonLabel;

	[SerializeField]
	private TextMeshProUGUI titleLabel;

	[SerializeField]
	private TextMeshProUGUI unlockTimerLabel;

	[SerializeField]
	private GameObject[] boxes;

	[SerializeField]
	private TextMeshProUGUI countLabel;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image ignBackground;

	private bool isUnlockableForFree;
}
