using System;
using System.Diagnostics;
using ACE.Ads;
using UnityEngine;
using System.Collections;

public class FishAdManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnAdAvailable;

	public static FishAdManager Instance { get; private set; }

	public bool IsAdActive { get; private set; }

	private bool HasReachedStartUpThresholdForCrossPromo
	{
		get
		{
			return this.startUpCount > 2 && this.startUpCount % 3 == 0;
		}
	}

	private void InitIncreaseAndSaveStartupCount()
	{
		this.startUpCount = PlayerPrefs.GetInt("KEY_START_UP_COUNT", this.startUpCount);
		this.startUpCount++;
		PlayerPrefs.SetInt("KEY_START_UP_COUNT", this.startUpCount);
	}

	private void Awake()
	{
		FishAdManager.Instance = this;
		AFKManager.Instance.OnUserReturnCallback += this.Instance_OnUserReturnCallback;
	}

	private void HandleCrossPromoVideoLogic(bool fromAppRestart, float afkTimeInSeconds)
	{
		UnityEngine.Debug.Log("ABC: HandleCrossPromoVideoLogic, was afk for: " + afkTimeInSeconds);
		if (fromAppRestart || afkTimeInSeconds > 120f)
		{
			this.InitIncreaseAndSaveStartupCount();
			if (this.HasReachedStartUpThresholdForCrossPromo)
			{
				UnityEngine.Debug.Log("ABC: Has reached threshold for Cross Promo, will init and show!");
				
			}
		}
	}

	private void Start()
	{
		ScreenManager.Instance.OnScreenTransitionStarted += this.Instance_OnScreenTransitionStarted;
	}

	private void Instance_OnUserReturnCallback(bool fromAppRestart, DateTime time, float afkTimeInSeconds)
	{
		this.HandleCrossPromoVideoLogic(fromAppRestart, afkTimeInSeconds);
	}

	private void OnAdEvent(AdResponse adResponse)
	{
		if (adResponse.Type == AdResponse.AdResponseType.DidCache && this.OnAdAvailable != null)
		{
			this.OnAdAvailable();
		}
	}

	private void Instance_OnScreenTransitionStarted(ScreenManager.Screen to, ScreenManager.Screen from)
	{
		if (to == ScreenManager.Screen.Shop)
		{
			AdPlacement adPlacement = AdPlacement.SpinWheel;
			
		}
	}

	public void CacheVideo()
	{
	}

	private void InternalShowVideo(Action<AdResponse> onShowVideoCallback, bool isFinalTry)
	{
		
		this.IsAdActive = true;
		UIIAPPendingBlocker.Instance.Show();
		FishAdManager.adManager.Show<VideoAdFormat>(delegate(AdResponse adResponse)
		{
			IsAdActive = false;
			if (adResponse.DidFail)
			{
				FHelper.HasInternet(delegate(bool hasInternet)
				{
					if (hasInternet)
					{
						if (isFinalTry)
						{
							UIIAPPendingBlocker.Instance.Hide();
							noAdsDialog.Show(UINoAdsDialog.Reason.NoAds, delegate(bool didUseGems)
							{
								FishAdManager.adManager.Cache<VideoAdFormat>(true);
								AdResponse obj = new AdResponse.AdResponseBuilder(AdResponse.AdResponseType.DidFinish).SetDidComplete(didUseGems).Build();
								if (onShowVideoCallback != null)
								{
									onShowVideoCallback(obj);
								}
							});
						}
						else
						{
							RunAfterDelay(3f, delegate()
							{
								InternalShowVideo(onShowVideoCallback, true);
							});
						}
					}
					else
					{
						UIIAPPendingBlocker.Instance.Hide();
						noAdsDialog.Show(UINoAdsDialog.Reason.NoInternet, delegate(bool didUseGems)
						{
							if (onShowVideoCallback != null)
							{
								onShowVideoCallback(adResponse);
							}
						});
					}
				});
			}
			else
			{
				UIIAPPendingBlocker.Instance.Hide();
				bool flag = adResponse.Type == AdResponse.AdResponseType.DidFinish;
				if (flag)
				{
					FishAdManager.adManager.Cache<VideoAdFormat>(true);
				}
				if (onShowVideoCallback != null)
				{
					onShowVideoCallback(adResponse);
				}
			}
		});
	}

   IEnumerator RunAfterDelay(float delay,Action showVideo)
    {
        yield return new WaitForSeconds(delay);
        showVideo();
    }


    private const int SHOW_CROSS_PROMO_NEW_SESSION_THRESHOLD_SECONDS = 120;

	private static IAdManager adManager;

	[SerializeField]
	private UINoAdsDialog noAdsDialog;

	private const string KEY_START_UP_COUNT = "KEY_START_UP_COUNT";

	private int startUpCount;
}
