using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIPlayerNameButton : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Start()
	{
		this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnTextSubmitted));
		this.SetLabel(SettingsManager.Instance.PlayerName);
	}

	private void OnDestroy()
	{
		this.inputField.onEndEdit.RemoveListener(new UnityAction<string>(this.OnTextSubmitted));
	}

	private void OnTextSubmitted(string text)
	{
		if (string.IsNullOrEmpty(text) || text == " ")
		{
			return;
		}
		this.SetLabel(text);
		SettingsManager.Instance.SetPlayerName(text, true, true);
	}

	public void SetLabel(string name)
	{
		if (!string.IsNullOrEmpty(name))
		{
			this.inputField.text = name;
		}
	}

	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private TMP_InputField inputField;
}
