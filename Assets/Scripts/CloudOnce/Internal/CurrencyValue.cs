using System;
using CloudOnce.Internal.Utils;

namespace CloudOnce.Internal
{
	public class CurrencyValue : IJsonConvertible, IJsonSerializeable, IJsonDeserializable
	{
		public CurrencyValue()
		{
		}

		public CurrencyValue(float additions, float subtractions)
		{
			this.Additions = additions;
			this.Subtractions = subtractions;
		}

		public CurrencyValue(float value)
		{
			this.Value = value;
		}

		public CurrencyValue(JSONObject jsonObject)
		{
			this.FromJSONObject(jsonObject);
		}

		public float Additions { get; set; }

		public float Subtractions { get; set; }

		public float Value
		{
			get
			{
				return this.Additions + this.Subtractions;
			}
			set
			{
				float num = value - this.Value;
				if (num > 0f)
				{
					this.Additions += num;
				}
				else
				{
					this.Subtractions += num;
				}
			}
		}

		public JSONObject ToJSONObject()
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.Object);
			jsonobject.AddField("a", this.Additions);
			jsonobject.AddField("s", this.Subtractions);
			return jsonobject;
		}

		public void FromJSONObject(JSONObject jsonObject)
		{
			string alias = CloudOnceUtils.GetAlias(typeof(CurrencyValue).Name, jsonObject, new string[]
			{
				"a",
				"cdAdd"
			});
			string alias2 = CloudOnceUtils.GetAlias(typeof(CurrencyValue).Name, jsonObject, new string[]
			{
				"s",
				"cdSub"
			});
			this.Additions = jsonObject[alias].F;
			this.Subtractions = jsonObject[alias2].F;
		}

		private const string oldAliasAdditions = "cdAdd";

		private const string oldAliasSubtractions = "cdSub";

		private const string aliasAdditions = "a";

		private const string aliasSubtractions = "s";
	}
}
