using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IGNReviewDialog : InGameNotificationDialog<IGNReview>
{
	protected override void OnAboutToOpen()
	{
	}

	protected override void OnAboutToReturn()
	{
		this.girlHolder.SetActive(false);
	}

	protected override void OnIGNHasBeenSet()
	{
		this.customSize = new Vector2?(this.dialogBackgroundHolder.rect.size);
	}

	protected override void OnOpened()
	{
		this.girlHolder.SetActive(true);
	}

	protected override void OnRemovedFromList()
	{
	}

	protected override void OnReturned()
	{
	}

	public void DoReview()
	{
		int amount = 5;
		ResourceChangeData gemChangeData = new ResourceChangeData("contentId_afterReview", null, amount, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.AfterReviewReward);
		GemGainVisual.Instance.GainGems(amount, Vector2.zero, gemChangeData);
		this.inGameNotification.OverrideClearable = true;
		this.Close(true);
		AppRatingHandler.Instance.OpenAppRatingPage();
	}

	public void NoReview()
	{
		this.inGameNotification.OverrideClearable = true;
		this.Close(true);
	}

	public void Select(int index)
	{
		bool interactable = false;
		for (int i = 0; i < this.stars.Count; i++)
		{
			bool flag = i < index;
			this.stars[i].color = ((!flag) ? Color.gray : Color.yellow);
			if (flag)
			{
				interactable = true;
			}
		}
		this.okayButton.interactable = interactable;
	}

	private const string contentId_afterReview = "contentId_afterReview";

	[SerializeField]
	private List<Image> stars = new List<Image>();

	[SerializeField]
	private RectTransform dialogBackgroundHolder;

	[SerializeField]
	private Button okayButton;

	[SerializeField]
	private GameObject girlHolder;
}
