using System;
using UnityEngine;

namespace Soomla
{
	public class SoomlaEditorScript : ScriptableObject
	{
		public static SoomlaEditorScript Instance
		{
			get
			{
				if (SoomlaEditorScript.instance == null)
				{
					SoomlaEditorScript.instance = (Resources.Load("SoomlaEditorScript") as SoomlaEditorScript);
					if (SoomlaEditorScript.instance == null)
					{
						SoomlaEditorScript.instance = ScriptableObject.CreateInstance<SoomlaEditorScript>();
					}
				}
				return SoomlaEditorScript.instance;
			}
		}

		public static void DirtyEditor()
		{
		}

		public static void SetConfigValue(string prefix, string key, string value)
		{
			EncryptedPlayerPrefs.SetString("Soomla." + prefix + "." + key, value, true);
			SoomlaEditorScript.Instance.SoomlaSettings["Soomla." + prefix + "." + key] = value;
			EncryptedPlayerPrefs.Save();
		}

		public static string GetConfigValue(string prefix, string key)
		{
			string @string;
			if (SoomlaEditorScript.Instance.SoomlaSettings.TryGetValue("Soomla." + prefix + "." + key, out @string) && @string.Length > 0)
			{
				return @string;
			}
			@string = EncryptedPlayerPrefs.GetString("Soomla." + prefix + "." + key);
			SoomlaEditorScript.SetConfigValue(prefix, key, @string);
			return (@string.Length <= 0) ? null : @string;
		}

		public static string AND_PUB_KEY_DEFAULT = "YOUR GOOGLE PLAY PUBLIC KEY";

		public static string ONLY_ONCE_DEFAULT = "SET ONLY ONCE";

		private const string soomSettingsAssetName = "SoomlaEditorScript";

		private const string soomSettingsPath = "Soomla/Resources";

		private const string soomSettingsAssetExtension = ".asset";

		private static SoomlaEditorScript instance;

		[SerializeField]
		public ObjectDictionary SoomlaSettings = new ObjectDictionary();
	}
}
