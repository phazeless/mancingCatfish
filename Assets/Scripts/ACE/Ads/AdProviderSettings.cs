using System;
using UnityEngine;

namespace ACE.Ads
{
	[Serializable]
	public class AdProviderSettings : IAdProviderSettings
	{
		public BaseAdFormat[] GetAvailableAdFormats()
		{
			return this.supportedAdFormats.AvailableAdFormats.ToArray();
		}

		public bool IsSupportedPlatform
		{
			get
			{
				return this.supportedPlatforms.Android;
			}
		}

		public string DEBUG_TAG
		{
			get
			{
				return base.GetType().Name;
			}
		}

		[SerializeField]
		[GotoLinkButton]
		private string adProviderWebpage;

		[SerializeField]
		[Header("General Ad - Settings")]
		private SupportedAdFormats supportedAdFormats;

		[SerializeField]
		public SupportedPlatforms supportedPlatforms;
	}
}
