using System;
using System.IO;
using FullInspector;
using UnityEngine;

namespace ACE
{
	public abstract class UnityBaseSettings<T> : BaseScriptableObject, IUnitySettings where T : UnityBaseSettings<T>
	{
		public static T Instance
		{
			get
			{
				if (UnityBaseSettings<T>.instance == null)
				{
					UnityEngine.Debug.Log("Will try to load existing '" + UnityBaseSettings<T>.SETTINGS_FILENAME_ON_DISK + "'");
					UnityBaseSettings<T>.instance = Resources.Load<T>(Path.Combine("Settings", UnityBaseSettings<T>.SETTINGS_FILENAME_ON_DISK));
					if (UnityBaseSettings<T>.instance == null)
					{
						UnityBaseSettings<T>.instance = ScriptableObject.CreateInstance<T>();
						UnityEngine.Debug.Log("Didnt find existing settings. Will create new settings asset...");
					}
				}
				return UnityBaseSettings<T>.instance;
			}
		}

		public abstract string GetMainFolderName();

		public virtual string GetRelativeSettingsPath()
		{
			return Path.Combine(this.GetMainFolderName(), "Resources/Settings/");
		}

		private static void CreateFolderIfNotExist(string existingPath, string newFolderName)
		{
			string path = Path.Combine(Application.dataPath, Path.Combine(existingPath, newFolderName));
			if (!Directory.Exists(path))
			{
			}
		}

		private const string ASSETS_FOLDER_NAME = "Assets";

		private const string RESOURCE_FOLDER_NAME = "Resources";

		private const string SETTINGS_FOLDER_NAME = "Settings";

		private const string RELATIVE_SETTINGS_PATH = "Resources/Settings/";

		private const string SETTINGS_FILENAME_EXT = "asset";

		private static readonly string SETTINGS_FILENAME_ON_DISK = typeof(T).Name;

		private static T instance = (T)((object)null);
	}
}
