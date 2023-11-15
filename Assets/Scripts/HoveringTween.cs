using System;
using DG.Tweening;
using UnityEngine;

public class HoveringTween : MonoBehaviour
{
	private void Awake()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.startYPosition = this.rectTransform.anchoredPosition.y;
	}

	private void OnEnable()
	{
		this.rectTransform.DOAnchorPosY(this.startYPosition + 10f, 1f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic).SetId("HowerTweenHover");
	}

	private void OnDisable()
	{
		this.Tweenkilling();
	}

	private void OnDestroy()
	{
		this.Tweenkilling();
	}

	private void Tweenkilling()
	{
		if (this.rectTransform != null)
		{
			DOTween.Kill("HowerTweenHover", false);
			this.rectTransform.anchoredPosition = new Vector2(this.rectTransform.anchoredPosition.x, this.startYPosition);
		}
	}

	private RectTransform rectTransform;

	private float startYPosition;
}
