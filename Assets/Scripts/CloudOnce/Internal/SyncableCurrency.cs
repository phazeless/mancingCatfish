using System;
using System.Collections.Generic;
using CloudOnce.Internal.Utils;
using UnityEngine;

namespace CloudOnce.Internal
{
	public class SyncableCurrency : IJsonConvertible, IJsonSerializeable, IJsonDeserializable
	{
		public SyncableCurrency(string currencyID)
		{
			this.CurrencyID = currencyID;
		}

		public SyncableCurrency(JSONObject jsonSerializedCurrency)
		{
			this.FromJSONObject(jsonSerializedCurrency);
		}

		public string CurrencyID { get; private set; }

		public Dictionary<string, CurrencyValue> DeviceCurrencyValues
		{
			get
			{
				return this.deviceCurrencyValues;
			}
			set
			{
				this.deviceCurrencyValues = value;
			}
		}

		public JSONObject ToJSONObject()
		{
			Dictionary<string, JSONObject> dictionary = new Dictionary<string, JSONObject>();
			foreach (KeyValuePair<string, CurrencyValue> keyValuePair in this.DeviceCurrencyValues)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value.ToJSONObject());
			}
			JSONObject obj = new JSONObject(dictionary);
			JSONObject obj2 = JSONObject.CreateStringObject(this.CurrencyID);
			JSONObject jsonobject = JSONObject.Create(JSONObject.Type.Object);
			jsonobject.AddField("i", obj2);
			jsonobject.AddField("d", obj);
			return jsonobject;
		}

		public void FromJSONObject(JSONObject jsonObject)
		{
			string alias = CloudOnceUtils.GetAlias(typeof(SyncableCurrency).Name, jsonObject, new string[]
			{
				"i",
				"cID"
			});
			string alias2 = CloudOnceUtils.GetAlias(typeof(SyncableCurrency).Name, jsonObject, new string[]
			{
				"d",
				"cData"
			});
			this.CurrencyID = jsonObject[alias].String;
			this.DeviceCurrencyValues = JsonHelper.Convert<Dictionary<string, CurrencyValue>>(jsonObject[alias2]);
		}

		public bool MergeWith(SyncableCurrency otherData)
		{
			bool result = false;
			if (otherData.CurrencyID != this.CurrencyID)
			{
				UnityEngine.Debug.LogError("Attempted to merge two different currencies, this is not allowed!");
				return false;
			}
			if (this.DeviceCurrencyValues == null)
			{
				this.DeviceCurrencyValues = otherData.DeviceCurrencyValues;
				result = true;
			}
			else
			{
				foreach (KeyValuePair<string, CurrencyValue> keyValuePair in otherData.DeviceCurrencyValues)
				{
					CurrencyValue currencyValue;
					if (this.DeviceCurrencyValues.TryGetValue(keyValuePair.Key, out currencyValue))
					{
						if (keyValuePair.Value.Additions > currencyValue.Additions)
						{
							currencyValue.Additions = keyValuePair.Value.Additions;
							result = true;
						}
						if (keyValuePair.Value.Subtractions < currencyValue.Subtractions)
						{
							currencyValue.Subtractions = keyValuePair.Value.Subtractions;
							result = true;
						}
					}
					else
					{
						this.DeviceCurrencyValues.Add(keyValuePair.Key, keyValuePair.Value);
						result = true;
					}
				}
			}
			return result;
		}

		public void ResetCurrency()
		{
			this.deviceCurrencyValues = new Dictionary<string, CurrencyValue>();
		}

		private const string oldAliasCurrencyID = "cID";

		private const string oldAliasCurrencyDatas = "cData";

		private const string aliasCurrencyID = "i";

		private const string aliasCurrencyDatas = "d";

		private Dictionary<string, CurrencyValue> deviceCurrencyValues = new Dictionary<string, CurrencyValue>();
	}
}
