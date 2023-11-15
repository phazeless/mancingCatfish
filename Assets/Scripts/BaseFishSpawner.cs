using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

public abstract class BaseFishSpawner : BaseBehavior, IFishSpawner
{
	public virtual void Spawn()
	{
		bool flag = false;
		FishBehaviour fishToSpawn = this.GetFishToSpawn(out flag);
		if (flag)
		{
			this.OnFishCreated(fishToSpawn);
		}
		this.ClearAndAddSpawnerSwimBehaviours(fishToSpawn);
		this.OnFishSpawned(fishToSpawn);
		fishToSpawn.GetSwimBehaviourMono().Start();
	}

	protected abstract FishBehaviour GetFishToSpawn(out bool didCreate);

	protected abstract void OnFishSpawned(FishBehaviour fish);

	protected abstract void OnReset(FishBehaviour fish);

	public void SetDeepWaterFishes(DeepWaterFishes fishesAtDeepWater)
	{
		FishBehaviour fish = null;
		if (this.spawnFishType == BaseFishSpawner.SpawnFishType.Main)
		{
			fish = fishesAtDeepWater.MainFish;
		}
		if (this.spawnFishType == BaseFishSpawner.SpawnFishType.Boss)
		{
			fish = fishesAtDeepWater.BossFish;
		}
		this.OnReset(fish);
	}

	protected bool IsReady
	{
		get
		{
			return true;
		}
	}

	private FishBehaviour InstantiateCallback(object prefab)
	{
		FishBehaviour component = UnityEngine.Object.Instantiate<GameObject>((GameObject)prefab, this.fishesParent).GetComponent<FishBehaviour>();
		this.OnFishCreated(component);
		return component;
	}

	public void SetSpawnedFishParent(Transform parent)
	{
		this.fishesParent = parent;
	}

	protected virtual void OnFishCreated(FishBehaviour fish)
	{
		this.ClearAndAddSpawnerSwimBehaviours(fish);
	}

	private void ClearAndAddSpawnerSwimBehaviours(FishBehaviour fish)
	{
		fish.GetSwimBehaviourMono().ClearSwimBehaviours();
		foreach (ISwimBehaviour swimBehaviour in this.swimBehaviours)
		{
			fish.GetSwimBehaviourMono().AddSwimBehaviour(swimBehaviour);
		}
	}

	[SerializeField]
	protected Skills.Spawner fishPerSecond = new Skills.CommonFishPerSecond();

	[SerializeField]
	private BaseFishSpawner.SpawnFishType spawnFishType;

	[SerializeField]
	protected List<ISwimBehaviour> swimBehaviours = new List<ISwimBehaviour>();

	private Transform fishesParent;

	public enum SpawnFishType
	{
		Main,
		Boss
	}
}
