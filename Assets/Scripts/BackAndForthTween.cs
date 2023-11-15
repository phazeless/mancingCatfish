using System;
using DG.Tweening;
using UnityEngine;

public class BackAndForthTween : MonoBehaviour
{
	private void Awake()
	{
		this.startingPos = this.rectTransform.anchoredPosition;
	}

	private void OnEnable()
	{
		this.StartTween();
	}

	public void StartTween()
	{
		this.TweenKiller();
		if (this.vertical)
		{
			this.rectTransform.DOAnchorPosY(this.startingPos.y + this.range, this.timeInterval, false).SetEase(this.ease).OnComplete(delegate
			{
				this.rectTransform.DOAnchorPosY(this.startingPos.y - this.range, this.timeInterval, false).SetEase(this.ease).OnComplete(delegate
				{
					this.StartTween();
				});
			});
		}
		else
		{
			this.rectTransform.DOAnchorPosX(this.startingPos.x + this.range, this.timeInterval, false).SetEase(this.ease).OnComplete(delegate
			{
				this.rectTransform.DOAnchorPosX(this.startingPos.x - this.range, this.timeInterval, false).SetEase(this.ease).OnComplete(delegate
				{
					this.StartTween();
				});
			});
		}
	}

	private void OnDisable()
	{
		this.TweenKiller();
		this.ResetState();
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	private void ResetState()
	{
		this.rectTransform.anchoredPosition = this.startingPos;
	}

	private void TweenKiller()
	{
		this.rectTransform.DOKill(false);
	}

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private float timeInterval = 1f;

	[SerializeField]
	private float range = 25f;

	[SerializeField]
	private Ease ease;

	[SerializeField]
	private bool vertical;

	private Vector2 startingPos;
}
