using System;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine;

public class BigIntegerToStringConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(BigInteger);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		object result;
		try
		{
			result = new BigInteger((int)BigInteger.Parse((string)reader.Value));
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Failed to convert string-value to BigInteger. Value that failed: ",
				reader.Value,
				". Exception: ",
				ex.Message
			}));
			result = default(BigInteger);
		}
		return result;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		writer.WriteValue(value.ToString());
	}
}
