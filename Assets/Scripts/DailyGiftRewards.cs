using System;
using System.Collections.Generic;
using System.Linq;
using FullInspector;
using UnityEngine;

public class DailyGiftRewards : BaseBehavior
{
	public List<int> AvailableStreaks
	{
		get
		{
			return (from x in this.dailyGiftContentPossibilities
			select x.AfterDayStreak).Distinct<int>().ToList<int>();
		}
	}

	public List<int> AvailableStreaksOrderedAsc
	{
		get
		{
			return (from x in this.AvailableStreaks
			orderby x
			select x).ToList<int>();
		}
	}

	public DailyGiftContent GetDailyGiftContent(int currentStreak)
	{
		return this.GetDailyGiftContent(this.GetDailyGiftPossibilitiesForStreak(currentStreak));
	}

	public DailyGiftContentPossibilities GetDailyGiftPossibilitiesForStreak(int currentStreak)
	{
		if (currentStreak == 1)
		{
			return this.dailyGiftContentPossibilities.Find((DailyGiftContentPossibilities x) => x.AfterDayStreak == 1);
		}
		return (from x in this.dailyGiftContentPossibilities
		orderby x.AfterDayStreak descending
		select x).ToList<DailyGiftContentPossibilities>().Find((DailyGiftContentPossibilities x) => currentStreak % x.AfterDayStreak == 0);
	}

	private DailyGiftContent GetDailyGiftContent(DailyGiftContentPossibilities reward)
	{
		DailyGiftContent dailyGiftContent = new DailyGiftContent();
		int gems = UnityEngine.Random.Range(reward.MinGems, reward.MaxGems + 1);
		bool flag = UnityEngine.Random.Range(0, 100) < reward.ChanceForFreeSpin;
		dailyGiftContent.Chest = reward.Chest;
		dailyGiftContent.CrewMember = reward.CrewMember;
		dailyGiftContent.Item = reward.Item;
		dailyGiftContent.Fish = reward.Fish;
		dailyGiftContent.FreeSpins = ((!flag) ? 0 : 1);
		if (reward.FishingExpProcentOfCurrent > 0)
		{
			float num = (float)reward.FishingExpProcentOfCurrent / 100f;
			float num2 = num * (float)SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong;
			int num3 = (num2 < 2.14748365E+09f) ? ((int)num2) : int.MaxValue;
			dailyGiftContent.FishingExp = 10 + num3;
		}
		for (int i = 0; i < reward.DiffItems; i++)
		{
			int num4 = UnityEngine.Random.Range(reward.MinItems, reward.MaxItems + 1);
			if (num4 > 0)
			{
				Item[] exceptItems = dailyGiftContent.Items.Keys.ToArray<Item>();
				Item randomItem = this.GetRandomItem(reward, exceptItems);
				dailyGiftContent.Items.Add(randomItem, num4);
			}
		}
		dailyGiftContent.Gems = gems;
		if (SkillTreeManager.Instance.IsSkillTreeEnabled)
		{
			dailyGiftContent.CrownExp = reward.CrownExp;
		}
		bool flag2 = reward.Grantable != null;
		bool flag3 = flag2 && reward.Grantable is GrantableConsumable;
		bool flag4 = flag3 && ((GrantableConsumable)reward.Grantable).Consumable == FireworkFishingManager.Instance.Firework;
		if (flag4)
		{
			bool flag5 = TimeManager.Instance.IsWithinPeriod(FireworkFishingManager.Instance.Availability);
			if (flag5)
			{
				dailyGiftContent.Grantable = reward.Grantable;
			}
		}
		return dailyGiftContent;
	}

	private Item GetRandomItem(DailyGiftContentPossibilities contentPossibilities, params Item[] exceptItems)
	{
		List<Item> list = ItemManager.Instance.GetItemsWith(Rarity.Common, contentPossibilities.IncludeHolidayItems).Except(exceptItems).ToList<Item>();
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public int GetRequiredStreakForReward(int currentStreak, int index, int fromIndex, int toIndex)
	{
		int num = toIndex - fromIndex;
		List<int> list = (from x in this.AvailableStreaks
		orderby x
		select x).ToList<int>();
		list = list.GetRange(fromIndex, toIndex);
		int num2 = list[num];
		int num3 = currentStreak / num2;
		int num4 = (index != 0) ? ((index != 2) ? (num / 2) : num) : 0;
		return (num3 * num2 / list[index] + 1) * list[index];
	}

	[SerializeField]
	private List<DailyGiftContentPossibilities> dailyGiftContentPossibilities = new List<DailyGiftContentPossibilities>();
}
