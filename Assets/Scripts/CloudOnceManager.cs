using System;
using System.Collections.Generic;
using CloudOnce;
using CloudOnce.CloudPrefs;
using CloudOnce.Internal.Utils;
using GooglePlayGames.OurUtils;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CloudOnceManager : MonoBehaviour
{
	public static CloudOnceManager Instance
	{
		get
		{
			return CloudOnceManager.instance;
		}
	}

	private bool HasSavedDataOnCloud
	{
		get
		{
			return this.cloudDataSavedAsJson != null && !string.IsNullOrEmpty(this.cloudDataSavedAsJson.Value);
		}
	}

	private string CurrentSceneName
	{
		get
		{
			return SceneManager.GetActiveScene().name;
		}
	}

	private bool IsGooglePlayGamesInstalled { get; set; }

	protected virtual void Awake()
	{
		if (CloudOnceManager.instance == null)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			SceneManager.sceneLoaded += this.SceneManager_sceneLoaded;
			CloudOnceManager.instance = this;
			bool flag = PlayerPrefs.HasKey("KEY_CASH_VALUE");
			this.IsGooglePlayGamesInstalled = PlatformUtils.Supported;
			if (!CloudOnceManagerHelper.ForcedLoadFromCloud)
			{
				if (flag || !this.IsGooglePlayGamesInstalled)
				{
					this.LoadMainScene();
				}
				else
				{
					this.Initialize(true);
				}
			}
			else
			{
				this.Initialize(true);
			}
		}
	}

	private void Initialize(bool fromCleanSlate)
	{
		this.initializedFromCleanSlate = fromCleanSlate;
		this.cloudDataSavedAsJson = new CloudString(CloudOnceManager.KEY_ALL_CLOUD_DATA, PersistenceType.Latest, string.Empty);
		if (fromCleanSlate)
		{
			Cloud.OnCloudLoadComplete += this.OnCloudLoadCompleteForCleanSlateUser;
		}
		else
		{
			Cloud.OnSignedInChanged += this.Cloud_OnSignedInChanged;
		}
		this.hasInitialized = true;
		CloudOnceManagerHelper.Init(this.cloudDataSavedAsJson);
		bool autoSignIn = !CloudOnceManagerHelper.HasRejectedAutoLoginCloudService;
		Cloud.OnInitializeComplete += this.Cloud_OnInitializeComplete;
		Cloud.Initialize(true, autoSignIn, fromCleanSlate);
	}

	private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode arg1)
	{
		if (scene.name == CloudOnceManager.MAIN_SCENE_NAME && this.IsGooglePlayGamesInstalled && !this.hasInitialized)
		{
			this.RunAfterDelay(10f, delegate()
			{
				this.Initialize(false);
			});
		}
	}

	public virtual void SaveDataToCloud()
	{
		if (TournamentManager.Instance != null && !TournamentManager.Instance.IsInsideTournament)
		{
			this.TrySaveDataToCloud();
		}
	}

	public virtual void Restart()
	{
		this.LoadMainScene();
	}

	public virtual void RestoreGameFromCloud()
	{
		if (this.HasSavedDataOnCloud)
		{
			this.TryLoadDataFromCloud();
		}
		else
		{
			UnityEngine.Debug.LogWarning("User shouldn't load data from the cloud in this state. Make sure user restarted from clean slate and that there actually exist data on the cloud.");
		}
	}

	public void UIDisableButtons(Button button)
	{
		button.interactable = false;
		this.restoreGameDialog.SetActive(false);
	}

	public virtual void SaveDataToCache()
	{
		if (TournamentManager.Instance != null && !TournamentManager.Instance.IsInsideTournament)
		{
			SettingsManager.Instance.Save();
			FishBook.Instance.SaveFishBook();
			TutorialManager.Instance.SaveTutorialCompleteStates();
			ResourceManager.Instance.SaveGems();
			ResourceManager.Instance.SaveCash();
			SkillManager.Instance.SaveCurrentStateOfSkills(false);
			ItemManager.Instance.SaveCurrentStateOfItems();
			ConsumableManager.Instance.Save();
			DailyGiftManager.Instance.Save();
		}
	}

	private void OnCloudLoadCompleteForCleanSlateUser(bool arg0)
	{
		UnityEngine.Debug.Log("ABC: OnCloudLoadCompleteForCleanSlateUser, successfully loaded data: " + arg0);
		UnityEngine.Debug.Log("ABC: OnCloudLoadCompleteForCleanSlateUser, IsLoggedIn: " + Cloud.IsSignedIn);
		Cloud.OnCloudLoadComplete -= this.OnCloudLoadCompleteForCleanSlateUser;
		if (!CloudOnceManagerHelper.ForcedLoadFromCloud)
		{
			if (this.HasSavedDataOnCloud)
			{
				this.restoreGameDialog.SetActive(true);
			}
			else
			{
				this.LoadMainScene();
			}
		}
		else
		{
			this.RestoreGameFromCloud();
		}
	}

	private void Cloud_OnInitializeComplete()
	{
		Cloud.OnInitializeComplete -= this.Cloud_OnInitializeComplete;
	}

	private void Cloud_OnSignedInChanged(bool didLogin)
	{
		UnityEngine.Debug.Log("ABC: Cloud_OnSignedInChanged, didLogin: " + didLogin);
		Cloud.OnSignedInChanged -= this.Cloud_OnSignedInChanged;
		if (didLogin && !TournamentManager.Instance.IsInsideTournament)
		{
			this.SaveDataToCache();
			this.SaveDataToCloud();
		}
	}

	private void TrySaveDataToCloud()
	{
		string text = JsonConvert.SerializeObject(EncryptedPlayerPrefs.CachedPlayerPrefs, Formatting.None);
		if (this.cloudDataSavedAsJson != null && text != null)
		{
			this.cloudDataSavedAsJson.Value = text.ToBase64String();
			Cloud.Storage.Save();
		}
		PlayerPrefs.SetInt(CloudOnceManager.KEY_HAS_LOCAL_DATA, 1);
	}

	private void TryLoadDataFromCloud()
	{
		string value = this.cloudDataSavedAsJson.Value.FromBase64StringToString();
		try
		{
			CloudOnceManagerHelper.ForcedLoadFromCloud = false;
			Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
			if (dictionary != null)
			{
				EncryptedPlayerPrefs.SetCachedPlayerPrefs(dictionary);
				EncryptedPlayerPrefs.Save();
			}
			else
			{
				UnityEngine.Debug.Log("dict is null");
			}
			this.LoadMainScene();
		}
		catch (Exception ex)
		{
			this.LoadMainScene();
			UnityEngine.Debug.LogError("Failed to deserialize Cloud Data. It may be corrupt. Error: " + ex.Message);
		}
	}

	public void LoadMainScene()
	{
		SceneManager.LoadScene("Main");
	}

	private static readonly string MAIN_SCENE_NAME = "Main";

	public static readonly string KEY_ALL_CLOUD_DATA = "KEY_ALL_CLOUD_DATA";

	public static readonly string KEY_HAS_LOCAL_DATA = "KEY_HAS_LOCAL_DATA";

	private static CloudOnceManager instance;

	[SerializeField]
	private GameObject restoreGameDialog;

	private bool initializedFromCleanSlate;

	private bool hasInitialized;

	private CloudString cloudDataSavedAsJson;
}
