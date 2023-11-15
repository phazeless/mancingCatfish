using System;
using UnityEngine;

public class DelayedActivator : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("ActivateThings", this.delayTime);
	}

	private void ActivateThings()
	{
		foreach (GameObject gameObject in this.thingsToActivate)
		{
			gameObject.SetActive(true);
		}
	}

	[SerializeField]
	private GameObject[] thingsToActivate;

	[SerializeField]
	private float delayTime = 2f;
}
