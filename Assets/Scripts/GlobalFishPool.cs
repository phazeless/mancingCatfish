using System;
using UnityEngine;

public class GlobalFishPool : MonoBehaviour
{
	public static GlobalFishPool Instance
	{
		get
		{
			if (GlobalFishPool.instance != null)
			{
				return GlobalFishPool.instance;
			}
			return GlobalFishPool.instance = new GameObject("GlobalFishPool").AddComponent<GlobalFishPool>();
		}
	}

	public PrefabObjectPool<FishBehaviour> Pool { get; private set; }

	private void Start()
	{
	}

	private void OnDestroy()
	{
		GlobalFishPool.instance = null;
	}

	private static GlobalFishPool instance;
}
