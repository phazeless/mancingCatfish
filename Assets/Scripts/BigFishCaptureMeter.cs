using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class BigFishCaptureMeter : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<FishBehaviour> OnFishCaught;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<FishBehaviour> OnFishEscaped;

	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	public void SetFishAsHooked(FishBehaviour fishAttr)
	{
		if (this.fish != null)
		{
			return;
		}
		base.gameObject.SetActive(true);
		this.fish = fishAttr;
	}

	public void HandleFishInWaterLogic()
	{
		if (!this.IsFishInWater)
		{
			return;
		}
		float num = -1f;
		float num2 = 0.05f * Time.deltaTime * num;
		this.imgMeter.fillAmount -= num2;
		if (this.imgMeter.fillAmount <= 0f)
		{
			if (this.OnFishEscaped != null)
			{
				this.OnFishEscaped(this.fish);
			}
			this.fish = null;
			base.gameObject.SetActive(false);
		}
	}

	public void PullFish()
	{
		float num = -1f;
		this.imgMeter.fillAmount += num;
		if (this.imgMeter.fillAmount >= 1f)
		{
			if (this.OnFishCaught != null)
			{
				this.OnFishCaught(this.fish);
			}
			this.fish = null;
			base.gameObject.SetActive(false);
		}
	}

	public bool IsFishInWater
	{
		get
		{
			return this.fish != null;
		}
	}

	[SerializeField]
	private Image imgMeter;

	private FishBehaviour fish;
}
