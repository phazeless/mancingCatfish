using System;
using CloudOnce.Internal.Utils;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal
{
	public class UnifiedLeaderboard
	{
		public UnifiedLeaderboard(string internalID, string platformID)
		{
			this.internalID = internalID;
			this.ID = platformID;
		}

		public string ID { get; private set; }

		public void SubmitScore(long score, Action<CloudRequestResult<bool>> onComplete = null)
		{
			CloudOnceUtils.LeaderboardUtils.SubmitScore(this.ID, score, onComplete, this.internalID);
		}

		public void ShowOverlay()
		{
			CloudOnceUtils.LeaderboardUtils.ShowOverlay(this.ID, this.internalID);
		}

		public void LoadScores(Action<IScore[]> callback)
		{
			CloudOnceUtils.LeaderboardUtils.LoadScores(this.ID, callback);
		}

		private readonly string internalID;
	}
}
