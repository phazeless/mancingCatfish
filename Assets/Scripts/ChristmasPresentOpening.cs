using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChristmasPresentOpening : MonoBehaviour
{
	private void Start()
	{
		if (this.ignoreStart)
		{
			return;
		}
		this.dcm = DailyChristmasManager.Instance;
		AudioManager.Instance.OneShooter(this.glimmerClip, 1f);
		this.RunAfterDelay(0.5f, delegate()
		{
			if (this != null)
			{
				AudioManager.Instance.OneShooter(this.landingClip1, 1f);
				AudioManager.Instance.OneShooter(this.landingClip2, 1f);
			}
		});
	}

	public void SpecialOffer()
	{
		this.comeBackTomorrowLabel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
		this.merryChristmasLabel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(delegate
		{
			this.ShowSpecialOffer();
		});
		foreach (DailyRewardItem dailyRewardItem in this.rewardHolder.GetComponentsInChildren<DailyRewardItem>())
		{
			dailyRewardItem.Remove();
		}
		this.isContinued = true;
	}

	private void ShowSpecialOffer()
	{
		this.santaLeftLabel.transform.localScale = Vector3.one * 1.3f;
		this.santaLeftLabel.GetComponent<VertexAttributeModifier>().enabled = true;
		this.santaLeftLabel.transform.DOMoveY(this.santaLeftLabel.transform.position.y + 3.1f, 1f, false).SetDelay(2f).SetEase(Ease.InOutQuad);
		this.santaLeftLabel.transform.DOScale(1f, 0.5f).SetDelay(2.3f);
		this.santaLeftLabel.GetComponent<TextMeshProUGUI>().DOFade(0.5f, 0.5f).SetDelay(2.3f);
		this.RunAfterDelay(2f, delegate()
		{
			if (this.BonusPresent == null)
			{
				return;
			}
			this.BonusPresent.gameObject.SetActive(true);
			this.BonusPresent.localScale = new Vector3(0f, 0.5f, 1f);
			this.BonusPresent.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.3f);
			this.RunAfterDelay(0.3f, delegate()
			{
				if (this != null)
				{
					if (this.megaPresentLabel == null)
					{
						return;
					}
					this.megaPresentLabel.transform.localScale = Vector3.one;
					this.megaPresentLabel.GetComponent<VertexAttributeModifier>().enabled = true;
				}
			});
		});
	}

	public void OpenOfferPresent(DailyGiftContent content)
	{
		this.holidayOfferRewardContent = content;
		this.isSpecialOfferActive = true;
		this.animator.SetTrigger("OPEN");
	}

	public void Open()
	{
		this.animator.SetTrigger("OPEN");
		AudioManager.Instance.OneShooter(this.openingClip, 1f);
		this.merryChristmasLabel.transform.localScale = Vector3.one;
		this.merryChristmasLabel.GetComponent<VertexAttributeModifier>().enabled = true;
		this.RunAfterDelay(2f, delegate()
		{
			if (this != null)
			{
				if (this.comeBackTomorrowLabel == null || this.isContinued)
				{
					return;
				}
				this.comeBackTomorrowLabel.transform.localScale = Vector3.one;
				this.comeBackTomorrowLabel.GetComponent<VertexAttributeModifier>().enabled = true;
			}
		});
	}

	public void Close()
	{
		this.rewardHolder.DOScale(0f, 0.2f);
		this.santaLeftLabel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
		this.megaPresentLabel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
		this.comeBackTomorrowLabel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
		this.merryChristmasLabel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
		this.BonusPresent.DOScale(0f, 0.3f).SetEase(Ease.OutBack);
	}

	public void ActivateParticles()
	{
		foreach (ParticleSystem particleSystem in this.particleHolder.GetComponentsInChildren<ParticleSystem>())
		{
			particleSystem.Play();
		}
	}

	public void ShowRewards()
	{
		DailyGiftContent reward = null;
		if (this.isSpecialOfferActive)
		{
			reward = this.holidayOfferRewardContent;
		}
		else
		{
			reward = this.dcm.OpenAndCollect();
		}
		float num = 0f;
		float num2 = 0.1f;
		float num3 = 0f;
		if (reward.Chest != null)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem = UnityEngine.Object.Instantiate<DailyRewardItem>(this.rewardPrefab, this.rewardHolder);
				dailyRewardItem.SetValues(reward.Chest.Icon, this.rewardVisualAttributes.itemBoxBgColor, "x1", false, 0.8f);
			});
			num += num2;
		}
		if (reward.Gems > 0)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem = UnityEngine.Object.Instantiate<DailyRewardItem>(this.rewardPrefab, this.rewardHolder);
				dailyRewardItem.SetValues(this.rewardVisualAttributes.gemRewardIcon, this.rewardVisualAttributes.gemRewardBgColor, reward.Gems + "<sprite=0>", false, 0.8f);
			});
			num += num2;
		}
		if (reward.FreeSpins > 0)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem = UnityEngine.Object.Instantiate<DailyRewardItem>(this.rewardPrefab, this.rewardHolder);
				dailyRewardItem.SetValues(this.rewardVisualAttributes.freeSpinIcon, this.rewardVisualAttributes.freeSpinColor, "x1", false, 1f);
			});
			num += num2;
		}
		if (reward.Fish != null)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem = UnityEngine.Object.Instantiate<DailyRewardItem>(this.rewardPrefab, this.rewardHolder);
				dailyRewardItem.SetValues(reward.Fish.FishInfo.FishIcon, this.rewardVisualAttributes.fishColor, "x1", false, 1f);
			});
			num += num2;
		}
		if (reward.Item != null)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem = UnityEngine.Object.Instantiate<DailyRewardItem>(this.rewardPrefab, this.rewardHolder);
				dailyRewardItem.SetValues(reward.Item.Icon, this.rewardVisualAttributes.GetItemColor((int)reward.Item.Rarity), "x1", false, 1f);
			});
			num += num2;
		}
		if (reward.Items.Count > 0)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				foreach (KeyValuePair<Item, int> keyValuePair in reward.Items)
				{
					Color itemColor;
					if (keyValuePair.Key.Rarity == Rarity.Common)
					{
						itemColor = this.rewardVisualAttributes.GetItemColor(0);
					}
					else if (keyValuePair.Key.Rarity == Rarity.Rare)
					{
						itemColor = this.rewardVisualAttributes.GetItemColor(1);
					}
					else
					{
						itemColor = this.rewardVisualAttributes.GetItemColor(2);
					}
					DailyRewardItem dailyRewardItem = UnityEngine.Object.Instantiate<DailyRewardItem>(this.rewardPrefab, this.rewardHolder);
					dailyRewardItem.SetValues(keyValuePair.Key.Icon, itemColor, "x" + keyValuePair.Value, false, 1f);
				}
			});
			num += num2;
		}
		if (reward.FishingExp > 0)
		{
			this.RunAfterDelay(num3 + num, delegate()
			{
				DailyRewardItem dailyRewardItem = UnityEngine.Object.Instantiate<DailyRewardItem>(this.rewardPrefab, this.rewardHolder);
				string count = CashFormatter.SimpleToCashRepresentation(reward.FishingExp, 0, false, false, "N0");
				dailyRewardItem.SetValues(this.rewardVisualAttributes.fishExpIcon, this.rewardVisualAttributes.fishExpColor, count, false, 0.8f);
			});
		}
	}

	private void OnDestroy()
	{
		this.rewardHolder.DOKill(false);
		this.BonusPresent.DOKill(false);
		this.merryChristmasLabel.transform.DOKill(false);
		this.comeBackTomorrowLabel.transform.DOKill(false);
		this.megaPresentLabel.transform.DOKill(false);
		this.santaLeftLabel.transform.DOKill(false);
	}

	[SerializeField]
	private Transform particleHolder;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AudioClip openingClip;

	[SerializeField]
	private AudioClip glimmerClip;

	[SerializeField]
	private AudioClip landingClip1;

	[SerializeField]
	private AudioClip landingClip2;

	[SerializeField]
	private Transform rewardHolder;

	[SerializeField]
	private DailyRewardItem rewardPrefab;

	[SerializeField]
	private GameObject merryChristmasLabel;

	[SerializeField]
	private GameObject comeBackTomorrowLabel;

	[SerializeField]
	private GameObject megaPresentLabel;

	[SerializeField]
	private GameObject santaLeftLabel;

	[SerializeField]
	private Transform BonusPresent;

	private DailyChristmasManager dcm;

	[SerializeField]
	private RewardVisualAttributes rewardVisualAttributes;

	[SerializeField]
	private bool ignoreStart;

	[SerializeField]
	private bool isSpecialOfferActive;

	private DailyGiftContent holidayOfferRewardContent;

	private bool isContinued;
}
