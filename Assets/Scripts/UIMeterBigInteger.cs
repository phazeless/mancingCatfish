using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class UIMeterBigInteger : MonoBehaviour
{
	public Image MeterImg
	{
		get
		{
			return this.meter.GetComponent<Image>();
		}
	}

	public Image MeterBgImg
	{
		get
		{
			return this.bg.GetComponent<Image>();
		}
	}

	public void SetMax(BigInteger max)
	{
		this.max = max;
		this.expOfMax = (int)BigInteger.Log10(max + 1);
	}

	public void SetCurrent(BigInteger current)
	{
		this.current = current;
		bool flag = current >= this.max;
		float num = 1f;
		if (!flag)
		{
			BigInteger bigInteger = BigInteger.Max(BigInteger.Pow(10, this.expOfMax), 1000);
			BigInteger bigInteger2 = current * bigInteger;
			BigInteger bigInteger3 = bigInteger2 / this.max;
			int num2 = 0;
			try
			{
				num2 = (int)(bigInteger3 * 1000 / bigInteger);
			}
			catch (Exception ex)
			{
				if (!this.hasSentStackOverflowDebugLog)
				{
					this.hasSentStackOverflowDebugLog = true;
					UnityEngine.Debug.LogWarning(string.Concat(new object[]
					{
						"StackOverflow: Current: ",
						current,
						", Max: ",
						this.max,
						", scaleFactor: ",
						bigInteger,
						", scaledCurrent: ",
						bigInteger2,
						", scaledWithSomePrecision: ",
						bigInteger3
					}));
				}
			}
			num = (float)num2 / 1000f;
		}
        UnityEngine.Vector2 v = UnityEngine.Vector2.zero;
		if (this.meterDirection == UIMeterBigInteger.MeterDirection.Backward)
		{
			v = (1f - num) * this.startScale;
		}
		else if (this.meterDirection == UIMeterBigInteger.MeterDirection.Foward)
		{
			v = num * this.startScale;
		}
		v.y = this.startScale.y;
		v.x = Mathf.Min(Mathf.Max(v.x, 0f), 1f);
		this.meter.transform.localScale = v;
	}

	private void Awake()
	{
		this.startScale = this.bg.transform.localScale;
	}

	[SerializeField]
	private Transform meter;

	[SerializeField]
	private Transform bg;

	[SerializeField]
	private UIMeterBigInteger.MeterDirection meterDirection;

	private BigInteger max = BigInteger.Zero;

	private BigInteger current = BigInteger.Zero;

	private UnityEngine.Vector2 startScale = UnityEngine.Vector2.zero;

	private float maxAsFloat;

	private int expOfMax;

	private bool hasSentStackOverflowDebugLog;

	public enum MeterDirection
	{
		Foward,
		Backward
	}
}
