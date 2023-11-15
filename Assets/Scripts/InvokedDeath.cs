using System;
using UnityEngine;

public class InvokedDeath : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("Suicide", this.timeToSuicide);
	}

	private void Suicide()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public float timeToSuicide = 1f;
}
