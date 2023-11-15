using System;
using System.Numerics;

public static class ValueModifier
{
	public static BigInteger GetTotalValueIncrease()
	{
		BigInteger left = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.AllIncome>();
		BigInteger bigInteger = SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong * (long)SkillManager.Instance.CollectStarsSkill.CurrentLevel;
		int currentLevel = SkillManager.Instance.FishValueBonusSkill.CurrentLevel;
		int value = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.FishValueMultiplier>();
		int value2 = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.SkillTier>();
		int value3 = (!FishValueFishHandler.Instance.IsBoostActive) ? 1 : ((int)ItemAndSkillValues.GetCurrentTotalValueFor<Skills.FishValueFishMultiplier>());
		BigInteger right = 1 + bigInteger.MultiplyFloat(0.01f);
		BigInteger bigInteger2 = left * right * currentLevel * value * value2 * value3;
		return bigInteger2.MultiplyFloat(ValueModifier.GetHolidayFishValueMultipliers());
	}

	private static float GetHolidayFishValueMultipliers()
	{
		float num = 1f;
		bool flag = DateTime.Now.Month == 12 || DateTime.Now.Month == 4;
		bool flag2 = DateTime.Now.Month == 12 || DateTime.Now.Month == 1 || DateTime.Now.Month == 2;
		if (flag)
		{
			num *= SkillManager.Instance.GetCurrentTotalValueFor<Skills.HolidayFishValueMultiplier>();
		}
		if (flag2)
		{
			num *= SkillManager.Instance.GetCurrentTotalValueFor<Skills.WinterFishValueMultiplier>();
		}
		return num;
	}
}
