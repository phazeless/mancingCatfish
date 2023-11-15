using System;
using System.Numerics;
using ACE.IAPS;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GoldFishingHandler : MonoBehaviour
{
	public static GoldFishingHandler Instance { get; private set; }

	public void DisableGoldFishing()
	{
		this.goldFishingSkill.FastForward(this.goldFishingSkill.GetTotalSecondsLeftOnCooldown());
	}

	private void Awake()
	{
		GoldFishingHandler.Instance = this;
		this.gemCost.SetVariableText(new string[]
		{
			5.ToString()
		});
		this.goldFishingSkill.OnSkillActivation += this.GoldFishingSkill_OnSkillActivation;
		this.goldFishingSkill.OnSkillDurationEnd += this.GoldFishingSkill_OnSkillDurationEnd;
		this.goldFishingSkill.OnSkillCooldownZero += this.GoldFishingSkill_OnSkillCooldownZero;
		StoreManager storeManager = ResourceManager.StoreManager;
		storeManager.OnGoodBalanceChanged = (Action<string, int, int>)Delegate.Combine(storeManager.OnGoodBalanceChanged, new Action<string, int, int>(this.Instance_OnGoodBalanceChanged));
	}

	private void Start()
	{
		SkillManager.Instance.DeepWaterSkill.OnSkillLevelUp += this.DeepWaterSkill_OnSkillLevelUp;
		SkillManager.Instance.OnSkillAttributeValueChanged += this.Instance_OnSkillAttributeValueChanged;
		this.UpdateUI();
	}

	private void Instance_OnSkillAttributeValueChanged(ISkillAttribute attr, float val)
	{
		if (attr is Skills.GoldFishBonus)
		{
			this.UpdateUI();
		}
	}

	private void DeepWaterSkill_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		if (levelChange == LevelChange.LevelUp && this.goldFishingSkill.IsActivated)
		{
			this.UpdateGoldFishValue();
		}
	}

	private void Instance_OnGoodBalanceChanged(string itemId, int balance, int amountAdded)
	{
		if ("se.ace.boost_goldfish" == itemId && amountAdded == 1)
		{
			this.goldFishingSkill.Activate();
		}
	}

	private void GoldFishingSkill_OnSkillDurationEnd(Skill obj)
	{
		Skills.DeepWaterTier deepWaterTier = (Skills.DeepWaterTier)SkillManager.Instance.DeepWaterSkill.GetExtraInfo().SkillSpecifics;
		Camera.main.DOColor(deepWaterTier.WaterColors[SkillManager.Instance.DeepWaterSkill.CurrentLevel], 3f);
		FishPoolManager.Instance.SetOverrideCurrentDWFishType(null, 0);
	}

	private void GoldFishingSkill_OnSkillCooldownZero(Skill skill)
	{
		this.buyBlocker.gameObject.SetActive(false);
	}

	private void GoldFishingSkill_OnSkillActivation(Skill skill)
	{
		this.audioSource.Play();
		ResourceManager.StoreManager.TakeItem("se.ace.boost_goldfish", 1);
		this.buyBlocker.SetActive(true);
		Camera.main.DOColor(Color.yellow, 3f);
		this.UpdateGoldFishValue();
		this.RunAfterDelay(0.5f, delegate()
		{
			ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Main);
		});
	}

	private void UpdateUI()
	{
		BigInteger goldFishValueMultiplier = this.GetGoldFishValueMultiplier();
		int goldFishDurationInMinutes = this.GetGoldFishDurationInMinutes();
		this.amountAndDurationLabel.SetVariableText(new string[]
		{
			goldFishValueMultiplier.ToString(),
			goldFishDurationInMinutes.ToString()
		});
	}

	private BigInteger GetGoldFishValueMultiplier()
	{
		FishBehaviour fishPrefab = FishPoolManager.Instance.GetFishPrefab(FishBehaviour.FishType.Special0);
		int value = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.GoldFishBonus>();
		return fishPrefab.FishInfo.BaseValue.Value + value;
	}

	private int GetGoldFishDurationInMinutes()
	{
		return this.goldFishingSkill.Duration / 60;
	}

	private void UpdateGoldFishValue()
	{
		FishBehaviour fishPrefabAtCurrentDW = FishPoolManager.Instance.GetFishPrefabAtCurrentDW();
		BigInteger overrideValue = this.GetGoldFishValueMultiplier() * fishPrefabAtCurrentDW.FishInfo.BaseValue.Value;
		FishPoolManager.Instance.SetOverrideCurrentDWFishType(new FishBehaviour.FishType?(FishBehaviour.FishType.Special0), overrideValue);
	}

	private void Update()
	{
		if (this.timeLeft.gameObject.activeInHierarchy)
		{
			this.timeLeft.SetVariableText(new string[]
			{
				Mathf.CeilToInt(this.goldFishingSkill.GetTotalSecondsLeftOnCooldown()).ToString()
			});
		}
	}

	[SerializeField]
	private Skill goldFishingSkill;

	[SerializeField]
	private GameObject buyBlocker;

	[SerializeField]
	private TextMeshProUGUI timeLeft;

	[SerializeField]
	private TextMeshProUGUI gemCost;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private TextMeshProUGUI amountAndDurationLabel;
}
