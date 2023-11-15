using System;
using System.Runtime.CompilerServices;
using CloudOnce.Internal.Providers;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Utils
{
	public class GoogleLeaderboardUtils : ILeaderboardUtils
	{
		public void SubmitScore(string id, long score, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				GoogleLeaderboardUtils.ReportError(string.Format("Can't submit score to {0} leaderboard. Platform ID is null or empty!", internalID), onComplete);
				return;
			}
			if (!PlayGamesPlatform.Instance.IsAuthenticated())
			{
				GoogleLeaderboardUtils.ReportError(string.Format("Can't submit score to leaderboard {0} ({1}). SubmitScore can only be called after authentication.", internalID, id), onComplete);
				return;
			}
			Action<bool> callback = delegate(bool response)
			{
				GoogleLeaderboardUtils.OnSubmitScoreCompleted(response, score, onComplete, id, internalID);
			};
			PlayGamesPlatform.Instance.ReportScore(score, id, callback);
		}

		public void ShowOverlay(string id = "", string internalID = "")
		{
			if (!PlayGamesPlatform.Instance.IsAuthenticated())
			{
				return;
			}
			if (string.IsNullOrEmpty(id))
			{
				PlayGamesPlatform instance = PlayGamesPlatform.Instance;
				string leaderboardId = null;
				if (GoogleLeaderboardUtils._003C_003Ef__mg_0024cache0 == null)
				{
					GoogleLeaderboardUtils._003C_003Ef__mg_0024cache0 = new Action<UIStatus>(GoogleLeaderboardUtils.OnShowOverlayCompleted);
				}
				instance.ShowLeaderboardUI(leaderboardId, GoogleLeaderboardUtils._003C_003Ef__mg_0024cache0);
			}
			else
			{
				PlayGamesPlatform instance2 = PlayGamesPlatform.Instance;
				if (GoogleLeaderboardUtils._003C_003Ef__mg_0024cache1 == null)
				{
					GoogleLeaderboardUtils._003C_003Ef__mg_0024cache1 = new Action<UIStatus>(GoogleLeaderboardUtils.OnShowOverlayCompleted);
				}
				instance2.ShowLeaderboardUI(id, GoogleLeaderboardUtils._003C_003Ef__mg_0024cache1);
			}
		}

		public void LoadScores(string leaderboardID, Action<IScore[]> callback)
		{
			PlayGamesPlatform.Instance.LoadScores(leaderboardID, callback);
		}

		private static void OnShowOverlayCompleted(UIStatus callback)
		{
			if (callback == UIStatus.NotAuthorized)
			{
				CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.ActivateGuestUserMode();
			}
		}

		private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
		{
			CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
		}

		private static void OnSubmitScoreCompleted(bool response, long score, Action<CloudRequestResult<bool>> callbackAction, string id, string internalID)
		{
			if (response)
			{
				CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(true));
			}
			else
			{
				string errorMessage = string.Format("Native API failed to submit a score of {0} to {1} ({2}) leaderboard. Cause unknown.", score, internalID, id);
				GoogleLeaderboardUtils.ReportError(errorMessage, callbackAction);
			}
		}

		[CompilerGenerated]
		private static Action<UIStatus> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Action<UIStatus> _003C_003Ef__mg_0024cache1;
	}
}
