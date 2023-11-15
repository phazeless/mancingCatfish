using System;
using System.Collections.Generic;
using CloudOnce.Internal.Utils;

namespace CloudOnce.Internal
{
	public class GameData
	{
		public GameData()
		{
			this.SyncableItems = new Dictionary<string, SyncableItem>();
			this.SyncableCurrencies = new Dictionary<string, SyncableCurrency>();
		}

		public GameData(string serializedData)
		{
			if (string.IsNullOrEmpty(serializedData))
			{
				this.SyncableItems = new Dictionary<string, SyncableItem>();
				this.SyncableCurrencies = new Dictionary<string, SyncableCurrency>();
				return;
			}
			JSONObject jsonobject = new JSONObject(serializedData, -2, false, false);
			string alias = CloudOnceUtils.GetAlias(typeof(GameData).Name, jsonobject, new string[]
			{
				"i",
				"SIs"
			});
			string alias2 = CloudOnceUtils.GetAlias(typeof(GameData).Name, jsonobject, new string[]
			{
				"c",
				"SCs"
			});
			this.SyncableItems = JsonHelper.Convert<Dictionary<string, SyncableItem>>(jsonobject[alias]);
			this.SyncableCurrencies = JsonHelper.Convert<Dictionary<string, SyncableCurrency>>(jsonobject[alias2]);
		}

		public Dictionary<string, SyncableItem> SyncableItems { get; set; }

		public Dictionary<string, SyncableCurrency> SyncableCurrencies { get; set; }

		public bool IsDirty { get; set; }

		public string[] GetAllKeys()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, SyncableItem> keyValuePair in this.SyncableItems)
			{
				list.Add(keyValuePair.Key);
			}
			foreach (KeyValuePair<string, SyncableCurrency> keyValuePair2 in this.SyncableCurrencies)
			{
				list.Add(keyValuePair2.Key);
			}
			return list.ToArray();
		}

		public string Serialize()
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.Object);
			jsonobject.AddField("i", JsonHelper.ToJsonObject<SyncableItem>(this.SyncableItems));
			jsonobject.AddField("c", JsonHelper.ToJsonObject<SyncableCurrency>(this.SyncableCurrencies));
			return jsonobject.ToString();
		}

		public string[] MergeWith(GameData otherData)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, SyncableItem> keyValuePair in otherData.SyncableItems)
			{
				SyncableItem syncableItem;
				if (this.SyncableItems.TryGetValue(keyValuePair.Key, out syncableItem))
				{
					SyncableItem syncableItem2 = ConflictResolver.ResolveConflict(syncableItem, keyValuePair.Value);
					if (!syncableItem2.Equals(syncableItem))
					{
						this.SyncableItems[keyValuePair.Key] = syncableItem2;
						list.Add(keyValuePair.Key);
					}
				}
				else
				{
					this.SyncableItems.Add(keyValuePair.Key, keyValuePair.Value);
					list.Add(keyValuePair.Key);
				}
			}
			foreach (KeyValuePair<string, SyncableCurrency> keyValuePair2 in otherData.SyncableCurrencies)
			{
				SyncableCurrency syncableCurrency;
				if (this.SyncableCurrencies.TryGetValue(keyValuePair2.Key, out syncableCurrency))
				{
					bool flag = syncableCurrency.MergeWith(keyValuePair2.Value);
					if (flag)
					{
						list.Add(keyValuePair2.Key);
					}
				}
				else
				{
					this.SyncableCurrencies.Add(keyValuePair2.Key, keyValuePair2.Value);
					list.Add(keyValuePair2.Key);
				}
			}
			return list.ToArray();
		}

		private const string oldSyncableItemsKey = "SIs";

		private const string oldSyncableCurrenciesKey = "SCs";

		private const string syncableItemsKey = "i";

		private const string syncableCurrenciesKey = "c";
	}
}
