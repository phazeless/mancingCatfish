using System;
using System.Collections.Generic;
using UnityEngine;

public class StimSpawner : BaseFishSpawner
{
	private int SpawnerId
	{
		get
		{
			return base.GetInstanceID();
		}
	}

	private void Start()
	{
		TournamentManager.Instance.OnJoinTournament += this.Instance_OnJoinTournament;
	}

	private void Instance_OnJoinTournament(string tournamentId)
	{
		this.joinedTournament = true;
	}

	public void Init(Transform spawnPositionsHolder)
	{
		this.spawnPositions = new List<SpawnPosition>(spawnPositionsHolder.GetComponentsInChildren<SpawnPosition>());
		ISwimBehaviour swimBehaviour = this.swimBehaviours.Find((ISwimBehaviour x) => x is SwimTowards);
		if (swimBehaviour != null)
		{
			((SwimTowards)swimBehaviour).ActualTurnAngle = UnityEngine.Random.Range(this.minMaxTurnAngle.x, this.minMaxTurnAngle.y);
			((SwimTowards)swimBehaviour).HasOverriddenTurnAngle = true;
		}
		List<SpawnPosition> list = new List<SpawnPosition>();
		if (TournamentManager.Instance.IsInsideTournament)
		{
			list = this.spawnPositions.FindAll((SpawnPosition x) => !x.DisableInTournament);
		}
		else
		{
			list = this.spawnPositions;
		}
		base.transform.position = list[UnityEngine.Random.Range(0, list.Count)].transform.position;
		Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		if (vector.x <= 1f || vector.x >= (float)Screen.width)
		{
			base.transform.LookAt2D(new Vector2(0f, base.transform.position.y));
		}
		else if (vector.y <= 1f || vector.y >= (float)Screen.height)
		{
			base.transform.LookAt2D(new Vector2(base.transform.position.x, 0f));
		}
	}

	public void Activate(int fishAmount)
	{
		this.spawnedFishesCounter = 0;
		this.fishAmount = fishAmount;
	}

	protected override void OnFishSpawned(FishBehaviour fish)
	{
		Transform parent = fish.transform.parent;
		fish.transform.SetParent(base.transform, false);
		fish.ActivateSwimming(base.transform);
		fish.ActualSpeed = fish.FishInfo.MaxSpeed;
		fish.transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), 0f, 200f);
		fish.transform.eulerAngles = base.transform.eulerAngles;
		fish.transform.SetParent(parent, true);
		fish.GiantismValueMultiplier = 1;
		fish.StimSpawnerId = this.SpawnerId;
	}

	protected override void OnReset(FishBehaviour fish)
	{
		this.spawnedFishesCounter = 0;
	}

	private void Update()
	{
		if (this.spawnedFishesCounter <= this.fishAmount && !this.joinedTournament)
		{
			if (FHelper.HasSecondsPassed(0.008f, ref this.timer, true))
			{
				if (this.spawnedFishesCounter == 0)
				{
					this.fishFromDWLvl = FishSpawnHelper.GetRandomFishDWLvl();
				}
				this.Spawn();
				this.spawnedFishesCounter++;
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		TournamentManager.Instance.OnJoinTournament -= this.Instance_OnJoinTournament;
	}

	protected override FishBehaviour GetFishToSpawn(out bool didCreate)
	{
		return FishPoolManager.Instance.GetFishAtDW(this.fishFromDWLvl, out didCreate);
	}

	private const float SPAWN_INTERVAL = 0.008f;

	private float timer;

	private int fishAmount;

	private int spawnedFishesCounter;

	private int fishFromDWLvl;

	private bool joinedTournament;

	[SerializeField]
	private Vector2 minMaxTurnAngle = new Vector2(-1f, 1f);

	private List<SpawnPosition> spawnPositions = new List<SpawnPosition>();

	private List<FishBehaviour> spawnedFishes = new List<FishBehaviour>();
}
