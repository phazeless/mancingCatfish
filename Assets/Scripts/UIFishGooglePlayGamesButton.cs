using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFishGooglePlayGamesButton : MonoBehaviour
{
	public void UpdateUI(bool isLoggedIn, bool hasInitialized, bool didFail = false)
	{
		if (hasInitialized)
		{
			if (isLoggedIn)
			{
				this.btnText.SetVariableText(new string[]
				{
					"<color=#D4FFC7FF>CONNECTED</color>"
				});
			}
			else
			{
				this.btnText.SetVariableText(new string[]
				{
					"<color=#FF9A9AFF>DISCONNECTED</color>"
				});
			}
		}
		else
		{
			this.btnText.SetVariableText(new string[]
			{
				"INITIALIZING"
			});
		}
	}

	[SerializeField]
	private TextMeshProUGUI btnText;

	[SerializeField]
	private Image bg;
}
