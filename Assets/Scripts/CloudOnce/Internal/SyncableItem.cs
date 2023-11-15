using System;
using System.Globalization;
using CloudOnce.Internal.Utils;

namespace CloudOnce.Internal
{
	public class SyncableItem : IEquatable<SyncableItem>, IJsonConvertible, IJsonSerializeable, IJsonDeserializable
	{
		public SyncableItem(JSONObject itemData)
		{
			this.FromJSONObject(itemData);
		}

		public SyncableItem(string value, SyncableItemMetaData metadata)
		{
			this.valueString = value;
			this.Metadata = metadata;
		}

		public SyncableItemMetaData Metadata { get; private set; }

		public string ValueString
		{
			get
			{
				string result;
				if ((result = this.valueString) == null)
				{
					result = (this.valueString = string.Empty);
				}
				return result;
			}
			set
			{
				this.valueString = value;
				if (this.Metadata.PersistenceType == PersistenceType.Latest)
				{
					this.Metadata.UpdateDateTime();
				}
			}
		}

		public bool Equals(SyncableItem other)
		{
			return other != null && string.Equals(this.valueString, other.valueString) && this.Metadata.Equals(other.Metadata);
		}

		public JSONObject ToJSONObject()
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.Object);
			jsonobject.AddField("v", this.ValueString.ToString(CultureInfo.InvariantCulture));
			jsonobject.AddField("m", this.Metadata.ToJSONObject());
			return jsonobject;
		}

		public void FromJSONObject(JSONObject jsonObject)
		{
			string alias = CloudOnceUtils.GetAlias(typeof(SyncableItem).Name, jsonObject, new string[]
			{
				"v",
				"_vs"
			});
			string alias2 = CloudOnceUtils.GetAlias(typeof(SyncableItem).Name, jsonObject, new string[]
			{
				"m",
				"_md"
			});
			this.valueString = jsonObject[alias].String;
			this.Metadata = new SyncableItemMetaData(jsonObject[alias2]);
		}

		public override string ToString()
		{
			return string.Format("Value: {0}" + Environment.NewLine + " Meta Data: {1}", this.ValueString, this.Metadata);
		}

		private const string oldAliasValueString = "_vs";

		private const string oldAliasMetadata = "_md";

		private const string aliasValueString = "v";

		private const string aliasMetadata = "m";

		private string valueString;
	}
}
