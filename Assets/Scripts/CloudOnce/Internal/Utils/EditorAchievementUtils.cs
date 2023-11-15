using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Utils
{
	public class EditorAchievementUtils : IAchievementUtils
	{
		public void Unlock(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				EditorAchievementUtils.ReportError("Can't unlock achievement. Supplied ID is null or empty!", onComplete);
				return;
			}
			EditorAchievementUtils.OnReportCompleted(true, onComplete, "unlock", id, internalID);
		}

		public void Reveal(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				EditorAchievementUtils.ReportError("Can't reveal achievement. Supplied ID is null or empty!", onComplete);
				return;
			}
			EditorAchievementUtils.OnReportCompleted(true, onComplete, "reveal", id, internalID);
		}

		public void Increment(string id, double progress, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				EditorAchievementUtils.ReportError("Can't increment achievement. Supplied ID is null or empty!", onComplete);
				return;
			}
			EditorAchievementUtils.OnReportCompleted(true, onComplete, "increment", id, internalID);
		}

		public void ShowOverlay()
		{
			UnityEngine.Debug.LogWarning("Achievements overlay is not supported in the Unity Editor.");
		}

		public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			CloudOnceUtils.SafeInvoke<IAchievementDescription[]>(callback, EditorAchievementUtils.GetTestAchievementDescriptions());
		}

		public void LoadAchievements(Action<IAchievement[]> callback)
		{
			CloudOnceUtils.SafeInvoke<IAchievement[]>(callback, EditorAchievementUtils.GetTestAchievements());
		}

		private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
		{
			CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
		}

		private static void OnReportCompleted(bool response, Action<CloudRequestResult<bool>> callbackAction, string action, string id, string internalID)
		{
			if (response)
			{
				CloudOnceUtils.SafeInvoke<CloudRequestResult<bool>>(callbackAction, new CloudRequestResult<bool>(true));
			}
			else
			{
				string errorMessage = (!string.IsNullOrEmpty(internalID)) ? string.Format("Native API failed to {0} achievement {1} ({2}). Cause unknown.", action, internalID, id) : string.Format("Native API failed to {0} achievement {1}. Cause unknown.", action, id);
				EditorAchievementUtils.ReportError(errorMessage, callbackAction);
			}
		}

		private static IAchievementDescription[] GetTestAchievementDescriptions()
		{
			return (from property in typeof(Achievements).GetProperties()
			where property.PropertyType == typeof(UnifiedAchievement)
			select new EditorAchievementUtils.TestAchievementDescription(property)).ToArray<IAchievementDescription>();
		}

		private static IAchievement[] GetTestAchievements()
		{
			return (from property in typeof(Achievements).GetProperties()
			where property.PropertyType == typeof(UnifiedAchievement)
			select new EditorAchievementUtils.TestAchievement(property)).ToArray<IAchievement>();
		}

		private const string unlockAction = "unlock";

		private const string revealAction = "reveal";

		private const string incrementAction = "increment";

		private class TestAchievement : IAchievement
		{
			public TestAchievement(PropertyInfo property)
			{
				UnifiedAchievement unifiedAchievement = (UnifiedAchievement)property.GetValue(null, null);
				this.id = unifiedAchievement.ID;
				this.percentCompleted = unifiedAchievement.Progress;
				this.completed = unifiedAchievement.IsUnlocked;
				this.hidden = false;
				this.lastReportedDate = DateTime.Now;
			}

			public string id { get; set; }

			public double percentCompleted { get; set; }

			public bool completed { get; private set; }

			public bool hidden { get; private set; }

			public DateTime lastReportedDate { get; private set; }

			public void ReportProgress(Action<bool> callback)
			{
				CloudOnceUtils.SafeInvoke<bool>(callback, true);
			}
		}

		private class TestAchievementDescription : IAchievementDescription
		{
			public TestAchievementDescription(PropertyInfo property)
			{
				UnifiedAchievement unifiedAchievement = (UnifiedAchievement)property.GetValue(null, null);
				this.id = unifiedAchievement.ID;
				this.title = property.Name;
				this.image = Texture2D.whiteTexture;
				this.achievedDescription = "Test description for " + property.Name + ".";
				this.unachievedDescription = this.achievedDescription;
				this.hidden = false;
				this.points = 0;
			}

			public string id { get; set; }

			public string title { get; private set; }

			public Texture2D image { get; private set; }

			public string achievedDescription { get; private set; }

			public string unachievedDescription { get; private set; }

			public bool hidden { get; private set; }

			public int points { get; private set; }
		}
	}
}
