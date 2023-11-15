using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CloudOnce.QuickStart
{
	[AddComponentMenu("CloudOnce/Show Achievements Button", 3)]
	public class AchievementsButton : MonoBehaviour
	{
		private static void OnSignedInChanged(bool isSignedIn)
		{
			if (AchievementsButton._003C_003Ef__mg_0024cache0 == null)
			{
				AchievementsButton._003C_003Ef__mg_0024cache0 = new UnityAction<bool>(AchievementsButton.OnSignedInChanged);
			}
			Cloud.OnSignedInChanged -= AchievementsButton._003C_003Ef__mg_0024cache0;
			if (isSignedIn)
			{
				Cloud.Achievements.ShowOverlay();
			}
		}

		private static void SubscribeEvent()
		{
			if (AchievementsButton._003C_003Ef__mg_0024cache1 == null)
			{
				AchievementsButton._003C_003Ef__mg_0024cache1 = new UnityAction<bool>(AchievementsButton.OnSignedInChanged);
			}
			Cloud.OnSignedInChanged -= AchievementsButton._003C_003Ef__mg_0024cache1;
			if (AchievementsButton._003C_003Ef__mg_0024cache2 == null)
			{
				AchievementsButton._003C_003Ef__mg_0024cache2 = new UnityAction<bool>(AchievementsButton.OnSignedInChanged);
			}
			Cloud.OnSignedInChanged += AchievementsButton._003C_003Ef__mg_0024cache2;
		}

		private static void OnButtonClicked()
		{
			if (Cloud.IsSignedIn)
			{
				Cloud.Achievements.ShowOverlay();
			}
			else
			{
				AchievementsButton.SubscribeEvent();
				Cloud.SignIn(true, null);
			}
		}

		private void Awake()
		{
			this.button = base.GetComponent<Button>();
			if (this.button == null)
			{
				UnityEngine.Debug.LogError("Show Achievements Button script placed on GameObject that is not a button. Script is only compatible with UI buttons created from GameOject menu (GameObjects -> UI -> Button).");
			}
		}

		private void Start()
		{
			UnityEvent onClick = this.button.onClick;
			if (AchievementsButton._003C_003Ef__mg_0024cache3 == null)
			{
				AchievementsButton._003C_003Ef__mg_0024cache3 = new UnityAction(AchievementsButton.OnButtonClicked);
			}
			onClick.AddListener(AchievementsButton._003C_003Ef__mg_0024cache3);
		}

		private void OnDestroy()
		{
			UnityEvent onClick = this.button.onClick;
			if (AchievementsButton._003C_003Ef__mg_0024cache4 == null)
			{
				AchievementsButton._003C_003Ef__mg_0024cache4 = new UnityAction(AchievementsButton.OnButtonClicked);
			}
			onClick.RemoveListener(AchievementsButton._003C_003Ef__mg_0024cache4);
			if (AchievementsButton._003C_003Ef__mg_0024cache5 == null)
			{
				AchievementsButton._003C_003Ef__mg_0024cache5 = new UnityAction<bool>(AchievementsButton.OnSignedInChanged);
			}
			Cloud.OnSignedInChanged -= AchievementsButton._003C_003Ef__mg_0024cache5;
		}

		private Button button;

		[CompilerGenerated]
		private static UnityAction<bool> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static UnityAction<bool> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static UnityAction<bool> _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static UnityAction _003C_003Ef__mg_0024cache4;

		[CompilerGenerated]
		private static UnityAction<bool> _003C_003Ef__mg_0024cache5;
	}
}
