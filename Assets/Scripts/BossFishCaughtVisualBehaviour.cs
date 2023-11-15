using System;
using DG.Tweening;
using UnityEngine;

public class BossFishCaughtVisualBehaviour : FishCaughtVisualBehaviour
{
	public override void Awake()
	{
		base.Awake();
		this.fishJumpAnimator = base.GetComponent<Animator>();
		this.hpBar = base.transform.parent.GetComponentInChildren<UIMeterBigInteger>();
	}

	public override void Jump(Vector2 jumpPosition)
	{
		this.hpBar.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
		AudioManager.Instance.BossCaught();
		this.fishSize = 1f;
		if (this.fishJumpAnimator != null)
		{
			this.fishJumpAnimator.speed = this.slowMotionModifier;
		}
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, -7f);
		if (BucketEffect.instance != null)
		{
			jumpPosition = BucketEffect.instance.transform.position;
		}
		else
		{
			UnityEngine.Debug.Log("FIXA ATT BucketEffect ÄR NULL!");
		}
		base.transform.localScale = base.transform.localScale * 0.9f;
		base.transform.DOScale(this.fishSize * 1.5f, 1f).SetEase(Ease.OutCirc).OnComplete(delegate
		{
			base.transform.DOScale(this.fishSize * 0.3f, 0.5f).SetEase(Ease.InCirc).OnUpdate(delegate
			{
				Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, 1.5f * Time.deltaTime);
			});
		}).OnUpdate(delegate
		{
			Time.timeScale = Mathf.Lerp(Time.timeScale, 0.05f, 1.5f * Time.deltaTime);
		});
		Vector3 endValue = new Vector3(jumpPosition.x + base.transform.position.x * 0f, jumpPosition.y + base.transform.position.y * 0f, base.transform.position.z);
		base.transform.DOMove(endValue, 1.5f, false).SetEase(Ease.InCirc).OnComplete(delegate
		{
			if (BucketEffect.instance != null)
			{
				AudioManager.Instance.BossLanding();
				BucketEffect.instance.Splash();
				if (BoatAnimationReferenceHelper.Instance != null)
				{
					BoatAnimationReferenceHelper.Instance.BossCaughtBoatAnimation();
				}
				this.waterRingBoatInstance = UnityEngine.Object.Instantiate<WaterRingBehaviour>(this.waterRingBoat, WaterEffectsHolderBehaviour.instance.behindSurfaceShaderPosition.transform);
				this.waterRingBoatInstance.transform.position = new Vector3(0f, 0f, WaterEffectsHolderBehaviour.instance.behindSurfaceShaderPosition.transform.position.z);
				this.waterRingBoatInstance.startDelay = 0f;
				this.waterRingBoatInstance.targetScale = 1.5f;
				this.waterRingBoatInstance.isDestroyOnFinish = true;
				this.waterRingBoatInstance.Go(0);
				this.waterRingBoatInstance2 = UnityEngine.Object.Instantiate<WaterRingBehaviour>(this.waterRingBoat, WaterEffectsHolderBehaviour.instance.behindSurfaceShaderPosition.transform);
				this.waterRingBoatInstance2.transform.position = new Vector3(0f, 0f, WaterEffectsHolderBehaviour.instance.behindSurfaceShaderPosition.transform.position.z - 1f);
				this.waterRingBoatInstance2.startDelay = 0.4f;
				this.waterRingBoatInstance2.targetScale = 1.2f;
				this.waterRingBoatInstance2.isDestroyOnFinish = true;
				this.waterRingBoatInstance2.Go(1);
			}
			else
			{
				UnityEngine.Debug.Log("FIXA ATT BucketEffect ÄR NULL!");
			}
			this.OnDestroy();
			Time.timeScale = 1f;
			this.fishAttributes.OnJumpFinished();
		});
		float num = Mathf.Atan2(jumpPosition.y - base.transform.position.y, jumpPosition.x - base.transform.position.x) * 57.29578f;
		base.transform.localEulerAngles = new Vector3(0f, 0f, num - 360f);
		this.waterRingInstance = UnityEngine.Object.Instantiate<WaterRingBehaviour>(this.waterRing, WaterEffectsHolderBehaviour.instance.behindSurfaceShaderPosition.transform);
		this.waterRingInstance.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, WaterEffectsHolderBehaviour.instance.behindSurfaceShaderPosition.transform.position.z);
		this.waterRingInstance.startDelay = 0f;
		this.waterRingInstance.targetScale = 3f;
		this.waterRingInstance.isDestroyOnFinish = true;
		this.waterRingInstance.Go(0);
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate<ParticleSystem>(this.waterSplash, WaterEffectsHolderBehaviour.instance.transform);
		particleSystem.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, particleSystem.transform.position.z);
		ParticleSystem particleSystem2 = UnityEngine.Object.Instantiate<ParticleSystem>(this.BossParticleSplash2, WaterEffectsHolderBehaviour.instance.transform);
		particleSystem2.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, particleSystem2.transform.position.z);
		this.waterRingInstance2 = UnityEngine.Object.Instantiate<WaterRingBehaviour>(this.waterRing, WaterEffectsHolderBehaviour.instance.behindSurfaceShaderPosition.transform);
		this.waterRingInstance2.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, WaterEffectsHolderBehaviour.instance.behindSurfaceShaderPosition.transform.position.z);
		this.waterRingInstance2.startDelay = 0.3f;
		this.waterRingInstance2.targetScale = 2.5f;
		this.waterRingInstance2.isDestroyOnFinish = true;
		this.waterRingInstance2.transform.localPosition = new Vector3(this.waterRingInstance2.transform.localPosition.x, this.waterRingInstance2.transform.localPosition.y, this.waterRingInstance2.transform.localPosition.z - 1f);
		this.waterRingInstance2.Go(1);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.hpBar.transform.DOKill(false);
	}

	private Animator fishJumpAnimator;

	private float slowMotionModifier = 0.4f;

	private UIMeterBigInteger hpBar;

	[SerializeField]
	private ParticleSystem BossParticleSplash2;

	protected WaterRingBehaviour waterRingInstance2;

	[SerializeField]
	private WaterRingBehaviour waterRingBoat;

	private WaterRingBehaviour waterRingBoatInstance;

	private WaterRingBehaviour waterRingBoatInstance2;
}
