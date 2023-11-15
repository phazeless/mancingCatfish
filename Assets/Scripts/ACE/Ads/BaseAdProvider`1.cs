using System;
using System.Diagnostics;
using UnityEngine;

namespace ACE.Ads
{
	public abstract class BaseAdProvider<T> : IAdProvider where T : class, IAdProviderSettings
	{
		[SerializeField]
		public virtual T AdProviderSettings { get; protected set; }

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<AdResponse> OnAdFinished;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<AdResponse> OnAdEventListener;

		public abstract void OnInitialize();

		public abstract void OnShow(BaseAdFormat adFormat);

		public abstract void OnCache(BaseAdFormat adFormat);

		public abstract void OnHide(BaseAdFormat adFormat);

		public abstract bool HasCachedAd(BaseAdFormat adFormat);

		public abstract void OnResetStates();

		public void SetOnAdFinishedListener(Action<AdResponse> onAdFinished)
		{
			this.OnAdFinished = onAdFinished;
		}

		public void AddAdEventListener(Action<AdResponse> onAdEvent)
		{
			this.OnAdEventListener += onAdEvent;
		}

		public void RemoveAdEventListener(Action<AdResponse> callback)
		{
			this.OnAdEventListener -= callback;
		}

		public void ClearAdEventListeners()
		{
			this.OnAdEventListener = null;
		}

		public K GetAdProviderSettings<K>() where K : IAdProviderSettings
		{
			return (K)((object)this.AdProviderSettings);
		}

		public void Notify(AdResponse.AdResponseBuilder adResponseBuilder)
		{
			adResponseBuilder.SetAdProvider(this);
			if (this.OnAdEventListener != null)
			{
				this.OnAdEventListener(adResponseBuilder.Build());
			}
			if (adResponseBuilder.Type == AdResponse.AdResponseType.DidFinish)
			{
				if (this.OnAdFinished != null)
				{
					this.OnAdFinished(adResponseBuilder.Build());
				}
				this.OnAdFinished = null;
				this.OnResetStates();
				if (this.OnAdEventListener != null)
				{
					this.OnAdEventListener(new AdResponse.AdResponseBuilder(adResponseBuilder.AdFormat, AdResponse.AdResponseType.DidResetStates).SetAdProvider(this).Build());
				}
			}
		}
	}
}
