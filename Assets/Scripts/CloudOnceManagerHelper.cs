using System;
using System.Runtime.CompilerServices;
using CloudOnce;
using CloudOnce.CloudPrefs;
using UnityEngine;
using UnityEngine.Events;

public static class CloudOnceManagerHelper
{
	private static bool HasSavedDataOnCloud
	{
		get
		{
			return CloudOnceManagerHelper.savedDataOnCloud != null && !string.IsNullOrEmpty(CloudOnceManagerHelper.savedDataOnCloud.Value);
		}
	}

	public static bool IsLoggedIntoCloudService
	{
		get
		{
			return Cloud.IsSignedIn;
		}
	}

	public static bool IsLoginInProgress
	{
		get
		{
			return CloudOnceManagerHelper.IsLoggedIntoCloudService && CloudOnceManagerHelper.isLoginInProgress;
		}
	}

	public static bool HasInitializedCloudService { get; private set; }

	public static bool HasRejectedAutoLoginCloudService
	{
		get
		{
			return PlayerPrefs.GetInt("HAS_REJECTED_AUTO_LOGIN_CLOUD_SERVICE", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("HAS_REJECTED_AUTO_LOGIN_CLOUD_SERVICE", (!value) ? 0 : 1);
		}
	}

	public static bool ForcedLoadFromCloud
	{
		get
		{
			return PlayerPrefs.GetInt("FORCED_LOAD_FROM_CLOUD", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("FORCED_LOAD_FROM_CLOUD", (!value) ? 0 : 1);
		}
	}

	public static void Init(CloudString cloudString)
	{
		CloudOnceManagerHelper.savedDataOnCloud = cloudString;
		if (CloudOnceManagerHelper._003C_003Ef__mg_0024cache0 == null)
		{
			CloudOnceManagerHelper._003C_003Ef__mg_0024cache0 = new UnityAction(CloudOnceManagerHelper.Cloud_OnInitializeComplete);
		}
		Cloud.OnInitializeComplete += CloudOnceManagerHelper._003C_003Ef__mg_0024cache0;
	}

	private static void Cloud_OnInitializeComplete()
	{
		UnityEngine.Debug.Log("ABC: Cloud_OnInitializeComplete, IsLoggedIn: " + Cloud.IsSignedIn);
		CloudOnceManagerHelper.HasInitializedCloudService = true;
		CloudOnceManagerHelper.HasRejectedAutoLoginCloudService = !Cloud.IsSignedIn;
		UnityEngine.Debug.Log("ABC: Cloud_OnInitializeComplete, HasRejectedCloudService: " + CloudOnceManagerHelper.HasRejectedAutoLoginCloudService);
	}

	public static void Login(Action<bool, bool> onLoginAndLoadedCloudData)
	{
		CloudOnceManagerHelper.isLoginInProgress = true;
		Cloud.SignIn(false, delegate(bool didLogin)
		{
			UnityEngine.Debug.Log("ABC: DidLogin: " + didLogin);
			CloudOnceManagerHelper.HasRejectedAutoLoginCloudService = !didLogin;
			CloudOnceManagerHelper.InternalLoadFromCloud(didLogin, onLoginAndLoadedCloudData);
		});
	}

	private static void InternalLoadFromCloud(bool didLoginSuccessfully, Action<bool, bool> onLoginAndLoadedCloudData)
	{
		Cloud.OnCloudLoadComplete -= CloudOnceManagerHelper.onCloudLoadComplete;
		CloudOnceManagerHelper.onCloudLoadComplete = delegate(bool didLoadFromCloudSuccessfully)
		{
			Cloud.OnCloudLoadComplete -= CloudOnceManagerHelper.onCloudLoadComplete;
			UnityEngine.Debug.Log("ABC: didLoadFromCloudSuccessfully: " + didLoadFromCloudSuccessfully);
			CloudOnceManagerHelper.isLoginInProgress = false;
			if (onLoginAndLoadedCloudData != null)
			{
				onLoginAndLoadedCloudData((didLoginSuccessfully && didLoadFromCloudSuccessfully) || Application.isEditor, CloudOnceManagerHelper.HasSavedDataOnCloud);
			}
		};
		Cloud.OnCloudLoadComplete += CloudOnceManagerHelper.onCloudLoadComplete;
		Cloud.Storage.Load();
	}

	public static void Logout()
	{
		CloudOnceManagerHelper.HasRejectedAutoLoginCloudService = true;
		Cloud.SignOut();
	}

	private static CloudString savedDataOnCloud;

	private static UnityAction<bool> onCloudLoadComplete;

	private static bool isLoginInProgress;

	[CompilerGenerated]
	private static UnityAction _003C_003Ef__mg_0024cache0;
}
