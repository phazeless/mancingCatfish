using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MaxSdkCallbacks : MonoBehaviour
{
	public static MaxSdkCallbacks Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<MaxSdkBase.SdkConfiguration> OnSdkInitializedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action OnVariablesUpdatedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnBannerAdLoadedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, int> OnBannerAdLoadFailedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnBannerAdClickedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnBannerAdExpandedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnBannerAdCollapsedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnInterstitialLoadedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, int> OnInterstitialLoadFailedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnInterstitialHiddenEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnInterstitialDisplayedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, int> OnInterstitialAdFailedToDisplayEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnInterstitialClickedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnRewardedAdLoadedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, int> OnRewardedAdLoadFailedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnRewardedAdDisplayedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnRewardedAdHiddenEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> OnRewardedAdClickedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, int> OnRewardedAdFailedToDisplayEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, MaxSdkBase.Reward> OnRewardedAdReceivedRewardEvent;

	private void Awake()
	{
		if (MaxSdkCallbacks.Instance == null)
		{
			MaxSdkCallbacks.Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	public void ForwardEvent(string eventPropsStr)
	{
		IDictionary<string, string> dictionary = MaxSdkUtils.PropsStringToDict(eventPropsStr);
		string text = dictionary["name"];
		if (text == "OnSdkInitializedEvent")
		{
			string value = dictionary["consentDialogState"];
			MaxSdkBase.SdkConfiguration sdkConfiguration = new MaxSdkBase.SdkConfiguration();
			if ("1".Equals(value))
			{
				sdkConfiguration.ConsentDialogState = MaxSdkBase.ConsentDialogState.Applies;
			}
			else if ("2".Equals(value))
			{
				sdkConfiguration.ConsentDialogState = MaxSdkBase.ConsentDialogState.DoesNotApply;
			}
			else
			{
				sdkConfiguration.ConsentDialogState = MaxSdkBase.ConsentDialogState.Unknown;
			}
			MaxSdkCallbacks.InvokeEvent<MaxSdkBase.SdkConfiguration>(MaxSdkCallbacks.OnSdkInitializedEvent, sdkConfiguration);
		}
		else if (text == "OnVariablesUpdatedEvent")
		{
			MaxSdkCallbacks.InvokeEvent(MaxSdkCallbacks.OnVariablesUpdatedEvent);
		}
		else
		{
			string text2 = dictionary["adUnitId"];
			if (text == "OnBannerAdLoadedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnBannerAdLoadedEvent, text2);
			}
			else if (text == "OnBannerAdLoadFailedEvent")
			{
				int param = 0;
				int.TryParse(dictionary["errorCode"], out param);
				MaxSdkCallbacks.InvokeEvent<string, int>(MaxSdkCallbacks.OnBannerAdLoadFailedEvent, text2, param);
			}
			else if (text == "OnBannerAdClickedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnBannerAdClickedEvent, text2);
			}
			else if (text == "OnBannerAdExpandedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnBannerAdExpandedEvent, text2);
			}
			else if (text == "OnBannerAdCollapsedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnBannerAdCollapsedEvent, text2);
			}
			else if (text == "OnInterstitialLoadedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnInterstitialLoadedEvent, text2);
			}
			else if (text == "OnInterstitialLoadFailedEvent")
			{
				int param2 = 0;
				int.TryParse(dictionary["errorCode"], out param2);
				MaxSdkCallbacks.InvokeEvent<string, int>(MaxSdkCallbacks.OnInterstitialLoadFailedEvent, text2, param2);
			}
			else if (text == "OnInterstitialHiddenEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnInterstitialHiddenEvent, text2);
			}
			else if (text == "OnInterstitialDisplayedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnInterstitialDisplayedEvent, text2);
			}
			else if (text == "OnInterstitialAdFailedToDisplayEvent")
			{
				int param3 = 0;
				int.TryParse(dictionary["errorCode"], out param3);
				MaxSdkCallbacks.InvokeEvent<string, int>(MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent, text2, param3);
			}
			else if (text == "OnInterstitialClickedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnInterstitialClickedEvent, text2);
			}
			else if (text == "OnRewardedAdLoadedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnRewardedAdLoadedEvent, text2);
			}
			else if (text == "OnRewardedAdLoadFailedEvent")
			{
				int param4 = 0;
				int.TryParse(dictionary["errorCode"], out param4);
				MaxSdkCallbacks.InvokeEvent<string, int>(MaxSdkCallbacks.OnRewardedAdLoadFailedEvent, text2, param4);
			}
			else if (text == "OnRewardedAdDisplayedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnRewardedAdDisplayedEvent, text2);
			}
			else if (text == "OnRewardedAdHiddenEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnRewardedAdHiddenEvent, text2);
			}
			else if (text == "OnRewardedAdClickedEvent")
			{
				MaxSdkCallbacks.InvokeEvent<string>(MaxSdkCallbacks.OnRewardedAdClickedEvent, text2);
			}
			else if (text == "OnRewardedAdFailedToDisplayEvent")
			{
				int param5 = 0;
				int.TryParse(dictionary["errorCode"], out param5);
				MaxSdkCallbacks.InvokeEvent<string, int>(MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent, text2, param5);
			}
			else if (text == "OnRewardedAdReceivedRewardEvent")
			{
				MaxSdkBase.Reward param6 = new MaxSdkBase.Reward
				{
					Label = dictionary["rewardLabel"]
				};
				int.TryParse(dictionary["rewardAmount"], out param6.Amount);
				MaxSdkCallbacks.InvokeEvent<string, MaxSdkBase.Reward>(MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent, text2, param6);
			}
			else
			{
				UnityEngine.Debug.LogWarning("[AppLovin MAX] Unknown MAX Ads event fired: " + text);
			}
		}
	}

	private static void InvokeEvent(Action evt)
	{
		if (!MaxSdkCallbacks.CanInvokeEvent(evt))
		{
			return;
		}
		UnityEngine.Debug.Log("[AppLovin MAX] Invoking event: " + evt);
		evt();
	}

	private static void InvokeEvent<T>(Action<T> evt, T param)
	{
		if (!MaxSdkCallbacks.CanInvokeEvent(evt))
		{
			return;
		}
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"[AppLovin MAX] Invoking event: ",
			evt,
			". Param: ",
			param
		}));
		evt(param);
	}

	private static void InvokeEvent<T1, T2>(Action<T1, T2> evt, T1 param1, T2 param2)
	{
		if (!MaxSdkCallbacks.CanInvokeEvent(evt))
		{
			return;
		}
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"[AppLovin MAX] Invoking event: ",
			evt,
			". Params: ",
			param1,
			", ",
			param2
		}));
		evt(param1, param2);
	}

	private static bool CanInvokeEvent(Delegate evt)
	{
		if (evt == null)
		{
			return false;
		}
		if (evt.GetInvocationList().Length > 5)
		{
			UnityEngine.Debug.LogWarning("[AppLovin MAX] Ads Event (" + evt + ") has over 5 subscribers. Please make sure you are properly un-subscribing to actions!!!");
		}
		return true;
	}
}
