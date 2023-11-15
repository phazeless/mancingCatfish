using System;
using UnityEngine;

public class IGNDailyGiftDialog : InGameNotificationDialog<IGNDailyGift>
{
	private int CurrentDayStreak
	{
		get
		{
			return DailyGiftManager.Instance.CurrentDayStreak;
		}
	}

	protected override void Start()
	{
		base.Start();
		
	}

	private void OnAdAvailable(string adUnitId)
	{
		if (base.IsOpen)
		{
			this.UpdateAdButton();
		}
	}

	private void UpdateAdButton()
	{
	
	}

	protected override void OnAboutToOpen()
	{
		int[] bigRewardDays = DailyGiftManager.Instance.GetBigRewardsDays().ToArray();
		this.dailyCatchHandler.UpdateBigRewards(this.CurrentDayStreak, bigRewardDays, true);
		this.dailyCatchHandler.UpdateDay(this.CurrentDayStreak);
		this.dailyCatchHandler.StartBobber();
		this.relevantDateForThisDialog = DailyGiftManager.Instance.Now.Date;
		if (!DailyGiftManager.Instance.IsWithinStreak)
		{
			this.dailyCatchHandler.BreakSayStreak();
		}
		this.UpdateAdButton();
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
	}

	protected override void OnOpened()
	{
	}

	protected override void OnRemovedFromList()
	{
	}

	protected override void OnReturned()
	{
	}

	public void RestartStreak()
	{
		DailyGiftManager.Instance.RestartStreak();
		this.inGameNotification.OverrideClearable = true;
		this.Close(true);
		this.RunAfterDelay(0f, delegate()
		{
			InGameNotificationManager.Instance.OpenFirstOccurrenceOfIGN<IGNDailyGift>();
		});
	}

	public void ContinueStreak()
	{
		
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.B))
		{
			this.RestartStreak();
		}
		if (base.IsOpen)
		{
			if (DailyGiftManager.Instance.Now.Date > this.relevantDateForThisDialog)
			{
				this.inGameNotification.OverrideClearable = true;
				this.Close(true);
			}
			if (!DailyGiftManager.Instance.IsGiftAvailable)
			{
				this.dailyCatchHandler.UpdateTapToPullLabel();
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		
	}

	[SerializeField]
	private DailyCatchHandler dailyCatchHandler;

	[SerializeField]
	private AdBonusIncreaseButton watchAdToMultiply;

	private DateTime relevantDateForThisDialog = DateTime.Now.Date;
}
