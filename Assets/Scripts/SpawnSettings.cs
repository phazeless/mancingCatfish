using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

public class SpawnSettings : BaseScriptableObject, ISpawnSettings
{
	public float GetChanceForSpawn()
	{
		return this.chanceForSpawn;
	}

	public InGameNotification GetIGN()
	{
		return (InGameNotification)this.ign.Clone();
	}

	public GameObject GetPrefabToSpawn()
	{
		return this.prefabToSpawn;
	}

	public float GetSpawnFrequencyCheckInSeconds()
	{
		float result = this.spawnFrequencyCheckInSeconds;
		if (this.ign is IGNPackage)
		{
			float currentTotalValueFor = ItemAndSkillValues.GetCurrentTotalValueFor<Skills.PackageSpawnModifier>();
			result = this.spawnFrequencyCheckInSeconds / currentTotalValueFor;
		}
		return result;
	}

	public float GetSpeed()
	{
		return this.speed;
	}

	public List<ISwimBehaviour> GetSwimBehaviours()
	{
		return this.swimBehaviours;
	}

	public virtual bool ShouldSpawn()
	{
		double totalSeconds = (DateTime.Now - this.lastCheck).TotalSeconds;
		if (totalSeconds > (double)this.GetSpawnFrequencyCheckInSeconds())
		{
			this.lastCheck = DateTime.Now;
			bool flag = (float)UnityEngine.Random.Range(0, 100) < this.GetChanceForSpawn();
			if (flag)
			{
				flag = ((float)InGameNotificationManager.Instance.NumberOfIGNsWithType(this.ign) < this.maxNumberInIGNList && this.minDWToSpawn <= (float)SkillManager.Instance.DeepWaterSkill.CurrentLevel);
			}
			return flag;
		}
		return false;
	}

	[SerializeField]
	private InGameNotification ign;

	[SerializeField]
	private GameObject prefabToSpawn;

	[SerializeField]
	private float speed;

	[SerializeField]
	private float spawnFrequencyCheckInSeconds;

	[SerializeField]
	private float chanceForSpawn;

	[SerializeField]
	private float maxNumberInIGNList = 10f;

	[SerializeField]
	private float minDWToSpawn;

	[SerializeField]
	private List<ISwimBehaviour> swimBehaviours = new List<ISwimBehaviour>();

	private DateTime lastCheck = DateTime.Now;
}
