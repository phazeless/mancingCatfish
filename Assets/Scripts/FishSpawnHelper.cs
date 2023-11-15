using System;
using System.Collections.Generic;
using UnityEngine;

public static class FishSpawnHelper
{
	public static int GetRandomFishDWLvl()
	{
		int num = (int)(SkillManager.Instance.GetCurrentTotalValueFor<Skills.Rod_ChanceForRareFish>() + 100f);
		bool flag = FishSpawnHelper.previousChance != num;
		bool flag2 = DWHelper.CurrentDWLevel != FishSpawnHelper.previousDWLvl;
		if (flag || flag2)
		{
			FishSpawnHelper.fishChances.Clear();
			for (int i = DWHelper.CurrentDWLevel; i >= 0; i--)
			{
				int a = num / (i + 1);
				int num2 = Mathf.Min(a, 100 - FishSpawnHelper.fishChances.Count);
				for (int j = 0; j < num2; j++)
				{
					FishSpawnHelper.fishChances.Add(i);
				}
			}
		}
		FishSpawnHelper.previousChance = num;
		FishSpawnHelper.previousDWLvl = DWHelper.CurrentDWLevel;
		return FishSpawnHelper.fishChances[UnityEngine.Random.Range(0, FishSpawnHelper.fishChances.Count)];
	}

	private static List<int> fishChances = new List<int>();

	private static int previousChance = 0;

	private static int previousDWLvl = 0;
}
