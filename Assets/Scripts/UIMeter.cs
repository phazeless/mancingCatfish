using System;
using UnityEngine;

public class UIMeter : MonoBehaviour
{
	public void SetMax(float max)
	{
		this.max = max;
	}

	public void SetCurrent(float current)
	{
		if (!this.hasSetStartScale)
		{
			this.startScale = this.bg.transform.localScale;
			this.hasSetStartScale = true;
		}
		this.current = current;
		Vector2 v = Vector2.zero;
		if (this.meterDirection == UIMeter.MeterDirection.Backward)
		{
			v = (this.max - current) / this.max * this.startScale;
		}
		else if (this.meterDirection == UIMeter.MeterDirection.Foward)
		{
			v = current / this.max * this.startScale;
		}
		v.y = this.startScale.y;
		v.x = Mathf.Min(Mathf.Max(v.x, 0f), 1f);
		this.meter.transform.localScale = v;
	}

	private void Awake()
	{
	}

	[SerializeField]
	private Transform meter;

	[SerializeField]
	private Transform bg;

	[SerializeField]
	private UIMeter.MeterDirection meterDirection;

	private float max;

	private float current;

	private Vector2 startScale = Vector2.zero;

	private bool hasSetStartScale;

	public enum MeterDirection
	{
		Foward,
		Backward
	}
}
