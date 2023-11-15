using System;
using TMPro;
using UnityEngine;

public class UIHapticsToggleButton : MonoBehaviour
{
	public void Awake()
	{
		HookedVibration.SetHapticsState((HapticsState)PlayerPrefs.GetInt("KEY_HAPTICS_SETTINGS", (int)HookedVibration.CurrentHapticsState));
		this.UpdateUI();
		AFKManager.Instance.OnUserLeaveCallback += this.Instance_OnUserLeaveCallback;
	}

	private void Instance_OnUserLeaveCallback(DateTime arg1, bool arg2)
	{
		PlayerPrefs.SetInt("KEY_HAPTICS_SETTINGS", (int)HookedVibration.CurrentHapticsState);
	}

	public void Toggle()
	{
		HookedVibration.ToggleHaptics();
		this.UpdateUI();
		SettingsManager.Instance.NotifySettingsChanged(SettingsType.Haptics, HookedVibration.CurrentHapticsState.ToString());
	}

	private void UpdateUI()
	{
		if (HookedVibration.CurrentHapticsState == HapticsState.Off)
		{
			this.label.SetText("Off");
		}
		else if (HookedVibration.CurrentHapticsState == HapticsState.OnForBossAndNewFishes)
		{
			this.label.SetText("Boss and new fish only");
		}
		if (HookedVibration.CurrentHapticsState == HapticsState.OnForAllFishes)
		{
			this.label.SetText("All fishes");
		}
	}

	private void OnDestroy()
	{
		AFKManager.Instance.OnUserLeaveCallback -= this.Instance_OnUserLeaveCallback;
	}

	private const string KEY_HAPTICS_SETTINGS = "KEY_HAPTICS_SETTINGS";

	[SerializeField]
	private TextMeshProUGUI label;
}
