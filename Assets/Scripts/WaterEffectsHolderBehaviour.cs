using System;
using UnityEngine;

public class WaterEffectsHolderBehaviour : MonoBehaviour
{
	private void Awake()
	{
		WaterEffectsHolderBehaviour.instance = this;
	}

	public static WaterEffectsHolderBehaviour instance;

	public Transform behindSurfaceShaderPosition;
}
