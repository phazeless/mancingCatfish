using System;
using System.Collections.Generic;
using UnityEngine;

public class TournamentFishSpawner : BaseFishSpawner
{
	protected override FishBehaviour GetFishToSpawn(out bool didCreate)
	{
		return FishPoolManager.Instance.GetFish(this.fishesAndChances[this.chosenFishIndex].fishType, out didCreate);
	}

	protected override void OnFishSpawned(FishBehaviour fish)
	{
		if (fish is BossFishAttributes)
		{
			((BossFishAttributes)fish).Reset();
		}
		fish.transform.rotation = base.transform.rotation;
		if (this.isHorisontalScreenArea)
		{
			fish.transform.position = new Vector3(UnityEngine.Random.Range(CameraMovement.LeftX, CameraMovement.RightX), base.transform.position.y, base.transform.position.z);
		}
		else if (this.isVerticalScreenArea)
		{
			fish.transform.position = new Vector3(base.transform.position.x, UnityEngine.Random.Range(CameraMovement.TopY, CameraMovement.BottomY), base.transform.position.z);
		}
		else if (this.isAreaOffset)
		{
			fish.transform.position = new Vector3(UnityEngine.Random.Range(base.transform.position.x - 0.5f, base.transform.position.x + 0.5f), UnityEngine.Random.Range(base.transform.position.y - 0.5f, base.transform.position.y + 0.5f), base.transform.position.z);
		}
		else
		{
			fish.transform.position = base.transform.position;
		}
		fish.ActivateSwimming(base.transform);
	}

	protected override void OnReset(FishBehaviour fish)
	{
	}

	private void Update()
	{
		if (!TournamentManager.Instance.IsInsideTournament)
		{
			return;
		}
		if (FHelper.HasSecondsPassed((float)this.trySpawnEveryXSeconds, ref this.timer, true))
		{
			for (int i = 0; i < this.fishesAndChances.Count; i++)
			{
				int num = UnityEngine.Random.Range(0, 100);
				int chance = this.fishesAndChances[i].chance;
				if (num < chance)
				{
					this.chosenFishIndex = i;
					this.Spawn();
				}
			}
		}
	}

	[SerializeField]
	private bool isHorisontalScreenArea;

	[SerializeField]
	private bool isVerticalScreenArea;

	[SerializeField]
	private bool isAreaOffset;

	[SerializeField]
	private int trySpawnEveryXSeconds = 999;

	[SerializeField]
	private List<TournamentFishSpawner.FishChance> fishesAndChances = new List<TournamentFishSpawner.FishChance>();

	private int chosenFishIndex;

	private float timer;

	[Serializable]
	public class FishChance
	{
		public FishBehaviour.FishType fishType = FishBehaviour.FishType.SpecialTournament0;

		public int chance;
	}
}
