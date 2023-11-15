using System;
using UnityEngine;

namespace CloudOnce.QuickStart
{
	[AddComponentMenu("CloudOnce/Deactivate On Event", 2)]
	public class DeactivateOnCloudOnceEvent : MonoBehaviour
	{
		private void Awake()
		{
			switch (this.cloudOnceEvent)
			{
			case DeactivateOnCloudOnceEvent.CloudOnceEvent.OnInitializeComplete:
				Cloud.OnInitializeComplete += this.OnInitializeComplete;
				break;
			case DeactivateOnCloudOnceEvent.CloudOnceEvent.OnCloudLoadComplete:
				Cloud.OnCloudLoadComplete += this.OnCloudLoadComplete;
				break;
			case DeactivateOnCloudOnceEvent.CloudOnceEvent.OnSignedInChanged:
				Cloud.OnSignedInChanged += this.OnSignedInChanged;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void OnInitializeComplete()
		{
			this.UnsubscribeEvents();
			base.gameObject.SetActive(false);
		}

		private void OnCloudLoadComplete(bool result)
		{
			this.UnsubscribeEvents();
			base.gameObject.SetActive(false);
		}

		private void OnSignedInChanged(bool isSignedIn)
		{
			this.UnsubscribeEvents();
			base.gameObject.SetActive(false);
		}

		private void UnsubscribeEvents()
		{
			switch (this.cloudOnceEvent)
			{
			case DeactivateOnCloudOnceEvent.CloudOnceEvent.OnInitializeComplete:
				Cloud.OnInitializeComplete -= this.OnInitializeComplete;
				break;
			case DeactivateOnCloudOnceEvent.CloudOnceEvent.OnCloudLoadComplete:
				Cloud.OnCloudLoadComplete -= this.OnCloudLoadComplete;
				break;
			case DeactivateOnCloudOnceEvent.CloudOnceEvent.OnSignedInChanged:
				Cloud.OnSignedInChanged -= this.OnSignedInChanged;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		[SerializeField]
		private DeactivateOnCloudOnceEvent.CloudOnceEvent cloudOnceEvent;

		private enum CloudOnceEvent
		{
			OnInitializeComplete,
			OnCloudLoadComplete,
			OnSignedInChanged
		}
	}
}
