using System;
using System.Diagnostics;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<int> OnCheatDetected;

	public static CheatManager Instance { get; private set; }

	private void Awake()
	{
		CheatManager.Instance = this;
		this.Load();
	}

	private void Start()
	{
		this.CheckForCheat();
	}

	private void CheckForCheat()
	{
		if (DateTime.Now < this.lastRegisteredTime)
		{
			this.lastRegisteredTime = DateTime.Now;
			this.cheatCount++;
			if (this.cheatCount == 1)
			{
				UnityEngine.Debug.LogWarning("Cheat: User has cheated 1 times.");
			}
			if (this.cheatCount == 5)
			{
				UnityEngine.Debug.LogWarning("Cheat: User has cheated 5 times.");
			}
			if (this.cheatCount == 10)
			{
				UnityEngine.Debug.LogWarning("Cheat: User has cheated 10 times.");
			}
			if (this.cheatCount == 30)
			{
				UnityEngine.Debug.LogWarning("Cheat: User has cheated 30 times.");
			}
			this.cheatProtectionDialog.Open();
			if (this.OnCheatDetected != null)
			{
				this.OnCheatDetected(this.cheatCount);
			}
		}
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause)
		{
			this.lastRegisteredTime = DateTime.Now;
			this.Save();
		}
		else
		{
			this.CheckForCheat();
		}
	}

	private void Load()
	{
		this.timeAtFirstStartup = new DateTime(long.Parse(EncryptedPlayerPrefs.GetString(CheatManager.KEY_TIME_AT_FIRST_STARTUP, DateTime.Now.Ticks.ToString())));
		this.lastRegisteredTime = new DateTime(long.Parse(EncryptedPlayerPrefs.GetString(CheatManager.KEY_LAST_REGISTERED_TIME, "0")));
		this.cheatCount = EncryptedPlayerPrefs.GetInt(CheatManager.KEY_CHEAT_COUNT, 0);
	}

	private void Save()
	{
		EncryptedPlayerPrefs.SetString(CheatManager.KEY_TIME_AT_FIRST_STARTUP, this.timeAtFirstStartup.Ticks.ToString(), true);
		EncryptedPlayerPrefs.SetString(CheatManager.KEY_LAST_REGISTERED_TIME, this.lastRegisteredTime.Ticks.ToString(), true);
		EncryptedPlayerPrefs.SetInt(CheatManager.KEY_CHEAT_COUNT, this.cheatCount, true);
	}

	private static readonly string KEY_TIME_AT_FIRST_STARTUP = "KEY_TIME_AT_FIRST_STARTUP";

	private static readonly string KEY_LAST_REGISTERED_TIME = "KEY_LAST_REGISTERED_TIME";

	private static readonly string KEY_CHEAT_COUNT = "KEY_CHEAT_COUNT";

	[SerializeField]
	private CheatProtectionDialog cheatProtectionDialog;

	private DateTime timeAtFirstStartup = DateTime.Now;

	private DateTime lastRegisteredTime = DateTime.Now;

	private int cheatCount;
}
