using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IGNQuestDialog : InGameNotificationDialog<IGNQuest>
{
	public bool HasGrantedAdBonus
	{
		get
		{
			return this.quest.BonusGems > 0;
		}
	}

	protected override void OnAboutToOpen()
	{
		this.quest = this.inGameNotification.Quest;
		this.UnregisterListeners();
		this.RegisterListeners();
		this.UpdateUI();
		this.UpdateUIAdButton(true);
	}

	private void UpdateUIAdButton(bool hideButtonIfBonusGranted)
	{
		if (this.quest == null || this.adButtonObject == null || this.adButtonObject.AdButton == null)
		{
			if (this.quest == null)
			{
				UnityEngine.Debug.LogError("IGNQuestDialog error: quest is null: ");
			}
			if (this.adButtonObject == null)
			{
				UnityEngine.Debug.LogError("IGNQuestDialog error: adButtonObject is null: ");
			}
			if (this.adButtonObject != null && this.adButtonObject.AdButton == null)
			{
				UnityEngine.Debug.LogError("IGNQuestDialog error: adButtonObject.AdButton is null: ");
			}
			return;
		}
		if (this.quest.IsCompleted)
		{
			if (hideButtonIfBonusGranted && this.HasGrantedAdBonus)
			{
				this.adButtonObject.gameObject.SetActive(false);
			}
			else
			{
				this.adButtonObject.gameObject.SetActive(true);
				this.watchAdGemLabel.SetVariableText(new string[]
				{
					3.ToString()
				});
				this.adButtonObject.AdButton.interactable = (this.quest.IsCompleted && !this.HasGrantedAdBonus);
			}
		}
		else
		{
			this.adButtonObject.gameObject.SetActive(false);
		}
	}

	public void IncreaseByWatchingAd()
	{
		
	}

	public void Claim()
	{
		if (this.inGameNotification != null)
		{
			this.inGameNotification.OverrideClearable = true;
		}
		if (this.quest != null)
		{
			this.quest.Claim();
			this.quest.SetBonusGems(0);
		}
		this.Close(true);
	}

	private void Quest_OnQuestCompleted(Quest obj)
	{
		this.UpdateUI();
		this.UpdateProgressLabel();
		this.UnregisterListeners();
	}

	private void Quest_OnQuestProgress(Quest obj)
	{
		this.UpdateProgressLabel();
		this.UpdateUI();
	}

	protected override void OnAboutToReturn()
	{
		this.UpdateProgressLabel();
	}

	protected override void OnIGNHasBeenSet()
	{
		this.progressIcon.SetText("#" + (QuestManager.Instance.QuestCount + 1));
	}

	protected override void OnOpened()
	{
	}

	protected override void OnRemovedFromList()
	{
		this.iconTween.IconTweenKiller();
		this.UnregisterListeners();
	}

	protected override void OnReturned()
	{
	}

	private void UpdateUI()
	{
		if (this.quest == null)
		{
			return;
		}
		this.claimButton.interactable = this.quest.IsCompleted;
		this.progressMeter.SetMax((float)this.quest.Goal);
		this.progressMeter.SetCurrent((float)this.quest.Progress);
		this.title.SetText(this.quest.Title);
		this.description.SetText(this.quest.QuestDescription);
		this.reward.SetVariableText(new string[]
		{
			this.quest.GemReward.ToString()
		});
	}

	private void UpdateProgressLabel()
	{
		if (this.quest == null)
		{
			return;
		}
		if (!this.quest.IsCompleted)
		{
			this.progressIcon.SetText(((int)((float)this.quest.Progress / (float)this.quest.Goal * 100f)).ToString() + "%");
			this.iconTween.SetOpened();
		}
		else
		{
			this.progressIcon.color = Color.white;
			this.progressIcon.SetText("100%");
			this.iconTween.SetCompleted();
		}
	}

	private void RegisterListeners()
	{
		if (this.quest != null)
		{
			this.quest.OnQuestProgress += this.Quest_OnQuestProgress;
			this.quest.OnQuestCompleted += this.Quest_OnQuestCompleted;
		}
	}

	private void UnregisterListeners()
	{
		if (this.quest != null)
		{
			this.quest.OnQuestProgress -= this.Quest_OnQuestProgress;
			this.quest.OnQuestCompleted -= this.Quest_OnQuestCompleted;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.UnregisterListeners();
	}

	private const int WATCH_AD_BONUS_GEMS = 3;

	private Quest quest;

	[SerializeField]
	private UIMeter progressMeter;

	[SerializeField]
	private Button claimButton;

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private TextMeshProUGUI description;

	[SerializeField]
	private TextMeshProUGUI reward;

	[SerializeField]
	private TextMeshProUGUI progressIcon;

	[SerializeField]
	private IGNQuestIconTween iconTween;

	[SerializeField]
	private ParticleSystem addDubbleEffects;

	[SerializeField]
	private TextMeshProUGUI watchAdGemLabel;

	[SerializeField]
	private AdBonusIncreaseButton adButtonObject;
}
