using System;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelBehaviour : MonoBehaviour
{
	public static WheelBehaviour Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action OnWheelSpun;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<bool, int> OnWheelSpinFinished;

	private void UpdateWheelRewardTexts(int forMultiplier, int increaseInMultiplier = 0)
	{
		List<TextMeshProUGUI> list = new List<TextMeshProUGUI>();
		list = ((forMultiplier != 2) ? ((forMultiplier != 3) ? ((forMultiplier != 4) ? list : this.multiplierLabels4X) : this.multiplierLabels3X) : this.multiplierLabels2X);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].SetVariableText(new string[]
			{
				(forMultiplier + increaseInMultiplier).ToString()
			});
		}
	}

	private void ShowSpinWheelAd(AdPlacement placement, Action<bool> onComplete)
	{
		this.spinWheelButton.interactable = false;
		this.spinWheelButtonText.SetText("Loading ads...");
		//MoPubMono.ShowVideo(placement, delegate(bool success)
		//{
		//	if (onComplete != null)
		//	{
		//		onComplete(success);
		//	}
		//}, delegate()
		//{
		//	this.spinWheelButton.interactable = true;
		//	if (this.freeSpinSkill.CurrentLevel <= 0)
		//	{
		//		this.spinWheelButtonText.SetText("Watch Ad to Spin");
		//	}
		//	else
		//	{
		//		this.spinWheelButtonText.SetText("Free Spin");
		//	}
		//});
	}

	private void Awake()
	{
		WheelBehaviour.Instance = this;
	}

	private void Start()
	{
		this.InitiateWheel();
		this.jackpotAmount.SetText(SkillManager.Instance.JackpotSkill.CurrentLevel.ToString());
		this.gemPileRect.transform.localPosition = new Vector2(4f, Mathf.Min(this.gemPileRectBaseYOffset + (float)(SkillManager.Instance.JackpotSkill.CurrentLevel * 5), -218f));
		this.wheelRectTransform = this.wheelTransform.GetComponent<RectTransform>();
		this.rewardAndChances = base.GetComponent<WheelRewardAndChances>();
		foreach (WheelRewardAndChances.RewardChance rewardChance in this.rewardAndChances.RewardChances)
		{
			if (rewardChance.GemRef != null)
			{
				this.Gems.Add(rewardChance.GemRef);
			}
			if (rewardChance.RewardFishValueIncrease != 4)
			{
				this.UpdateWheelRewardTexts(rewardChance.RewardFishValueIncrease, 0);
			}
			else
			{
				int increaseInMultiplier = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.WheelHighestMultiplierBonus>();
				this.UpdateWheelRewardTexts(rewardChance.RewardFishValueIncrease, increaseInMultiplier);
			}
		}
		SkillManager.Instance.OnSkillAttributeValueChanged += this.Instance_OnSkillAttributeValueChanged;
		this.freeSpinSkill.OnSkillLevelUp += this.FreeSpinSkill_OnSkillLevelUp;
	}

	private void FreeSpinSkill_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		CrownExpGranterManager.Instance.UpdateCrownExpGranterState(GranterLocation.Create(AdPlacement.SpinWheel));
	}

	private void Instance_OnSkillAttributeValueChanged(ISkillAttribute attr, float val)
	{
		if (attr is Skills.WheelHighestMultiplierBonus)
		{
			this.UpdateWheelRewardTexts(2, 0);
			this.UpdateWheelRewardTexts(3, 0);
			this.UpdateWheelRewardTexts(4, (int)val);
		}
	}

	public void InitiateWheel()
	{
		float num = 0f;
		for (int i = 0; i < this.wheelSlices.Length; i++)
		{
			this.wheelSize += this.wheelSlices[i].size;
		}
		for (int j = 0; j < this.wheelSlices.Length; j++)
		{
			float num2 = this.wheelSlices[j].size / this.wheelSize * 360f;
			this.wheelSlices[j].degreesFromTo = new Vector2(num, num + num2);
			num += num2;
		}
	}

	public void WatchAdAndSpinWheel(AdPlacement placement)
	{
		if (this.freeSpinSkill.CurrentLevel > 0)
		{
			this.SpinTheWheel(base.GetComponent<WheelRewardAndChances>().GetRandomAnglePair());
			this.freeSpinSkill.SetCurrentLevel(this.freeSpinSkill.CurrentLevel - 1, LevelChange.SoftReset);
		}
		else
		{
			this.ShowSpinWheelAd(placement, delegate(bool shouldReward)
			{
				if (shouldReward)
				{
					this.SpinTheWheel(base.GetComponent<WheelRewardAndChances>().GetRandomAnglePair());
				}
			});
		}
	}

	public void GetRandomSlice()
	{
		this.WatchAdAndSpinWheel(AdPlacement.SpinWheel);
	}

	private bool IsSpinning
	{
		get
		{
			return !this.backButtons.gameObject.activeSelf;
		}
	}

	private void SpinTheWheel(WheelRewardAndChances.AnglePair anglePair)
	{
		if (this.IsSpinning)
		{
			return;
		}
		if (WheelBehaviour.OnWheelSpun != null)
		{
			WheelBehaviour.OnWheelSpun();
		}
		this.TweenKiller();
		this.wheelAudioSource.Play();
		int num = anglePair.RandomAngle();
		if (num % 45 < 10)
		{
			num += 15;
		}
		else if (num % 45 > 35)
		{
			num -= 15;
		}
		this.backButtons.gameObject.SetActive(false);
		int num2 = (num + 720) / 360;
		this.wheelTransform.DORotate(new Vector3(0f, 0f, (float)(num + 720)), (float)num2, RotateMode.FastBeyond360).SetEase(Ease.OutCubic).OnComplete(delegate
		{
			this.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.5f, 10, 1f);
			this.backButtons.gameObject.SetActive(true);
			WheelRewardAndChances.RewardChance reward = anglePair.Reward;
			int num3 = reward.RewardFishValueIncrease;
			if (this.IsHighestMultiplierReward(num3))
			{
				num3 += (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.WheelHighestMultiplierBonus>();
			}
			bool flag = num3 < 0;
			bool flag2 = num3 > 0 && num3 > SkillManager.Instance.FishValueBonusSkill.CurrentLevel;
			if (flag2)
			{
				SkillManager.Instance.FishValueBonusSkill.SetCurrentLevel(num3, LevelChange.SoftReset);
				SkillManager.Instance.FishValueBonusSkill.Activate();
			}
			for (int i = 0; i < this.Gems.Count; i++)
			{
				if (reward.GemRef != this.Gems[i])
				{
					this.AnimateGemToJackpot(i);
				}
				else
				{
					this.AnimateGemToUser(i);
				}
			}
			int num4 = 0;
			if (flag)
			{
				GameAnalyticsEvents.ResourceGemsIncreased(AnalyticsEvents.REType.Jackpot, AnalyticsEvents.RECategory.Wheel, SkillManager.Instance.JackpotSkill.CurrentLevel);
				int num5 = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.WheelJackpotAmountBonus>();
				num4 = SkillManager.Instance.JackpotSkill.CurrentLevel + num5;
				ResourceChangeData gemChangeData = new ResourceChangeData("contentId_wheelJackpot", null, num4, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.WheelJackpot);
				GemGainVisual.Instance.GainGems(num4, this.wheelTransform.position, gemChangeData);
				SkillManager.Instance.JackpotSkill.SetCurrentLevel(0, LevelChange.SoftReset);
				this.particleController.SetWheelEffect(WheelRewardAndChances.Reward.Jackpot);
			}
			else
			{
				this.particleController.SetWheelEffect(WheelRewardAndChances.Reward._2X);
			}
			if (WheelBehaviour.OnWheelSpinFinished != null)
			{
				WheelBehaviour.OnWheelSpinFinished(flag, num4);
			}
		});
	}

	private bool IsHighestMultiplierReward(int multiplierReward)
	{
		return multiplierReward == 4;
	}

	private void AnimateGemToUser(int gemIndex)
	{
		RectTransform rectTransform = this.Gems[gemIndex];
		GameAnalyticsEvents.ResourceGemsIncreased(AnalyticsEvents.REType.WheelGem, AnalyticsEvents.RECategory.Wheel, 1);
		int amount = 1;
		ResourceChangeData gemChangeData = new ResourceChangeData("contentId_wheelSingleReward", null, amount, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.WheelSingle);
		GemGainVisual.Instance.GainGems(amount, rectTransform.position, gemChangeData);
		rectTransform.localScale = Vector3.zero;
		rectTransform.DOScale(1f, 0.2f).SetDelay(0.4f);
	}

	private void AnimateGemToJackpot(int gemIndex)
	{
		RectTransform gem = this.Gems[gemIndex];
		Vector2 startPosition = gem.anchoredPosition;
		gem.DOAnchorPos(this.wheelRectTransform.anchoredPosition, 0.4f, false).SetEase(Ease.InBack).SetDelay(0.2f * (float)gemIndex).OnComplete(delegate
		{
			gem.anchoredPosition = startPosition;
			gem.localScale = Vector3.zero;
			gem.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.3f);
			Skill jackpotSkill = SkillManager.Instance.JackpotSkill;
			jackpotSkill.TryLevelUp();
			this.jackpotAmount.transform.parent.DOKill(true);
			this.jackpotAmount.transform.parent.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.3f, 10, 1f);
			this.jackpotAmount.SetText(jackpotSkill.CurrentLevel.ToString());
			this.gemPileRect.DOKill(true);
			this.gemPileRect.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.5f, 5, 0.5f);
			this.gemPileRect.transform.localPosition = new Vector2(4f, Mathf.Min(this.gemPileRectBaseYOffset + (float)(jackpotSkill.CurrentLevel * 5), -218f));
		});
	}

	private void OnDisable()
	{
		this.TweenKiller();
	}

	private void TweenKiller()
	{
		if (this.wheelTransform != null)
		{
			this.wheelTransform.DOKill(true);
		}
		if (this.gemPileRect != null)
		{
			this.gemPileRect.DOKill(true);
		}
		if (base.transform != null || this != null)
		{
			base.transform.DOKill(false);
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.L))
		{
			this.SpinTheWheel(base.GetComponent<WheelRewardAndChances>().GetRandomAnglePair());
		}
	}

	private const string contentId_wheelJackpot = "contentId_wheelJackpot";

	private const string contentId_wheelSingleReward = "contentId_wheelSingleReward";

	[SerializeField]
	private Skill freeSpinSkill;

	[SerializeField]
	private Transform wheelTransform;

	[SerializeField]
	private WheelSlice[] wheelSlices;

	[SerializeField]
	private HingeBehaviour hingeBehaviour;

	[SerializeField]
	private WheelParticleController particleController;

	[SerializeField]
	private TextMeshProUGUI jackpotAmount;

	[SerializeField]
	private RectTransform GemPrefab;

	[SerializeField]
	private RectTransform gemPileRect;

	[SerializeField]
	private Button spinWheelButton;

	[SerializeField]
	private TextMeshProUGUI spinWheelButtonText;

	[SerializeField]
	private List<TextMeshProUGUI> multiplierLabels2X = new List<TextMeshProUGUI>();

	[SerializeField]
	private List<TextMeshProUGUI> multiplierLabels3X = new List<TextMeshProUGUI>();

	[SerializeField]
	private List<TextMeshProUGUI> multiplierLabels4X = new List<TextMeshProUGUI>();

	private float gemPileRectBaseYOffset = -416f;

	private float wheelSize;

	private InGameNotification fishValueBonusNotification;

	[SerializeField]
	private Transform backButtons;

	[SerializeField]
	private AudioSource wheelAudioSource;

	private List<RectTransform> Gems = new List<RectTransform>();

	private RectTransform wheelRectTransform;

	private WheelRewardAndChances rewardAndChances;
}
