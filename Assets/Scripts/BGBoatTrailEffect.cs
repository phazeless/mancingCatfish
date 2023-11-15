using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BGBoatTrailEffect : MonoBehaviour
{
	private void Awake()
	{
		this.rndID = UnityEngine.Random.Range(0, 999999);
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.sortingOrder = -10;
		this.animationCurve.AddKey(0f, 0.3f);
		this.animationCurve.AddKey(0.01f, 0.3f);
		this.animationCurve.AddKey(1f, 0.9f);
	}

	private void OnEnable()
	{
		this.TweenKiller();
		this.v0 = 0f;
		this.v1 = 0f;
		this.v2 = 0f;
		this.t1 = 0f;
		DOTween.To(() => this.v2, delegate(float x)
		{
			this.v2 = x;
		}, 1f, 0.5f).SetEase(Ease.Linear).SetId("BGBoatTrailEffectValueBackStartTween" + this + this.rndID);
		DOTween.To(() => this.v1, delegate(float x)
		{
			this.v1 = x;
		}, 0.3f, 0.4f).SetEase(Ease.Linear).SetId("BGBoatTrailEffectValueStartTween" + this + this.rndID).OnComplete(delegate
		{
			DOTween.To(() => this.v1, delegate(float x)
			{
				this.v1 = x;
			}, 0.99f, 0.8f).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).SetId("BGBoatTrailEffectValueTween" + this + this.rndID);
		});
		DOTween.To(() => this.t1, delegate(float x)
		{
			this.t1 = x;
		}, 0.01f, 0.4f).SetEase(Ease.Linear).SetId("BGBoatTrailEffectTimeStartTween" + this + this.rndID).OnComplete(delegate
		{
			DOTween.To(() => this.t1, delegate(float x)
			{
				this.t1 = x;
			}, 0.99f, 0.8f).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).SetId("BGBoatTrailEffectTimeTween" + this + this.rndID);
		});
	}

	private void Update()
	{
		this.lineRenderer.widthCurve = this.animationCurve;
		this.animationCurve.MoveKey(0, new Keyframe(0f, 0.3f * BGMovementHandler.boatMovementSpeed));
		this.animationCurve.MoveKey(1, new Keyframe(this.t1, this.v1 * BGMovementHandler.boatMovementSpeed));
		this.animationCurve.MoveKey(2, new Keyframe(1f, this.v2 * BGMovementHandler.boatMovementSpeed));
	}

	private float getter()
	{
		return this.animationCurve[1].value;
	}

	private void setter(float x)
	{
		this.v1 = x;
	}

	private void TweenKiller()
	{
		DOTween.Kill("BGBoatTrailEffectValueTween" + this + this.rndID, true);
		DOTween.Kill("BGBoatTrailEffectTimeTween" + this + this.rndID, true);
		DOTween.Kill("BGBoatTrailEffectValueStartTween" + this + this.rndID, true);
		DOTween.Kill("BGBoatTrailEffectTimeStartTween" + this + this.rndID, true);
		DOTween.Kill("BGBoatTrailEffectValueBackStartTween" + this + this.rndID, true);
	}

	private void OnDisable()
	{
		this.TweenKiller();
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	private LineRenderer lineRenderer;

	private AnimationCurve animationCurve = new AnimationCurve();

	[SerializeField]
	private ParticleSystem boatBackParticle;

	private float v0;

	private float v1 = 0.3f;

	private float v2 = 1f;

	private float t1 = 0.01f;

	private int rndID;
}
