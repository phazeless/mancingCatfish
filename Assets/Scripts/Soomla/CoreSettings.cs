using System;

namespace Soomla
{
	public class CoreSettings : ISoomlaSettings
	{
		public static string SoomlaSecret
		{
			get
			{
				if (CoreSettings.soomlaSecret == null)
				{
					CoreSettings.soomlaSecret = SoomlaEditorScript.GetConfigValue(CoreSettings.CoreModulePrefix, "SoomlaSecret");
					if (CoreSettings.soomlaSecret == null)
					{
						CoreSettings.soomlaSecret = CoreSettings.ONLY_ONCE_DEFAULT;
					}
				}
				return CoreSettings.soomlaSecret;
			}
			set
			{
				if (CoreSettings.soomlaSecret != value)
				{
					CoreSettings.soomlaSecret = value;
					SoomlaEditorScript.SetConfigValue(CoreSettings.CoreModulePrefix, "SoomlaSecret", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool DebugMessages
		{
			get
			{
				if (CoreSettings.debugMessages == null)
				{
					CoreSettings.debugMessages = SoomlaEditorScript.GetConfigValue(CoreSettings.CoreModulePrefix, "DebugMessages");
					if (CoreSettings.debugMessages == null)
					{
						CoreSettings.debugMessages = false.ToString();
					}
				}
				return Convert.ToBoolean(CoreSettings.debugMessages);
			}
			set
			{
				if (Convert.ToBoolean(CoreSettings.debugMessages) != value)
				{
					CoreSettings.debugMessages = value.ToString();
					SoomlaEditorScript.SetConfigValue(CoreSettings.CoreModulePrefix, "DebugMessages", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool DebugUnityMessages
		{
			get
			{
				if (CoreSettings.debugUnityMessages == null)
				{
					CoreSettings.debugUnityMessages = SoomlaEditorScript.GetConfigValue(CoreSettings.CoreModulePrefix, "DebugUnityMessages");
					if (CoreSettings.debugUnityMessages == null)
					{
						CoreSettings.debugUnityMessages = true.ToString();
					}
				}
				return Convert.ToBoolean(CoreSettings.debugUnityMessages);
			}
			set
			{
				if (Convert.ToBoolean(CoreSettings.debugUnityMessages) != value)
				{
					CoreSettings.debugUnityMessages = value.ToString();
					SoomlaEditorScript.SetConfigValue(CoreSettings.CoreModulePrefix, "DebugUnityMessages", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		private static string CoreModulePrefix = "Core";

		public static string ONLY_ONCE_DEFAULT = "SET ONLY ONCE";

		private static string soomlaSecret;

		private static string debugMessages;

		private static string debugUnityMessages;
	}
}
