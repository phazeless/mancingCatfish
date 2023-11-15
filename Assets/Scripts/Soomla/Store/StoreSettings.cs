using System;

namespace Soomla.Store
{
	public class StoreSettings : ISoomlaSettings
	{
		public static string AndroidPublicKey
		{
			get
			{
				if (StoreSettings.androidPublicKey == null)
				{
					StoreSettings.androidPublicKey = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "AndroidPublicKey");
					if (StoreSettings.androidPublicKey == null)
					{
						StoreSettings.androidPublicKey = StoreSettings.AND_PUB_KEY_DEFAULT;
					}
				}
				return StoreSettings.androidPublicKey;
			}
			set
			{
				if (StoreSettings.androidPublicKey != value)
				{
					StoreSettings.androidPublicKey = value;
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "AndroidPublicKey", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static string PlayClientId
		{
			get
			{
				if (StoreSettings.playClientId == null)
				{
					StoreSettings.playClientId = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "PlayClientId");
					if (StoreSettings.playClientId == null)
					{
						StoreSettings.playClientId = StoreSettings.PLAY_CLIENT_ID_DEFAULT;
					}
				}
				return StoreSettings.playClientId;
			}
			set
			{
				if (StoreSettings.playClientId != value)
				{
					StoreSettings.playClientId = value;
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "PlayClientId", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static string PlayClientSecret
		{
			get
			{
				if (StoreSettings.playClientSecret == null)
				{
					StoreSettings.playClientSecret = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "PlayClientSecret");
					if (StoreSettings.playClientSecret == null)
					{
						StoreSettings.playClientSecret = StoreSettings.PLAY_CLIENT_SECRET_DEFAULT;
					}
				}
				return StoreSettings.playClientSecret;
			}
			set
			{
				if (StoreSettings.playClientSecret != value)
				{
					StoreSettings.playClientSecret = value;
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "PlayClientSecret", value);
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static string PlayRefreshToken
		{
			get
			{
				if (StoreSettings.playRefreshToken == null)
				{
					StoreSettings.playRefreshToken = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "PlayRefreshToken");
					if (StoreSettings.playRefreshToken == null)
					{
						StoreSettings.playRefreshToken = StoreSettings.PLAY_REFRESH_TOKEN_DEFAULT;
					}
				}
				return StoreSettings.playRefreshToken;
			}
			set
			{
				if (StoreSettings.playRefreshToken != value)
				{
					StoreSettings.playRefreshToken = value;
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "PlayRefreshToken", value);
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool PlayVerifyOnServerFailure
		{
			get
			{
				if (StoreSettings.playVerifyOnServerFailure == null)
				{
					StoreSettings.playVerifyOnServerFailure = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "PlayVerifyOnServerFailure");
					if (StoreSettings.playVerifyOnServerFailure == null)
					{
						StoreSettings.playVerifyOnServerFailure = false.ToString();
					}
				}
				return Convert.ToBoolean(StoreSettings.playVerifyOnServerFailure);
			}
			set
			{
				if (StoreSettings.playVerifyOnServerFailure != value.ToString())
				{
					StoreSettings.playVerifyOnServerFailure = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "PlayVerifyOnServerFailure", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool AndroidTestPurchases
		{
			get
			{
				if (StoreSettings.androidTestPurchases == null)
				{
					StoreSettings.androidTestPurchases = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "AndroidTestPurchases");
					if (StoreSettings.androidTestPurchases == null)
					{
						StoreSettings.androidTestPurchases = false.ToString();
					}
				}
				return Convert.ToBoolean(StoreSettings.androidTestPurchases);
			}
			set
			{
				if (StoreSettings.androidTestPurchases != value.ToString())
				{
					StoreSettings.androidTestPurchases = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "AndroidTestPurchases", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool PlaySsvValidation
		{
			get
			{
				if (StoreSettings.playSsvValidation == null)
				{
					StoreSettings.playSsvValidation = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "PlaySsvValidation");
					if (StoreSettings.playSsvValidation == null)
					{
						StoreSettings.playSsvValidation = false.ToString();
					}
				}
				return Convert.ToBoolean(StoreSettings.playSsvValidation);
			}
			set
			{
				if (StoreSettings.playSsvValidation != value.ToString())
				{
					StoreSettings.playSsvValidation = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "PlaySsvValidation", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool IosSSV
		{
			get
			{
				if (StoreSettings.iosSSV == null)
				{
					StoreSettings.iosSSV = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "IosSSV");
					if (StoreSettings.iosSSV == null)
					{
						StoreSettings.iosSSV = false.ToString();
					}
				}
				return Convert.ToBoolean(StoreSettings.iosSSV);
			}
			set
			{
				if (StoreSettings.iosSSV != value.ToString())
				{
					StoreSettings.iosSSV = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "IosSSV", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool IosVerifyOnServerFailure
		{
			get
			{
				if (StoreSettings.iosVerifyOnServerFailure == null)
				{
					StoreSettings.iosVerifyOnServerFailure = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "IosVerifyOnServerFailure");
					if (StoreSettings.iosVerifyOnServerFailure == null)
					{
						StoreSettings.iosVerifyOnServerFailure = false.ToString();
					}
				}
				return Convert.ToBoolean(StoreSettings.iosVerifyOnServerFailure);
			}
			set
			{
				if (StoreSettings.iosVerifyOnServerFailure != value.ToString())
				{
					StoreSettings.iosVerifyOnServerFailure = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "IosVerifyOnServerFailure", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool NoneBP
		{
			get
			{
				if (StoreSettings.noneBP == null)
				{
					StoreSettings.noneBP = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "NoneBP");
					if (StoreSettings.noneBP == null)
					{
						StoreSettings.noneBP = false.ToString();
					}
				}
				return Convert.ToBoolean(StoreSettings.noneBP);
			}
			set
			{
				if (StoreSettings.noneBP != value.ToString())
				{
					StoreSettings.noneBP = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "NoneBP", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool GPlayBP
		{
			get
			{
				if (StoreSettings.gPlayBP == null)
				{
					StoreSettings.gPlayBP = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "GPlayBP");
					if (StoreSettings.gPlayBP == null)
					{
						StoreSettings.gPlayBP = false.ToString();
					}
				}
				return Convert.ToBoolean(StoreSettings.gPlayBP);
			}
			set
			{
				if (StoreSettings.gPlayBP != value.ToString())
				{
					StoreSettings.gPlayBP = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "GPlayBP", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool AmazonBP
		{
			get
			{
				if (StoreSettings.amazonBP == null)
				{
					StoreSettings.amazonBP = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "AmazonBP");
					if (StoreSettings.amazonBP == null)
					{
						StoreSettings.amazonBP = false.ToString();
					}
				}
				return Convert.ToBoolean(StoreSettings.amazonBP);
			}
			set
			{
				if (StoreSettings.amazonBP != value.ToString())
				{
					StoreSettings.amazonBP = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "AmazonBP", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool WP8SimulatorBuild
		{
			get
			{
				if (StoreSettings.wP8SimulatorBuild == null)
				{
					StoreSettings.wP8SimulatorBuild = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "WP8SimulatorBuild");
					if (StoreSettings.wP8SimulatorBuild == null)
					{
						StoreSettings.wP8SimulatorBuild = false.ToString();
					}
				}
				return Convert.ToBoolean(StoreSettings.wP8SimulatorBuild);
			}
			set
			{
				if (StoreSettings.wP8SimulatorBuild != value.ToString())
				{
					StoreSettings.wP8SimulatorBuild = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "WP8SimulatorBuild", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool WP8TestMode
		{
			get
			{
				if (StoreSettings.wP8TestMode == null)
				{
					StoreSettings.wP8TestMode = SoomlaEditorScript.GetConfigValue(StoreSettings.StoreModulePrefix, "WP8TestMode");
					if (StoreSettings.wP8TestMode == null)
					{
						StoreSettings.wP8TestMode = false.ToString();
					}
				}
				return Convert.ToBoolean(StoreSettings.wP8TestMode);
			}
			set
			{
				if (StoreSettings.wP8TestMode != value.ToString())
				{
					StoreSettings.wP8TestMode = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreSettings.StoreModulePrefix, "WP8TestMode", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		private static string StoreModulePrefix = "Store";

		public static string AND_PUB_KEY_DEFAULT = "YOUR GOOGLE PLAY PUBLIC KEY";

		public static string PLAY_CLIENT_ID_DEFAULT = "YOUR CLIENT ID";

		public static string PLAY_CLIENT_SECRET_DEFAULT = "YOUR CLIENT SECRET";

		public static string PLAY_REFRESH_TOKEN_DEFAULT = "YOUR REFRESH TOKEN";

		private static string androidPublicKey;

		private static string playClientId;

		private static string playClientSecret;

		private static string playRefreshToken;

		public static string playVerifyOnServerFailure;

		private static string androidTestPurchases;

		private static string playSsvValidation;

		private static string iosSSV;

		private static string iosVerifyOnServerFailure;

		private static string noneBP;

		private static string gPlayBP;

		private static string amazonBP;

		private static string wP8SimulatorBuild;

		private static string wP8TestMode;
	}
}
