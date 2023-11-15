using System;
using UnityEngine;

public class LightCurves : MonoBehaviour
{
	private void Start()
	{
		this.lightSource = base.GetComponent<Light>();
	}

	private void OnEnable()
	{
		this.startTime = Time.time;
	}

	private void Update()
	{
		float num = Time.time - this.startTime;
		if (num <= this.GraphScaleX)
		{
			float intensity = this.LightCurve.Evaluate(num / this.GraphScaleX) * this.GraphScaleY;
			this.lightSource.intensity = intensity;
		}
	}

	public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public float GraphScaleX = 1f;

	public float GraphScaleY = 1f;

	private float startTime;

	private Light lightSource;
}
