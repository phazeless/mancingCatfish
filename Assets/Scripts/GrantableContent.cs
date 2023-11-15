using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GrantableContent
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
			return this.Items != null && this.Items.Count > 0;
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

	public bool HasConsumables
	{
		get
		{
			return this.Consumables != null && this.Consumables.Count > 0;
		}
	}

	public bool HasContent
	{
		get
		{
			return this.HasChest || this.HasCrewMember || this.HasItem || this.HasFish || this.HasItems || this.HasGems || this.HasCrownExp || this.HasFreeSpins || this.HasFishingExp || this.HasConsumables;
		}
	}

	public void Grant(string contentIdForAnalytics, ResourceChangeReason resourceChangeReason)
	{
		if (this.HasContent)
		{
			if (this.HasChest)
			{
				ChestManager.Instance.CreateReceivedChest(this.Chest);
			}
			if (this.HasCrewMember)
			{
				PurchaseCrewMemberHandler.Instance.GetCrewMember(this.CrewMember, resourceChangeReason, 0);
			}
			if (this.HasFish)
			{
				FishBook.Instance.TryAddToBook(this.Fish);
			}
			if (this.HasItem)
			{
				this.Item.ChangeItemAmount(1, resourceChangeReason);
			}
			if (this.HasItems)
			{
				foreach (KeyValuePair<Item, int> keyValuePair in this.Items)
				{
					keyValuePair.Key.ChangeItemAmount(keyValuePair.Value, resourceChangeReason);
				}
			}
			if (this.Gems > 0)
			{
				ResourceChangeData gemChangeData = new ResourceChangeData(contentIdForAnalytics, null, this.Gems, ResourceType.Gems, ResourceChangeType.Earn, resourceChangeReason);
				ResourceManager.Instance.GiveGems(this.Gems, gemChangeData);
			}
			if (this.FreeSpins > 0)
			{
				ResourceManager.Instance.GiveFreeSpin();
			}
			if (this.CrownExp > 0)
			{
				ResourceChangeData gemChangeData2 = new ResourceChangeData(contentIdForAnalytics, null, this.CrownExp, ResourceType.CrownExp, ResourceChangeType.Earn, resourceChangeReason);
				ResourceManager.Instance.GiveCrownExp(this.CrownExp, gemChangeData2);
			}
			if (this.HasConsumables)
			{
				foreach (KeyValuePair<BaseConsumable, int> keyValuePair2 in this.Consumables)
				{
					ConsumableManager.Instance.Grant(keyValuePair2.Key, keyValuePair2.Value, resourceChangeReason, false);
				}
			}
		}
	}

	public Sprite Icon;

	public ItemChest Chest;

	public Skill CrewMember;

	public Item Item;

	public FishBehaviour Fish;

	public Dictionary<Item, int> Items = new Dictionary<Item, int>();

	public int Gems;

	public int CrownExp;

	public int FreeSpins;

	public int FishingExp;

	public Dictionary<BaseConsumable, int> Consumables = new Dictionary<BaseConsumable, int>();
}
