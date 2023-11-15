using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AdBonusIncreaseButton : MonoBehaviour
{
	private void Update()
	{
		if (!this.hasStartedTween && this.adButton.interactable)
		{
			this.StartActivatedTween();
		}
	}

	public Button AdButton
	{
		get
		{
			return this.adButton;
		}
	}

	private void OnEnable()
	{
		this.clipperImage = this.clipperRect.GetComponent<Image>();
		this.SetInactiveTween();
		this.hasStartedTween = false;
	}

	private void SetInactiveTween()
	{
		this.clipperRect.anchoredPosition = Vector2.zero;
		this.clipperRect.localEulerAngles = Vector3.zero;
		this.clipperRect.localScale = Vector3.one;
		this.clipperImage.sprite = this.clipperSprites[0];
		this.glimmer.anchoredPosition = new Vector2(-150f, 0f);
		this.clipperImage.color = new Color(1f, 1f, 1f, 0.3f);
		foreach (GameObject gameObject in this.buttonContent)
		{
			gameObject.transform.localScale = Vector3.zero;
		}
	}

	private void StartActivatedTween()
	{
		this.hasStartedTween = true;
		float height = base.GetComponent<RectTransform>().rect.height;
		float width = base.GetComponent<RectTransform>().rect.width;
		this.clipperImage.sprite = this.clipperSprites[1];
		this.clipperImage.color = new Color(1f, 1f, 1f, 1f);
		this.clipperRect.localEulerAngles = new Vector3(0f, 0f, -4f);
		this.glimmer.DOAnchorPosX(width + 150f, 0.3f, false).SetDelay(0.7f);
		this.clipperRect.DOAnchorPosY(height / 2f + 2f, 0.5f, false).SetEase(Ease.OutBack);
		this.clipperRect.DOScale(1.2f, 0.25f).SetLoops(2, LoopType.Yoyo);
		this.clipperRect.DORotate(new Vector3(0f, 0f, 7f), 0.25f, RotateMode.Fast).SetEase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo).OnComplete(delegate
		{
			this.clipperImage.sprite = this.clipperSprites[2];
			this.clipperRect.localEulerAngles = Vector3.zero;
			this.clipperRect.DOPunchScale(Vector2.one * 0.2f, 0.4f, 10, 1f);
		});
		foreach (GameObject gameObject in this.buttonContent)
		{
			gameObject.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
		}
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	private void TweenKiller()
	{
		foreach (GameObject gameObject in this.buttonContent)
		{
			gameObject.transform.DOKill(false);
		}
		this.clipperRect.DOKill(false);
		this.glimmer.DOKill(false);
	}

	[SerializeField]
	private Button adButton;

	[SerializeField]
	private RectTransform glimmer;

	[SerializeField]
	private GameObject[] buttonContent;

	[SerializeField]
	private Sprite[] clipperSprites;

	[SerializeField]
	private RectTransform clipperRect;

	private Image clipperImage;

	private bool hasStartedTween;
}
