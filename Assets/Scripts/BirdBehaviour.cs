using System;
using System.Numerics;
using DG.Tweening;
using UnityEngine;

public class BirdBehaviour : FishBehaviour
{
	protected override void Awake()
	{
		base.Awake();
		this.myTransform = base.GetComponent<Transform>();
		this.cam = Camera.main;
	}

	private void OnEnable()
	{
		this.myTransform.position = this.cam.ViewportToWorldPoint(new UnityEngine.Vector2(UnityEngine.Random.Range(0f, 1f), 0f));
		this.myTransform.position = new UnityEngine.Vector3(this.myTransform.position.x, base.transform.position.y, 0f);
		this.myTransform.localEulerAngles = UnityEngine.Vector3.zero;
		this.myTransform.DORotate(new UnityEngine.Vector3(0f, 0f, UnityEngine.Random.Range(-30f, 30f)), 3f, RotateMode.Fast);
	}

	private void Update()
	{
		this.myTransform.Translate(new UnityEngine.Vector2(0f, 3f * Time.deltaTime));
        UnityEngine.Vector3 vector = this.cam.WorldToViewportPoint(this.myTransform.position);
		if (vector.x < 0f || (vector.x > 1f && vector.y < 0f) || vector.y > 1f)
		{
			this.spawner.PoolBird(this.myTransform);
		}
	}

	public override bool IsCaught()
	{
		bool flag = ItemAndSkillValues.GetCurrentTotalValueFor<Skills.BirdCatchAbility>() > 0f;
		if (flag)
		{
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate<ParticleSystem>(this.birdCatchParticle);
			particleSystem.transform.position = base.transform.position;
			if (this.birdCatchCounter != null)
			{
				this.birdCatchCounter.TryLevelUp();
			}
		}
		return flag;
	}

	public override BigInteger GetValue(out bool isCriticalValue, bool isAfk = false)
	{
		BigInteger value = base.GetValue(out isCriticalValue, isAfk);
		BigInteger left = BigInteger.Pow(5, DWHelper.CurrentDWLevel);
		float value2 = ItemAndSkillValues.GetCurrentTotalValueFor<Skills.BirdValueModifier>() * ItemAndSkillValues.GetCurrentTotalValueFor<Skills.CargoModifier>();
		return left * value.MultiplyFloat(value2);
	}

	private Transform myTransform;

	private Camera cam;

	public BirdSpawner spawner;

	[SerializeField]
	private ParticleSystem birdCatchParticle;

	[SerializeField]
	private Skill birdCatchCounter;
}
