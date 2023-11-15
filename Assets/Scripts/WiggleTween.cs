using System;
using DG.Tweening;
using UnityEngine;

public class WiggleTween : MonoBehaviour
{
	private void Awake()
	{
		this.startingRotation = this.rectTransform.localEulerAngles;
	}

	private void OnEnable()
	{
		this.StartTween();
	}

	public void StartTween()
	{
		this.TweenKiller();
		this.rectTransform.DORotate(new Vector3(0f, 0f, this.startingRotation.z + this.range), this.timeInterval, RotateMode.Fast).SetEase(this.ease).OnComplete(delegate
		{
			this.rectTransform.DORotate(new Vector3(0f, 0f, this.startingRotation.z - this.range), this.timeInterval, RotateMode.Fast).SetEase(this.ease).OnComplete(delegate
			{
				this.StartTween();
			});
		});
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
		this.rectTransform.localEulerAngles = this.startingRotation;
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

	private Vector3 startingRotation;
}
