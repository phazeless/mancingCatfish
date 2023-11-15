using System;
using UnityEngine;

public class DemoReactivator : MonoBehaviour
{
	private void Start()
	{
		base.InvokeRepeating("Reactivate", this.TimeDelayToReactivate, this.TimeDelayToReactivate);
	}

	private void Reactivate()
	{
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
	}

	public float TimeDelayToReactivate = 3f;
}
