using System;
using System.Collections.Generic;

[Serializable]
public class DailyGiftContent
{
	public bool HasChest
	{
		get
		{
			return this.Chest != null;
		}
	}

	public bool HasCrewMember
	{
		get
		{
			return this.CrewMember != null;
		}
	}

	public bool HasItem
	{
		get
		{
			return this.Item != null;
		}
	}

	public bool HasFish
	{
		get
		{
			return this.Fish != null;
		}
	}

	public bool HasItems
	{
		get
		{
			return this.Items != null;
		}
	}

	public bool HasGems
	{
		get
		{
			return this.Gems > 0;
		}
	}

	public bool HasCrownExp
	{
		get
		{
			return this.CrownExp > 0;
		}
	}

	public bool HasFreeSpins
	{
		get
		{
			return this.FreeSpins > 0;
		}
	}

	public bool HasFishingExp
	{
		get
		{
			return this.FishingExp > 0;
		}
	}

	public bool HasGrantable
	{
		get
		{
			return this.Grantable != null;
		}
	}

	public bool HasContent
	{
		get
		{
			return this.HasChest || this.HasCrewMember || this.HasItem || this.HasFish || this.HasItems || this.HasGems || this.HasCrownExp || this.HasFreeSpins || this.HasFishingExp || this.HasGrantable;
		}
	}

	public ItemChest Chest;

	public Skill CrewMember;

	public Item Item;

	public FishBehaviour Fish;

	public Dictionary<Item, int> Items = new Dictionary<Item, int>();

	public int Gems;

	public int CrownExp;

	public int FreeSpins;

	public int FishingExp;

	public BaseGrantable Grantable;
}
