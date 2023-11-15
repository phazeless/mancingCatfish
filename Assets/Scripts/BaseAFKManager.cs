using System;
using System.Diagnostics;
using UnityEngine;

public abstract class BaseAFKManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<bool, DateTime, float> OnUserReturnCallback;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<DateTime, bool> OnUserLeaveCallback;

	protected virtual void OnUserReturn(bool fromAppRestart, DateTime time, float afkTimeInSeconds)
	{
		if (this.OnUserReturnCallback != null)
		{
			this.OnUserReturnCallback(fromAppRestart, time, afkTimeInSeconds);
		}
	}

	protected virtual void OnUserLeave(DateTime time, bool fromApplicationQuit)
	{
		if (this.hasHadTheChanceToCallStart && this.OnUserLeaveCallback != null)
		{
			this.OnUserLeaveCallback(time, fromApplicationQuit);
		}
	}

	protected virtual void Start()
	{
		this.LoadWentOfflineTime();
		this.lastAFKTimeInSeconds = (float)(DateTime.Now - this.wentOffline).TotalSeconds;
		this.OnUserReturn(true, DateTime.Now, this.lastAFKTimeInSeconds);
		this.hasHadTheChanceToCallStart = true;
	}

	private void OnApplicationPause(bool didLeaveApplication)
	{
		if (didLeaveApplication)
		{
			this.SaveWentOfflineTime();
			this.OnUserLeave(DateTime.Now, false);
		}
		else
		{
			this.lastAFKTimeInSeconds = (float)(DateTime.Now - this.wentOffline).TotalSeconds;
			this.OnUserReturn(false, DateTime.Now, this.lastAFKTimeInSeconds);
		}
	}

	private void OnApplicationQuit()
	{
		this.SaveWentOfflineTime();
		this.OnUserLeave(DateTime.Now, true);
	}

	private void SaveWentOfflineTime()
	{
		this.wentOffline = DateTime.Now;
		EncryptedPlayerPrefs.SetString("wentOffline", this.wentOffline.Ticks.ToString(), true);
	}

	private void LoadWentOfflineTime()
	{
		this.wentOffline = new DateTime(long.Parse(EncryptedPlayerPrefs.GetString("wentOffline", "0")));
	}

	private DateTime wentOffline = DateTime.Now;

	protected float lastAFKTimeInSeconds;

	private bool hasHadTheChanceToCallStart;
}
