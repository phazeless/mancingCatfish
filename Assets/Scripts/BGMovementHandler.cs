using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class BGMovementHandler : MonoBehaviour
{
	private void Awake()
	{
	}

	public void ActivateEffects()
	{
		this.TweenKiller(true);
		BGMovementHandler.boatMovementSpeed = this.localboatMovementSpeedTimerFixer;
		BGMovementHandler.waterShaderMovementFix = 2f;
		for (int i = 0; i < this.objectsToActivateWhen.Length; i++)
		{
			this.objectsToActivateWhen[i].SetActive(true);
		}
		DOTween.To(() => BGMovementHandler.boatMovementSpeed, delegate(float x)
		{
			BGMovementHandler.boatMovementSpeed = x;
		}, 0f, 2.1f * this.localboatMovementSpeedTimerFixer).SetDelay(1.2f * this.localboatMovementSpeedTimerFixer).OnComplete(delegate
		{
			this.localboatMovementSpeedTimerFixer = 1f;
			for (int j = 0; j < this.objectsToActivateWhen.Length; j++)
			{
				this.objectsToActivateWhen[j].SetActive(false);
			}
		}).SetId("BGBoatMovementHandlerSpeedValueTweener");
		DOTween.To(() => BGMovementHandler.waterShaderMovementFix, delegate(float x)
		{
			BGMovementHandler.waterShaderMovementFix = x;
		}, 1f, 3f * this.localboatMovementSpeedTimerFixer).SetEase(Ease.Linear).SetId("BGBoatMovementHandlerwaterShaderMovementFixTweener");
	}

	private void TweenKiller(bool complete = true)
	{
		DOTween.Kill("BGBoatMovementHandlerSpeedValueTweener", complete);
		DOTween.Kill("BGBoatMovementHandlerwaterShaderMovementFixTweener", complete);
	}

	private void OnDestroy()
	{
		this.TweenKiller(true);
	}

	[SerializeField]
	private GameObject[] objectsToActivateWhen;

	public static float boatMovementSpeed = 1f;

	public static float waterShaderMovementFix;

	private float localboatMovementSpeedTimerFixer = 1f;

	private int numberOfActivationCalls;
}
