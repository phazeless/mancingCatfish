using System;
using UnityEngine;

public class WaterEffect : MonoBehaviour, IPooledObject<WaterEffect>
{
	public WaterRingBehaviour RingEffect
	{
		get
		{
			return this.ringEffect;
		}
	}

	public ParticleSystem SplashEffect
	{
		get
		{
			return this.splashEffect;
		}
	}

	private void Awake()
	{
		this.ringEffect.OnTweenFinished += this.RingEffect_OnTweenFinished;
	}

	private void RingEffect_OnTweenFinished()
	{
		this.pool.ReturnObject(this);
	}

	public void OnAddedToPool(ObjectPool<WaterEffect> pool)
	{
		this.pool = pool;
	}

	public void OnRetrieved(ObjectPool<WaterEffect> pool)
	{
		base.gameObject.SetActive(true);
	}

	public void OnReturned(ObjectPool<WaterEffect> pool)
	{
		base.gameObject.SetActive(false);
	}

	[SerializeField]
	private WaterRingBehaviour ringEffect;

	[SerializeField]
	private ParticleSystem splashEffect;

	private ObjectPool<WaterEffect> pool;
}
