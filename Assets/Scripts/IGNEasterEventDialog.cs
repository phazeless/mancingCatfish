using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IGNEasterEventDialog : InGameNotificationDialog<IGNEasterEvent>
{
	private void Init()
	{
		for (int i = 0; i < EasterManager.Instance.MaxEggsCount; i++)
		{
			easterEggBox item = UnityEngine.Object.Instantiate<easterEggBox>(this.eggBoxPrefab, this.eggBoxesHolder);
			this.instantiatedEggs.Add(item);
		}
		this.rewardBoxes = this.CreateRewardBoxes(EasterManager.Instance.BasketContent);
		this.rewardPreviewDialog.Init(this.rewardBoxes.ToArray());
	}

	private void UpdateUI()
	{
		for (int i = 0; i < this.instantiatedEggs.Count; i++)
		{
			List<int> eggsFoundIndexes = EasterManager.Instance.GetEggsFoundIndexes();
			easterEggBox easterEggBox = this.instantiatedEggs[i];
			if (eggsFoundIndexes.Contains(i))
			{
				easterEggBox.SetSprite(EasterManager.Instance.GetEggPrefab(i).EggSprite);
			}
		}
		this.expiresLabel.SetVariableText(new string[]
		{
			FHelper.FromSecondsToDaysHoursMinutesSecondsFormatMaxTwo((float)EasterManager.Instance.SecondsLeftOnEvent)
		});
		this.effectsHolder.SetActive(false);
		if (!EasterManager.Instance.HasUnlockedBigReward)
		{
			this.rewardBoxHolder.gameObject.SetActive(false);
			this.buttonLabel.SetVariableText(new string[]
			{
				EasterManager.Instance.CostToUnlockRemaining.ToString()
			});
		}
		else
		{
			this.rewardBoxHolder.gameObject.SetActive(true);
			for (int j = 0; j < this.rewardBoxes.Count; j++)
			{
				this.rewardBoxes[j].transform.SetParent(this.rewardBoxHolder, false);
			}
			this.giftHolder.SetActive(false);
			this.InfoButton.SetActive(false);
			this.buttonLabel.SetText("Close");
		}
	}

	private List<RewardBox> CreateRewardBoxes(EasterBasketRewardContent content)
	{
		List<RewardBox> list = new List<RewardBox>();
		RewardBox rewardBox = UnityEngine.Object.Instantiate<RewardBox>(this.rewardBoxPrefab, this.rewardBoxHolder);
		RewardBox rewardBox2 = UnityEngine.Object.Instantiate<RewardBox>(this.rewardBoxPrefab, this.rewardBoxHolder);
		RewardBox rewardBox3 = UnityEngine.Object.Instantiate<RewardBox>(this.rewardBoxPrefab, this.rewardBoxHolder);
		rewardBox.SetContent(content.Crew.GetExtraInfo().Icon, Color.white, 1, 1f);
		rewardBox2.SetContent(content.Item.Icon, HookedColors.ItemEpic, 1, 1f);
		rewardBox3.SetContentAsGems(content.GemAmount, 1f);
		list.Add(rewardBox);
		list.Add(rewardBox2);
		list.Add(rewardBox3);
		return list;
	}

	public void UnlockBigReward()
	{
		if (EasterManager.Instance.UnlockBigReward())
		{
			this.UpdateUI();
			this.effectsHolder.SetActive(true);
		}
		else
		{
			this.Close(false);
		}
	}

	protected override void Start()
	{
		base.Start();
		this.Init();
		this.UpdateUI();
	}

	protected override void OnAboutToOpen()
	{
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
	}

	protected override void OnRemovedFromList()
	{
	}

	protected override void OnReturned()
	{
	}

	[SerializeField]
	private easterEggBox eggBoxPrefab;

	[SerializeField]
	private Transform eggBoxesHolder;

	[SerializeField]
	private GameObject giftHolder;

	[SerializeField]
	private GameObject effectsHolder;

	[SerializeField]
	private GameObject InfoButton;

	[SerializeField]
	private TextMeshProUGUI buttonLabel;

	[SerializeField]
	private TextMeshProUGUI expiresLabel;

	[SerializeField]
	private RewardBox rewardBoxPrefab;

	[SerializeField]
	private Transform rewardBoxHolder;

	[SerializeField]
	private RewardPreviewDialog rewardPreviewDialog;

	private List<easterEggBox> instantiatedEggs = new List<easterEggBox>();

	private List<RewardBox> rewardBoxes = new List<RewardBox>();
}
