using System;
using UnityEngine;

public class DelayedScreenDisabler : MonoBehaviour
{
	private void Start()
	{
		ScreenManager.Instance.DelayedScreenDisable();
	}
}
