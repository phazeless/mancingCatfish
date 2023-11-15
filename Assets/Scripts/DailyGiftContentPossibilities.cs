using System;

[Serializable]
public class DailyGiftContentPossibilities
{
	public int AfterDayStreak;

	public ItemChest Chest;

	public Skill CrewMember;

	public Item Item;

	public FishBehaviour Fish;

	public int DiffItems = 1;

	public int MinItems;

	public int MaxItems;

	public bool IncludeHolidayItems;

	public int MinGems;

	public int MaxGems;

	public int CrownExp;

	public int ChanceForFreeSpin;

	public int FishingExpProcentOfCurrent;

	public BaseGrantable Grantable;

	public DailyGiftVisualContent Visuals;
}
