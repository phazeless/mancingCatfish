using System;
using System.Numerics;
using UnityEngine;

public class FireworkSpawner : MonoBehaviour
{
	public void Spawn()
	{
		bool flag = false;
		float currentTotalValueFor = SkillManager.Instance.GetCurrentTotalValueFor<Skills.FireworkAmericanoFishChance>();
		FishBehaviour fishBehaviour;
		if (FHelper.DidRollWithChance(currentTotalValueFor))
		{
			fishBehaviour = FishPoolManager.Instance.GetFish(FishBehaviour.FishType.Special12, out flag);
			FishBehaviour fishPrefabAtDW = FishPoolManager.Instance.GetFishPrefabAtDW(FishSpawnHelper.GetRandomFishDWLvl());
			BigInteger overrideValue = fishPrefabAtDW.FishInfo.BaseValue.Value * (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.AmericanoCurrentDWFishMultiplier>();
			fishBehaviour.OverrideValue = overrideValue;
		}
		else
		{
			fishBehaviour = FishPoolManager.Instance.GetFishAtDW(FishSpawnHelper.GetRandomFishDWLvl(), out flag);
		}
		fishBehaviour.transform.position = base.transform.position + new UnityEngine.Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
		fishBehaviour.ActivateSwimming(base.transform);
		fishBehaviour.OnCaught(1f);
		fishBehaviour.OnCollected(fishBehaviour.transform.position);
		BaseCatcher.NotifyFishCollected(fishBehaviour);
	}
}
