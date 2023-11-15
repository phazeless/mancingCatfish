using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoreFireworkDialogTween : MonoBehaviour
{
	private void OnEnable()
	{
		this.UpdateUI();
		this.Open();
	}

	public void UpdateUI()
	{
		FireworkFishingManager instance = FireworkFishingManager.Instance;
		this.adButton.gameObject.SetActive(instance.RocketsFromAdsLeft > 0);
		this.adButton.AdButton.interactable = instance.CanWatchAdForRockets;
		this.receiveAmountFromAdLbl.SetVariableText(new string[]
		{
			1.ToString()
		});
		this.rocketsFromAdsLbl.SetVariableText(new string[]
		{
			instance.RocketsFromAdsLeft.ToString(),
			2.ToString()
		});
	}

	public void WatchAd()
	{
		base.gameObject.SetActive(false);
		FireworkFishingManager.Instance.WatchAdForRockets();
	}

	public void GoToShop()
	{
		base.gameObject.SetActive(false);
		ScreenManager.Instance.GoToScreen(2);
	}

	private void Open()
	{
		if (this.isOpen)
		{
			return;
		}
		this.isOpen = true;
		base.transform.localScale = Vector3.zero;
		base.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
		foreach (RectTransform rectTransform in this.leftRockets)
		{
			rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x - 50f, 0.5f, false).SetDelay(0.2f);
			rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 50f, 0.5f, false).SetDelay(0.2f);
		}
		foreach (RectTransform rectTransform2 in this.rightRockets)
		{
			rectTransform2.DOAnchorPosX(rectTransform2.anchoredPosition.x + 50f, 0.5f, false).SetDelay(0.2f);
			rectTransform2.DOAnchorPosY(rectTransform2.anchoredPosition.y + 50f, 0.5f, false).SetDelay(0.2f);
		}
	}

	public void Close()
	{
		if (!this.isOpen)
		{
			return;
		}
		this.isOpen = false;
		this.TweenKiller(true);
		base.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(delegate
		{
			base.gameObject.SetActive(false);
		});
	}

	private void OnDisable()
	{
		foreach (RectTransform rectTransform in this.leftRockets)
		{
			rectTransform.anchoredPosition += new Vector2(50f, -50f);
		}
		foreach (RectTransform rectTransform2 in this.rightRockets)
		{
			rectTransform2.anchoredPosition += new Vector2(-50f, -50f);
		}
	}

	private void OnDestroy()
	{
		this.TweenKiller(false);
	}

	private void TweenKiller(bool complete = false)
	{
		base.transform.DOKill(complete);
		foreach (RectTransform target in this.leftRockets)
		{
			target.DOKill(complete);
		}
		foreach (RectTransform target2 in this.rightRockets)
		{
			target2.DOKill(complete);
		}
	}

	[SerializeField]
	private RectTransform[] leftRockets;

	[SerializeField]
	private RectTransform[] rightRockets;

	[SerializeField]
	private AdBonusIncreaseButton adButton;

	[SerializeField]
	private TextMeshProUGUI receiveAmountFromAdLbl;

	[SerializeField]
	private TextMeshProUGUI rocketsFromAdsLbl;

	private bool isOpen;
}
