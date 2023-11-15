using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IGNQuestIconTween : MonoBehaviour
{
	private void Start()
	{
		this.SetUnOpened();
		this.symbolStartPosition = this.checkboxHolder.anchoredPosition;
		this.symbolShadowStartPosition = this.checkboxHolder.anchoredPosition;
		this.SetIconPassiveAnimation();
	}

	public void SetIconPassiveAnimation()
	{
		this.IconTweenKiller();
		this.boxJump = this.checkboxHolder.DOAnchorPosY(this.symbolStartPosition.y + 10f, 0.2f, false).SetAutoKill(false).SetEase(Ease.OutCubic).SetLoops(2, LoopType.Yoyo);
		this.boxSqueezeJump = this.checkboxHolder.DOScale(new Vector3(1.1f, 0.8f), 0.15f).SetAutoKill(false).SetLoops(2, LoopType.Yoyo);
		this.boxSqueezeLand = this.checkboxHolder.DOScale(new Vector3(1.1f, 0.8f), 0.15f).SetAutoKill(false).SetLoops(2, LoopType.Yoyo);
		this.boxShadowJump = this.symbolShadowRect.DOScale(new Vector3(1.1f, 0.8f), 0.15f).SetAutoKill(false).SetLoops(2, LoopType.Yoyo);
		this.boxShadowLand = this.symbolShadowRect.DOScale(new Vector3(1.1f, 0.8f), 0.15f).SetAutoKill(false).SetLoops(2, LoopType.Yoyo);
		this.boxJump.Pause<Tween>();
		this.boxSqueezeJump.Pause<Tween>();
		this.boxSqueezeLand.Pause<Tween>();
		this.boxShadowJump.Pause<Tween>();
		this.boxShadowLand.Pause<Tween>();
		this.boxSequence = DOTween.Sequence();
		this.boxSequence.Append(this.boxSqueezeJump).Join(this.boxShadowJump).Append(this.boxJump).Append(this.boxSqueezeLand).Join(this.boxShadowLand).AppendInterval(2f).SetLoops(-1, LoopType.Restart);
		this.boxSequence.Pause<Sequence>();
		this.boxSequence.Play<Sequence>();
	}

	public void IconTweenKiller()
	{
		this.boxSequence.Kill(true);
		this.checkboxHolder.DOKill(true);
		this.checkboxHolder.localScale = Vector2.one;
		this.checkboxHolder.anchoredPosition = this.symbolStartPosition;
		this.symbolShadowRect.DOKill(true);
	}

	public void SetUnOpened()
	{
	}

	public void SetOpened()
	{
		this.IconTweenKiller();
		this.checkbox.gameObject.SetActive(false);
		this.symbolShadowRect.gameObject.SetActive(false);
		this.progressLabelRect.gameObject.SetActive(true);
	}

	public void SetCompleted()
	{
		this.checkboxOutline.color = Color.white;
		this.symbolShadowRect.gameObject.SetActive(true);
		this.checkbox.gameObject.SetActive(true);
		this.SetIconPassiveAnimation();
		this.progressLabelRect.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		this.IconTweenKiller();
	}

	[SerializeField]
	[Header("Icon Specific")]
	private RectTransform checkboxHolder;

	[SerializeField]
	private Image checkboxOutline;

	[SerializeField]
	private RectTransform checkbox;

	[SerializeField]
	private RectTransform symbolShadowRect;

	[SerializeField]
	private RectTransform progressLabelRect;

	public bool hasBeenOpened;

	private Vector2 symbolStartPosition;

	private Vector2 symbolShadowStartPosition;

	private Tween boxJump;

	private Tween boxSqueezeJump;

	private Tween boxSqueezeLand;

	private Tween boxShadowJump;

	private Tween boxShadowLand;

	private Sequence boxSequence;
}
