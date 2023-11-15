using System;
using DG.Tweening;
using UnityEngine;

public class DailyGiftChestTween : MonoBehaviour
{
	private void OnEnable()
	{
		this.UpdateTweenState();
	}

	public void UpdateTweenState()
	{
		this.TweenKiller();
		base.transform.localScale = Vector2.one;
		if (!this.isActive)
		{
			base.transform.DOScale(0.9f, 0.5f).SetEase(Ease.OutElastic);
		}
		else
		{
			base.transform.DOScale(1.05f, 1f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
		}
	}

	private void OnDisable()
	{
		this.TweenKiller();
	}

	private void TweenKiller()
	{
		base.transform.DOKill(true);
	}

	public bool isActive;
}
