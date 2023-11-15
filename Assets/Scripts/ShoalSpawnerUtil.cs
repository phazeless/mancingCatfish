using System;
using UnityEngine;

public class ShoalSpawnerUtil : MonoBehaviour
{
	public static ShoalSpawnerUtil Instance { get; private set; }

	private void Awake()
	{
		ShoalSpawnerUtil.Instance = this;
	}

	public StimSpawner SpawnShoal()
	{
		StimSpawner stimSpawner = this.CreateShoalSpawner();
		stimSpawner.Activate((int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.ShoalAmount>());
		return stimSpawner;
	}

	private StimSpawner CreateShoalSpawner()
	{
		StimSpawner stimSpawner = UnityEngine.Object.Instantiate<StimSpawner>(this.shoalSpawnerPrefab);
		stimSpawner.Init(this.shoalPositions);
		return stimSpawner;
	}

	[SerializeField]
	private StimSpawner shoalSpawnerPrefab;

	[SerializeField]
	private Transform shoalPositions;
}
