using System;
using DG.Tweening;
using UnityEngine;

public class RecurringButtonShine : MonoBehaviour
{
	private void Awake()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.startingX = this.rectTransform.anchoredPosition.x;
	}

	private void SetTween()
	{
		this.rectTransform.anchoredPosition = new Vector2(this.startingX, this.rectTransform.anchoredPosition.y);
		this.rectTransform.DOAnchorPosX(-this.startingX, 0.4f, false).SetDelay(this.delay).OnComplete(delegate
		{
			this.SetTween();
		});
	}

	private void OnDisable()
	{
		this.rectTransform.DOKill(false);
		this.delay -= this.interval;
	}

	private void OnEnable()
	{
		this.SetTween();
		this.delay += this.interval;
	}

	private void OnDestroy()
	{
		this.rectTransform.DOKill(false);
	}

	private RectTransform rectTransform;

	private float startingX;

	[SerializeField]
	private float delay = 0.5f;

	[SerializeField]
	private float interval = 5f;
}
