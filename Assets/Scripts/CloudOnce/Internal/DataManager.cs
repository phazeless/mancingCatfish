using System;
using System.Collections.Generic;
using System.Globalization;
using CloudOnce.Internal.Providers;
using CloudOnce.Internal.Utils;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace CloudOnce.Internal
{
	public static class DataManager
	{
		public static bool IsLocalDataDirty
		{
			get
			{
				return DataManager.s_localGameData.IsDirty;
			}
			set
			{
				DataManager.s_localGameData.IsDirty = value;
			}
		}

		public static Dictionary<string, IPersistent> CloudPrefs
		{
			get
			{
				Dictionary<string, IPersistent> result;
				if ((result = DataManager.s_cloudPrefs) == null)
				{
					result = (DataManager.s_cloudPrefs = new Dictionary<string, IPersistent>());
				}
				return result;
			}
		}

		public static void InitDataManager()
		{
			if (!DataManager.s_isInitialized)
			{
				DataManager.LoadFromDisk();
				DataManager.s_isInitialized = true;
			}
		}

		public static void InitializeCurrency(string key)
		{
			if (!DataManager.s_localGameData.SyncableCurrencies.ContainsKey(key))
			{
				DataManager.s_localGameData.SyncableCurrencies.Add(key, new SyncableCurrency(key));
				DataManager.IsLocalDataDirty = true;
			}
		}

		public static void InitializeBool(string key, PersistenceType persistenceType, bool value)
		{
			if (!DataManager.s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Bool, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString(), metadata);
				DataManager.CreateItem(key, item);
			}
		}

		public static void InitializeInt(string key, PersistenceType persistenceType, int value)
		{
			if (!DataManager.s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Int, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metadata);
				DataManager.CreateItem(key, item);
			}
		}

		public static void InitializeUInt(string key, PersistenceType persistenceType, uint value)
		{
			if (!DataManager.s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.UInt, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metadata);
				DataManager.CreateItem(key, item);
			}
		}

		public static void InitializeFloat(string key, PersistenceType persistenceType, float value)
		{
			if (!DataManager.s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Float, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString("R", CultureInfo.InvariantCulture), metadata);
				DataManager.CreateItem(key, item);
			}
		}

		public static void InitializeDouble(string key, PersistenceType persistenceType, double value)
		{
			if (!DataManager.s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Double, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString("R", CultureInfo.InvariantCulture), metadata);
				DataManager.CreateItem(key, item);
			}
		}

		public static void InitializeString(string key, PersistenceType persistenceType, string value)
		{
			if (!DataManager.s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.String, persistenceType);
				SyncableItem item = new SyncableItem(value, metadata);
				DataManager.CreateItem(key, item);
			}
		}

		public static void InitializeLong(string key, PersistenceType persistenceType, long value)
		{
			if (!DataManager.s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Long, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metadata);
				DataManager.CreateItem(key, item);
			}
		}

		public static void InitializeDateTime(string key, PersistenceType persistenceType, DateTime value)
		{
			if (!DataManager.s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Long, persistenceType);
				SyncableItem item = new SyncableItem(value.ToBinary().ToString(CultureInfo.InvariantCulture), metadata);
				DataManager.CreateItem(key, item);
			}
		}

		public static void InitializeDecimal(string key, PersistenceType persistenceType, decimal value)
		{
			if (!DataManager.s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Decimal, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metadata);
				DataManager.CreateItem(key, item);
			}
		}

		public static void SetCurrencyValues(string key, Dictionary<string, CurrencyValue> currencyValues)
		{
			SyncableCurrency syncableCurrency;
			if (DataManager.s_localGameData.SyncableCurrencies.TryGetValue(key, out syncableCurrency))
			{
				syncableCurrency.DeviceCurrencyValues = currencyValues;
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new KeyNotFoundException(key);
		}

		public static void SetBool(string key, bool value)
		{
			if (DataManager.s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Bool)
			{
				DataManager.s_localGameData.SyncableItems[key].ValueString = ((!value) ? 0.ToString(CultureInfo.InvariantCulture) : 1.ToString(CultureInfo.InvariantCulture));
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(bool));
		}

		public static void SetInt(string key, int value)
		{
			if (DataManager.s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Int)
			{
				DataManager.s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(int));
		}

		public static void SetUInt(string key, uint value)
		{
			if (DataManager.s_localGameData.SyncableItems[key].Metadata.DataType == DataType.UInt)
			{
				DataManager.s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(uint));
		}

		public static void SetFloat(string key, float value)
		{
			if (DataManager.s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Float)
			{
				DataManager.s_localGameData.SyncableItems[key].ValueString = value.ToString("R", CultureInfo.InvariantCulture);
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(float));
		}

		public static void SetDouble(string key, double value)
		{
			if (DataManager.s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Double)
			{
				DataManager.s_localGameData.SyncableItems[key].ValueString = value.ToString("R", CultureInfo.InvariantCulture);
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(double));
		}

		public static void SetString(string key, string value)
		{
			if (DataManager.s_localGameData.SyncableItems[key].Metadata.DataType == DataType.String)
			{
				DataManager.s_localGameData.SyncableItems[key].ValueString = value;
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(string));
		}

		public static void SetLong(string key, long value)
		{
			if (DataManager.s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Long)
			{
				DataManager.s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(long));
		}

		public static void SetDateTime(string key, DateTime value)
		{
			if (DataManager.s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Long)
			{
				DataManager.s_localGameData.SyncableItems[key].ValueString = value.ToBinary().ToString(CultureInfo.InvariantCulture);
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(long));
		}

		public static void SetDecimal(string key, decimal value)
		{
			if (DataManager.s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Decimal)
			{
				DataManager.s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(decimal));
		}

		public static Dictionary<string, CurrencyValue> GetCurrencyValues(string key)
		{
			SyncableCurrency syncableCurrency;
			return (!DataManager.s_localGameData.SyncableCurrencies.TryGetValue(key, out syncableCurrency)) ? null : syncableCurrency.DeviceCurrencyValues;
		}

		public static bool GetBool(string key)
		{
			SyncableItem syncableItem;
			if (!DataManager.s_localGameData.SyncableItems.TryGetValue(key, out syncableItem))
			{
				throw new KeyNotFoundException(key);
			}
			if (syncableItem.Metadata.DataType != DataType.Bool)
			{
				throw new UnexpectedCollectionElementTypeException(key, typeof(bool));
			}
			int num;
			if (int.TryParse(syncableItem.ValueString, out num))
			{
				return num == 1;
			}
			return Convert.ToBoolean(syncableItem.ValueString, CultureInfo.InvariantCulture);
		}

		public static int GetInt(string key)
		{
			SyncableItem syncableItem;
			if (!DataManager.s_localGameData.SyncableItems.TryGetValue(key, out syncableItem))
			{
				throw new KeyNotFoundException(key);
			}
			if (syncableItem.Metadata.DataType == DataType.Int)
			{
				return Convert.ToInt32(syncableItem.ValueString);
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(int));
		}

		public static uint GetUInt(string key)
		{
			SyncableItem syncableItem;
			if (!DataManager.s_localGameData.SyncableItems.TryGetValue(key, out syncableItem))
			{
				throw new KeyNotFoundException(key);
			}
			if (syncableItem.Metadata.DataType == DataType.UInt)
			{
				return Convert.ToUInt32(syncableItem.ValueString, CultureInfo.InvariantCulture);
			}
			return 0u;
		}

		public static float GetFloat(string key)
		{
			SyncableItem syncableItem;
			if (!DataManager.s_localGameData.SyncableItems.TryGetValue(key, out syncableItem))
			{
				throw new KeyNotFoundException(key);
			}
			if (syncableItem.Metadata.DataType == DataType.Float)
			{
				return Convert.ToSingle(syncableItem.ValueString, CultureInfo.InvariantCulture);
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(float));
		}

		public static double GetDouble(string key)
		{
			SyncableItem syncableItem;
			if (!DataManager.s_localGameData.SyncableItems.TryGetValue(key, out syncableItem))
			{
				throw new KeyNotFoundException(key);
			}
			if (syncableItem.Metadata.DataType == DataType.Double)
			{
				return Convert.ToDouble(syncableItem.ValueString, CultureInfo.InvariantCulture);
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(double));
		}

		public static string GetString(string key)
		{
			SyncableItem syncableItem;
			if (!DataManager.s_localGameData.SyncableItems.TryGetValue(key, out syncableItem))
			{
				throw new KeyNotFoundException(key);
			}
			if (syncableItem.Metadata.DataType == DataType.String)
			{
				return syncableItem.ValueString;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(string));
		}

		public static long GetLong(string key)
		{
			SyncableItem syncableItem;
			if (!DataManager.s_localGameData.SyncableItems.TryGetValue(key, out syncableItem))
			{
				throw new KeyNotFoundException(key);
			}
			if (syncableItem.Metadata.DataType == DataType.Long)
			{
				return Convert.ToInt64(syncableItem.ValueString, CultureInfo.InvariantCulture);
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(long));
		}

		public static DateTime GetDateTime(string key)
		{
			SyncableItem syncableItem;
			if (!DataManager.s_localGameData.SyncableItems.TryGetValue(key, out syncableItem))
			{
				throw new KeyNotFoundException(key);
			}
			if (syncableItem.Metadata.DataType == DataType.Long)
			{
				return DateTime.FromBinary(Convert.ToInt64(syncableItem.ValueString, CultureInfo.InvariantCulture));
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(long));
		}

		public static decimal GetDecimal(string key)
		{
			SyncableItem syncableItem;
			if (!DataManager.s_localGameData.SyncableItems.TryGetValue(key, out syncableItem))
			{
				throw new KeyNotFoundException(key);
			}
			if (syncableItem.Metadata.DataType == DataType.Decimal)
			{
				return Convert.ToDecimal(syncableItem.ValueString, CultureInfo.InvariantCulture);
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(decimal));
		}

		public static void RefreshCloudValues()
		{
			foreach (KeyValuePair<string, IPersistent> keyValuePair in DataManager.CloudPrefs)
			{
				keyValuePair.Value.Load();
			}
		}

		public static void ResetSyncableCurrency(string key)
		{
			SyncableCurrency syncableCurrency;
			if (DataManager.s_localGameData.SyncableCurrencies.TryGetValue(key, out syncableCurrency))
			{
				syncableCurrency.ResetCurrency();
				DataManager.IsLocalDataDirty = true;
				return;
			}
			throw new KeyNotFoundException(key);
		}

		public static bool ResetCloudPref(string key)
		{
			if (DataManager.CloudPrefs.ContainsKey(key))
			{
				DataManager.CloudPrefs[key].Reset();
				return true;
			}
			return false;
		}

		public static bool DeleteCloudPref(string key)
		{
			if (DataManager.s_localGameData.SyncableItems.ContainsKey(key))
			{
				DataManager.s_localGameData.SyncableItems.Remove(key);
				return true;
			}
			if (DataManager.s_localGameData.SyncableCurrencies.ContainsKey(key))
			{
				DataManager.s_localGameData.SyncableCurrencies.Remove(key);
				return true;
			}
			return false;
		}

		public static string[] ResetAllData()
		{
			foreach (KeyValuePair<string, IPersistent> keyValuePair in DataManager.CloudPrefs)
			{
				keyValuePair.Value.Reset();
			}
			return DataManager.s_localGameData.GetAllKeys();
		}

		public static void DeleteAllCloudVariables()
		{
			DataManager.DeleteCloudData();
			DataManager.ClearStowawayVariablesFromGameData();
			foreach (KeyValuePair<string, IPersistent> keyValuePair in DataManager.CloudPrefs)
			{
				keyValuePair.Value.Reset();
			}
		}

		public static string[] ClearStowawayVariablesFromGameData()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, SyncableItem> keyValuePair in DataManager.s_localGameData.SyncableItems)
			{
				if (!DataManager.s_cloudPrefs.ContainsKey(keyValuePair.Key))
				{
					list.Add(keyValuePair.Key);
				}
			}
			List<string> list2 = new List<string>();
			foreach (KeyValuePair<string, SyncableCurrency> keyValuePair2 in DataManager.s_localGameData.SyncableCurrencies)
			{
				if (!DataManager.s_cloudPrefs.ContainsKey(keyValuePair2.Key))
				{
					list2.Add(keyValuePair2.Key);
				}
			}
			foreach (string key in list)
			{
				DataManager.s_localGameData.SyncableItems.Remove(key);
			}
			foreach (string text in list2)
			{
				DataManager.s_localGameData.SyncableCurrencies.Remove(text);
				list.Add(text);
			}
			return list.ToArray();
		}

		public static void SaveToDisk()
		{
			foreach (KeyValuePair<string, IPersistent> keyValuePair in DataManager.CloudPrefs)
			{
				keyValuePair.Value.Flush();
			}
			if (DataManager.IsLocalDataDirty)
			{
				PlayerPrefs.SetString("CloudOnceDevString", DataManager.SerializeLocalData().ToBase64String());
			}
		}

		public static void LoadFromDisk()
		{
			string text = PlayerPrefs.GetString("CloudOnceDevString");
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (!text.IsJson())
			{
				try
				{
					text = text.FromBase64StringToString();
				}
				catch (FormatException)
				{
					UnityEngine.Debug.LogWarning("Unable to deserialize local data!");
					return;
				}
			}
			if (!DataManager.s_isInitialized)
			{
				DataManager.s_localGameData = new GameData(text);
			}
			else
			{
				string[] array = DataManager.MergeLocalDataWith(text);
				if (array.Length > 0)
				{
					DataManager.RefreshCloudValues();
				}
			}
		}

		public static string SerializeLocalData()
		{
			return DataManager.s_localGameData.Serialize();
		}

		public static string[] MergeLocalDataWith(string otherData)
		{
			string[] array = DataManager.s_localGameData.MergeWith(new GameData(otherData));
			if (array.Length > 0)
			{
				DataManager.RefreshCloudValues();
				DataManager.SaveToDisk();
			}
			return array;
		}

		public static string[] ReplaceLocalDataWith(string otherData)
		{
			DataManager.s_localGameData = new GameData(otherData);
			foreach (KeyValuePair<string, IPersistent> keyValuePair in DataManager.CloudPrefs)
			{
				keyValuePair.Value.Reset();
			}
			DataManager.RefreshCloudValues();
			DataManager.SaveToDisk();
			return DataManager.s_localGameData.GetAllKeys();
		}

		private static void CreateItem(string key, SyncableItem item)
		{
			SyncableItem syncableItem;
			if (DataManager.s_localGameData.SyncableItems.TryGetValue(key, out syncableItem))
			{
				if (syncableItem.Metadata.PersistenceType == item.Metadata.PersistenceType && !syncableItem.Equals(item))
				{
					DataManager.s_localGameData.SyncableItems[key] = ConflictResolver.ResolveConflict(syncableItem, item);
					DataManager.IsLocalDataDirty = true;
				}
			}
			else
			{
				DataManager.s_localGameData.SyncableItems.Add(key, item);
				DataManager.IsLocalDataDirty = true;
			}
		}

		private static void DeleteCloudData()
		{
			if (CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated())
			{
				PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution("GameData", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, delegate(SavedGameRequestStatus status, ISavedGameMetadata metadata)
				{
					if (status == SavedGameRequestStatus.Success)
					{
						PlayGamesPlatform.Instance.SavedGame.Delete(metadata);
					}
				});
			}
			PlayerPrefs.DeleteKey("CloudOnceDevString");
			PlayerPrefs.Save();
		}

		public const string DevStringKey = "CloudOnceDevString";

		private static Dictionary<string, IPersistent> s_cloudPrefs;

		private static GameData s_localGameData = new GameData();

		private static bool s_isInitialized;
	}
}
