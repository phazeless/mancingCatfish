using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUtil : MonoBehaviour
{
	public static SpawnUtil Instance { get; private set; }

	private void Awake()
	{
		SpawnUtil.Instance = this;
		this.fishSpawners.AddRange(this.spawnHolder.GetComponentsInChildren<MultiPurposeSpawner>());
	}

	public void SpawnFish()
	{
		this.fishSpawners[UnityEngine.Random.Range(0, this.fishSpawners.Count)].Spawn();
	}

	[SerializeField]
	private GameObject spawnHolder;

	private List<MultiPurposeSpawner> fishSpawners = new List<MultiPurposeSpawner>();
}
