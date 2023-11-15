using System;

[Serializable]
public class OneFishSpawner : BaseFishSpawner
{
	private void Update()
	{
		if (!base.IsReady)
		{
			return;
		}
		float seconds = 1f / SkillManager.Instance.GetCurrentTotalValueFor(this.fishPerSecond);
		if (FHelper.HasSecondsPassed(seconds, ref this.timer, true))
		{
			this.Spawn();
		}
	}

	protected override void OnFishSpawned(FishBehaviour fish)
	{
		fish.transform.rotation = base.transform.rotation;
		fish.transform.position = base.transform.position;
		fish.ActivateSwimming(base.transform);
	}

	protected override void OnFishCreated(FishBehaviour fish)
	{
	}

	protected override void OnReset(FishBehaviour fish)
	{
	}

	protected override FishBehaviour GetFishToSpawn(out bool didCreate)
	{
		return FishPoolManager.Instance.GetFishAtCurrentDW(out didCreate);
	}

	private float timer = 4f;
}
