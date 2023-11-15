using System;
using UnityEngine;

public class StuffInWaterSpawner : MonoBehaviour
{
	private void Start()
	{
		base.InvokeRepeating("TryPackageSpawn", this.packageSpawnTime, this.packageSpawnTime);
	}

	private void TryPackageSpawn()
	{
		int num = UnityEngine.Random.Range(0, 50 + this.pachageSpawnChance);
		if (num > 50)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.packagePrefab, base.transform);
		}
	}

	[SerializeField]
	private GameObject packagePrefab;

	public float packageSpawnTime = 10f;

	public int pachageSpawnChance = 50;
}
