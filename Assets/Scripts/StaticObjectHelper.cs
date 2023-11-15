using System;
using UnityEngine;

public class StaticObjectHelper : MonoBehaviour
{
	private void Start()
	{
		StaticObjectHelper.instance = this;
	}

	public static StaticObjectHelper instance;
}
