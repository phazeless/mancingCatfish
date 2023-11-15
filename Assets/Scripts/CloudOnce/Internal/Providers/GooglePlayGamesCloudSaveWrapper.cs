using System;
using System.Text;
using CloudOnce.Internal.Utils;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.OurUtils;
using UnityEngine;

namespace CloudOnce.Internal.Providers
{
	public class GooglePlayGamesCloudSaveWrapper : ICloudStorageProvider
	{
		public GooglePlayGamesCloudSaveWrapper(CloudOnceEvents events)
		{
			this.cloudOnceEvents = events;
		}

		public void Save()
		{
			if (GooglePlayGamesCloudSaveWrapper.s_saveInitialized)
			{
				return;
			}
			GooglePlayGamesCloudSaveWrapper.s_saveInitialized = true;
			DataManager.SaveToDisk();
			if (!CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.CloudSaveInitialized || !Cloud.CloudSaveEnabled)
			{
				GooglePlayGamesCloudSaveWrapper.s_saveInitialized = false;
				this.cloudOnceEvents.RaiseOnCloudSaveComplete(false);
				return;
			}
			if (DataManager.IsLocalDataDirty)
			{
				if (CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated())
				{
					PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution("GameData", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, new Action<SavedGameRequestStatus, ISavedGameMetadata>(this.OnSavedGameOpenedForSave));
				}
				else
				{
					GooglePlayGamesCloudSaveWrapper.s_saveInitialized = false;
					this.cloudOnceEvents.RaiseOnCloudSaveComplete(false);
				}
			}
			else
			{
				GooglePlayGamesCloudSaveWrapper.s_saveInitialized = false;
				this.cloudOnceEvents.RaiseOnCloudSaveComplete(false);
			}
		}

		public void Load()
		{
			if (!CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.CloudSaveInitialized || !Cloud.CloudSaveEnabled)
			{
				this.cloudOnceEvents.RaiseOnCloudLoadComplete(false);
				return;
			}
			if (GooglePlayGamesCloudSaveWrapper.s_loadInitialized)
			{
				return;
			}
			GooglePlayGamesCloudSaveWrapper.s_loadInitialized = true;
			if (CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.d("Loading default save game.");
				PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution("GameData", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, new Action<SavedGameRequestStatus, ISavedGameMetadata>(this.OnSavedGameOpenedForLoad));
			}
			else
			{
				GooglePlayGames.OurUtils.Logger.w("Load can only be called after authentication.");
				this.OnSavedGameDataRead(SavedGameRequestStatus.AuthenticationError, null);
			}
		}

		public void Synchronize()
		{
			if (GooglePlayGamesCloudSaveWrapper.s_isSynchronising)
			{
				return;
			}
			GooglePlayGamesCloudSaveWrapper.s_isSynchronising = true;
			Cloud.OnCloudLoadComplete += this.OnCloudLoadComplete;
			this.Load();
		}

		public bool ResetVariable(string key)
		{
			return DataManager.ResetCloudPref(key);
		}

		public bool DeleteVariable(string key)
		{
			return DataManager.DeleteCloudPref(key);
		}

		public string[] ClearUnusedVariables()
		{
			return DataManager.ClearStowawayVariablesFromGameData();
		}

		public void DeleteAll()
		{
			DataManager.DeleteAllCloudVariables();
		}

		public void SubscribeToAuthenticationEvent()
		{
			
		}

		private static byte[] StringToBytes(string s)
		{
			if (s == null)
			{
				s = string.Empty;
			}
			return Encoding.Default.GetBytes(s);
		}

		private static string BytesToString(byte[] bytes)
		{
			return Encoding.Default.GetString(bytes);
		}

		private void OnSavedGameOpenedForLoad(SavedGameRequestStatus status, ISavedGameMetadata game)
		{
			if (status == SavedGameRequestStatus.Success)
			{
				PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, new Action<SavedGameRequestStatus, byte[]>(this.OnSavedGameDataRead));
			}
			else
			{
				GooglePlayGamesCloudSaveWrapper.s_loadInitialized = false;
				GooglePlayGames.OurUtils.Logger.w("Failed to open saved game. Request status: " + status);
				this.cloudOnceEvents.RaiseOnCloudLoadComplete(false);
			}
		}

		private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
		{
			if (status == SavedGameRequestStatus.Success)
			{
				GooglePlayGamesCloudSaveWrapper.s_timeWhenCloudSaveWasLoaded = Time.realtimeSinceStartup;
				this.ProcessCloudData(data);
			}
			else
			{
				GooglePlayGamesCloudSaveWrapper.s_loadInitialized = false;
				GooglePlayGames.OurUtils.Logger.w("Failed to load saved game. Request status: " + status);
				this.cloudOnceEvents.RaiseOnCloudLoadComplete(false);
			}
		}

		private void OnSavedGameOpenedForSave(SavedGameRequestStatus status, ISavedGameMetadata game)
		{
			if (status == SavedGameRequestStatus.Success)
			{
				TimeSpan timeSpan = game.TotalTimePlayed;
				timeSpan += TimeSpan.FromSeconds((double)(Time.realtimeSinceStartup - GooglePlayGamesCloudSaveWrapper.s_timeWhenCloudSaveWasLoaded));
				this.SaveGame(game, GooglePlayGamesCloudSaveWrapper.StringToBytes(DataManager.SerializeLocalData().ToBase64String()), timeSpan);
			}
			else
			{
				GooglePlayGamesCloudSaveWrapper.s_saveInitialized = false;
				GooglePlayGames.OurUtils.Logger.w("Failed to open saved game. Request status: " + status);
				this.cloudOnceEvents.RaiseOnCloudSaveComplete(false);
			}
		}

		private void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
		{
			ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
			savedGame.CommitUpdate(game, default(SavedGameMetadataUpdate.Builder).WithUpdatedPlayedTime(totalPlaytime).WithUpdatedDescription("Saved game at " + DateTime.Now).Build(), savedData, new Action<SavedGameRequestStatus, ISavedGameMetadata>(this.OnSavedGameWritten));
		}

		private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
		{
			if (status == SavedGameRequestStatus.Success)
			{
				GooglePlayGames.OurUtils.Logger.d("Save successful!");
				DataManager.IsLocalDataDirty = false;
				this.cloudOnceEvents.RaiseOnCloudSaveComplete(true);
			}
			else
			{
				GooglePlayGames.OurUtils.Logger.w("Failed to write saved game. Request status: " + status);
				this.cloudOnceEvents.RaiseOnCloudSaveComplete(false);
			}
			GooglePlayGamesCloudSaveWrapper.s_saveInitialized = false;
		}

		private void ProcessCloudData(byte[] cloudData)
		{
			if (cloudData == null)
			{
				GooglePlayGamesCloudSaveWrapper.s_loadInitialized = false;
				this.cloudOnceEvents.RaiseOnCloudLoadComplete(true);
				return;
			}
			string text = GooglePlayGamesCloudSaveWrapper.BytesToString(cloudData);
			if (!string.IsNullOrEmpty(text))
			{
				if (!text.IsJson())
				{
					try
					{
						text = text.FromBase64StringToString();
					}
					catch (FormatException)
					{
						UnityEngine.Debug.LogWarning("Unable to deserialize cloud data!");
						this.cloudOnceEvents.RaiseOnCloudLoadComplete(false);
						return;
					}
				}
				string[] array = DataManager.MergeLocalDataWith(text);
				if (array.Length > 0)
				{
					this.cloudOnceEvents.RaiseOnNewCloudValues(array);
				}
				GooglePlayGamesCloudSaveWrapper.s_loadInitialized = false;
				this.cloudOnceEvents.RaiseOnCloudLoadComplete(true);
			}
			else
			{
				GooglePlayGamesCloudSaveWrapper.s_loadInitialized = false;
				this.cloudOnceEvents.RaiseOnCloudLoadComplete(true);
			}
		}

		private void OnCloudLoadComplete(bool arg0)
		{
			Cloud.OnCloudLoadComplete -= this.OnCloudLoadComplete;
			this.Save();
			GooglePlayGamesCloudSaveWrapper.s_isSynchronising = false;
		}

		private const string saveGameFileName = "GameData";

		private static float s_timeWhenCloudSaveWasLoaded;

		private static bool s_saveInitialized;

		private static bool s_loadInitialized;

		private static bool s_isSynchronising;

		private readonly CloudOnceEvents cloudOnceEvents;
	}
}
