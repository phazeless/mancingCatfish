using System;
using DG.Tweening;
using UnityEngine;

public class CrownLevelCrownTween : MonoBehaviour
{
	private void Awake()
	{
		this.crownHolderStartingScale = this.crownHolder.localScale;
		this.fishStartingPosition = this.fish.localPosition;
		this.bobberStartingPosition = this.bobber.localPosition;
	}

	private void OnEnable()
	{
		this.TweenKiller();
		this.crownHolder.localScale = Vector2.zero;
		this.crownHolder.DOScale(this.crownHolderStartingScale, 0.3f).SetEase(Ease.OutBack);
		this.crownHolder.localEulerAngles = new Vector3(0f, 0f, 8f);
		this.crownHolder.DORotate(Vector3.zero, 0.8f, RotateMode.Fast).SetEase(Ease.OutBack);
		this.crownLevelLabel.localScale = Vector2.zero;
		this.crownLevelLabel.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(0.1f);
		this.fish.localPosition = Vector2.zero;
		this.fish.DOLocalMove(this.fishStartingPosition, 0.5f, false).SetEase(Ease.OutCirc).SetDelay(0.2f);
		this.bobber.localPosition = Vector2.zero;
		this.bobber.DOLocalMove(this.bobberStartingPosition, 0.5f, false).SetEase(Ease.OutCirc).SetDelay(0.3f);
		this.bobber.localEulerAngles = new Vector3(0f, 0f, -10f);
		this.bobber.DORotate(Vector3.zero, 0.8f, RotateMode.Fast).SetDelay(0.3f).SetEase(Ease.OutBack);
		this.fish.localEulerAngles = new Vector3(0f, 0f, 10f);
		this.fish.DORotate(Vector3.zero, 0.8f, RotateMode.Fast).SetDelay(0.2f).SetEase(Ease.OutBack);
	}

	private void TweenKiller()
	{
		this.crownHolder.DOKill(true);
		this.crownLevelLabel.DOKill(true);
		this.fish.DOKill(true);
		this.bobber.DOKill(true);
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	[Header("References")]
	[SerializeField]
	private Transform crownHolder;

	[SerializeField]
	private Transform fish;

	[SerializeField]
	private Transform bobber;

	[SerializeField]
	private Transform crownLevelLabel;

	private Vector2 crownHolderStartingScale = Vector2.one;

	private Vector2 fishStartingPosition = Vector2.one;

	private Vector2 bobberStartingPosition = Vector2.one;
}
