using System;
using System.Collections.Generic;
using Soomla.Singletons;
using UnityEngine;

namespace Soomla
{
	public class CoreEvents : CodeGeneratedSingleton
	{
		protected override bool DontDestroySingleton
		{
			get
			{
				return true;
			}
		}

		public static void Initialize()
		{
			if (CoreEvents.Instance == null)
			{
				CoreEvents.Instance = UnitySingleton.GetSynchronousCodeGeneratedInstance<CoreEvents>();
				SoomlaUtils.LogDebug("SOOMLA CoreEvents", "Initializing CoreEvents and Soomla Core ...");
				
			}
		}

		public void onRewardGiven(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA CoreEvents", "SOOMLA/UNITY onRewardGiven:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			string str = jsonobject["rewardId"].str;
			CoreEvents.OnRewardGiven(Reward.GetReward(str));
		}

		public void onRewardTaken(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA CoreEvents", "SOOMLA/UNITY onRewardTaken:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			string str = jsonobject["rewardId"].str;
			CoreEvents.OnRewardTaken(Reward.GetReward(str));
		}

		public void onCustomEvent(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA CoreEvents", "SOOMLA/UNITY onCustomEvent:" + message);
			JSONObject jsonobject = new JSONObject(message, -2, false, false);
			string str = jsonobject["name"].str;
			Dictionary<string, string> arg = jsonobject["extra"].ToDictionary();
			CoreEvents.OnCustomEvent(str, arg);
		}

		private const string TAG = "SOOMLA CoreEvents";

		public static CoreEvents Instance = null;

		public static Action<Reward> OnRewardGiven = delegate(Reward A_0)
		{
		};

		public static Action<Reward> OnRewardTaken = delegate(Reward A_0)
		{
		};

		public static Action<string, Dictionary<string, string>> OnCustomEvent = delegate(string A_0, Dictionary<string, string> A_1)
		{
		};

		public delegate void Action();
	}
}
