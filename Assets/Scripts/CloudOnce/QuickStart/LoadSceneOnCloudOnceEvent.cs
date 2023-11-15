using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CloudOnce.QuickStart
{
	[AddComponentMenu("CloudOnce/Load Scene On Event", 1)]
	public class LoadSceneOnCloudOnceEvent : MonoBehaviour
	{
		private void Awake()
		{
			switch (this.cloudOnceEvent)
			{
			case LoadSceneOnCloudOnceEvent.CloudOnceEvent.OnInitializeComplete:
				Cloud.OnInitializeComplete += this.OnInitializeComplete;
				break;
			case LoadSceneOnCloudOnceEvent.CloudOnceEvent.OnCloudLoadComplete:
				Cloud.OnCloudLoadComplete += this.OnCloudLoadComplete;
				break;
			case LoadSceneOnCloudOnceEvent.CloudOnceEvent.OnSignedInChanged:
				Cloud.OnSignedInChanged += this.OnSignedInChanged;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void OnInitializeComplete()
		{
			this.LoadScene();
		}

		private void OnCloudLoadComplete(bool result)
		{
			this.LoadScene();
		}

		private void OnSignedInChanged(bool isSignedIn)
		{
			this.LoadScene();
		}

		private void LoadScene()
		{
			this.UnsubscribeEvents();
			if (string.IsNullOrEmpty(this.sceneName))
			{
				UnityEngine.Debug.LogWarning("Scene name was empty, aborting load.");
				return;
			}
			if (this.loadAdditive && this.loadAsync)
			{
				SceneManager.LoadSceneAsync(this.sceneName, LoadSceneMode.Additive);
			}
			else if (this.loadAdditive && !this.loadAsync)
			{
				SceneManager.LoadScene(this.sceneName, LoadSceneMode.Additive);
			}
			else if (!this.loadAdditive && this.loadAsync)
			{
				SceneManager.LoadSceneAsync(this.sceneName);
			}
			else
			{
				SceneManager.LoadScene(this.sceneName);
			}
		}

		private void UnsubscribeEvents()
		{
			switch (this.cloudOnceEvent)
			{
			case LoadSceneOnCloudOnceEvent.CloudOnceEvent.OnInitializeComplete:
				Cloud.OnInitializeComplete -= this.OnInitializeComplete;
				break;
			case LoadSceneOnCloudOnceEvent.CloudOnceEvent.OnCloudLoadComplete:
				Cloud.OnCloudLoadComplete -= this.OnCloudLoadComplete;
				break;
			case LoadSceneOnCloudOnceEvent.CloudOnceEvent.OnSignedInChanged:
				Cloud.OnSignedInChanged -= this.OnSignedInChanged;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		[SerializeField]
		private LoadSceneOnCloudOnceEvent.CloudOnceEvent cloudOnceEvent;

		[SerializeField]
		private string sceneName;

		[SerializeField]
		private bool loadAdditive;

		[SerializeField]
		private bool loadAsync;

		private enum CloudOnceEvent
		{
			OnInitializeComplete,
			OnCloudLoadComplete,
			OnSignedInChanged
		}
	}
}
