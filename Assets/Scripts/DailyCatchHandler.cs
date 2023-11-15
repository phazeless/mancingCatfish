using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyCatchHandler : MonoBehaviour
{
	public Color GemRewardBgColor
	{
		get
		{
			return this.rewardVisualAttributes.gemRewardBgColor;
		}
	}

	public Color ItemBoxBgColor
	{
		get
		{
			return this.rewardVisualAttributes.itemBoxBgColor;
		}
	}

	public Color FishExpColor
	{
		get
		{
			return this.rewardVisualAttributes.fishExpColor;
		}
	}

	public Color FishColor
	{
		get
		{
			return this.rewardVisualAttributes.fishColor;
		}
	}

	public Color CrownExpColor
	{
		get
		{
			return this.rewardVisualAttributes.crownExpColor;
		}
	}

	public Color FreeSpinColor
	{
		get
		{
			return this.rewardVisualAttributes.freeSpinColor;
		}
	}

	public Sprite GeneralItemIcon
	{
		get
		{
			return this.rewardVisualAttributes.generalItemIcon;
		}
	}

	public Sprite GemRewardIcon
	{
		get
		{
			return this.rewardVisualAttributes.gemRewardIcon;
		}
	}

	public Sprite FishExpIcon
	{
		get
		{
			return this.rewardVisualAttributes.fishExpIcon;
		}
	}

	public Sprite CrownExpIcon
	{
		get
		{
			return this.rewardVisualAttributes.crownExpIcon;
		}
	}

	public Sprite FreeSpinIcon
	{
		get
		{
			return this.rewardVisualAttributes.freeSpinIcon;
		}
	}

	public Color GetItemColor(int rarity)
	{
		return this.rewardVisualAttributes.GetItemColor(rarity);
	}

	private int CurrentDayStreak
	{
		get
		{
			return DailyGiftManager.Instance.CurrentDayStreak;
		}
	}

	private void Start()
	{
		this.dfManager = DailyGiftManager.Instance;
	}

	public void UpdateBigRewards(int currentStreak, int[] bigRewardDays, bool popTween = true)
	{
		for (int i = 0; i < this.bigRewards.Length; i++)
		{
			this.bigRewards[i].SetReward(currentStreak, bigRewardDays[i]);
			if (popTween)
			{
				this.bigRewards[i].transform.localScale = Vector3.zero;
				this.bigRewards[i].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f * (float)i + 0.2f);
			}
		}
	}

	public void StartBobber()
	{
		if (!this.isBobberStarted)
		{
			this.bobberTween.CatchBobbing();
		}
		this.isBobberStarted = true;
	}

	public void UpdateDay(int currentStreak)
	{
		int num = currentStreak + 1;
		this.bobberTween.SetBobber(num);
		this.oldOldDay.GetComponent<DailyCatchDayBehaviour>().SetDay(num - 3, currentStreak, currentStreak > 2);
		this.oldDay.GetComponent<DailyCatchDayBehaviour>().SetDay(num - 2, currentStreak, currentStreak > 1);
		this.prevDay.GetComponent<DailyCatchDayBehaviour>().SetDay(num - 1, currentStreak, currentStreak > 0);
		this.currentDay.GetComponent<DailyCatchDayBehaviour>().SetDay(num, currentStreak, true);
		this.nextDay.GetComponent<DailyCatchDayBehaviour>().SetDay(num + 1, currentStreak, true);
		this.nextNextDay.GetComponent<DailyCatchDayBehaviour>().SetDay(num + 2, currentStreak, true);
		this.RunAfterDelay(0.4f, delegate()
		{
			this.PushDays();
		});
	}

	public void UpdateTapToPullLabel()
	{
		if (this.dfManager.IsGiftAvailable)
		{
			this.tapToPullLabel.SetText("TAP TO PULL");
		}
		else if (!this.isAnimating)
		{
			this.isAnimating = true;
			Color newColor = this.tapToPullLabel.color;
			newColor.a = 0.8f;
			this.tapToPullLabel.color = newColor;
			this.tapToPullLabel.SetText("Come back tomorrow for more!");
			this.tapToPullLabel.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1f).OnComplete(delegate
			{
				this.tapToPullLabel.DOFade(0f, 0.3f).SetDelay(2f).OnComplete(delegate
				{
					this.tapToPullLabel.color = newColor;
					this.tapToPullLabel.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1f);
					this.tapToPullLabel.SetText("Next in:" + FHelper.FromSecondsToHoursMinutesSecondsFormat(this.dfManager.GetSecondsUntilDailyGiftAvailable(true)));
					this.isDoneAnimating = true;
				});
			});
		}
		else if (this.isDoneAnimating)
		{
			this.tapToPullLabel.SetText("Next in:" + FHelper.FromSecondsToHoursMinutesSecondsFormat(this.dfManager.GetSecondsUntilDailyGiftAvailable(true)));
		}
	}

	public void Tapped()
	{
		this.bobberTween.Pull();
		this.currentDay.GetComponent<DailyCatchDayBehaviour>().Opened();
		int[] array = this.dfManager.GetBigRewardsDays().ToArray();
		DailyGiftContent reward = this.dfManager.OpenAndCollect();
		this.GenerateReward(reward);
		if (array[array.Length - 1] == this.CurrentDayStreak)
		{
			this.bigRewards[this.bigRewards.Length - 1].SetReward(this.CurrentDayStreak, array[array.Length - 1]);
			this.bigRewards[this.bigRewards.Length - 1].transform.localScale = Vector3.zero;
			this.bigRewards[this.bigRewards.Length - 1].transform.DOScale(1f, 0.4f);
			this.RunAfterDelay(1f, delegate()
			{
				if (this != null)
				{
					this.UpdateBigRewards(this.CurrentDayStreak, this.dfManager.GetBigRewardsDays().ToArray(), true);
				}
			});
		}
		else
		{
			this.UpdateBigRewards(this.CurrentDayStreak, array, false);
		}
		this.tapToPullLabel.DOKill(false);
		this.UpdateTapToPullLabel();
	}

	public void BreakSayStreak()
	{
		if (!this.isBroken)
		{
			this.RewardButtonHolder.gameObject.SetActive(false);
			this.BrokenRewardButtonHolder.gameObject.SetActive(true);
			this.BrokenRewardButtonHolder.localScale = new Vector3(0f, 0.5f, 0f);
			this.BrokenRewardButtonHolder.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
			this.RunAfterDelay(0.6f, delegate()
			{
				AudioManager.Instance.OneShooter(this.brokenStreak, 1f);
				this.currentDay.GetComponent<Image>().DOColor(this.streakBreakColor, 0.3f);
				this.currentDay.DOShakeAnchorPos(0.4f, 10f, 50, 90f, false, true).OnComplete(delegate
				{
					this.currentDay.gameObject.SetActive(false);
					this.streakBreakTween.BreakStreakEffect(this.CurrentDayStreak);
				});
				this.isBroken = true;
			});
		}
	}

	private void GenerateReward(DailyGiftContent reward)
	{
		float num = 0f;
		float num2 = 0.25f;
		float num3 = 0.3f;
		if (reward.Chest != null)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem2 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.RewardHolder);
				dailyRewardItem2.SetValues(reward.Chest.Icon, this.rewardVisualAttributes.itemBoxBgColor, "x1", false, 0.8f);
			});
			num += num2;
		}
		if (reward.Gems > 0)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem2 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.RewardHolder);
				dailyRewardItem2.SetValues(this.rewardVisualAttributes.gemRewardIcon, this.rewardVisualAttributes.gemRewardBgColor, reward.Gems + "<sprite=0>", false, 0.8f);
			});
			num += num2;
		}
		if (reward.FreeSpins > 0)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem2 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.RewardHolder);
				dailyRewardItem2.SetValues(this.rewardVisualAttributes.freeSpinIcon, this.rewardVisualAttributes.freeSpinColor, "x1", false, 1f);
			});
			num += num2;
		}
		if (reward.Fish != null)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem2 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.RewardHolder);
				dailyRewardItem2.SetValues(reward.Fish.FishInfo.FishIcon, this.rewardVisualAttributes.fishColor, "x1", false, 1f);
			});
			num += num2;
		}
		if (reward.Item != null)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem2 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.RewardHolder);
				dailyRewardItem2.SetValues(reward.Item.Icon, reward.Item.IconBgColor, "x1", false, 1f);
			});
			num += num2;
		}
		if (reward.Items.Count > 0)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				foreach (KeyValuePair<Item, int> keyValuePair in reward.Items)
				{
					Color color = Color.gray;
					if (keyValuePair.Key.Rarity == Rarity.Common)
					{
						color = this.GetItemColor(0);
					}
					else if (keyValuePair.Key.Rarity == Rarity.Rare)
					{
						color = this.GetItemColor(1);
					}
					else
					{
						color = this.GetItemColor(2);
					}
					DailyRewardItem dailyRewardItem2 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.RewardHolder);
					dailyRewardItem2.SetValues(keyValuePair.Key.Icon, color, "x" + keyValuePair.Value, false, 1f);
				}
			});
			num += num2;
		}
		if (reward.CrownExp > 0)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem2 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.RewardHolder);
				string count = reward.CrownExp.ToString();
				dailyRewardItem2.SetValues(this.rewardVisualAttributes.crownExpIcon, this.rewardVisualAttributes.crownExpColor, count, false, 0.8f);
			});
			num += num2;
		}
		if (reward.FishingExp > 0)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem2 = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.RewardHolder);
				string count = CashFormatter.SimpleToCashRepresentation(reward.FishingExp, 0, false, false, "N0");
				dailyRewardItem2.SetValues(this.rewardVisualAttributes.fishExpIcon, this.rewardVisualAttributes.fishExpColor, count, false, 0.8f);
			});
		}
		if (reward.Grantable != null)
		{
			DailyRewardItem dailyRewardItem = UnityEngine.Object.Instantiate<DailyRewardItem>(this.dailyRewardItemPrefab, this.RewardHolder);
			dailyRewardItem.SetValues(reward.Grantable.Icon, reward.Grantable.IconBg, "x" + reward.Grantable.Amount, true, 1f);
			num += num2;
		}
	}

	private void PushDays()
	{
		this.oldOldDay.GetComponent<Image>().DOFade(0f, 0.3f);
		this.oldDay.GetComponent<Image>().DOFade(0.5f, 0.3f);
		this.prevDay.GetComponent<Image>().DOFade(0.5f, 0.3f);
		this.currentDay.GetComponent<Image>().DOFade(1f, 0.3f);
		this.nextDay.GetComponent<Image>().DOFade(0.5f, 0.3f);
		this.nextNextDay.GetComponent<Image>().DOFade(0.5f, 0.3f);
		this.oldOldDay.DOScale(0f, 0.3f);
		this.prevDay.DOScale(0.9f, 0.3f);
		this.nextNextDay.DOScale(0.9f, 0.3f);
		this.currentDay.DOScale(1f, 0.3f).OnComplete(delegate
		{
			this.currentDay.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1f);
		});
		this.oldOldDay.DOAnchorPosX(this.oldOldDay.anchoredPosition.x - 100f, 0.3f, false);
		this.oldDay.DOAnchorPosX(this.oldOldDay.anchoredPosition.x, 0.3f, false);
		this.prevDay.DOAnchorPos(this.oldDay.anchoredPosition, 0.3f, false);
		this.currentDay.DOAnchorPos(this.prevDay.anchoredPosition, 0.3f, false);
		this.nextDay.DOAnchorPosX(this.currentDay.anchoredPosition.x, 0.3f, false);
		this.nextNextDay.DOAnchorPosX(this.nextDay.anchoredPosition.x, 0.3f, false);
	}

	private void TweenKiller()
	{
		this.oldOldDay.GetComponent<Image>().DOKill(false);
		this.nextNextDay.GetComponent<Image>().DOKill(false);
		this.oldDay.GetComponent<Image>().DOKill(false);
		this.prevDay.GetComponent<Image>().DOKill(false);
		this.currentDay.GetComponent<Image>().DOKill(false);
		this.nextDay.GetComponent<Image>().DOKill(false);
		this.BrokenRewardButtonHolder.DOKill(false);
		this.tapToPullLabel.transform.DOKill(false);
		this.tapToPullLabel.DOKill(false);
		this.oldDay.DOKill(false);
		this.prevDay.DOKill(false);
		this.currentDay.DOKill(false);
		this.nextDay.DOKill(false);
		for (int i = 0; i < this.bigRewards.Length; i++)
		{
			this.bigRewards[i].transform.DOKill(false);
		}
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.C))
		{
			this.dailyCatchInfoPopup.Show(this, 3, 1);
		}
	}

	public void ShowInfoPopup(int bigRewardIndex)
	{
		List<int> bigRewardsDays = DailyGiftManager.Instance.GetBigRewardsDays();
		this.dailyCatchInfoPopup.Show(this, bigRewardsDays[bigRewardIndex], bigRewardIndex);
	}

	public void HideInfoPopup()
	{
		this.dailyCatchInfoPopup.Hide();
	}

	[SerializeField]
	private DailyCatchInfoPopup dailyCatchInfoPopup;

	[SerializeField]
	private DailyCatchBobberTween bobberTween;

	[SerializeField]
	private RectTransform oldOldDay;

	[SerializeField]
	private RectTransform oldDay;

	[SerializeField]
	private RectTransform prevDay;

	[SerializeField]
	private RectTransform currentDay;

	[SerializeField]
	private RectTransform nextDay;

	[SerializeField]
	private RectTransform nextNextDay;

	[SerializeField]
	private DailyRewardItem dailyRewardItemPrefab;

	[SerializeField]
	private TextMeshProUGUI tapToPullLabel;

	[SerializeField]
	private Transform RewardHolder;

	[SerializeField]
	private StreakBreakTween streakBreakTween;

	[SerializeField]
	private Color streakBreakColor;

	[SerializeField]
	private Transform RewardButtonHolder;

	[SerializeField]
	private Transform BrokenRewardButtonHolder;

	[SerializeField]
	private RewardVisualAttributes rewardVisualAttributes;

	[SerializeField]
	[Header("Big Rewards")]
	private BigDailyRewards[] bigRewards;

	private DailyGiftManager dfManager;

	[SerializeField]
	private AudioClip brokenStreak;

	private bool isBobberStarted;

	private bool isAnimating;

	private bool isDoneAnimating;

	private bool isBroken;
}
