using System;
using UnityEngine;

namespace Soomla
{
	public static class SoomlaUtils
	{
		public static void LogDebug(string tag, string message)
		{
			if (!SoomlaUtils.isDebugBuildSet)
			{
				try
				{
					SoomlaUtils.isDebugBuild = UnityEngine.Debug.isDebugBuild;
				}
				catch (Exception ex)
				{
					SoomlaUtils.isDebugBuild = true;
					UnityEngine.Debug.Log(string.Format("{0} {1}", tag, ex.Message));
				}
				SoomlaUtils.isDebugBuildSet = true;
			}
			if (SoomlaUtils.isDebugBuild && CoreSettings.DebugUnityMessages)
			{
				UnityEngine.Debug.Log(string.Format("{0} {1}", tag, message));
			}
		}

		public static void LogError(string tag, string message)
		{
			UnityEngine.Debug.LogError(string.Format("{0} {1}", tag, message));
		}

		public static void LogWarning(string tag, string message)
		{
			UnityEngine.Debug.LogWarning(string.Format("{0} {1}", tag, message));
		}

		public static string GetClassName(object target)
		{
			return target.GetType().Name;
		}

		private static bool isDebugBuild;

		private static bool isDebugBuildSet;
	}
}
