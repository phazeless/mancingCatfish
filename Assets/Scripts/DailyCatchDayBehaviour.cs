using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyCatchDayBehaviour : MonoBehaviour
{
	public void SetDay(int day, int currentStreak, bool showlabels = true)
	{
		DailyGiftContentPossibilities dailyGiftContentPossibilitiesForStreak = DailyGiftManager.Instance.GetDailyGiftContentPossibilitiesForStreak(day);
		if (!showlabels)
		{
			this.dayCountLabel.gameObject.SetActive(false);
			this.dayLabel.gameObject.SetActive(false);
		}
		if (currentStreak >= day)
		{
			this.bgImage.color = this.grey;
		}
		else
		{
			this.bgImage.color = dailyGiftContentPossibilitiesForStreak.Visuals.color;
		}
		this.dayCountLabel.SetText(day.ToString());
	}

	public void Opened()
	{
		base.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 10, 1f);
		this.bgImage.DOColor(this.grey, 0.5f);
	}

	private void OnDestroy()
	{
		base.transform.DOKill(false);
		this.bgImage.DOKill(false);
	}

	[SerializeField]
	private TextMeshProUGUI dayCountLabel;

	[SerializeField]
	private TextMeshProUGUI dayLabel;

	[SerializeField]
	private Image bgImage;

	private Color grey = new Color(0.9f, 0.9f, 0.9f);
}
