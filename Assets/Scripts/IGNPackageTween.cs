using System;
using DG.Tweening;
using UnityEngine;

public class IGNPackageTween : MonoBehaviour
{
	private void Start()
	{
		this.symbolStartPosition = this.symbolRect.anchoredPosition;
		this.symbolShadowStartPosition = this.symbolRect.anchoredPosition;
		this.SetIconPassiveAnimation();
	}

	public void SetIconPassiveAnimation()
	{
		this.IconTweenKiller();
		this.boxJump = this.symbolRect.DOAnchorPosY(this.symbolStartPosition.y + 10f, 0.2f, false).SetAutoKill(false).SetEase(Ease.OutCubic).SetLoops(2, LoopType.Yoyo);
		this.boxSqueezeJump = this.symbolRect.DOScale(new Vector3(1.1f, 0.8f), 0.15f).SetAutoKill(false).SetLoops(2, LoopType.Yoyo);
		this.boxSqueezeLand = this.symbolRect.DOScale(new Vector3(1.1f, 0.8f), 0.15f).SetAutoKill(false).SetLoops(2, LoopType.Yoyo);
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
		this.symbolRect.DOKill(true);
		this.symbolShadowRect.DOKill(true);
	}

	private void OnDestroy()
	{
		this.IconTweenKiller();
	}

	[SerializeField]
	[Header("Icon Specific")]
	private RectTransform symbolRect;

	[SerializeField]
	private RectTransform symbolShadowRect;

	private Vector2 symbolStartPosition;

	private Vector2 symbolShadowStartPosition;

	private Tween boxJump;

	private Tween boxSqueezeJump;

	private Tween boxSqueezeLand;

	private Tween boxShadowJump;

	private Tween boxShadowLand;

	private Sequence boxSequence;
}
