using System;
using DG.Tweening;
using UnityEngine;

public class PowerupAnimationAuto : PowerupAnimationBaseClass
{
	public override void SetOffCooldownAnimation()
	{
		this.fish1.gameObject.SetActive(true);
		this.fish2.gameObject.SetActive(true);
		this.fish3.gameObject.SetActive(true);
		this.surface.SetActive(true);
	}

	private void Start()
	{
		float animationspeed = 0.5f;
		this.sequence2 = DOTween.Sequence();
		this.fish2.localScale = Vector2.zero;
		this.fish2.anchoredPosition = new Vector2(this.fish2.anchoredPosition.x, -55f);
		this.sequence2.Append(this.fish2.DOAnchorPos(new Vector2(0f, 55f), animationspeed, false).SetEase(Ease.Linear)).Join(this.fish2.DOScale(1f, 0.2f).SetEase(Ease.Linear)).Join(this.fish2.DOScale(0f, 0.2f).SetDelay(animationspeed - 0.2f).SetEase(Ease.Linear)).SetLoops(-1, LoopType.Restart);
		this.RunAfterDelay(0.25f, delegate()
		{
			this.sequence1 = DOTween.Sequence();
			this.sequence3 = DOTween.Sequence();
			this.fish1.localScale = Vector2.zero;
			this.fish1.anchoredPosition = new Vector2(this.fish1.anchoredPosition.x, -55f);
			this.sequence1.Append(this.fish1.DOAnchorPos(new Vector2(0f, 55f), animationspeed, false).SetEase(Ease.Linear)).Join(this.fish1.DOScale(1f, 0.2f).SetEase(Ease.Linear)).Join(this.fish1.DOScale(0f, 0.2f).SetDelay(animationspeed - 0.2f).SetEase(Ease.Linear)).SetLoops(-1, LoopType.Restart);
			this.fish3.localScale = Vector2.zero;
			this.fish3.anchoredPosition = new Vector2(this.fish3.anchoredPosition.x, -55f);
			this.sequence3.Append(this.fish3.DOAnchorPos(new Vector2(0f, 55f), 0.5f, false).SetEase(Ease.Linear).SetDelay(0.25f)).Join(this.fish3.DOScale(1f, 0.2f).SetEase(Ease.Linear)).Join(this.fish3.DOScale(0f, 0.2f).SetDelay(animationspeed - 0.2f).SetEase(Ease.Linear)).SetLoops(-1, LoopType.Restart);
		});
	}

	public override void SetOnCooldownAnimation()
	{
		this.fish1.gameObject.SetActive(false);
		this.fish2.gameObject.SetActive(false);
		this.fish3.gameObject.SetActive(false);
		this.surface.SetActive(false);
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

	[SerializeField]
	private GameObject surface;

	private Sequence sequence1;

	private Sequence sequence2;

	private Sequence sequence3;
}
