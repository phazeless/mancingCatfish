using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class FishPoolManager : MonoBehaviour
{
	public static FishPoolManager Instance { get; private set; }

	public List<FishBehaviour> TierFishes { get; private set; }

	public List<FishBehaviour> BossFishes { get; private set; }

	public List<FishBehaviour> SpecialFishes { get; private set; }

	public List<FishBehaviour> FishPrefabs
	{
		get
		{
			return this.fishPrefabs;
		}
	}

	private void Awake()
	{
		FishPoolManager.Instance = this;
		foreach (FishBehaviour fishBehaviour in this.fishPrefabs)
		{
			this.fishPools.Add(fishBehaviour.FishInfo.FishType, new FishPool(new Func<object, FishBehaviour>(this.InstantiateCallback), 0, fishBehaviour));
			this.parents.Add(fishBehaviour.FishInfo.FishType, new GameObject(fishBehaviour.name + "-Fishes").transform);
		}
		this.waterEffectParent = new GameObject("Pool-WaterEffects").transform;
		this.waterEffectPool = new WaterEffectPool(new Func<object, WaterEffect>(this.InstantiateCallbackWaterEffect), 10, this.waterEffectPrefab);
		this.TierFishes = this.fishPrefabs.FindAll((FishBehaviour x) => x.FishInfo.FishType.ToString().StartsWith("DW") && !x.FishInfo.FishType.ToString().EndsWith("Boss"));
		this.BossFishes = this.fishPrefabs.FindAll((FishBehaviour x) => x.FishInfo.FishType.ToString().EndsWith("Boss"));
		this.SpecialFishes = this.fishPrefabs.FindAll((FishBehaviour x) => x.FishInfo.FishType.ToString().StartsWith("Special"));
	}

	private void Start()
	{
		SkillManager.Instance.PrestigeSkill.OnSkillLevelUp += this.PrestigeSkill_OnSkillLevelUp;
	}

	private void PrestigeSkill_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		foreach (KeyValuePair<FishBehaviour.FishType, FishPool> keyValuePair in this.fishPools)
		{
			keyValuePair.Value.ClearAndDestroyPoolObjects();
		}
	}

	public FishBehaviour GetFishPrefabAtDW(int dwLvl)
	{
		return this.fishPrefabs.Find((FishBehaviour x) => x.FishInfo.FishType == this.GetFishTypeAtDW(dwLvl));
	}

	public FishBehaviour GetFishAtDW(int dwLvl, out bool didCreate)
	{
		FishBehaviour.FishType? fishType = this.overrideFishType;
		FishBehaviour fish;
		if (fishType == null)
		{
			fish = this.GetFish(this.GetFishTypeAtDW(dwLvl), out didCreate);
		}
		else
		{
			FishBehaviour.FishType? fishType2 = this.overrideFishType;
			fish = this.GetFish(fishType2.Value, out didCreate);
		}
		fish.OverrideValue = this.overrideValue;
		return fish;
	}

	public FishBehaviour GetFishAtCurrentDW(out bool didCreate)
	{
		FishBehaviour.FishType? fishType = this.overrideFishType;
		FishBehaviour fish;
		if (fishType == null)
		{
			fish = this.GetFish(this.GetFishTypeAtCurrentDW(), out didCreate);
		}
		else
		{
			FishBehaviour.FishType? fishType2 = this.overrideFishType;
			fish = this.GetFish(fishType2.Value, out didCreate);
		}
		fish.OverrideValue = this.overrideValue;
		return fish;
	}

	public FishBehaviour GetBossFishAtCurrentDW(out bool didCreate)
	{
		return this.GetFish(this.GetBossFishTypeAtCurrentDW(), out didCreate);
	}

	private FishBehaviour GetFish(FishBehaviour.FishType fishType)
	{
		bool flag = false;
		return this.fishPools[fishType].GetObject(out flag);
	}

	public FishBehaviour GetFish(FishBehaviour.FishType fishType, out bool didCreate)
	{
		return this.fishPools[fishType].GetObject(out didCreate);
	}

	private void ReturnFish(FishBehaviour fish)
	{
		this.fishPools[fish.FishInfo.FishType].ReturnObject(fish);
	}

	public Sprite GetFishIcon(FishBehaviour.FishType fishType)
	{
		foreach (FishBehaviour fishBehaviour in this.fishPrefabs)
		{
			if (fishBehaviour.FishInfo.FishType == fishType)
			{
				return fishBehaviour.FishInfo.FishIcon;
			}
		}
		return null;
	}

	private FishBehaviour InstantiateCallback(object metaData)
	{
		FishBehaviour fishBehaviour = (FishBehaviour)metaData;
		return UnityEngine.Object.Instantiate<FishBehaviour>(fishBehaviour, this.parents[fishBehaviour.FishInfo.FishType], false);
	}

	private WaterEffect InstantiateCallbackWaterEffect(object metaData)
	{
		WaterEffect original = (WaterEffect)metaData;
		return UnityEngine.Object.Instantiate<WaterEffect>(original, this.waterEffectParent, false);
	}

	private string GetDWFishTypeAsString(int dwLvl)
	{
		return "DW" + dwLvl;
	}

	private FishBehaviour.FishType GetFishTypeAtDW(int dwLvl)
	{
		return (FishBehaviour.FishType)Enum.Parse(typeof(FishBehaviour.FishType), this.GetDWFishTypeAsString(dwLvl));
	}

	private FishBehaviour.FishType GetBossFishTypeAtDW(int dwLvl)
	{
		return (FishBehaviour.FishType)Enum.Parse(typeof(FishBehaviour.FishType), this.GetDWFishTypeAsString(dwLvl) + "Boss");
	}

	private FishBehaviour.FishType GetFishTypeAtCurrentDW()
	{
		int currentDWLevel = DWHelper.CurrentDWLevel;
		return this.GetFishTypeAtDW(currentDWLevel);
	}

	private FishBehaviour.FishType GetBossFishTypeAtCurrentDW()
	{
		int currentDWLevel = DWHelper.CurrentDWLevel;
		return this.GetBossFishTypeAtDW(currentDWLevel);
	}

	public void SetOverrideCurrentDWFishType(FishBehaviour.FishType? overrideFishType, BigInteger overrideValue)
	{
		this.overrideFishType = overrideFishType;
		this.overrideValue = overrideValue;
	}

	public FishBehaviour GetFishPrefab(FishBehaviour.FishType type)
	{
		foreach (FishBehaviour fishBehaviour in this.fishPrefabs)
		{
			if (fishBehaviour.FishInfo.FishType == type)
			{
				return fishBehaviour;
			}
		}
		return null;
	}

	public FishBehaviour GetFishPrefabAtCurrentDW()
	{
		FishBehaviour.FishType fishTypeAtCurrentDW = this.GetFishTypeAtCurrentDW();
		foreach (FishBehaviour fishBehaviour in this.TierFishes)
		{
			if (fishBehaviour.FishInfo.FishType == fishTypeAtCurrentDW)
			{
				return fishBehaviour;
			}
		}
		return null;
	}

	public WaterEffect GetWaterEffect()
	{
		return this.waterEffectPool.GetObject();
	}

	[SerializeField]
	private WaterEffect waterEffectPrefab;

	[SerializeField]
	private List<FishBehaviour> fishPrefabs = new List<FishBehaviour>();

	private Dictionary<FishBehaviour.FishType, FishPool> fishPools = new Dictionary<FishBehaviour.FishType, FishPool>();

	private Dictionary<FishBehaviour.FishType, Transform> parents = new Dictionary<FishBehaviour.FishType, Transform>();

	private WaterEffectPool waterEffectPool;

	private Transform waterEffectParent;

	private FishBehaviour.FishType? overrideFishType;

	private BigInteger overrideValue = 0;
}
