using System;
using CloudOnce.Internal.Providers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal
{
	public abstract class CloudProviderBase<T> : MonoBehaviour, ICloudProvider where T : Component
	{
		public static T Instance
		{
			get
			{
				if (!object.ReferenceEquals(CloudProviderBase<T>.s_instance, null))
				{
					return CloudProviderBase<T>.s_instance;
				}
				UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(T));
				if (!object.ReferenceEquals(array, null) && array.Length > 0)
				{
					CloudProviderBase<T>.s_instance = (array[0] as T);
					if (array.Length > 1)
					{
						for (int i = 1; i < array.Length; i++)
						{
							UnityEngine.Object.Destroy(array[i]);
						}
					}
				}
				if (!object.ReferenceEquals(CloudProviderBase<T>.s_instance, null))
				{
					return CloudProviderBase<T>.s_instance;
				}
				GameObject gameObject = new GameObject
				{
					name = string.Format("NewTransient{0}Singleton", typeof(T)),
					hideFlags = HideFlags.HideAndDontSave
				};
				CloudProviderBase<T>.s_instance = (gameObject.AddComponent(typeof(T)) as T);
				return CloudProviderBase<T>.s_instance;
			}
		}

		public string ServiceName { get; protected set; }

		public abstract string PlayerID { get; }

		public abstract string PlayerDisplayName { get; }

		public abstract Texture2D PlayerImage { get; }

		public abstract bool IsSignedIn { get; }

		public abstract bool CloudSaveEnabled { get; set; }

		public abstract ICloudStorageProvider Storage { get; protected set; }

		public abstract void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true);

		public abstract void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null);

		public abstract void SignOut();

		public abstract void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback);

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnOnDestroy()
		{
		}

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			this.OnAwake();
		}

		private void Start()
		{
			this.currentLoadTimer = (float)Cloud.AutoLoadInterval;
		}

		private void Update()
		{
			if (Cloud.AutoLoadInterval == Interval.Disabled)
			{
				return;
			}
			if (this.currentLoadTimer > 0f)
			{
				this.currentLoadTimer -= Time.deltaTime;
			}
			else
			{
				Cloud.Storage.Load();
				this.currentLoadTimer = (float)Cloud.AutoLoadInterval;
			}
		}

		private void OnDestroy()
		{
			CloudProviderBase<T>.s_instance = (T)((object)null);
			this.OnOnDestroy();
		}

		private static T s_instance;

		private float currentLoadTimer;
	}
}
