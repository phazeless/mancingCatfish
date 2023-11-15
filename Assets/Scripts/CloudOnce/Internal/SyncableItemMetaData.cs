using System;
using System.Globalization;
using CloudOnce.Internal.Utils;

namespace CloudOnce.Internal
{
	public class SyncableItemMetaData : IEquatable<SyncableItemMetaData>, IJsonConvertible, IJsonSerializeable, IJsonDeserializable
	{
		public SyncableItemMetaData(DataType dataType, PersistenceType persistenceType)
		{
			this.DataType = dataType;
			this.PersistenceType = persistenceType;
			if (persistenceType == PersistenceType.Latest)
			{
				this.Timestamp = new DateTime(2014, 6, 30);
			}
		}

		public SyncableItemMetaData(JSONObject jsonObject)
		{
			this.FromJSONObject(jsonObject);
		}

		public DataType DataType { get; private set; }

		public PersistenceType PersistenceType { get; private set; }

		public DateTime Timestamp { get; private set; }

		public void UpdateDateTime()
		{
			this.Timestamp = DateTime.UtcNow;
		}

		public bool Equals(SyncableItemMetaData other)
		{
			if (other == null)
			{
				return false;
			}
			bool flag = object.Equals(this.DataType, other.DataType);
			bool flag2 = object.Equals(this.PersistenceType, other.PersistenceType);
			if (this.PersistenceType == PersistenceType.Latest)
			{
				return this.Timestamp.Equals(other.Timestamp) && flag && flag2;
			}
			return flag && flag2;
		}

		public override string ToString()
		{
			if (this.PersistenceType == PersistenceType.Latest)
			{
				return string.Format("DataType: {0}, PersistenceType: {1}, TimeStamp: {2}", this.DataType, this.PersistenceType, this.Timestamp);
			}
			return string.Format("DataType: {0}, PersistenceType: {1}", this.DataType, this.PersistenceType);
		}

		public void FromJSONObject(JSONObject jsonObject)
		{
			string alias = CloudOnceUtils.GetAlias(typeof(SyncableItemMetaData).Name, jsonObject, new string[]
			{
				"d",
				"dT"
			});
			string alias2 = CloudOnceUtils.GetAlias(typeof(SyncableItemMetaData).Name, jsonObject, new string[]
			{
				"p",
				"pT"
			});
			if (!string.IsNullOrEmpty(jsonObject[alias].String))
			{
				this.DataType = (DataType)Enum.Parse(typeof(DataType), jsonObject[alias].String);
			}
			else
			{
				this.DataType = (DataType)jsonObject[alias].F;
			}
			if (!string.IsNullOrEmpty(jsonObject[alias2].String))
			{
				this.PersistenceType = (PersistenceType)Enum.Parse(typeof(PersistenceType), jsonObject[alias2].String);
			}
			else
			{
				this.PersistenceType = (PersistenceType)jsonObject[alias2].F;
			}
			if (jsonObject.HasFields(new string[]
			{
				"t"
			}))
			{
				this.Timestamp = DateTime.FromBinary(Convert.ToInt64(jsonObject["t"].String));
			}
			else if (jsonObject.HasFields(new string[]
			{
				"tS"
			}))
			{
				this.Timestamp = DateTime.FromBinary(Convert.ToInt64(jsonObject["tS"].String));
			}
		}

		public JSONObject ToJSONObject()
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.Object);
			jsonobject.AddField("d", (float)this.DataType);
			jsonobject.AddField("p", (float)this.PersistenceType);
			if (this.PersistenceType == PersistenceType.Latest)
			{
				jsonobject.AddField("t", this.Timestamp.ToBinary().ToString(CultureInfo.InvariantCulture));
			}
			return jsonobject;
		}

		private const string oldAliasDataType = "dT";

		private const string oldAliasPersistenceType = "pT";

		private const string oldAliasTimestamp = "tS";

		private const string aliasDataType = "d";

		private const string aliasPersistenceType = "p";

		private const string aliasTimestamp = "t";
	}
}
