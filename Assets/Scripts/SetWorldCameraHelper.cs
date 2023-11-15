using System;
using UnityEngine;

public class SetWorldCameraHelper : MonoBehaviour
{
	private void Awake()
	{
		base.GetComponent<Canvas>().worldCamera = Camera.main;
	}
}
