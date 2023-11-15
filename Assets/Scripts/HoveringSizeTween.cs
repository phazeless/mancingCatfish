using System;
using DG.Tweening;
using UnityEngine;

public class HoveringSizeTween : MonoBehaviour
{
	private void Awake()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.startYPosition = this.rectTransform.anchoredPosition.y;
	}

	private void OnEnable()
	{
		this.rectTransform.DOScale(1.1f, 2f).SetLoops(-1, LoopType.Yoyo);
		this.rectTransform.DOAnchorPosY(this.startYPosition + 10f, 1f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
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
		this.rectTransform.DOKill(true);
	}

	private RectTransform rectTransform;

	private float startYPosition;
}
