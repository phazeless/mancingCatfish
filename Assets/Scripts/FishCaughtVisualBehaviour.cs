using System;
using DG.Tweening;
using UnityEngine;

public class FishCaughtVisualBehaviour : MonoBehaviour
{
	private bool DidPassSanityCheck()
	{
		if (base.gameObject == null || base.transform == null)
		{
			UnityEngine.Debug.LogError("gameObject or transform is null inside FishCaughtVisualBehaviour.Jump, can't determine on what object since they are null");
			return false;
		}
		if (this.fishAttributes == null)
		{
			string str = "Unknown";
			if (base.transform.parent != null)
			{
				str = base.transform.parent.name;
				this.fishAttributes = base.transform.parent.GetComponent<FishBehaviour>();
			}
			UnityEngine.Debug.LogError("fishAttributes was null inside FishCaughtVisualBehaviour on GameObject: " + str + ", will try to set it again");
			return this.fishAttributes != null;
		}
		return true;
	}

	public virtual void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.notifyOutOfScreen = base.transform.parent.GetComponent<NotifyOutOfScreen>();
		this.fishAttributes = base.transform.parent.GetComponent<FishBehaviour>();
	}

	public virtual void Jump(Vector2 jumpPosition)
	{
		if (!this.DidPassSanityCheck())
		{
			return;
		}
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.FishJump();
		}
		this.fishSize = Mathf.Pow(this.fishAttributes.ActualWeight / 100f, 0.2f);
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, -2f);
		if (BucketEffect.instance != null)
		{
			jumpPosition = BucketEffect.instance.transform.position;
		}
		base.transform.DOScale(this.fishSize * 1.6f, 0.25f).OnComplete(delegate
		{
			base.transform.DOScale(this.fishSize * 1f, 0.3f);
		});
		base.transform.DOMove(jumpPosition, 0.5f, false).SetEase(Ease.Linear).OnComplete(delegate
		{
			if (BucketEffect.instance != null && AudioManager.Instance != null)
			{
				BucketEffect.instance.Splash();
				AudioManager.Instance.FishLand();
			}
			if (this.fishAttributes != null)
			{
				this.fishAttributes.OnJumpFinished();
			}
			else
			{
				UnityEngine.Debug.LogError("fishAttributes is null and somehow got passed sanity check...");
			}
		});
		float num = Mathf.Atan2(jumpPosition.y - base.transform.position.y, jumpPosition.x - base.transform.position.x) * 57.29578f;
		base.transform.localEulerAngles = new Vector3(0f, 0f, num - 360f);
		WaterEffect waterEffect = FishPoolManager.Instance.GetWaterEffect();
		if (waterEffect != null)
		{
			this.waterRingInstance = waterEffect.RingEffect;
			if (this.waterRingInstance != null)
			{
				this.waterRingInstance.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, this.waterRingInstance.transform.position.z);
				this.waterRingInstance.startDelay = 0f;
				this.waterRingInstance.Go(0);
			}
			else
			{
				UnityEngine.Debug.LogError("waterRingInstance is null and somehow got passed sanity check...");
			}
			this.waterSplashInstance = waterEffect.SplashEffect;
			if (this.waterSplashInstance != null)
			{
				this.waterSplashInstance.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z - 1f);
			}
			else
			{
				UnityEngine.Debug.LogError("waterSplashInstance is null and somehow got passed sanity check...");
			}
		}
		else
		{
			UnityEngine.Debug.LogError("waterEffect is null and somehow got passed sanity check...");
		}
	}

	protected virtual void OnDestroy()
	{
		base.transform.DOKill(false);
	}

	[SerializeField]
	protected WaterRingBehaviour waterRing;

	[SerializeField]
	protected ParticleSystem waterSplash;

	[SerializeField]
	protected Transform effectsHolder;

	[SerializeField]
	protected FishShadowBehaviour fishShadowBehaviour;

	protected float fishSize = 1f;

	protected SpriteRenderer spriteRenderer;

	protected SpriteRenderer fishShadowSpriteRenderer;

	protected NotifyOutOfScreen notifyOutOfScreen;

	protected ParticleSystem waterSplashInstance;

	protected WaterRingBehaviour waterRingInstance;

	protected FishBehaviour fishAttributes;
}
