using System;
using System.Collections.Generic;

namespace Soomla
{
	public class KeyValueStorageUnity : KeyValueStorage
	{
		protected override string _getValue(string key)
		{
			return EncryptedPlayerPrefs.GetString(key);
		}

		protected override void _setValue(string key, string val)
		{
			List<string> list = new List<string>(EncryptedPlayerPrefs.GetString(KeyValueStorageUnity.ALL_KEYS_KEY, string.Empty).Split(new char[]
			{
				','
			}));
			if (!list.Contains(key))
			{
				list.Add(key);
			}
			EncryptedPlayerPrefs.SetString(key, val, true);
			EncryptedPlayerPrefs.SetString(KeyValueStorageUnity.ALL_KEYS_KEY, string.Join(",", list.ToArray()), true);
		}

		protected override void _deleteKeyValue(string key)
		{
			List<string> list = new List<string>(EncryptedPlayerPrefs.GetString(KeyValueStorageUnity.ALL_KEYS_KEY, string.Empty).Split(new char[]
			{
				','
			}));
			if (list.Contains(key))
			{
				list.Remove(key);
			}
			EncryptedPlayerPrefs.DeleteKey(key);
			EncryptedPlayerPrefs.SetString(KeyValueStorageUnity.ALL_KEYS_KEY, string.Join(",", list.ToArray()), true);
		}

		protected override List<string> _getEncryptedKeys()
		{
			return new List<string>(EncryptedPlayerPrefs.GetString(KeyValueStorageUnity.ALL_KEYS_KEY, string.Empty).Split(new char[]
			{
				','
			}));
		}

		protected override void _purge()
		{
			EncryptedPlayerPrefs.DeleteAll();
		}

		protected static string ALL_KEYS_KEY = "allSoomlaKeys";
	}
}
