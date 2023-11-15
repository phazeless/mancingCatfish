using System;
using System.Collections.Generic;
using System.Reflection;

namespace CloudOnce.Internal
{
	public static class JsonHelper
	{
		public static T Convert<T>(JSONObject jsonObject)
		{
			return (T)((object)JsonHelper.Convert(jsonObject, typeof(T)));
		}

		public static JSONObject ToJsonObject<T>(Dictionary<string, T> serializableDictionary) where T : IJsonSerializeable
		{
			Dictionary<string, IJsonSerializeable> dictionary = JsonHelper.ConvertToSerializable<T>(serializableDictionary);
			Dictionary<string, JSONObject> dictionary2 = new Dictionary<string, JSONObject>();
			foreach (KeyValuePair<string, IJsonSerializeable> keyValuePair in dictionary)
			{
				dictionary2.Add(keyValuePair.Key, keyValuePair.Value.ToJSONObject());
			}
			return new JSONObject(dictionary2);
		}

		public static JSONObject ToJsonObject<T>(List<T> serializableList) where T : IJsonSerializeable
		{
			List<JSONObject> list = new List<JSONObject>();
			foreach (T t in serializableList)
			{
				list.Add(t.ToJSONObject());
			}
			return new JSONObject(list);
		}

		private static object Convert(JSONObject jsonObject, Type type)
		{
			if (type == typeof(Dictionary<string, float>))
			{
				return JsonHelper.ToStringFloatDictionary(jsonObject);
			}
			if (type == typeof(Dictionary<string, SyncableItem>))
			{
				return JsonHelper.ConstructDictionaryOfType<SyncableItem>(jsonObject);
			}
			if (type == typeof(Dictionary<string, SyncableCurrency>))
			{
				return JsonHelper.ConstructDictionaryOfType<SyncableCurrency>(jsonObject);
			}
			if (type == typeof(Dictionary<string, CurrencyValue>))
			{
				return JsonHelper.ConstructDictionaryOfType<CurrencyValue>(jsonObject);
			}
			return null;
		}

		private static Dictionary<string, IJsonSerializeable> ConvertToSerializable<T>(Dictionary<string, T> dictionary) where T : IJsonSerializeable
		{
			Dictionary<string, IJsonSerializeable> dictionary2 = new Dictionary<string, IJsonSerializeable>();
			foreach (KeyValuePair<string, T> keyValuePair in dictionary)
			{
				dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return dictionary2;
		}

		private static Dictionary<string, float> ToStringFloatDictionary(JSONObject jObject)
		{
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			foreach (string text in jObject.Keys)
			{
				dictionary.Add(text, jObject[text].F);
			}
			return dictionary;
		}

		private static Dictionary<string, T> ConstructDictionaryOfType<T>(JSONObject jsonObject) where T : class
		{
			ConstructorInfo constructor = typeof(T).GetConstructor(new Type[]
			{
				typeof(JSONObject)
			});
			if (constructor != null)
			{
				Dictionary<string, T> dictionary = new Dictionary<string, T>();
				foreach (string text in jsonObject.Keys)
				{
					dictionary.Add(text, (T)((object)constructor.Invoke(new object[]
					{
						jsonObject[text]
					})));
				}
				return dictionary;
			}
			return null;
		}
	}
}
