using System;
using TMPro;
using UnityEngine;

public class UIMusicOnOffButton : MonoBehaviour
{
	private void Start()
	{
		this.SetLabel(SettingsManager.Instance.WasMusicOn);
	}

	public void Toggle()
	{
		if (this.musicAudioSource.isPlaying)
		{
			this.musicAudioSource.Pause();
			this.SetLabel(false);
		}
		else
		{
			this.musicAudioSource.UnPause();
			this.SetLabel(true);
		}
		SettingsManager.Instance.NotifySettingsChanged(SettingsType.Music, this.musicAudioSource.isPlaying.ToString());
	}

	private void SetLabel(bool isOn)
	{
		if (isOn)
		{
			this.label.SetVariableText(new string[]
			{
				"ON"
			});
		}
		else
		{
			this.label.SetVariableText(new string[]
			{
				"OFF"
			});
		}
	}

	[SerializeField]
	private AudioSource musicAudioSource;

	[SerializeField]
	private TextMeshProUGUI label;
}
