using System;
using UnityEngine;

public class HingeBehaviour : MonoBehaviour
{
	public void WheelActivated(float spinTime)
	{
		this.isWheelActive = true;
		this.isWheelFastspinning = true;
		base.Invoke("Slowdown", spinTime * 0.8f);
	}

	private void Slowdown()
	{
		this.isWheelFastspinning = false;
	}

	public void WheelStopped()
	{
		this.isWheelActive = false;
	}

	private void Update()
	{
		if (!this.isWheelActive)
		{
			return;
		}
		if (this.isWheelFastspinning)
		{
			this.t += Time.deltaTime * 4.9f;
			base.transform.localEulerAngles = new Vector3(0f, 0f, -50f * (this.t % 0.5f + 0.5f));
		}
		else if (this.isWheelActive)
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, -30f * -((25f - Mathf.Clamp(this.wheel.localEulerAngles.z % 30f, 25f, 30f)) / 5f + (25f - Mathf.Clamp(30f - this.wheel.localEulerAngles.z % 30f, 25f, 30f)) / 5f));
		}
	}

	[SerializeField]
	private Transform wheel;

	public bool isWheelActive;

	public bool isWheelFastspinning;

	private float t;
}
