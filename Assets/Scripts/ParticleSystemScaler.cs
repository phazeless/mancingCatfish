using System;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleSystemScaler : MonoBehaviour
{
	private void Start()
	{
		this.oldScale = this.particlesScale;
	}

	private void Update()
	{
	}

	public float particlesScale = 1f;

	private float oldScale;
}
