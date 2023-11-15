using System;
using CloudOnce.Internal.Utils;

namespace CloudOnce.Internal
{
	public class UnifiedAchievement
	{
		public UnifiedAchievement(string internalID, string platformID)
		{
			this.internalID = internalID;
			this.ID = platformID;
		}

		public string ID { get; private set; }

		public bool IsUnlocked { get; private set; }

		public double Progress
		{
			get
			{
				return this.achievementProgress;
			}
			private set
			{
				if (value < this.achievementProgress)
				{
					return;
				}
				this.achievementProgress = ((value <= 100.0) ? value : 100.0);
			}
		}

		public void Unlock(Action<CloudRequestResult<bool>> onComplete = null)
		{
			if (!this.IsUnlocked)
			{
				Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
				{
					this.OnUnlockCompleted(response, onComplete);
				};
				CloudOnceUtils.AchievementUtils.Unlock(this.ID, onComplete2, this.internalID);
			}
			else
			{
				string errorMessage = string.Format("Can't unlock {0}. Achievement has already been unlocked.", this.ID);
				UnifiedAchievement.ReportError(errorMessage, onComplete);
			}
		}

		public void Reveal(Action<CloudRequestResult<bool>> onComplete = null)
		{
			if (this.isAchievementHidden)
			{
				Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
				{
					this.OnRevealCompleted(response, onComplete);
				};
				CloudOnceUtils.AchievementUtils.Reveal(this.ID, onComplete2, this.internalID);
			}
			else
			{
				string errorMessage = string.Format("Can't reveal {0}. Achievement has already been revealed.", this.ID);
				UnifiedAchievement.ReportError(errorMessage, onComplete);
			}
		}

		public void Increment(double current, double goal, Action<CloudRequestResult<bool>> onComplete = null)
		{
			this.Increment(current / goal * 100.0, onComplete);
		}

		public void Increment(double progress, Action<CloudRequestResult<bool>> onComplete = null)
		{
			if (this.IsUnlocked)
			{
				string errorMessage = string.Format("Can't increment {0} ({1}). Achievement is already unlocked.", this.internalID, this.ID);
				UnifiedAchievement.ReportError(errorMessage, onComplete);
			}
			else
			{
				if (progress < 0.0)
				{
					throw new ArgumentException("Value must not be negative!", "progress");
				}
				if (progress.Equals(0.0))
				{
					this.Reveal(onComplete);
				}
				else if (progress >= 100.0)
				{
					this.Unlock(onComplete);
				}
				else if (progress <= this.Progress)
				{
					string errorMessage2 = string.Format("Can't increment {0} ({1}) to {2:F2}%. Achievement is already at {3:F2}%.", new object[]
					{
						this.internalID,
						this.ID,
						progress,
						this.Progress
					});
					UnifiedAchievement.ReportError(errorMessage2, onComplete);
				}
				else
				{
					Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
					{
						this.OnIncrementCompleted(response, progress, onComplete);
					};
					CloudOnceUtils.AchievementUtils.Increment(this.ID, progress, onComplete2, this.internalID);
				}
			}
		}

		public void UpdateData(bool isUnlocked, double progress, bool isHidden)
		{
			if (this.IsUnlocked && !isUnlocked)
			{
				Action<CloudRequestResult<bool>> onComplete = delegate(CloudRequestResult<bool> response)
				{
					this.OnUnlockCompleted(response, null);
				};
				CloudOnceUtils.AchievementUtils.Unlock(this.ID, onComplete, this.internalID);
				return;
			}
			if (this.Progress > progress)
			{
				Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
				{
					this.OnIncrementCompleted(response, progress, null);
				};
				CloudOnceUtils.AchievementUtils.Increment(this.ID, progress, onComplete2, this.internalID);
				return;
			}
			this.IsUnlocked = isUnlocked;
			this.Progress = progress;
			this.isAchievementHidden = isHidden;
			if (!this.IsUnlocked && this.Progress.Equals(100.0))
			{
				Action<CloudRequestResult<bool>> onComplete3 = delegate(CloudRequestResult<bool> response)
				{
					this.OnUnlockCompleted(response, null);
				};
				CloudOnceUtils.AchievementUtils.Unlock(this.ID, onComplete3, this.internalID);
			}
		}

		private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
		{
			CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
		}

		private void OnUnlockCompleted(CloudRequestResult<bool> response, Action<CloudRequestResult<bool>> callbackAction)
		{
			if (response.Result)
			{
				this.IsUnlocked = true;
				this.isAchievementHidden = false;
				this.Progress = 100.0;
				CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(true));
			}
			else
			{
				CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(false, response.Error));
			}
		}

		private void OnRevealCompleted(CloudRequestResult<bool> response, Action<CloudRequestResult<bool>> callbackAction)
		{
			if (response.Result)
			{
				this.isAchievementHidden = false;
				CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(true));
			}
			else
			{
				CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(false, response.Error));
			}
		}

		private void OnIncrementCompleted(CloudRequestResult<bool> response, double progress, Action<CloudRequestResult<bool>> callbackAction)
		{
			if (response.Result)
			{
				this.Progress = progress;
				CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(true));
			}
			else
			{
				CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(false, response.Error));
			}
		}

		private readonly string internalID;

		private bool isAchievementHidden = true;

		private double achievementProgress;
	}
}
