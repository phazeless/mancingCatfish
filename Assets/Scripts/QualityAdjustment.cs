using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Water;

public class QualityAdjustment : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Start()
	{
		if (this.isFPSCheckOn)
		{
			this.RunAfterDelay((float)this.StartCheckDelay, delegate()
			{
				this.FPSCheck();
			});
		}
	}

	private void FPSCheck()
	{
		this.isFPSCheckOn = false;
		for (int i = 0; i < this.Checks; i++)
		{
			this.RunAfterDelay((float)(i * (this.CheckInterval / this.Checks)), delegate()
			{
				this.fpsSlices.Add(1f / Time.smoothDeltaTime);
				if (this.fpsSlices.Count == this.Checks)
				{
					foreach (float num in this.fpsSlices)
					{
						this.averageFps += num;
					}
					this.fps = this.averageFps / (float)this.Checks;
					if (this.fps < (float)this.MediumMinFps)
					{
						this.water.SetLowQuality();
					}
					else if (this.fps < (float)this.HighMinFps)
					{
						this.water.SetMediumQuality();
					}
					else
					{
						this.water.SetHighQuality();
					}
				}
			});
		}
	}

	[SerializeField]
	public Water water;

	public int HighMinFps;

	public int MediumMinFps;

	public int StartCheckDelay;

	public int CheckInterval;

	public int Checks;

	public List<float> fpsSlices = new List<float>();

	public bool isFPSCheckOn = true;

	private float averageFps;

	private float fps;
}
