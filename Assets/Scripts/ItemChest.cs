using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

public class ItemChest : BaseScriptableObject
{
	public string Id
	{
		get
		{
			return this.id;
		}
	}

	public Sprite Icon
	{
		get
		{
			return this.chestIcon;
		}
	}

	public int MinItemAmount
	{
		get
		{
			return this.minItemAmount;
		}
	}

	public int MaxItemAmount
	{
		get
		{
			return this.maxItemAmount;
		}
	}

	public int Tier
	{
		get
		{
			return this.tier;
		}
	}

	public string ChestName
	{
		get
		{
			return this.chestName;
		}
	}

	public int RareChance
	{
		get
		{
			return this.rareChance;
		}
	}

	public int MinRare
	{
		get
		{
			return this.minRare;
		}
	}

	public int MaxRare
	{
		get
		{
			return this.maxRare;
		}
	}

	public int EpicChance
	{
		get
		{
			return this.epicChance;
		}
	}

	public int MinEpic
	{
		get
		{
			return this.minEpic;
		}
	}

	public int MaxEpic
	{
		get
		{
			return this.maxEpic;
		}
	}

	public int CostToPurchase
	{
		get
		{
			return this.costToPurchase;
		}
	}

	public int GetSecondsUntilUnlocked(int secondsPassed)
	{
		return this.unlockTimeInSeconds - secondsPassed;
	}

	public int GetCostToUnlock(float secondsPassed)
	{
		return Mathf.Max(0, Mathf.CeilToInt((float)this.costToUnlockBase - (float)this.costToUnlockBase * (secondsPassed / (float)this.unlockTimeInSeconds)));
	}

	public List<Rarity> GetRandomCardRarities_NEW_VERSION()
	{
		int num = UnityEngine.Random.Range(this.minCommon, this.maxCommon + 1);
		int num2 = this.minRare;
		for (int i = this.minRare; i < this.maxRare; i++)
		{
			int num3 = UnityEngine.Random.Range(0, 101);
			if (num3 <= this.rareChance)
			{
				num2++;
			}
		}
		int num4 = this.minEpic;
		for (int j = this.minEpic; j < this.maxEpic; j++)
		{
			int num5 = UnityEngine.Random.Range(0, 101);
			if (num5 <= this.epicChance)
			{
				num4++;
			}
		}
		List<Rarity> list = new List<Rarity>();
		for (int k = 0; k < num; k++)
		{
			list.Add(Rarity.Common);
		}
		for (int l = 0; l < num2; l++)
		{
			list.Add(Rarity.Rare);
		}
		for (int m = 0; m < num4; m++)
		{
			list.Add(Rarity.Epic);
		}
		return list;
	}

	public List<Rarity> GetRandomCardRarities()
	{
		int num = UnityEngine.Random.Range(this.minItemAmount, this.maxItemAmount + 1);
		List<Rarity> list = new List<Rarity>();
		for (int i = 0; i < this.minCommon; i++)
		{
			list.Add(Rarity.Common);
		}
		for (int j = 0; j < this.minRare; j++)
		{
			list.Add(Rarity.Rare);
		}
		for (int k = 0; k < this.minEpic; k++)
		{
			list.Add(Rarity.Epic);
		}
		int num2 = num - list.Count;
		int num3 = this.minRare;
		int num4 = this.minEpic;
		for (int l = 0; l < num2; l++)
		{
			bool flag = true;
			int num5 = UnityEngine.Random.Range(0, 101);
			if (num5 <= this.rareChance)
			{
				int num6 = UnityEngine.Random.Range(0, 101);
				if (num6 <= this.epicChance)
				{
					if (num4 < this.maxEpic)
					{
						num4++;
						list.Add(Rarity.Epic);
						flag = false;
					}
				}
				else if (num3 < this.maxRare)
				{
					num3++;
					list.Add(Rarity.Rare);
					flag = false;
				}
			}
			if (flag)
			{
				list.Add(Rarity.Common);
			}
		}
		return list;
	}

	public void GenerateId(bool overrideIdEvenIfNotNull = false)
	{
		if (string.IsNullOrEmpty(this.id) || overrideIdEvenIfNotNull)
		{
			this.id = Guid.NewGuid().ToString();
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
	}

	[SerializeField]
	[ShowInInspector]
	[InspectorDisabled]
	private string id;

	[SerializeField]
	private int tier;

	[SerializeField]
	private Sprite chestIcon;

	[SerializeField]
	private string chestName;

	[SerializeField]
	private int minItemAmount;

	[SerializeField]
	private int maxItemAmount;

	[SerializeField]
	private int minCommon;

	[SerializeField]
	private int maxCommon;

	[SerializeField]
	private int minRare;

	[SerializeField]
	private int maxRare;

	[SerializeField]
	private int minEpic;

	[SerializeField]
	private int maxEpic;

	[SerializeField]
	private int rareChance;

	[SerializeField]
	private int epicChance;

	[SerializeField]
	private int costToPurchase;

	[SerializeField]
	private int costToUnlockBase;

	[SerializeField]
	private int unlockTimeInSeconds = 3600;
}
