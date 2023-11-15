using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DWIconPassiveTween : MonoBehaviour
{
	private void Awake()
	{
		this.rectTrans = base.GetComponent<RectTransform>();
	}

	public void SetNotCurrentAnymore()
	{
		this.TweenKiller();
		this.isCurrentPassive = false;
	}

	public void SetCurrentPassiveTweens()
	{
		this.TweenKiller();
		this.isCurrentPassive = true;
		if (this.rectTrans != null)
		{
			this.rectTrans.anchoredPosition = Vector2.zero;
			this.rectTrans.localScale = Vector2.one;
			this.rectTrans.DOScale(1.05f, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
			this.rectTrans.DOAnchorPosY(5f, 3f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
		}
		if (this.bubble0 != null && this.bubble1 != null)
		{
			this.bubble0.anchoredPosition = new Vector2(-42f, -70f);
			this.bubble1.anchoredPosition = new Vector2(30f, -70f);
			this.bubble0.localScale = Vector2.one * 1.5f;
			this.bubble1.localScale = Vector2.one * 1.5f;
			float num = 5f;
			this.bubble0.DOAnchorPosY(70f, num, false).SetLoops(-1, LoopType.Restart);
			this.bubble0.DOAnchorPosX(-48f, num / 2f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
			this.bubble0.DOScale(0.4f, num).SetLoops(-1, LoopType.Restart);
			this.bubble1.DOAnchorPosY(75f, num, false).SetLoops(-1, LoopType.Restart).SetDelay(num * 0.3f);
			this.bubble1.DOAnchorPosX(36f, num / 2f, false).SetLoops(-1, LoopType.Yoyo).SetDelay(num * 0.3f);
			this.bubble1.DOScale(0.4f, num).SetLoops(-1, LoopType.Restart).SetDelay(num * 0.3f);
		}
		if (this.fishRight0 != null)
		{
			this.fishRight0.anchoredPosition = new Vector2(90f - this.fishRight0.sizeDelta.x / 2f, 7f);
			this.fishRight0.localEulerAngles = new Vector3(0f, 0f, -4f);
			this.fishRight0.DOAnchorPosX(this.fishRight0.anchoredPosition.x - 5f, 1.5f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
			this.fishRight0.DOAnchorPosY(this.fishRight0.anchoredPosition.y - 5f, 2.5f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
			this.fishRight0.DORotate(new Vector3(0f, 0f, 4f), 3f, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
		}
		if (this.fishRight1 != null)
		{
			this.fishRight1.anchoredPosition = new Vector2(85f - this.fishRight1.sizeDelta.x / 2f, 30f);
			this.fishRight1.DOAnchorPosX(this.fishRight1.anchoredPosition.x - 5f, 1.5f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
			this.fishRight1.DOAnchorPosY(this.fishRight1.anchoredPosition.y - 5f, 2.5f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
			this.fishRight1.DORotate(new Vector3(0f, 0f, 4f), 3f, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
		}
		if (this.fishLeft != null)
		{
			this.fishLeft.anchoredPosition = new Vector2(-87f + this.fishLeft.sizeDelta.x / 2f, 5f);
			this.fishLeft.DOAnchorPosX(this.fishLeft.anchoredPosition.x + 5f, 1.5f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
			this.fishLeft.DOAnchorPosY(this.fishLeft.anchoredPosition.y + 5f, 2.5f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
			this.fishLeft.DORotate(new Vector3(0f, 0f, 4f), 3f, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
		}
	}

	private void TweenKiller()
	{
		if (this.rectTrans != null)
		{
			this.rectTrans.DOKill(false);
		}
		if (this.bubble0 != null)
		{
			this.bubble0.DOKill(false);
		}
		if (this.bubble1 != null)
		{
			this.bubble1.DOKill(false);
		}
		if (this.fishRight0 != null)
		{
			this.fishRight0.DOKill(false);
		}
		if (this.fishRight1 != null)
		{
			this.fishRight1.DOKill(false);
		}
		if (this.fishLeft != null)
		{
			this.fishLeft.DOKill(false);
		}
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	private void OnEnable()
	{
		if (this.isCurrentPassive)
		{
			this.SetCurrentPassiveTweens();
		}
	}

	private void OnDisable()
	{
		if (this.isCurrentPassive)
		{
			this.TweenKiller();
		}
	}

	[SerializeField]
	private RectTransform bubble0;

	[SerializeField]
	private RectTransform bubble1;

	[SerializeField]
	private RectTransform fishRight0;

	[SerializeField]
	private RectTransform fishRight1;

	[SerializeField]
	private RectTransform fishLeft;

	private RectTransform rectTrans;

	private bool isCurrentPassive;

	[SerializeField]
	private Image color;
}
