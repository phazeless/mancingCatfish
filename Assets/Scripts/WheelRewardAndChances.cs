using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

[fiInspectorOnly]
public class WheelRewardAndChances : MonoBehaviour
{
	public List<WheelRewardAndChances.RewardChance> RewardChances
	{
		get
		{
			return this.rewardChance;
		}
	}

	public WheelRewardAndChances.AnglePair GetRandomAnglePair()
	{
		int index = this.chanceIndexes[UnityEngine.Random.Range(0, this.chanceIndexes.Count)];
		return this.rewardAngles[index];
	}

	private void Start()
	{
		this.UpdateChances();
		if (SkillManager.Instance != null)
		{
			SkillManager.Instance.OnSkillAttributeValueChanged += this.Instance_OnSkillAttributeValueChanged;
		}
	}

	private void Instance_OnSkillAttributeValueChanged(ISkillAttribute attr, float val)
	{
		if (attr is Skills.WheelHighestMultiplierChanceBonus)
		{
			this.UpdateChances();
		}
	}

	private void UpdateChances()
	{
		this.rewardAngles.Clear();
		this.chanceIndexes.Clear();
		for (int i = 0; i < this.rewardChance.Count; i++)
		{
			int num = i * 45;
			this.rewardAngles.Add(new WheelRewardAndChances.AnglePair(this.rewardChance[i], num, num + 44));
			float num2 = this.rewardChance[i].Chance;
			if (this.rewardChance[i].RewardFishValueIncrease == 4)
			{
				float num3 = (!(SkillManager.Instance != null)) ? 0f : SkillManager.Instance.GetCurrentTotalValueFor<Skills.WheelHighestMultiplierChanceBonus>();
				num2 += num2 * (num3 / 100f);
			}
			else if (this.rewardChance[i].RewardFishValueIncrease < 0)
			{
				float num4 = (!(SkillManager.Instance != null)) ? 0f : SkillManager.Instance.GetCurrentTotalValueFor<Skills.WheelJackpotChanceBonus>();
				num2 += num2 * (num4 / 100f);
			}
			int num5 = 0;
			while ((float)num5 < num2 * 10f)
			{
				this.chanceIndexes.Add(i);
				num5++;
			}
		}
	}

	[SerializeField]
	private List<WheelRewardAndChances.RewardChance> rewardChance = new List<WheelRewardAndChances.RewardChance>();

	[InspectorDisabled]
	[SerializeField]
	private float totalPercent;

	private List<int> chanceIndexes = new List<int>();

	private List<WheelRewardAndChances.AnglePair> rewardAngles = new List<WheelRewardAndChances.AnglePair>();

	public enum Reward
	{
		_2X,
		_3X,
		_4X,
		Jackpot
	}

	[Serializable]
	public class RewardChance
	{
		public float Chance;

		public int RewardGems;

		public int RewardFishValueIncrease;

		public RectTransform GemRef;
	}

	public class AnglePair
	{
		public AnglePair(WheelRewardAndChances.RewardChance reward, int startAngle, int endAngle)
		{
			this.Reward = reward;
			this.StartAngle = startAngle;
			this.EndAngle = endAngle;
		}

		public int StartAngle { get; private set; }

		public int EndAngle { get; private set; }

		public int RandomAngle()
		{
			return UnityEngine.Random.Range(this.StartAngle, this.EndAngle);
		}

		public WheelRewardAndChances.RewardChance Reward;
	}
}
