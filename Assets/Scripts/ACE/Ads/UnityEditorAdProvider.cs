using System;
using UnityEngine;

namespace ACE.Ads
{
	public class UnityEditorAdProvider : BaseAdProvider<UnityEditorAdProviderSettings>
	{
		public override bool HasCachedAd(BaseAdFormat adFormat)
		{
			return true;
		}

		public override void OnCache(BaseAdFormat adFormat)
		{
		}

		public override void OnHide(BaseAdFormat adFormat)
		{
			UnityEngine.Object.Destroy(this.UnityEditorAd);
		}

		public override void OnInitialize()
		{
		}

		private void AdPopup_OnAdShow(BaseAdFormat adFormat)
		{
			base.Notify(new AdResponse.AdResponseBuilder(adFormat, AdResponse.AdResponseType.DidShow));
		}

		private void AdPopup_OnAdClosed(BaseAdFormat adFormat, bool didClick, bool didComplete)
		{
			base.Notify(new AdResponse.AdResponseBuilder(adFormat, AdResponse.AdResponseType.DidHide));
			base.Notify(new AdResponse.AdResponseBuilder(adFormat, AdResponse.AdResponseType.DidFinish).SetDidClick(didClick).SetDidComplete(didComplete));
			UnityEngine.Object.Destroy(this.UnityEditorAd.gameObject);
		}

		public override void OnResetStates()
		{
		}

		public override void OnShow(BaseAdFormat adFormat)
		{
			this.UnityEditorAd = UnityEngine.Object.Instantiate<UnityEditorAdPopup>(base.GetAdProviderSettings<UnityEditorAdProviderSettings>().UnityEditorAd);
			this.UnityEditorAd.OnAdShow += this.AdPopup_OnAdShow;
			this.UnityEditorAd.OnAdClosed += this.AdPopup_OnAdClosed;
			this.UnityEditorAd.Show(adFormat);
		}

		private UnityEditorAdPopup UnityEditorAd;
	}
}
