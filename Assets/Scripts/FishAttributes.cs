using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class FishAttributes : IListItemContent
{
	public float BiggestCatch
	{
		get
		{
			return this.biggestCatch;
		}
		set
		{
			this.biggestCatch = value;
		}
	}

	public bool IsCaught
	{
		get
		{
			return this.isCaught;
		}
		set
		{
			this.isCaught = value;
		}
	}

	public void SetPrefab(UIListItem prefab)
	{
		this.prefab = prefab;
	}

	public UIListItem GetPrefab()
	{
		return this.prefab;
	}

	public FishAttributes ShallowCopy()
	{
		return (FishAttributes)base.MemberwiseClone();
	}

	[JsonIgnore]
	public Sprite FishIcon;

	public FishBehaviour.FishType FishType = FishBehaviour.FishType.DW1;

	public string Name = string.Empty;

	public string Description = string.Empty;

	public BigIntWrapper BaseValue = new BigIntWrapper();

	public float BaseWeight = 100f;

	public int Rarity;

	public float MinSpeed;

	public float MaxSpeed;

	public int Stars;

	private float biggestCatch;

	private bool isCaught;

	private UIListItem prefab;
}
