using System;
using UnityEngine;

public class FPSHandler : MonoBehaviour
{
	private void Start()
	{
		Application.targetFrameRate = 60;
	}
}
