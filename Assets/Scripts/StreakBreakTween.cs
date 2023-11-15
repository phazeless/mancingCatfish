using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StreakBreakTween : MonoBehaviour
{
	public void BreakStreakEffect(int day)
	{
		base.gameObject.SetActive(true);
		this.leftLabel.SetText((day + 1).ToString());
		this.rightLabel.SetText((day + 1).ToString());
		this.BreakParticle.Play();
		this.leftPart.DORotate(new Vector3(0f, 0f, 4f), 0.5f, RotateMode.Fast).SetEase(Ease.OutBack).OnComplete(delegate
		{
			this.leftPart.DORotate(new Vector3(0f, 0f, 2f), 1f, RotateMode.Fast).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
		});
		this.rightPart.DORotate(new Vector3(0f, 0f, -4f), 1f, RotateMode.Fast).SetEase(Ease.InOutQuad).OnComplete(delegate
		{
			this.rightPart.DORotate(new Vector3(0f, 0f, -2f), 0.5f, RotateMode.Fast).SetEase(Ease.OutBack).SetLoops(-1, LoopType.Yoyo);
		});
		this.leftPart.DOAnchorPosX(this.leftPart.anchoredPosition.x - 6f, 0.5f, false).SetEase(Ease.OutBack);
		this.rightPart.DOAnchorPosX(this.rightPart.anchoredPosition.x + 6f, 0.5f, false).SetEase(Ease.OutBack);
		this.leftPart.DOScale(1.1f, 0.5f).SetEase(Ease.OutBack);
		this.rightPart.DOScale(-1.1f, 0.5f).SetEase(Ease.OutBack);
		base.GetComponent<RectTransform>().DOAnchorPosY(base.GetComponent<RectTransform>().anchoredPosition.y - 4f, 1.5f, false).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
	}

	private void Tweenkiller()
	{
		this.leftPart.DOKill(false);
		this.rightPart.DOKill(false);
		base.GetComponent<RectTransform>().DOKill(false);
	}

	private void OnDestroy()
	{
		this.Tweenkiller();
	}

	[SerializeField]
	private RectTransform leftPart;

	[SerializeField]
	private TextMeshProUGUI leftLabel;

	[SerializeField]
	private RectTransform rightPart;

	[SerializeField]
	private TextMeshProUGUI rightLabel;

	[SerializeField]
	private ParticleSystem BreakParticle;
}
