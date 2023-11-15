using System;
using UnityEngine;

public abstract class ACEMonoSingleton<T> : MonoBehaviour where T : ACEMonoSingleton<T>
{
	public static T Instance
	{
		get
		{
			if (ACEMonoSingleton<T>.m_Instance == null)
			{
				object @lock = ACEMonoSingleton<T>.m_Lock;
				lock (@lock)
				{
					if (ACEMonoSingleton<T>.m_Instance == null)
					{
						ACEMonoSingleton<T>.m_Instance = UnityEngine.Object.FindObjectOfType<T>();
						if (ACEMonoSingleton<T>.m_Instance == null)
						{
							ACEMonoSingleton<T>.m_Instance = new GameObject().AddComponent<T>();
							ACEMonoSingleton<T>.m_Instance.gameObject.name = ACEMonoSingleton<T>.m_Instance.GetObjectName();
							UnityEngine.Debug.Log("No '" + typeof(T).Name + "'-gameObject found in scene. Created new.", ACEMonoSingleton<T>.m_Instance);
						}
					}
				}
			}
			return ACEMonoSingleton<T>.m_Instance;
		}
	}

	public static bool HasInstance
	{
		get
		{
			return ACEMonoSingleton<T>.m_Instance != null;
		}
	}

	public void DummyInstantiate()
	{
		if (ACEMonoSingleton<T>.Instance == null)
		{
			return;
		}
	}

	protected virtual string GetObjectName()
	{
		return "_" + base.GetType().Name;
	}

	private static object m_Lock = new object();

	protected static volatile T m_Instance = (T)((object)null);
}
