using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IGNEventChallengesDialog : InGameNotificationDialog<IGNChallengeEvent>
{
	public EventContent EventContent
	{
		get
		{
			return this.eventContent;
		}
	}

	public UIInGameNotificationItem ParentHolder
	{
		get
		{
			return this.parentHolder;
		}
	}

	public InGameNotification GetIGNInstance()
	{
		return new IGNChallengeEvent();
	}

	public void SetTab(int tabIndex)
	{
		this.currentTab = tabIndex;
		if (tabIndex == 0)
		{
			this.tabInfo.color = Color.white;
			this.tabChallenges.color = new Color(1f, 1f, 1f, 0.7f);
			this.tabViewInfo.SetActive(true);
			this.tabViewChallenges.SetActive(false);
		}
		else if (tabIndex == 1)
		{
			this.tabChallenges.color = Color.white;
			this.tabInfo.color = new Color(1f, 1f, 1f, 0.7f);
			this.tabViewChallenges.SetActive(true);
			this.tabViewInfo.SetActive(false);
			this.challengeTabBadge.SetActive(false);
		}
	}

	public void OpenBigReward()
	{
		int costToUnlockRemaining = this.CostToUnlockRemaining;
		ResourceChangeData changeData = new ResourceChangeData(this.eventContent.Name, ResourceChangeReason.UnlockEventTreasure.ToString(), costToUnlockRemaining, ResourceType.Gems, ResourceChangeType.Spend, ResourceChangeReason.UnlockEventTreasure);
		if (!this.eventContent.HasUnlockedBigReward)
		{
			if (ResourceManager.Instance.TakeGems(costToUnlockRemaining, changeData))
			{
				this.eventContent.Grant(costToUnlockRemaining);
				this.UpdateUI(true);
			}
			else
			{
				NotEnoughGemsDialog dialog = DialogInteractionHandler.Instance.GetDialog<NotEnoughGemsDialog>();
				int missingGems = (int)(costToUnlockRemaining - ResourceManager.Instance.GetResourceAmount(ResourceType.Gems));
				dialog.OpenWithMissingGems(missingGems);
			}
		}
		else if (this.eventContent.HasUnlockedBigReward)
		{
			this.Close(false);
		}
	}

	private List<RewardBox> CreateRewardBoxes(List<BaseGrantable> contents)
	{
		List<RewardBox> list = new List<RewardBox>();
		for (int i = 0; i < contents.Count; i++)
		{
			BaseGrantable baseGrantable = contents[i];
			if (baseGrantable is GrantableCrew)
			{
				GrantableCrew grantableCrew = (GrantableCrew)baseGrantable;
				RewardBox rewardBox = UnityEngine.Object.Instantiate<RewardBox>(this.rewardBoxPrefab, this.rewardBoxHolder);
				rewardBox.SetContent(grantableCrew.Crew.GetExtraInfo().Icon, Color.white, 1, 1f);
				list.Add(rewardBox);
			}
			if (baseGrantable is GrantableItem)
			{
				GrantableItem grantableItem = (GrantableItem)baseGrantable;
				RewardBox rewardBox2 = UnityEngine.Object.Instantiate<RewardBox>(this.rewardBoxPrefab, this.rewardBoxHolder);
				rewardBox2.SetContent(grantableItem.Item.Icon, HookedColors.ItemEpic, grantableItem.Amount, 1f);
				list.Add(rewardBox2);
			}
			if (baseGrantable is GrantableGems)
			{
				GrantableGems grantableGems = (GrantableGems)baseGrantable;
				RewardBox rewardBox3 = UnityEngine.Object.Instantiate<RewardBox>(this.rewardBoxPrefab, this.rewardBoxHolder);
				rewardBox3.SetContentAsGems(grantableGems.Amount, 1f);
				list.Add(rewardBox3);
			}
			if (baseGrantable is GrantableConsumable)
			{
				GrantableConsumable grantableConsumable = (GrantableConsumable)baseGrantable;
				RewardBox rewardBox4 = UnityEngine.Object.Instantiate<RewardBox>(this.rewardBoxPrefab, this.rewardBoxHolder);
				rewardBox4.SetContent(grantableConsumable.Icon, grantableConsumable.IconBg, grantableConsumable.Amount, 1f);
				list.Add(rewardBox4);
			}
		}
		return list;
	}

	protected override void OnAboutToOpen()
	{
		this.UpdateUI(false);
	}

	protected virtual void UpdateUI(bool triggerOpenEffect = false)
	{
		this.title.SetText(this.eventContent.Title);
		this.subTitle.SetText(this.eventContent.SubTitle);
		this.description.SetText(this.eventContent.Description);
		this.ignBadge.SetActive(false);
		this.expiresLabel.SetVariableText(new string[]
		{
			FHelper.FromSecondsToDaysHoursMinutesSecondsFormatMaxTwo((float)this.eventContent.SecondsLeftOnEvent)
		});
		if (triggerOpenEffect)
		{
			this.openRewardEffectHolder.SetActive(true);
		}
		this.rewardBoxHolder.gameObject.SetActive(this.eventContent.HasUnlockedBigReward);
		this.rewardHolder.SetActive(!this.eventContent.HasUnlockedBigReward);
		this.buttonInfoRewardContent.SetActive(!this.eventContent.HasUnlockedBigReward);
		if (this.eventContent.HasUnlockedBigReward)
		{
			for (int i = 0; i < this.rewardBoxes.Count; i++)
			{
				this.rewardBoxes[i].transform.SetParent(this.rewardBoxHolder, false);
			}
			this.btnBigRewardCostLabel.SetText("Close");
		}
		int a = 6;
		int amount = ConsumableManager.Instance.GetAmount(this.eventContent.ReductionConsumable);
		int num = Mathf.Min(a, this.ticketsOnButton.Count);
		for (int j = 0; j < num; j++)
		{
			Image image = this.ticketsOnButton[j];
			image.gameObject.SetActive(true);
			if (j < amount)
			{
				image.color = Color.white;
			}
			else
			{
				image.color = new Color(1f, 1f, 1f, 0.3f);
			}
		}
		this.costOriginalLbl.transform.parent.gameObject.SetActive(!this.eventContent.HasUnlockedBigReward);
		this.costOriginalLbl.SetVariableText(new string[]
		{
			this.eventContent.BigRewardInitialCost.ToString()
		});
		this.costDiscountedLbl.SetVariableText(new string[]
		{
			this.CostToUnlockRemaining.ToString()
		});
		this.costDiscountedLbl.gameObject.SetActive(amount > 0);
		this.costOriginalStrikeImg.gameObject.SetActive(amount > 0);
		if (this.instantiatedOffer != null && HolidayOfferManager.Instance.IsBought(this.eventContent.SpecialOfferForEvent))
		{
			this.instantiatedOffer.gameObject.SetActive(false);
		}
	}

	public virtual void Init(EventContent eventContentAsset, bool showInfoTab)
	{
		this.eventContent = eventContentAsset.Create();
		this.showInfoTabAsFirst = showInfoTab;
	}

	protected override void Start()
	{
		base.Start();
		this.eventContent.Load();
		this.eventContent.Save();
		this.eventContent.ReductionConsumable.OnGranted += this.ReductionConsumable_OnGranted;
		this.rewardBoxes = this.CreateRewardBoxes(this.eventContent.BigRewardContents);
		this.rewardPreviewDialog.Init(this.rewardBoxes.ToArray());
		foreach (EventChallenge eventChallenge in this.eventContent.Challenges)
		{
			eventChallenge.OnGoalCompleted += this.Challenge_OnGoalCompleted;
			UIEventChallenge uieventChallenge = UnityEngine.Object.Instantiate<UIEventChallenge>(this.eventContent.UIEventChallengePrefab, this.uiChallengesParent);
			uieventChallenge.SetupChallenge(eventChallenge);
			this.uiChallenges.Add(uieventChallenge);
		}
		this.ignIconHolderObject = UnityEngine.Object.Instantiate<GameObject>(this.eventContent.IGNIconPrefab, this.ignIconParent);
		this.rewardHolder = UnityEngine.Object.Instantiate<GameObject>(this.eventContent.RewardBasketPrefab, this.rewardBasketPosition);
		this.SetupThemeSpecifics();
		this.UpdateUI(false);
		this.tabViewChallenges.SetActive(true);
		this.tabViewInfo.SetActive(true);
		HolidayOffer specialOfferForEvent = this.eventContent.SpecialOfferForEvent;
		if (!HolidayOfferManager.Instance.IsBought(specialOfferForEvent))
		{
			this.instantiatedOffer = UnityEngine.Object.Instantiate<HolidayOfferBehaviour>(specialOfferForEvent.BehaviourToInstantiate, this.specialOfferPositioner);
			this.instantiatedOffer.Init(specialOfferForEvent, new Action<HolidayOffer>(this.OnBought));
			this.instantiatedOffer.DisableShopSpecificUI();
		}
		AFKManager.Instance.OnAfkDialogClosed += this.Instance_OnAfkDialogClosed;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.eventContent.InfoContentPrefab, this.infoContentPositioner);
		this.SetTab((!this.showInfoTabAsFirst) ? 1 : 0);
	}

	private void Instance_OnAfkDialogClosed(UIAfkFishingDialog obj)
	{
		if ((DateTime.Now - this.eventContent.LastShownEventInfo).TotalHours >= 24.0)
		{
			this.eventContent.LastShownEventInfo = DateTime.Now;
			this.SetTab(0);
			this.challengeTabBadge.SetActive(true);
			InGameNotificationManager.Instance.OpenFirstOccurrenceOfIGN<IGNChallengeEvent>(this.inGameNotification);
		}
	}

	private void OnBought(HolidayOffer offer)
	{
		HolidayOfferManager.Instance.MarkOfferAsBought(offer);
		UnityEngine.Object.Destroy(this.instantiatedOffer.gameObject);
		this.instantiatedOffer = null;
	}

	private void Challenge_OnGoalCompleted(EventChallenge arg1, BaseGoal arg2)
	{
		this.ignBadge.SetActive(true);
	}

	private void ReductionConsumable_OnGranted(BaseConsumable arg, int amount1)
	{
		this.UpdateUI(false);
	}

	private void SetupThemeSpecifics()
	{
		this.title.color = this.eventContent.ColorMain;
		this.subTitle.color = this.eventContent.ColorMain;
		this.description.color = this.eventContent.ColorTextOnMainBackground;
		this.mainBg.color = this.eventContent.ColorMainBackground;
		this.ignIconBg.color = this.eventContent.ColorMain;
		this.headerBg.color = this.eventContent.ColorMain;
		this.buttonMainColor.color = this.eventContent.ColorMain;
		this.buttonOutlineColor.color = this.eventContent.ColorMainLighter;
		this.buttonDropColor.color = this.eventContent.ColorMain;
		this.expiresLabel.color = this.eventContent.ColorMain;
		this.rewardPreviewDialog.SetColorTheme(this.eventContent.ColorMain);
	}

	public int CostToUnlockRemaining
	{
		get
		{
			int amount = ConsumableManager.Instance.GetAmount(this.eventContent.ReductionConsumable);
			float num = 1f - (float)amount * this.eventContent.ReductionConsumable.PriceReduction;
			return Mathf.Max(0, Mathf.RoundToInt((float)this.eventContent.BigRewardInitialCost * num));
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.eventContent.Save();
		this.eventContent.ReductionConsumable.OnGranted -= this.ReductionConsumable_OnGranted;
		foreach (EventChallenge eventChallenge in this.eventContent.Challenges)
		{
			eventChallenge.OnGoalCompleted -= this.Challenge_OnGoalCompleted;
			List<BaseGoal> goals = eventChallenge.Goals;
			foreach (BaseGoal baseGoal in goals)
			{
				baseGoal.Disable();
			}
		}
		AFKManager.Instance.OnAfkDialogClosed -= this.Instance_OnAfkDialogClosed;
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			this.eventContent.Save();
		}
	}

	protected override void OnOpened()
	{
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnReturned()
	{
		this.SetTab(1);
	}

	protected override void OnRemovedFromList()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
	}

	[SerializeField]
	private UIInGameNotificationItem parentHolder;

	[SerializeField]
	private Transform ignIconParent;

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private TextMeshProUGUI subTitle;

	[SerializeField]
	private TextMeshProUGUI description;

	[SerializeField]
	private Image mainBg;

	[SerializeField]
	private Image ignIconBg;

	[SerializeField]
	private Image headerBg;

	[SerializeField]
	private Image buttonMainColor;

	[SerializeField]
	private Image buttonOutlineColor;

	[SerializeField]
	private Image buttonDropColor;

	[SerializeField]
	private GameObject openRewardEffectHolder;

	[SerializeField]
	private GameObject buttonInfoRewardContent;

	[SerializeField]
	private Transform rewardBasketPosition;

	[SerializeField]
	protected Transform uiChallengesParent;

	[SerializeField]
	private Transform rewardBoxHolder;

	[SerializeField]
	private RewardPreviewDialog rewardPreviewDialog;

	[SerializeField]
	private RewardBox rewardBoxPrefab;

	[SerializeField]
	private TextMeshProUGUI expiresLabel;

	[SerializeField]
	private TextMeshProUGUI costDiscountedLbl;

	[SerializeField]
	private TextMeshProUGUI costOriginalLbl;

	[SerializeField]
	private Image costOriginalStrikeImg;

	[SerializeField]
	private TextMeshProUGUI btnBigRewardCostLabel;

	[SerializeField]
	private GameObject ignBadge;

	[SerializeField]
	private GameObject challengeTabBadge;

	[SerializeField]
	private Image tabInfo;

	[SerializeField]
	private Image tabChallenges;

	[SerializeField]
	private GameObject tabViewChallenges;

	[SerializeField]
	private GameObject tabViewInfo;

	[SerializeField]
	private Transform specialOfferPositioner;

	[SerializeField]
	private Transform infoContentPositioner;

	[SerializeField]
	private List<Image> ticketsOnButton = new List<Image>();

	private EventContent eventContent;

	protected List<RewardBox> rewardBoxes = new List<RewardBox>();

	protected List<UIEventChallenge> uiChallenges = new List<UIEventChallenge>();

	private GameObject ignIconHolderObject;

	private GameObject rewardHolder;

	private HolidayOfferBehaviour instantiatedOffer;

	private int currentTab;

	private bool showInfoTabAsFirst;
}
