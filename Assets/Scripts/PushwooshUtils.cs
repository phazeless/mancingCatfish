using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public static class PushwooshUtils
{
	public static IDictionary<string, object> JsonToDictionary(string json)
	{
		SimpleJSON.JSONObject jsonObject = JSON.Parse(json) as SimpleJSON.JSONObject;
		return PushwooshUtils.JsonObjectToDictionary(jsonObject);
	}

	private static IDictionary<string, object> JsonObjectToDictionary(SimpleJSON.JSONObject jsonObject)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		IEnumerator enumerator = jsonObject.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				KeyValuePair<string, JSONNode> keyValuePair = (KeyValuePair<string, JSONNode>)obj;
				string key = keyValuePair.Key;
				JSONNode value = keyValuePair.Value;
				dictionary.Add(key, PushwooshUtils.JsonNodeToPOCO(value));
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		return dictionary;
	}

	private static List<object> JsonArrayToList(JSONArray jsonArray)
	{
		List<object> list = new List<object>();
		IEnumerator enumerator = jsonArray.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				JSONNode node = (JSONNode)obj;
				list.Add(PushwooshUtils.JsonNodeToPOCO(node));
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		return list;
	}

	private static object JsonNodeToPOCO(JSONNode node)
	{
		if (node.IsNumber)
		{
			return node.AsDouble;
		}
		if (node.IsBoolean)
		{
			return node.AsBool;
		}
		if (node.IsString)
		{
			return node.Value;
		}
		if (node.IsObject)
		{
			return PushwooshUtils.JsonObjectToDictionary(node as SimpleJSON.JSONObject);
		}
		if (node.IsArray)
		{
			return PushwooshUtils.JsonArrayToList(node as JSONArray);
		}
		if (node.IsNull)
		{
			return null;
		}
		throw new Exception("Unrecognized json node type");
	}

	public static string DictionaryToJson(IDictionary<string, object> dictionary)
	{
		List<string> list = new List<string>();
		if (dictionary != null)
		{
			foreach (KeyValuePair<string, object> keyValuePair in dictionary)
			{
				string key = keyValuePair.Key;
				string arg = PushwooshUtils.POCOToJson(keyValuePair.Value);
				list.Add(string.Format("\"{0}\": {1}", key, arg));
			}
		}
		return "{" + string.Join(",", list.ToArray()) + "}";
	}

	private static string POCOToJson(object value)
	{
		if (value is string)
		{
			return "\"" + value + "\"";
		}
		if (value is IDictionary)
		{
			return PushwooshUtils.DictionaryToJson(value as IDictionary<string, object>);
		}
		if (value is List<object>)
		{
			return PushwooshUtils.ListToJson(value as List<object>);
		}
		return value.ToString();
	}

	private static string ListToJson(List<object> list)
	{
		List<string> list2 = new List<string>();
		foreach (object value in list)
		{
			list2.Add(PushwooshUtils.POCOToJson(value));
		}
		return "[" + string.Join(",", list2.ToArray()) + "]";
	}
}
