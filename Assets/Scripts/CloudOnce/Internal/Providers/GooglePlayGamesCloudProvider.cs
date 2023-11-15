using System;
using System.Collections;
using System.Runtime.CompilerServices;
using CloudOnce.Internal.Utils;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.OurUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Providers
{
	public sealed class GooglePlayGamesCloudProvider : CloudProviderBase<GooglePlayGamesCloudProvider>
	{
		public static bool DebugLogEnabled
		{
			get
			{
				return GooglePlayGames.OurUtils.Logger.DebugLogEnabled;
			}
			set
			{
				GooglePlayGames.OurUtils.Logger.DebugLogEnabled = value;
			}
		}

		public static bool IsGuestUserDefault { get; private set; }

		public override string PlayerID
		{
			get
			{
				return (!this.IsGpgsInitialized) ? string.Empty : PlayGamesPlatform.Instance.localUser.id;
			}
		}

		public override string PlayerDisplayName
		{
			get
			{
				return (!this.IsGpgsInitialized) ? string.Empty : PlayGamesPlatform.Instance.localUser.userName;
			}
		}

		public override Texture2D PlayerImage
		{
			get
			{
				return (!this.IsGpgsInitialized) ? Texture2D.whiteTexture : (this.playerImage ?? Texture2D.whiteTexture);
			}
		}

		public override bool IsSignedIn
		{
			get
			{
				return this.IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated();
			}
		}

		public bool CloudSaveInitialized { get; private set; }

		public override bool CloudSaveEnabled
		{
			get
			{
				return this.cloudSaveEnabled;
			}
			set
			{
				if (!this.CloudSaveInitialized)
				{
					UnityEngine.Debug.LogWarning("Cloud Save has not been initialized. Call Cloud.Initialize before attempting to set CloudSaveEnabled.");
					return;
				}
				this.cloudSaveEnabled = value;
			}
		}

		public bool IsGpgsInitialized { get; private set; }

		public override ICloudStorageProvider Storage { get; protected set; }

		public override void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true)
		{
			if (this.initializing)
			{
				return;
			}
			this.initializing = true;
			this.cloudSaveEnabled = activateCloudSave;
			PlayGamesClientConfiguration.Builder builder = new PlayGamesClientConfiguration.Builder();
			if (activateCloudSave)
			{
				builder.EnableSavedGames();
				this.CloudSaveInitialized = true;
			}
			PlayGamesPlatform.InitializeInstance(builder.Build());
			this.SubscribeOnAuthenticatedEvent();
			PlayGamesPlatform.DebugLogEnabled = false;
			UnityEngine.Debug.Log("PlayGamesPlatform debug logs disabled.");
			this.IsGpgsInitialized = true;
			if (!GooglePlayGamesCloudProvider.IsGuestUserDefault && autoSignIn)
			{
				UnityAction<bool> callback = delegate(bool arg0)
				{
					this.cloudOnceEvents.RaiseOnInitializeComplete();
					this.initializing = false;
				};
				this.SignIn(autoCloudLoad, callback);
			}
			else
			{
				if (GooglePlayGamesCloudProvider.IsGuestUserDefault && autoSignIn)
				{
					GooglePlayGames.OurUtils.Logger.d("Guest user mode active, ignoring auto sign-in. Please call SignIn directly.");
				}
				if (autoCloudLoad)
				{
					this.cloudOnceEvents.RaiseOnCloudLoadComplete(false);
				}
				this.cloudOnceEvents.RaiseOnInitializeComplete();
				this.initializing = false;
			}
		}

		public override void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null)
		{
			if (!this.IsGpgsInitialized)
			{
				UnityEngine.Debug.LogWarning("SignIn called, but Google Play Game Services has not been initialized. Ignoring call.");
				CloudOnceUtils.SafeInvoke<bool>(callback, false);
				return;
			}
			if (autoCloudLoad)
			{
				this.SetUpAutoCloudLoad();
			}
			GooglePlayGamesCloudProvider.IsGuestUserDefault = false;
			GooglePlayGames.OurUtils.Logger.d("Attempting to sign in to Google Play Game Services.");
			PlayGamesPlatform.Instance.Authenticate(delegate(bool success)
			{
				if (!success)
				{
					GooglePlayGames.OurUtils.Logger.w("Failed to sign in to Google Play Game Services.");
					bool flag;
					try
					{
						flag = (InternetConnectionUtils.GetConnectionStatus() != InternetConnectionStatus.Connected);
					}
					catch (NotSupportedException)
					{
						flag = (Application.internetReachability == NetworkReachability.NotReachable);
					}
					if (flag)
					{
						GooglePlayGames.OurUtils.Logger.d("Failure seems to be due to lack of Internet. Will try to connect again next time.");
					}
					else
					{
						GooglePlayGames.OurUtils.Logger.d("Must assume the failure is due to player opting out of the sign-in process, setting guest user as default");
						GooglePlayGamesCloudProvider.IsGuestUserDefault = true;
					}
					this.cloudOnceEvents.RaiseOnSignInFailed();
					if (autoCloudLoad)
					{
						this.cloudOnceEvents.RaiseOnCloudLoadComplete(false);
					}
				}
				CloudOnceUtils.SafeInvoke<bool>(callback, success);
			});
		}

		public override void SignOut()
		{
			GooglePlayGames.OurUtils.Logger.d("Signing out of Google Play Game Services.");
			PlayGamesPlatform.Instance.SignOut();
			this.ActivateGuestUserMode();
		}

		public override void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
		{
			if (!this.IsGpgsInitialized)
			{
				UnityEngine.Debug.LogWarning("LoadUsers called, but Google Play Game Services has not been initialized. Ignoring call.");
				CloudOnceUtils.SafeInvoke<IUserProfile[]>(callback, new IUserProfile[0]);
				return;
			}
			PlayGamesPlatform.Instance.LoadUsers(userIDs, callback);
		}

		public void InternalInit(CloudOnceEvents events)
		{
			this.cloudOnceEvents = events;
			this.Storage = new GooglePlayGamesCloudSaveWrapper(events);
			base.ServiceName = "Google Play Game Services";
		}

		public void ActivateGuestUserMode()
		{
			GooglePlayGamesCloudProvider.IsGuestUserDefault = true;
			this.cloudOnceEvents.RaiseOnSignedInChanged(false);
		}

		protected override void OnAwake()
		{
			GooglePlayGamesCloudProvider.IsGuestUserDefault = (PlayerPrefs.GetInt("GooglePlayWantsToUseGuest", 0) == 1);
		}

		protected override void OnOnDestroy()
		{
			PlayerPrefs.SetInt("GooglePlayWantsToUseGuest", (!GooglePlayGamesCloudProvider.IsGuestUserDefault) ? 0 : 1);
		}

		private static void UpdateAchievementsData(IAchievement[] achievements)
		{
			foreach (IAchievement achievement in achievements)
			{
				if (achievement != null && !string.IsNullOrEmpty(achievement.id))
				{
					bool flag = false;
					foreach (UnifiedAchievement unifiedAchievement in Achievements.All)
					{
						if (unifiedAchievement.ID == achievement.id)
						{
							unifiedAchievement.UpdateData(achievement.completed, achievement.percentCompleted, achievement.hidden);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
					}
				}
			}
		}

		private void SetUpAutoCloudLoad()
		{
			GooglePlayGamesCloudSaveWrapper googlePlayGamesCloudSaveWrapper = (GooglePlayGamesCloudSaveWrapper)this.Storage;
			googlePlayGamesCloudSaveWrapper.SubscribeToAuthenticationEvent();
		}

		private void SubscribeOnAuthenticatedEvent()
		{
			
		}

		private void OnAuthenticated()
		{
			PlayGamesHelperObject.RunOnGameThread(delegate
			{
				this.cloudOnceEvents.RaiseOnSignedInChanged(true);
				GooglePlayGames.OurUtils.Logger.d("Successfully signed in to Google Play Game Services.");
				GooglePlayGamesCloudProvider.IsGuestUserDefault = false;
				this.GetPlayerImage();
				if (Achievements.All.Length > 0)
				{
					PlayGamesPlatform instance = PlayGamesPlatform.Instance;
					
					instance.LoadAchievements(new Action<IAchievement[]>(GooglePlayGamesCloudProvider.UpdateAchievementsData));
				}
			});
		}

		private void GetPlayerImage()
		{
			string userImageUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
			if (!string.IsNullOrEmpty(userImageUrl))
			{
				base.StartCoroutine(this.DownloadPlayerImage(userImageUrl));
			}
		}

		private IEnumerator DownloadPlayerImage(string url)
		{
			WWW www = new WWW(url);
			yield return www;
			this.playerImage = www.texture;
			this.cloudOnceEvents.RaiseOnPlayerImageDownloaded(this.playerImage);
			yield break;
		}

		private const string guestPreferenceKey = "GooglePlayWantsToUseGuest";

		private CloudOnceEvents cloudOnceEvents;

		private bool cloudSaveEnabled = true;

		private Texture2D playerImage;

		private bool initializing;

		[CompilerGenerated]
		private static Action<IAchievement[]> _003C_003Ef__mg_0024cache0;
	}
}
