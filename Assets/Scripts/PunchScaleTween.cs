using System;
using DG.Tweening;
using UnityEngine;

public class PunchScaleTween : MonoBehaviour
{
	private void OnEnable()
	{
		this.OnTweenActivated();
	}

	private void OnTweenActivated()
	{
		this.thingToBounce.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f, 7, 0.7f).SetDelay(this.delay).OnComplete(delegate
		{
			this.OnTweenActivated();
		});
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	private void TweenKiller()
	{
		this.thingToBounce.DOKill(false);
	}

	[SerializeField]
	private Transform thingToBounce;

	[SerializeField]
	private float delay = 2.3f;
}
