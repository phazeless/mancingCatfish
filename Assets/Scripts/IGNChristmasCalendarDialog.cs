using System;
using UnityEngine;
using UnityEngine.UI;

public class IGNChristmasCalendarDialog : InGameNotificationDialog<IGNChristmasGift>
{
	protected override void Start()
	{
		base.Start();
		this.dcm = DailyChristmasManager.Instance;
		this.UpdateUI();
		this.isSpecialDay = (this.dcm.GetDailyGiftContentPossibilitiesForStreak(this.dcm.Now.Day).Visuals.Stars > 0);
	}

	protected override void OnAboutToOpen()
	{
		this.relevantDateForThisDialog = this.dcm.Now.Date;
		AudioManager.Instance.OneShooter(this.christmasAudio, 1f);
		this.UpdateUI();
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
	}

	protected override void OnOpened()
	{
		this.dayHolder.GetComponent<GridLayoutGroup>().enabled = false;
	}

	protected override void OnRemovedFromList()
	{
	}

	protected override void OnReturned()
	{
	}

	public void Claim(ChristmasGiftDayBehaviour christmasGiftDayBehaviour = null)
	{
		if (this.isSpecialDay)
		{
			if (this.hasGift && this.isOpened && !this.isAnimating && !this.hasSpecial)
			{
				this.isAnimating = true;
				this.RunAfterDelay(3f, delegate()
				{
					this.isAnimating = false;
				});
				this.christmasPresentInstance.GetComponentInChildren<ChristmasPresentOpening>().SpecialOffer();
				this.hasSpecial = true;
			}
			else if (this.hasGift && this.isOpened && !this.isAnimating && this.hasSpecial && !this.isClosing)
			{
				this.isClosing = true;
				this.RunAfterDelay(0.5f, delegate()
				{
					this.Close(false);
				});
				this.christmasPresentInstance.GetComponentInChildren<ChristmasPresentOpening>().Close();
				if (christmasGiftDayBehaviour != null)
				{
					christmasGiftDayBehaviour.FadeOut();
				}
			}
		}
		else if (this.isOpened && !this.isAnimating && !this.isClosing)
		{
			this.isClosing = true;
			this.RunAfterDelay(0.5f, delegate()
			{
				this.Close(false);
			});
			this.christmasPresentInstance.GetComponentInChildren<ChristmasPresentOpening>().Close();
			if (christmasGiftDayBehaviour != null)
			{
				christmasGiftDayBehaviour.FadeOut();
			}
		}
		if (this.hasGift && !this.isOpened && !this.isAnimating)
		{
			this.christmasPresentInstance.GetComponentInChildren<ChristmasPresentOpening>().Open();
			this.isOpened = true;
			this.isAnimating = true;
			this.RunAfterDelay(1.5f, delegate()
			{
				this.isAnimating = false;
			});
		}
		if (!this.hasGift && !this.isOpened && !this.isAnimating)
		{
			this.christmasPresentInstance = UnityEngine.Object.Instantiate<GameObject>(this.christmasPresentPrefab, base.transform);
			this.hasGift = true;
			this.isAnimating = true;
			AudioManager.Instance.OneShooter(this.selectClip1, 1f);
			AudioManager.Instance.OneShooter(this.selectClip2, 1f);
			this.RunAfterDelay(1f, delegate()
			{
				this.isAnimating = false;
			});
		}
	}

	public void UpdateUI()
	{
		if (this.inGameNotification == null)
		{
			return;
		}
		ChristmasGiftDayBehaviour[] componentsInChildren = this.dayHolder.GetComponentsInChildren<ChristmasGiftDayBehaviour>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			ChristmasGiftDayBehaviour christmasGiftDayBehaviour = componentsInChildren[i];
			int num = i + 1;
			DailyGiftContentPossibilities dailyGiftContentPossibilitiesForStreak = this.dcm.GetDailyGiftContentPossibilitiesForStreak(num);
			christmasGiftDayBehaviour.SetDay(num, this.dcm.Now.Day, dailyGiftContentPossibilitiesForStreak.Visuals.Stars);
		}
	}

	private void Update()
	{
		if (base.IsOpen && this.dcm.Now.Date > this.relevantDateForThisDialog)
		{
			this.inGameNotification.OverrideClearable = true;
			this.Close(true);
		}
	}

	[SerializeField]
	private Transform dayHolder;

	[SerializeField]
	private GameObject christmasPresentPrefab;

	[SerializeField]
	private AudioClip christmasAudio;

	[SerializeField]
	private AudioClip selectClip1;

	[SerializeField]
	private AudioClip selectClip2;

	private bool hasGift;

	private bool isAnimating;

	private bool isOpened;

	private DailyChristmasManager dcm;

	private DateTime relevantDateForThisDialog = DateTime.Now.Date;

	private Image buttonBg;

	private GameObject christmasPresentInstance;

	private bool isSpecialDay = true;

	private bool hasSpecial;

	private bool isClosing;
}
