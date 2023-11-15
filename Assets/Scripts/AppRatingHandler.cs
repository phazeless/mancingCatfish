using System;
using UnityEngine;

public class AppRatingHandler : MonoBehaviour
{
	public static AppRatingHandler Instance { get; private set; }

	private void Awake()
	{
		AppRatingHandler.Instance = this;
		this.showReviewOnQuestComplete.OnQuestClaimed += this.ShowReviewOnQuestComplete_OnQuestClaimed;
	}

	private void ShowReviewOnQuestComplete_OnQuestClaimed(Quest obj)
	{
		InGameNotificationManager.Instance.Create<IGNReview>(new IGNReview());
	}

	public void OpenAppRatingPage()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Application.OpenURL("market://details?id=" + this.androidAppId);
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Application.OpenURL("itms-apps://itunes.apple.com/app/apple-store/id" + this.iOSAppId + "?mt=8");
		}
	}

	[SerializeField]
	private string androidAppId;

	[SerializeField]
	private string iOSAppId;

	[SerializeField]
	private Quest showReviewOnQuestComplete;
}
