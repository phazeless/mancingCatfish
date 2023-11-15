using System;
using UnityEngine;

public class FHelperMono : MonoBehaviour
{
	public static FHelperMono Instance
	{
		get
		{
			if (FHelperMono.instance == null)
			{
				return FHelperMono.instance = new GameObject("FHelperMono").AddComponent<FHelperMono>();
			}
			return FHelperMono.instance;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private static FHelperMono instance;
}
