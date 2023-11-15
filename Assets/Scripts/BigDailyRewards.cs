using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BigDailyRewards : MonoBehaviour
{
	private void Awake()
	{
		this.originalColor = this.bgImage.color;
	}

	public void SetReward(int currentDay, int rewardDay)
	{
		this.dayLabel.SetText(rewardDay.ToString());
		DailyGiftContentPossibilities dailyGiftContentPossibilitiesForStreak = DailyGiftManager.Instance.GetDailyGiftContentPossibilitiesForStreak(rewardDay);
		this.dayImage.color = dailyGiftContentPossibilitiesForStreak.Visuals.color;
		if (currentDay >= rewardDay)
		{
			if (this.bobberHolder.activeInHierarchy)
			{
				this.bobberHolder.SetActive(false);
			}
			if (!this.checkMark.activeInHierarchy)
			{
				this.checkMark.SetActive(true);
			}
			Graphic graphic = this.bgImage;
			Color color = dailyGiftContentPossibilitiesForStreak.Visuals.color;
			this.dayImage.color = color;
			graphic.color = color;
		}
		else
		{
			if (!this.bobberHolder.activeInHierarchy)
			{
				this.bobberHolder.SetActive(true);
			}
			if (this.checkMark.activeInHierarchy)
			{
				this.checkMark.SetActive(false);
			}
			this.bgImage.color = this.originalColor;
		}
		foreach (Image image in this.bobber)
		{
			image.sprite = dailyGiftContentPossibilitiesForStreak.Visuals.bobblerIcon;
		}
		this.bobber[0].GetComponent<RectTransform>().DORotate(Vector3.zero, UnityEngine.Random.Range(1.3f, 1.7f), RotateMode.Fast).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
	}

	private void OnDestroy()
	{
		foreach (Image image in this.bobber)
		{
			image.GetComponent<RectTransform>().DOKill(false);
		}
	}

	[SerializeField]
	private Image[] bobber;

	[SerializeField]
	private Image bgImage;

	[SerializeField]
	private Image dayImage;

	[SerializeField]
	private GameObject bobberHolder;

	[SerializeField]
	private GameObject checkMark;

	[SerializeField]
	private TextMeshProUGUI dayLabel;

	[SerializeField]
	private Color claimedColor;

	private Color originalColor = Color.white;
}
