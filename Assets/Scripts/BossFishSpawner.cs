using System;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;

[Serializable]
public class BossFishSpawner : BaseFishSpawner
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action onBossSpawned;

	protected override void Awake()
	{
		base.Awake();
		BossFishSpawner.Instance = this;
		ResourceManager.Instance.OnResourceChanged += this.ResourceManager_OnResourceChanged;
		SkillManager.Instance.DeepWaterSkill.OnSkillLevelUp += this.DeepWaterSkill_OnSkillLevelUp;
		SkillManager.Instance.OnSkillsReset += this.Instance_OnSkillsReset;
		AFKManager.Instance.OnUserLeaveCallback += this.Instance_OnUserLeaveCallback;
		AFKManager.Instance.OnUserReturnCallback += this.Instance_OnUserReturnCallback;
	}

	private void Instance_OnSkillsReset()
	{
	}

	private void Instance_OnUserReturnCallback(bool arg1, DateTime arg2, float arg3)
	{
		this.hasSpawnedBossFish = (EncryptedPlayerPrefs.GetInt(BossFishSpawner.KEY_HAS_SPAWNED_BOSS_FISH, 0) == 1);
	}

	private void Instance_OnUserLeaveCallback(DateTime arg1, bool didApplicationQuit)
	{
		if (didApplicationQuit)
		{
			return;
		}
		EncryptedPlayerPrefs.SetInt(BossFishSpawner.KEY_HAS_SPAWNED_BOSS_FISH, (!this.hasSpawnedBossFish) ? 0 : 1, true);
	}

	private void DeepWaterSkill_OnSkillLevelUp(Skill skill, LevelChange levelChange)
	{
		if (levelChange == LevelChange.LevelUp)
		{
			this.hasSpawnedBossFish = false;
		}
	}

	private void ResourceManager_OnResourceChanged(ResourceType resourceType, BigInteger amount, BigInteger totalAmount)
	{
		bool flag = TournamentManager.Instance != null && TournamentManager.Instance.IsInsideTournament;
		if (resourceType == ResourceType.Cash && !flag)
		{
			BigInteger costForNextLevelUp = SkillManager.Instance.DeepWaterSkill.CostForNextLevelUp;
			BigInteger right = costForNextLevelUp.MultiplyFloat(this.requiredPercentUntilSpawn);
			if (!this.hasSpawnedBossFish && totalAmount >= right && !SwimStraight.isSimulatingBoatMovement && !CameraMovement.bossTime)
			{
				this.Spawn();
			}
		}
	}

	public override void Spawn()
	{
		this.hasSpawnedBossFish = true;
		CameraMovement.Instance.BossTimeZoomStart();
		AudioManager.Instance.BossTimeStart();
		ColorChangerHandler.BossModeColorStart();
		AudioManager.Instance.BossStormAudio();
		if (this.onBossSpawned != null)
		{
			this.onBossSpawned();
		}
		this.RunAfterDelay(0.5f, delegate()
		{
			base.Spawn();
		});
	}

	protected override void OnFishSpawned(FishBehaviour fish)
	{
		((BossFishAttributes)fish).Reset();
		fish.transform.rotation = base.transform.rotation;
		fish.transform.position = base.transform.position;
		fish.ActivateSwimming(base.transform);
	}

	protected override void OnReset(FishBehaviour fish)
	{
		this.hasSpawnedBossFish = false;
	}

	protected override FishBehaviour GetFishToSpawn(out bool didCreate)
	{
		return FishPoolManager.Instance.GetBossFishAtCurrentDW(out didCreate);
	}

	private static string KEY_HAS_SPAWNED_BOSS_FISH = "KEY_HAS_SPAWNED_BOSS_FISH";

	public static BossFishSpawner Instance;

	[SerializeField]
	private float requiredPercentUntilSpawn = 1f;

	private bool hasSpawnedBossFish;
}
