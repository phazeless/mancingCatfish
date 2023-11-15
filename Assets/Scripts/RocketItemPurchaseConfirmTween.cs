using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RocketItemPurchaseConfirmTween : MonoBehaviour
{
	public void Open(int rocketAmount = 10)
	{
		this.startingPosition = this.reward.localPosition;
		base.transform.SetParent(ScreenManager.Instance.transform);
		base.transform.position = Vector3.zero;
		base.gameObject.SetActive(true);
		this.amountLabel.SetVariableText(new string[]
		{
			rocketAmount.ToString()
		});
		this.bg.DOFade(0.7f, 0.3f);
		this.isAnimatingIn = true;
		this.reward.localEulerAngles = new Vector3(0f, 0f, 60f);
		this.reward.localScale = new Vector3(0f, 0f, 0f);
		this.reward.DOScale(1.3f, 0.5f).SetEase(Ease.OutElastic);
		this.reward.DORotate(new Vector3(0f, 0f, -3f), 1f, RotateMode.Fast).SetEase(Ease.OutElastic).OnComplete(delegate
		{
			this.reward.DORotate(new Vector3(0f, 0f, 3f), 3f, RotateMode.Fast).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
		});
		this.reward.DOMoveY(this.reward.transform.position.y + 0.2f, 2f, false).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
		this.amountTransform.DOScale(1.2f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.3f).OnStart(delegate
		{
			this.particles.Play();
			this.isAnimatingIn = false;
		});
		this.tapToContinueTransform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack).SetDelay(2f);
	}

	public void Close()
	{
		if (this.isAnimatingIn)
		{
			return;
		}
		this.TweenKiller(false);
		this.isAnimatingIn = true;
		this.tapToContinueTransform.DOScale(0f, 0.2f);
		this.bg.DOFade(0f, 0.2f);
		this.reward.DOScale(0f, 0.2f).SetEase(Ease.InBack);
		this.amountTransform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(delegate
		{
			this.reward.localPosition = this.startingPosition;
			base.gameObject.SetActive(false);
		});
	}

	private void TweenKiller(bool complete)
	{
		this.tapToContinueTransform.DOKill(complete);
		this.amountTransform.DOKill(complete);
		this.reward.DOKill(complete);
		this.bg.DOKill(complete);
	}

	private void OnDestroy()
	{
		this.TweenKiller(true);
	}

	[SerializeField]
	private Image bg;

	[SerializeField]
	private Transform reward;

	[SerializeField]
	private ParticleSystem particles;

	[SerializeField]
	private Transform amountTransform;

	[SerializeField]
	private TextMeshProUGUI amountLabel;

	[SerializeField]
	private Transform tapToContinueTransform;

	private Vector3 startingPosition;

	private bool isAnimatingIn;
}
