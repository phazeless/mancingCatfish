using System;
using UnityEngine;

namespace ACE.Ads
{
	public class SupersonicAdProviderSettings : AdProviderSettings
	{
		[Header("Main Ad - Settings")]
		public SupersonicAdProviderSettings.OSSpecifics OsSpecifics;

		public string UniqueUserId;

		public bool UseClientSideCallback = true;

		[Serializable]
		public class OSSpecifics
		{
			public string Key
			{
				get
				{
					return this.androidKey;
				}
			}

			public string androidKey;

			public string iOSKey;

			public string windowsKey;
		}
	}
}
