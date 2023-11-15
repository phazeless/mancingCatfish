using System;
using CloudOnce.Internal;
using CloudOnce.Internal.Providers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace CloudOnce
{
	public static class Cloud
	{
		static Cloud()
		{
			Cloud.Leaderboards = new GenericLeaderboardsWrapper();
		}

		public static event UnityAction OnInitializeComplete;

		public static event UnityAction<bool> OnSignedInChanged;

		public static event UnityAction OnSignInFailed;

		public static event UnityAction<Texture2D> OnPlayerImageDownloaded;

		public static event UnityAction<bool> OnCloudSaveComplete;

		public static event UnityAction<bool> OnCloudLoadComplete;

		public static event UnityAction<string[]> OnNewCloudValues;

		public static string ServiceName
		{
			get
			{
				return Cloud.Provider.ServiceName;
			}
		}

		public static string PlayerID
		{
			get
			{
				return Cloud.Provider.PlayerID;
			}
		}

		public static string PlayerDisplayName
		{
			get
			{
				return Cloud.Provider.PlayerDisplayName;
			}
		}

		public static Texture2D PlayerImage
		{
			get
			{
				return Cloud.Provider.PlayerImage;
			}
		}

		public static bool IsSignedIn
		{
			get
			{
				return Cloud.Provider.IsSignedIn;
			}
		}

		public static bool CloudSaveEnabled
		{
			get
			{
				return Cloud.Provider.CloudSaveEnabled;
			}
			set
			{
				Cloud.Provider.CloudSaveEnabled = value;
			}
		}

		public static Interval AutoLoadInterval
		{
			get
			{
				return Cloud.s_autoLoadInterval;
			}
			set
			{
				Cloud.s_autoLoadInterval = value;
			}
		}

		public static GenericLeaderboardsWrapper Leaderboards { get; private set; }

		public static GenericAchievementsWrapper Achievements { get; private set; } = new GenericAchievementsWrapper();

		public static ICloudStorageProvider Storage
		{
			get
			{
				return Cloud.Provider.Storage;
			}
		}

		private static ICloudProvider Provider
		{
			get
			{
				if (!Cloud.s_isProviderInitialized)
				{
					CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.InternalInit(Cloud.s_cloudOnceEvents);
					Cloud.s_isProviderInitialized = true;
				}
				return CloudProviderBase<GooglePlayGamesCloudProvider>.Instance;
			}
		}

		public static void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true)
		{
			Cloud.Provider.Initialize(activateCloudSave, autoSignIn, autoCloudLoad);
		}

		public static void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null)
		{
			Cloud.Provider.SignIn(autoCloudLoad, callback);
		}

		public static void SignOut()
		{
			Cloud.Provider.SignOut();
		}

		public static void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
		{
			Cloud.Provider.LoadUsers(userIDs, callback);
		}

		private static readonly CloudOnceEvents s_cloudOnceEvents = new CloudOnceEvents();

		private static Interval s_autoLoadInterval;

		private static bool s_isProviderInitialized;
	}
}
