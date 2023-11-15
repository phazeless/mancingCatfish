using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CloudOnce.QuickStart
{
	[AddComponentMenu("CloudOnce/Show Leaderboards Button", 4)]
	public class LeaderboardsButton : MonoBehaviour
	{
		private static void OnSignedInChanged(bool isSignedIn)
		{
			if (LeaderboardsButton._003C_003Ef__mg_0024cache0 == null)
			{
				LeaderboardsButton._003C_003Ef__mg_0024cache0 = new UnityAction<bool>(LeaderboardsButton.OnSignedInChanged);
			}
			Cloud.OnSignedInChanged -= LeaderboardsButton._003C_003Ef__mg_0024cache0;
			if (isSignedIn)
			{
				Cloud.Leaderboards.ShowOverlay(string.Empty);
			}
		}

		private static void SubscribeEvent()
		{
			if (LeaderboardsButton._003C_003Ef__mg_0024cache1 == null)
			{
				LeaderboardsButton._003C_003Ef__mg_0024cache1 = new UnityAction<bool>(LeaderboardsButton.OnSignedInChanged);
			}
			Cloud.OnSignedInChanged -= LeaderboardsButton._003C_003Ef__mg_0024cache1;
			if (LeaderboardsButton._003C_003Ef__mg_0024cache2 == null)
			{
				LeaderboardsButton._003C_003Ef__mg_0024cache2 = new UnityAction<bool>(LeaderboardsButton.OnSignedInChanged);
			}
			Cloud.OnSignedInChanged += LeaderboardsButton._003C_003Ef__mg_0024cache2;
		}

		private void OnButtonClicked()
		{
			if (Cloud.IsSignedIn)
			{
				Cloud.Leaderboards.ShowOverlay(string.Empty);
			}
			else
			{
				LeaderboardsButton.SubscribeEvent();
				Cloud.SignIn(true, null);
			}
		}

		private void Awake()
		{
			this.button = base.GetComponent<Button>();
			if (this.button == null)
			{
				UnityEngine.Debug.LogError("Show Leaderboards Button script placed on GameObject that is not a button. Script is only compatible with UI buttons created from GameOject menu (GameObjects -> UI -> Button).");
			}
		}

		private void Start()
		{
			this.button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
		}

		private void OnDestroy()
		{
			this.button.onClick.RemoveListener(new UnityAction(this.OnButtonClicked));
			if (LeaderboardsButton._003C_003Ef__mg_0024cache3 == null)
			{
				LeaderboardsButton._003C_003Ef__mg_0024cache3 = new UnityAction<bool>(LeaderboardsButton.OnSignedInChanged);
			}
			Cloud.OnSignedInChanged -= LeaderboardsButton._003C_003Ef__mg_0024cache3;
		}

		private Button button;

		[CompilerGenerated]
		private static UnityAction<bool> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static UnityAction<bool> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static UnityAction<bool> _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static UnityAction<bool> _003C_003Ef__mg_0024cache3;
	}
}
