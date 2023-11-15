using System;
using System.Numerics;
using UnityEngine;

public class IGNPackage : InGameNotification
{
	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.Package;
		}
	}

	public override bool IsClearable
	{
		get
		{
			return true;
		}
	}

	public override bool RemoveOnReset
	{
		get
		{
			return true;
		}
	}

	public override void OnCreated()
	{
		if (this.CashAmount < 0L)
		{
			int currentDWLevel = DWHelper.CurrentDWLevel;
			int value = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.PackageValue>();
			int value2 = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.PackageMultiplier>();
			int num = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.SkillTier>();
			int value3 = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.CargoModifier>();
			BigInteger totalValueIncrease = ValueModifier.GetTotalValueIncrease();
			int value4 = 5;
			int value5 = UnityEngine.Random.Range(1, Mathf.Min(1 + num, 4));
			BigInteger right = BigInteger.Pow(value4, currentDWLevel);
			BigInteger cashAmount = totalValueIncrease * value * value2 * value3 * value5 * right;
			this.CashAmount = cashAmount;
		}
	}

	[HideInInspector]
	public BigInteger CashAmount = -1;
}
