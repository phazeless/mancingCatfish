using System;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawnersHolder : MonoBehaviour
{
	public List<IFishSpawner> GeneralSpawners { get; private set; }

	public List<List<IFishSpawner>> DeepWaterSpawners { get; private set; }

	private void Awake()
	{
		this.GeneralSpawners = new List<IFishSpawner>(this.generalSpawners.transform.GetComponentsInChildren<IFishSpawner>(true));
		this.DeepWaterSpawners = new List<List<IFishSpawner>>();
		for (int i = 0; i < this.deepWaterSpawners.Count; i++)
		{
			this.DeepWaterSpawners.Add(new List<IFishSpawner>());
			this.DeepWaterSpawners[i].AddRange(this.deepWaterSpawners[i].transform.GetComponentsInChildren<IFishSpawner>(true));
		}
	}

	public StimSpawner CreateStimSpawner()
	{
		StimSpawner stimSpawner = UnityEngine.Object.Instantiate<StimSpawner>(this.stimSpawnerPrefab);
		stimSpawner.Init(this.spawnPositions);
		return stimSpawner;
	}

	[SerializeField]
	private Transform spawnPositions;

	[SerializeField]
	private GameObject generalSpawners;

	[SerializeField]
	private List<GameObject> deepWaterSpawners = new List<GameObject>();

	[SerializeField]
	private StimSpawner stimSpawnerPrefab;
}
