using System;
using UnityEngine;

namespace ACE.Analytics
{
	public class GameAnalyticsService : IAnalyticsService
	{
		public GameAnalyticsService(string gameKey, string secretKey, string version, GameObject gameObject)
		{
			
		}

		private static void SetBuildVersionForAllPlatforms(string version)
		{
			
		}

		private static void ResetGameKeysForAllPlatforms()
		{
			
		}

		private static void InitializeGA(string gameKey, string secretKey)
		{
			
		}

		public void Post(CustomEvent evnt)
		{
			
		}

		public void Post(ProgressionEvent evnt)
		{
			
		}

	

		public void Post(RevenueEvent evnt)
		{
			
		}

		public void Post(ResourceEvent evnt)
		{
			
		}

	

		private const char ID_SEPARATOR = ':';
	}
}
