using System;
using UnityEngine;

namespace SimpleFirebaseUnity
{
	[ExecuteInEditMode]
	public class FirebaseManager : MonoBehaviour
	{
		protected FirebaseManager()
		{
		}

		public static FirebaseManager Instance
		{
			get
			{
				if (FirebaseManager.applicationIsQuitting)
				{
					UnityEngine.Debug.LogWarning("[Firebase Manager] Instance already destroyed on application quit. Won't create again - returning null.");
					return null;
				}
				object @lock = FirebaseManager._lock;
				FirebaseManager instance;
				lock (@lock)
				{
					if (FirebaseManager._instance == null)
					{
						FirebaseManager[] array = UnityEngine.Object.FindObjectsOfType<FirebaseManager>();
						FirebaseManager._instance = ((array.Length <= 0) ? null : array[0]);
						if (array.Length > 1)
						{
							UnityEngine.Debug.LogError("[Firebase Manager] Something went really wrong  - there should never be more than 1 Firebase Manager! Reopening the scene might fix it.");
							return FirebaseManager._instance;
						}
						if (FirebaseManager._instance == null)
						{
							GameObject gameObject = new GameObject();
							FirebaseManager._instance = gameObject.AddComponent<FirebaseManager>();
							gameObject.name = "Firebase Manager [Singleton]";
							UnityEngine.Object.DontDestroyOnLoad(gameObject);
							UnityEngine.Debug.Log("[Firebase Manager] Instance '" + gameObject + "' was generated in the scene with DontDestroyOnLoad.");
						}
						else
						{
							UnityEngine.Debug.Log("[Firebase Manager] Using instance already created: " + FirebaseManager._instance.gameObject.name);
						}
					}
					instance = FirebaseManager._instance;
				}
				return instance;
			}
		}

		public void OnDestroy()
		{
			if (Application.isPlaying)
			{
				FirebaseManager.applicationIsQuitting = true;
			}
		}

		private static FirebaseManager _instance;

		private static object _lock = new object();

		private static bool applicationIsQuitting = false;
	}
}
