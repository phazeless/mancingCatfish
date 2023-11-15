using System;
using UnityEngine;

public class DailyCatchSplash : MonoBehaviour
{
	public void Splash()
	{
		foreach (ParticleSystem particleSystem in this.splashParticles)
		{
			particleSystem.Play();
		}
		foreach (ParticleSystem particleSystem2 in this.fishParticles)
		{
			particleSystem2.Play();
		}
	}

	public void SetColor(Color color)
	{
		foreach (ParticleSystem particleSystem in this.splashParticles)
		{
            var temp = particleSystem.main;

            temp.startColor = color;
		}
	}

	[SerializeField]
	private ParticleSystem[] splashParticles;

	[SerializeField]
	private ParticleSystem[] fishParticles;
}
