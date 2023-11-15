using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CloudOnce.QuickStart
{
	[AddComponentMenu("CloudOnce/Google Sign In-Out Button", 5)]
	public class GoogleSignOutButton : MonoBehaviour
	{
		private Button CachedButton
		{
			get
			{
				Button result;
				if ((result = this.cachedButton) == null)
				{
					result = (this.cachedButton = base.GetComponent<Button>());
				}
				return result;
			}
		}

		private Text TextComponent
		{
			get
			{
				Text result;
				if ((result = this.textComponent) == null)
				{
					result = (this.textComponent = base.GetComponentInChildren<Text>());
				}
				return result;
			}
		}

		private void UpdateButtonText(bool isSignedIn)
		{
			this.TextComponent.text = ((!isSignedIn) ? "Sign in" : "Sign out");
		}

		private void Awake()
		{
			Cloud.OnSignedInChanged += this.UpdateButtonText;
			if (this.CachedButton != null)
			{
				this.CachedButton.onClick.AddListener(new UnityAction(this.OnButtonClicked));
				this.UpdateButtonText(Cloud.IsSignedIn);
				return;
			}
			UnityEngine.Debug.LogError("Google Sign In/Out Button script placed on GameObject that is not a button. Script is only compatible with UI buttons created from GameOject menu (GameObjects -> UI -> Button).");
		}

		private void OnButtonClicked()
		{
			if (Cloud.IsSignedIn)
			{
				Cloud.SignOut();
			}
			else
			{
				Cloud.SignIn(true, null);
			}
		}

		private void OnEnable()
		{
			this.UpdateButtonText(Cloud.IsSignedIn);
		}

		private void OnDestroy()
		{
			Cloud.OnSignedInChanged -= this.UpdateButtonText;
		}

		private Button cachedButton;

		private Text textComponent;
	}
}
