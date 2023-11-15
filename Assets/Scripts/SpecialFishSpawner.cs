using System;
using UnityEngine;

public class SpecialFishSpawner : BaseFishSpawner
{
	protected override void Awake()
	{
		base.Awake();
		this.currentDate = DateTime.Now;
		this.specialNightFishSpawned = (EncryptedPlayerPrefs.GetInt(SpecialFishSpawner.SPECIAL_FISH_2_DID_SPAWN_KEY, 0) != 0);
		if (this.TimeBetween(DateTime.Now, this.specialNightFishSpawnEndTime.Add(new TimeSpan(0, 1, 0)), this.specialNightFishSpawnTime.Subtract(new TimeSpan(0, 1, 0))))
		{
			this.specialNightFishSpawned = false;
		}
		this.specialMorningFishSpawned = (EncryptedPlayerPrefs.GetInt(SpecialFishSpawner.SPECIAL_MORNING_FISH_DID_SPAWN_KEY, 0) != 0);
		if (this.TimeBetween(DateTime.Now, this.specialMorningFishSpawnEndTime.Add(new TimeSpan(0, 1, 0)), this.specialMorningFishSpawnTime.Subtract(new TimeSpan(0, 1, 0))))
		{
			this.specialMorningFishSpawned = false;
		}
		SkillManager.Instance.DeepWaterSkill.OnSkillLevelUp += this.DeepWaterSkill_OnSkillLevelUp;
	}

	private void DeepWaterSkill_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		if (levelChange == LevelChange.LevelUp && skill.CurrentLevel == SpecialFishSpawner.SPECIAL_FISH_3_DWLEVEL_AT_WHICH_IT_SPAWN && UnityEngine.Random.Range(0, 100) <= SpecialFishSpawner.SPECIAL_FISH_3_CHANCE_FOR_SPAWN)
		{
			this.RunAfterDelay((float)SpecialFishSpawner.SPECIAL_FISH_3_DELAY_UNTIL_SPAWN_SECONDS, delegate()
			{
				this.fishToSpawn = FishBehaviour.FishType.Special3;
				this.Spawn();
			});
		}
	}

	protected override FishBehaviour GetFishToSpawn(out bool didCreate)
	{
		return FishPoolManager.Instance.GetFish(this.fishToSpawn, out didCreate);
	}

	protected override void OnFishSpawned(FishBehaviour fish)
	{
		fish.transform.rotation = base.transform.rotation;
		if (this.overrideTransform == null)
		{
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
		}
		else
		{
			fish.transform.rotation = this.overrideTransform.rotation;
			fish.transform.position = this.overrideTransform.position;
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
			if (FHelper.HasSecondsPassed((float)SpecialFishSpawner.SPECIAL_FISH_1_CONSECUTIVE_PLAY_TIME_REQUIREMENT, ref this.specialFishConsecutiveTimer, true) && UnityEngine.Random.Range(0, 10) == 0)
			{
				this.fishToSpawn = FishBehaviour.FishType.Special1;
				this.Spawn();
			}
			if (!this.specialNightFishSpawned && this.TimeBetween(DateTime.Now, this.specialNightFishSpawnTime, this.specialNightFishSpawnEndTime))
			{
				this.RunAfterDelay(240f, delegate()
				{
					this.fishToSpawn = FishBehaviour.FishType.Special2;
					this.Spawn();
				});
				this.specialNightFishSpawned = true;
			}
			if (!this.specialMorningFishSpawned && this.TimeBetween(DateTime.Now, this.specialMorningFishSpawnTime, this.specialMorningFishSpawnEndTime))
			{
				this.RunAfterDelay(240f, delegate()
				{
					this.fishToSpawn = FishBehaviour.FishType.Special10;
					this.Spawn();
				});
				this.specialMorningFishSpawned = true;
			}
			if ((this.currentDate.Month == 12 || this.currentDate.Month == 1 || this.currentDate.Month == 2) && FHelper.HasSecondsPassed(500f, ref this.winterFishSpawnTimer, true) && FHelper.DidRollWithChance(SkillManager.Instance.GetCurrentTotalValueFor<Skills.SpawnWinterChance>()))
			{
				this.fishToSpawn = FishBehaviour.FishType.Special9;
				this.Spawn();
			}
			if (FHelper.HasSecondsPassed(80f - SkillManager.Instance.GetCurrentTotalValueFor<Skills.SpawnPinkyAfterSeconds>(), ref this.pinkyFishSpawnTimer, true) && FHelper.DidRollWithChance(SkillManager.Instance.GetCurrentTotalValueFor<Skills.SpawnPinkyChance>()))
			{
				StimSpawner stimSpawner = ShoalSpawnerUtil.Instance.SpawnShoal();
				this.overrideTransform = stimSpawner.transform;
				this.fishToSpawn = FishBehaviour.FishType.Special5;
				this.Spawn();
			}
			if (FHelper.HasSecondsPassed(40f, ref this.fishValueFishSpawnTimer, true) && FHelper.DidRollWithChance(SkillManager.Instance.GetCurrentTotalValueFor<Skills.SpawnFishValueFishChance>()))
			{
				this.fishToSpawn = FishBehaviour.FishType.Special6;
				this.Spawn();
			}
			if (SkillManager.Instance.DeepWaterSkill.CurrentLevel == 3 && FHelper.HasSecondsPassed(40f, ref this.specialHeartFishSpawnTimer, true) && FHelper.DidRollWithChance(SkillManager.Instance.GetCurrentTotalValueFor<Skills.SpawnHeartFishChance>()))
			{
				this.fishToSpawn = FishBehaviour.FishType.Special11;
				this.Spawn();
			}
		}
	}

	private bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
	{
		TimeSpan timeOfDay = datetime.TimeOfDay;
		if (start < end)
		{
			return timeOfDay >= start && timeOfDay <= end;
		}
		return !(timeOfDay > end) || !(timeOfDay < start);
	}

	private void OnApplicationPause(bool didPause)
	{
		if (didPause)
		{
			EncryptedPlayerPrefs.SetInt(SpecialFishSpawner.SPECIAL_FISH_2_DID_SPAWN_KEY, (!this.specialNightFishSpawned) ? 0 : 1, true);
			EncryptedPlayerPrefs.SetInt(SpecialFishSpawner.SPECIAL_MORNING_FISH_DID_SPAWN_KEY, (!this.specialMorningFishSpawned) ? 0 : 1, true);
		}
	}

	private static readonly int SPECIAL_FISH_1_CONSECUTIVE_PLAY_TIME_REQUIREMENT = 1800;

	private static readonly string SPECIAL_FISH_2_DID_SPAWN_KEY = "SPECIAL_FISH_2_DID_SPAWN_KEY";

	private static readonly string SPECIAL_MORNING_FISH_DID_SPAWN_KEY = "SPECIAL_MORNING_FISH_DID_SPAWN_KEY";

	private static readonly int SPECIAL_FISH_3_DWLEVEL_AT_WHICH_IT_SPAWN = 5;

	private static readonly int SPECIAL_FISH_3_DELAY_UNTIL_SPAWN_SECONDS = 2;

	private static readonly int SPECIAL_FISH_3_CHANCE_FOR_SPAWN = 20;

	[SerializeField]
	private bool isHorisontalScreenArea;

	[SerializeField]
	private bool isVerticalScreenArea;

	[SerializeField]
	private bool isAreaOffset;

	private FishBehaviour.FishType fishToSpawn = FishBehaviour.FishType.Special1;

	private float specialFishConsecutiveTimer;

	private TimeSpan specialNightFishSpawnTime = new TimeSpan(23, 0, 0);

	private TimeSpan specialNightFishSpawnEndTime = new TimeSpan(3, 0, 0);

	private TimeSpan specialMorningFishSpawnTime = new TimeSpan(5, 30, 0);

	private TimeSpan specialMorningFishSpawnEndTime = new TimeSpan(7, 30, 0);

	private bool specialNightFishSpawned;

	private bool specialMorningFishSpawned;

	private float pinkyFishSpawnTimer;

	private float fishValueFishSpawnTimer;

	private float winterFishSpawnTimer;

	private float specialHeartFishSpawnTimer;

	private DateTime currentDate;

	private Transform overrideTransform;
}
