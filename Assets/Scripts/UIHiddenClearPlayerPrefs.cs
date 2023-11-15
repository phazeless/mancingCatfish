using System;
using TMPro;
using UnityEngine;

public class UIHiddenClearPlayerPrefs : MonoBehaviour
{
	private void Awake()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void ClearPlayerPrefs()
	{
		EncryptedPlayerPrefs.DeleteAll();
		this.text.text = "Cleared!";
	}

	[SerializeField]
	private TextMeshProUGUI text;
}
