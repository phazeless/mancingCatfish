using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Utils
{
	public class EditorLeaderboardUtils : ILeaderboardUtils
	{
		public void SubmitScore(string id, long score, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				EditorLeaderboardUtils.ReportError(string.Format("Can't submit score to {0} leaderboard. Platform ID is null or empty!", internalID), onComplete);
				return;
			}
			CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(onComplete, new CloudRequestResult<bool>(true));
		}

		public void ShowOverlay(string id = "", string internalID = "")
		{
			UnityEngine.Debug.LogWarning("Leaderboards overlay is not supported in the Unity Editor.");
		}

		public void LoadScores(string leaderboardID, Action<IScore[]> callback)
		{
			UnityEngine.Debug.LogWarning("Leaderboards overlay is not supported in the Unity Editor.");
			CloudOnceUtils.SafeInvoke<IScore[]>(callback, new IScore[0]);
		}

		private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
		{
			CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
		}
	}
}
