using System;
using System.Collections.Generic;

namespace Soomla
{
	public class KeyValueStorage
	{
		private static KeyValueStorage instance
		{
			get
			{
				return KeyValueStorage._instance = ((KeyValueStorage._instance != null) ? KeyValueStorage._instance : new KeyValueStorageUnity());
			}
		}

		public static string GetValue(string key)
		{
			return KeyValueStorage.instance._getValue(key);
		}

		public static void SetValue(string key, string val)
		{
			KeyValueStorage.instance._setValue(key, val);
		}

		public static void DeleteKeyValue(string key)
		{
			KeyValueStorage.instance._deleteKeyValue(key);
		}

		public static List<string> GetEncryptedKeys()
		{
			return KeyValueStorage.instance._getEncryptedKeys();
		}

		public static void Purge()
		{
			KeyValueStorage.instance._purge();
		}

		protected virtual string _getValue(string key)
		{
			return null;
		}

		protected virtual void _setValue(string key, string val)
		{
		}

		protected virtual void _deleteKeyValue(string key)
		{
		}

		protected virtual List<string> _getEncryptedKeys()
		{
			return null;
		}

		protected virtual void _purge()
		{
		}

		protected const string TAG = "SOOMLA KeyValueStorage";

		private static KeyValueStorage _instance;
	}
}
