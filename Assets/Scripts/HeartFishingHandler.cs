using System;
using System.Numerics;
using DG.Tweening;
using UnityEngine;

public class HeartFishingHandler : MonoBehaviour
{
	public static HeartFishingHandler Instance { get; private set; }

	public void DisableGoldFishing()
	{
		this.heartFishingSkill.FastForward(this.heartFishingSkill.GetTotalSecondsLeftOnCooldown());
	}

	private void Awake()
	{
		HeartFishingHandler.Instance = this;
		this.heartFishingSkill.OnSkillActivation += this.HeartFishingSkill_OnSkillActivation;
		this.heartFishingSkill.OnSkillDurationEnd += this.HeartFishingSkill_OnSkillDurationEnd;
		this.heartFishingSkill.OnSkillCooldownZero += this.HeartFishingSkill_OnSkillCooldownZero;
	}

	private void Start()
	{
		SkillManager.Instance.DeepWaterSkill.OnSkillLevelUp += this.DeepWaterSkill_OnSkillLevelUp;
	}

	private void DeepWaterSkill_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		if (levelChange == LevelChange.LevelUp && this.heartFishingSkill.IsActivated)
		{
			this.UpdateHeartFishValue();
		}
	}

	private void Instance_OnGoodBalanceChanged(string itemId, int balance, int amountAdded)
	{
		if ("se.ace.boost_goldfish" == itemId && amountAdded == 1)
		{
			this.heartFishingSkill.Activate();
		}
	}

	private void HeartFishingSkill_OnSkillDurationEnd(Skill obj)
	{
		Skills.DeepWaterTier deepWaterTier = (Skills.DeepWaterTier)SkillManager.Instance.DeepWaterSkill.GetExtraInfo().SkillSpecifics;
		Camera.main.DOColor(deepWaterTier.WaterColors[SkillManager.Instance.DeepWaterSkill.CurrentLevel], 3f);
		FishPoolManager.Instance.SetOverrideCurrentDWFishType(null, 0);
	}

	private void HeartFishingSkill_OnSkillCooldownZero(Skill skill)
	{
	}

	private void HeartFishingSkill_OnSkillActivation(Skill skill)
	{
		this.audioSource.Play();
		Camera.main.DOColor(Color.red, 3f);
		this.UpdateHeartFishValue();
	}

	private void UpdateHeartFishValue()
	{
		FishBehaviour fishPrefab = FishPoolManager.Instance.GetFishPrefab(FishBehaviour.FishType.Special11);
		FishBehaviour fishPrefabAtCurrentDW = FishPoolManager.Instance.GetFishPrefabAtCurrentDW();
		float value = (float)((int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.HeartFishValueMultiplier>()) / 100f;
		BigInteger right = fishPrefab.FishInfo.BaseValue.Value.MultiplyFloat(value);
		BigInteger overrideValue = (fishPrefab.FishInfo.BaseValue.Value + right) * fishPrefabAtCurrentDW.FishInfo.BaseValue.Value;
		FishPoolManager.Instance.SetOverrideCurrentDWFishType(new FishBehaviour.FishType?(FishBehaviour.FishType.Special11), overrideValue);
	}

	private void OnDestroy()
	{
		SkillManager.Instance.DeepWaterSkill.OnSkillLevelUp -= this.DeepWaterSkill_OnSkillLevelUp;
	}

	[SerializeField]
	private Skill heartFishingSkill;

	[SerializeField]
	private AudioSource audioSource;
}
