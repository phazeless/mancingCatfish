using System;
using DG.Tweening;
using UnityEngine;

public class PowerupAnimationShoal : PowerupAnimationBaseClass
{
	public override void SetOffCooldownAnimation()
	{
		this.fish1.gameObject.SetActive(true);
		this.fish2.gameObject.SetActive(true);
		this.fish3.gameObject.SetActive(true);
	}

	private void Start()
	{
		this.sequence2 = DOTween.Sequence();
		float animationspeed = 0.7f;
		this.fish2.localScale = Vector2.zero;
		this.fish2.anchoredPosition = new Vector2(50f, this.fish2.anchoredPosition.y);
		this.sequence2.Append(this.fish2.DOAnchorPosX(-50f, animationspeed, false).SetEase(Ease.Linear)).Join(this.fish2.DOScale(1f, 0.2f).SetEase(Ease.Linear)).Join(this.fish2.DOScale(0f, 0.2f).SetDelay(animationspeed - 0.2f).SetEase(Ease.Linear)).SetLoops(-1, LoopType.Restart);
		this.RunAfterDelay(0.3f, delegate()
		{
			this.sequence1 = DOTween.Sequence();
			this.sequence3 = DOTween.Sequence();
			this.fish1.localScale = Vector2.zero;
			this.fish1.anchoredPosition = new Vector2(50f, this.fish1.anchoredPosition.y);
			this.sequence1.Append(this.fish1.DOAnchorPosX(-50f, animationspeed, false).SetEase(Ease.Linear)).Join(this.fish1.DOScale(1f, 0.2f).SetEase(Ease.Linear)).Join(this.fish1.DOScale(0f, 0.2f).SetDelay(animationspeed - 0.2f).SetEase(Ease.Linear)).SetLoops(-1, LoopType.Restart);
			this.fish3.localScale = Vector2.zero;
			this.fish3.anchoredPosition = new Vector2(50f, this.fish2.anchoredPosition.y);
			this.sequence3.Append(this.fish3.DOAnchorPosX(-50f, animationspeed, false).SetEase(Ease.Linear)).Join(this.fish3.DOScale(1f, 0.2f).SetEase(Ease.Linear)).Join(this.fish3.DOScale(0f, 0.2f).SetDelay(animationspeed - 0.2f).SetEase(Ease.Linear)).SetLoops(-1, LoopType.Restart);
		});
		this.fish1.DOAnchorPosY(this.fish1.anchoredPosition.y + 7f, 0.6f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(0.2f);
		this.fish2.DOAnchorPosY(this.fish2.anchoredPosition.y + 7f, 0.6f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
		this.fish3.DOAnchorPosY(this.fish3.anchoredPosition.y + 7f, 0.5f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(0.2f);
	}

	public override void SetOnCooldownAnimation()
	{
		this.fish1.gameObject.SetActive(false);
		this.fish2.gameObject.SetActive(false);
		this.fish3.gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		DOTween.Pause(this.fish1);
		DOTween.Pause(this.fish2);
		DOTween.Pause(this.fish3);
	}

	private void OnEnable()
	{
		DOTween.Play(this.fish1);
		DOTween.Play(this.fish2);
		DOTween.Play(this.fish3);
	}

	[SerializeField]
	private RectTransform fish1;

	[SerializeField]
	private RectTransform fish2;

	[SerializeField]
	private RectTransform fish3;

	private Sequence sequence1;

	private Sequence sequence2;

	private Sequence sequence3;
}
