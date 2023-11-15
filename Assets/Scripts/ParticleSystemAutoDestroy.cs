using System;
using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
	public void Start()
	{
		this.ps = base.GetComponent<ParticleSystem>();
	}

	public void Update()
	{
		if (this.ps && !this.ps.IsAlive())
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private ParticleSystem ps;
}
