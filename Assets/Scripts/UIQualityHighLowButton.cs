using System;
using TMPro;
using UnityEngine;

public class UIQualityHighLowButton : MonoBehaviour
{
	private void Start()
	{
		this.SetLabel(SettingsManager.Instance.IsHighQuality);
	}

	public void Toggle()
	{
		if (SettingsManager.Instance.IsHighQuality)
		{
			this.shader.enabled = false;
			this.SetLabel(false);
			SettingsManager.Instance.IsHighQuality = false;
		}
		else
		{
			this.shader.enabled = true;
			this.SetLabel(true);
			SettingsManager.Instance.IsHighQuality = true;
		}
		SettingsManager.Instance.NotifySettingsChanged(SettingsType.Quality, SettingsManager.Instance.IsHighQuality.ToString());
	}

	private void SetLabel(bool isOn)
	{
		if (isOn)
		{
			this.label.SetVariableText(new string[]
			{
				"HIGH"
			});
		}
		else
		{
			this.label.SetVariableText(new string[]
			{
				"LOW"
			});
		}
	}

	[SerializeField]
	private MeshRenderer shader;

	[SerializeField]
	private TextMeshProUGUI label;
}
