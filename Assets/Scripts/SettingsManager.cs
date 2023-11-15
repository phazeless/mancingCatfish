using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
	public static SettingsManager Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<string> OnNameInitialized;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<SettingsType, string> OnSettingsChanged;

	public void NotifySettingsChanged(SettingsType type, string changeValue)
	{
		if (this.OnSettingsChanged != null)
		{
			this.OnSettingsChanged(type, changeValue);
		}
	}

	public string PlayerName
	{
		get
		{
			return this.playerName;
		}
	}

	private void Awake()
	{
		SettingsManager.Instance = this;
	}

	private void Start()
	{
		this.Load();
		if (!this.WasMusicOn)
		{
			this.musicAudioSource.Pause();
		}
		this.shader.enabled = this.IsHighQuality;
		this.volumeSlider.value = SettingsManager.masterVolume;
		this.hasHadTheChanceToCallStart = true;
	}

	public void SetMasterVolume()
	{
		SettingsManager.masterVolume = this.volumeSlider.value;
		AudioListener.volume = SettingsManager.masterVolume;
	}

	public void SetPlayerName(string playerName, bool notifyChange, bool changedFromSettingsMenu = false)
	{
		this.playerName = playerName;
		if (this.OnNameInitialized != null && notifyChange)
		{
			this.OnNameInitialized(playerName);
		}
		if (changedFromSettingsMenu && this.OnSettingsChanged != null)
		{
			this.NotifySettingsChanged(SettingsType.Username, playerName);
		}
		if (playerName.ToLower() == "showmaxdebugger")
		{
			
		}
		else if (playerName.ToLower() == "liontestuser2019")
		{
			TimeManager.Instance.OverrideUseLocalTime();
		}
	}

	public void OpenSettingsDialog()
	{
		this.settingsDialog.Open();
	}

	public void CloseSettingsDialog()
	{
		this.settingsDialog.Close(false);
	}

	private void Load()
	{
		this.WasMusicOn = (EncryptedPlayerPrefs.GetInt(SettingsManager.KEY_PLAY_MUSIC, 1) == 1);
		this.IsHighQuality = (EncryptedPlayerPrefs.GetInt(SettingsManager.KEY_QUALITY, 1) == 1);
		SettingsManager.masterVolume = EncryptedPlayerPrefs.GetFloat(SettingsManager.KEY_MASTER_VOLUME, 1f);
		string text = EncryptedPlayerPrefs.GetString(SettingsManager.KEY_PLAYER_NAME, null);
		bool flag = text != null;
		if (flag)
		{
			this.SetPlayerName(text, true, false);
		}
		else
		{
			text = "User" + UnityEngine.Random.Range(0, 99999999);
			this.SetPlayerName(text, true, false);
			if (SkillManager.Instance.UnlockCrewMemberSkill.CurrentLevel > 0)
			{
				this.OpenSettingsDialog();
			}
		}
	}

	public void Save()
	{
		EncryptedPlayerPrefs.SetFloat(SettingsManager.KEY_MASTER_VOLUME, SettingsManager.masterVolume, true);
		EncryptedPlayerPrefs.SetInt(SettingsManager.KEY_PLAY_MUSIC, (!this.musicAudioSource.isPlaying) ? 0 : 1, true);
		EncryptedPlayerPrefs.SetInt(SettingsManager.KEY_QUALITY, (!this.IsHighQuality) ? 0 : 1, true);
		EncryptedPlayerPrefs.SetString(SettingsManager.KEY_PLAYER_NAME, this.PlayerName, true);
	}

	private void OnApplicationPause(bool didPause)
	{
		if (this.hasHadTheChanceToCallStart && didPause)
		{
			this.Save();
		}
	}

	private static readonly string KEY_MASTER_VOLUME = "KEY_MASTER_VOLUME";

	private static readonly string KEY_PLAY_MUSIC = "KEY_PLAY_MUSIC";

	private static readonly string KEY_PLAYER_NAME = "KEY_PLAYER_NAME";

	private static readonly string KEY_QUALITY = "KEY_QUALITY";

	[SerializeField]
	private AudioSource musicAudioSource;

	[SerializeField]
	private MeshRenderer shader;

	[SerializeField]
	private UISettingDialog settingsDialog;

	[SerializeField]
	private Slider volumeSlider;

	private string playerName;

	private bool hasHadTheChanceToCallStart;

	public static float masterVolume = 1f;

	public bool WasMusicOn = true;

	public bool IsHighQuality = true;
}
