using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class EncryptedPlayerPrefs
{
	public static Dictionary<string, object> CachedPlayerPrefs
	{
		get
		{
			return EncryptedPlayerPrefs.cachedPlayerPrefs;
		}
	}

	public static void SetCachedPlayerPrefs(Dictionary<string, object> playerPrefs)
	{
		EncryptedPlayerPrefs.cachedPlayerPrefs = playerPrefs;
		foreach (KeyValuePair<string, object> keyValuePair in EncryptedPlayerPrefs.cachedPlayerPrefs)
		{
			object value = keyValuePair.Value;
			string key = keyValuePair.Key;
			if (value is string)
			{
				EncryptedPlayerPrefs.SetString(key, (string)value, false);
			}
			else if (EncryptedPlayerPrefs.IsWholeNumber(value))
			{
				EncryptedPlayerPrefs.SetInt(key, Convert.ToInt32(value), false);
			}
			else if (EncryptedPlayerPrefs.IsDecimalNumber(value))
			{
				EncryptedPlayerPrefs.SetFloat(key, Convert.ToSingle(value), false);
			}
		}
	}

	public static string Md5(string strToEncrypt)
	{
		byte[] data = EncryptedPlayerPrefs.md5Hasher.ComputeHash(Encoding.Default.GetBytes(strToEncrypt));
		return EncryptedPlayerPrefs.RemoveSymbolsAndLowerCaseFromMd5Hash(data);
	}

	public static string RemoveSymbolsAndLowerCaseFromMd5Hash(byte[] data)
	{
		string text = BitConverter.ToString(data);
		int i = 0;
		int num = 0;
		while (i < text.Length)
		{
			if (text[i] != '-')
			{
				EncryptedPlayerPrefs.sb[num] = char.ToLower(text[i]);
				num++;
			}
			i++;
		}
		return EncryptedPlayerPrefs.sb.ToString();
	}

	public static void SaveEncryption(string key, string type, string value)
	{
		int num = (int)Mathf.Floor(Mathf.Min(UnityEngine.Random.value, 0.99f) * (float)EncryptedPlayerPrefs.keys.Length);
		string text = EncryptedPlayerPrefs.keys[num];
		string value2 = EncryptedPlayerPrefs.Md5(string.Concat(new string[]
		{
			type,
			"_",
			EncryptedPlayerPrefs.privateKey,
			"_",
			text,
			"_",
			value
		}));
		PlayerPrefs.SetString(key + "_encryption_check", value2);
		PlayerPrefs.SetInt(key + "_used_key", num);
	}

	public static bool CheckEncryption(string key, string type, string value)
	{
		int @int = PlayerPrefs.GetInt(key + "_used_key");
		string text = EncryptedPlayerPrefs.keys[@int];
		string b = EncryptedPlayerPrefs.Md5(string.Concat(new string[]
		{
			type,
			"_",
			EncryptedPlayerPrefs.privateKey,
			"_",
			text,
			"_",
			value
		}));
		if (!PlayerPrefs.HasKey(key + "_encryption_check"))
		{
			return false;
		}
		string @string = PlayerPrefs.GetString(key + "_encryption_check");
		return @string == b;
	}

	public static void SetInt(string key, int value, bool addToCache = true)
	{
		PlayerPrefs.SetInt(key, value);
		EncryptedPlayerPrefs.SaveEncryption(key, "int", value.ToString());
		if (addToCache)
		{
			EncryptedPlayerPrefs.AddKeyValueToCache(key, value);
		}
	}

	public static void SetFloat(string key, float value, bool addToCache = true)
	{
		PlayerPrefs.SetFloat(key, value);
		EncryptedPlayerPrefs.SaveEncryption(key, "float", Mathf.Floor(value * 1000f).ToString());
		if (addToCache)
		{
			EncryptedPlayerPrefs.AddKeyValueToCache(key, value);
		}
	}

	public static void SetString(string key, string value, bool addToCache = true)
	{
		PlayerPrefs.SetString(key, value);
		EncryptedPlayerPrefs.SaveEncryption(key, "string", value);
		if (addToCache)
		{
			EncryptedPlayerPrefs.AddKeyValueToCache(key, value);
		}
	}

	public static int GetInt(string key)
	{
		return EncryptedPlayerPrefs.GetInt(key, 0);
	}

	public static float GetFloat(string key)
	{
		return EncryptedPlayerPrefs.GetFloat(key, 0f);
	}

	public static string GetString(string key)
	{
		return EncryptedPlayerPrefs.GetString(key, string.Empty);
	}

	public static int GetInt(string key, int defaultValue)
	{
		int @int = PlayerPrefs.GetInt(key);
		if (!EncryptedPlayerPrefs.CheckEncryption(key, "int", @int.ToString()))
		{
			return defaultValue;
		}
		return @int;
	}

	public static float GetFloat(string key, float defaultValue)
	{
		float @float = PlayerPrefs.GetFloat(key);
		if (!EncryptedPlayerPrefs.CheckEncryption(key, "float", Mathf.Floor(@float * 1000f).ToString()))
		{
			return defaultValue;
		}
		return @float;
	}

	public static string GetString(string key, string defaultValue)
	{
		string @string = PlayerPrefs.GetString(key);
		if (!EncryptedPlayerPrefs.CheckEncryption(key, "string", @string))
		{
			return defaultValue;
		}
		return @string;
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
		PlayerPrefs.DeleteKey(key + "_encryption_check");
		PlayerPrefs.DeleteKey(key + "_used_key");
	}

	public static void Save()
	{
		PlayerPrefs.Save();
	}

	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	private static void AddKeyValueToCache(string key, object value)
	{
		if (key == "meta.storeinfo" || key == "allSoomlaKeys")
		{
			return;
		}
		if (EncryptedPlayerPrefs.cachedPlayerPrefs.ContainsKey(key))
		{
			EncryptedPlayerPrefs.cachedPlayerPrefs[key] = value;
		}
		else
		{
			EncryptedPlayerPrefs.cachedPlayerPrefs.Add(key, value);
		}
	}

	private static bool IsWholeNumber(object value)
	{
		return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong;
	}

	private static bool IsDecimalNumber(object value)
	{
		return value is float || value is double || value is decimal;
	}

	private static readonly string privateKey = "56yvgHBHuy769jbhgf7mbjhHH";

	private static string[] keys = new string[]
	{
		"67tygubjkn78",
		"asf333HJ",
		"SOJ77gG",
		"ahjLKJ77bh",
		"LKJbDrr4",
		"ILILHhhf45",
		"8uhuGHannBBv"
	};

	private static Dictionary<string, object> cachedPlayerPrefs = new Dictionary<string, object>();

	private static MD5 md5Hasher = MD5.Create();

	private const char minusChar = '-';

	private static StringBuilder sb = new StringBuilder("00000000000000000000000000000000", 0, 32, 32);
}
