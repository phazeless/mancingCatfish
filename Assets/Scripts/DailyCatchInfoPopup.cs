using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyCatchInfoPopup : MonoBehaviour
{
	public void Show(DailyCatchHandler dailyCatchHandler, int bobblerStreakClicked, int buttonIndex)
	{
		base.gameObject.SetActive(true);
		base.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)(225 * buttonIndex - 225), base.GetComponent<RectTransform>().anchoredPosition.y);
		this.TweenKiller();
		base.transform.localScale = new Vector3(0f, 0.5f);
		base.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
		this.dailyCatchHandler = dailyCatchHandler;
		if (this.previousBobblerStreakClicked != bobblerStreakClicked)
		{
			for (int i = 0; i < this.dailyRewardItemHolder.childCount; i++)
			{
				UnityEngine.Object.Destroy(this.dailyRewardItemHolder.GetChild(i).gameObject);
			}
			this.GenerateReward(bobblerStreakClicked);
		}
		this.previousBobblerStreakClicked = bobblerStreakClicked;
	}

	public void Hide()
	{
		this.TweenKiller();
		base.transform.DOScale(new Vector3(0f, 0.5f), 0.2f).SetEase(Ease.InBack).OnComplete(delegate
		{
			base.gameObject.SetActive(false);
		});
	}

	private void GenerateReward(int bobblerStreakClicked)
	{
		DailyGiftContentPossibilities dailyGiftContentPossibilitiesForStreak = DailyGiftManager.Instance.GetDailyGiftContentPossibilitiesForStreak(bobblerStreakClicked);
		this.titleLabel.SetText(dailyGiftContentPossibilitiesForStreak.Visuals.title);
		float num = 0f;
		float num2 = 0.25f;
		if (dailyGiftContentPossibilitiesForStreak.Chest != null)
		{
			DailyRewardItem dailyRewardItem = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.dailyRewardItemHolder);
			dailyRewardItem.SetValues(dailyGiftContentPossibilitiesForStreak.Chest.Icon, this.dailyCatchHandler.ItemBoxBgColor, "x1", true, 0.8f);
			num += num2;
		}
		if (dailyGiftContentPossibilitiesForStreak.MaxGems > 0)
		{
			DailyRewardItem dailyRewardItem2 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.dailyRewardItemHolder);
			string str = dailyGiftContentPossibilitiesForStreak.MinGems + "-" + dailyGiftContentPossibilitiesForStreak.MaxGems;
			dailyRewardItem2.SetValues(this.dailyCatchHandler.GemRewardIcon, this.dailyCatchHandler.GemRewardBgColor, str + "<sprite=0>", true, 0.8f);
			num += num2;
		}
		if (dailyGiftContentPossibilitiesForStreak.ChanceForFreeSpin > 0)
		{
			DailyRewardItem dailyRewardItem3 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.dailyRewardItemHolder);
			dailyRewardItem3.SetValues(this.dailyCatchHandler.FreeSpinIcon, this.dailyCatchHandler.FreeSpinColor, "x1", true, 1f);
			num += num2;
		}
		if (dailyGiftContentPossibilitiesForStreak.Fish != null)
		{
			DailyRewardItem dailyRewardItem4 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.dailyRewardItemHolder);
			dailyRewardItem4.SetValues(dailyGiftContentPossibilitiesForStreak.Fish.FishInfo.FishIcon, this.dailyCatchHandler.FishColor, "x1", true, 1f);
			num += num2;
		}
		if (dailyGiftContentPossibilitiesForStreak.Item != null)
		{
			DailyRewardItem dailyRewardItem5 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.dailyRewardItemHolder);
			dailyRewardItem5.SetValues(dailyGiftContentPossibilitiesForStreak.Item.Icon, dailyGiftContentPossibilitiesForStreak.Item.IconBgColor, "x1", true, 1f);
			num += num2;
		}
		if (dailyGiftContentPossibilitiesForStreak.MaxItems > 0)
		{
			Color itemColor = this.dailyCatchHandler.GetItemColor(0);
			DailyRewardItem dailyRewardItem6 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.dailyRewardItemHolder);
			string count = dailyGiftContentPossibilitiesForStreak.MinItems * dailyGiftContentPossibilitiesForStreak.DiffItems + "-" + dailyGiftContentPossibilitiesForStreak.MaxItems * dailyGiftContentPossibilitiesForStreak.DiffItems;
			dailyRewardItem6.SetValues(this.dailyCatchHandler.GeneralItemIcon, itemColor, count, true, 1f);
			num += num2;
		}
		if (dailyGiftContentPossibilitiesForStreak.CrownExp > 0 && SkillTreeManager.Instance.IsSkillTreeEnabled)
		{
			DailyRewardItem dailyRewardItem7 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.dailyRewardItemHolder);
			string count2 = dailyGiftContentPossibilitiesForStreak.CrownExp.ToString();
			dailyRewardItem7.SetValues(this.dailyCatchHandler.CrownExpIcon, this.dailyCatchHandler.CrownExpColor, count2, false, 0.8f);
			num += num2;
		}
		if (dailyGiftContentPossibilitiesForStreak.Grantable != null)
		{
			DailyRewardItem dailyRewardItem8 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.dailyRewardItemHolder);
			dailyRewardItem8.SetValues(dailyGiftContentPossibilitiesForStreak.Grantable.Icon, dailyGiftContentPossibilitiesForStreak.Grantable.IconBg, dailyGiftContentPossibilitiesForStreak.Grantable.Amount.ToString(), true, 1f);
			num += num2;
		}
		if (dailyGiftContentPossibilitiesForStreak.FishingExpProcentOfCurrent > 0)
		{
			DailyRewardItem dailyRewardItem9 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.dailyRewardItemHolder);
			dailyRewardItem9.SetValues(this.dailyCatchHandler.FishExpIcon, this.dailyCatchHandler.FishExpColor, "+" + dailyGiftContentPossibilitiesForStreak.FishingExpProcentOfCurrent + "%", true, 0.8f);
		}
	}

	private void TweenKiller()
	{
		base.transform.DOKill(false);
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	[SerializeField]
	private Image bobblerIcon;

	[SerializeField]
	private TextMeshProUGUI titleLabel;

	[SerializeField]
	private DailyRewardItem dailyRewardItemPrefab;

	[SerializeField]
	private Transform dailyRewardItemHolder;

	private DailyCatchHandler dailyCatchHandler;

	private int previousBobblerStreakClicked = -1;
}
