using System;
using UnityEngine;

[Serializable]
public class MultiPurposeSpawner : BaseFishSpawner
{
	private void Update()
	{
		if (!base.IsReady)
		{
			return;
		}
		float num = 1f / SkillManager.Instance.GetCurrentTotalValueFor(this.fishPerSecond);
		if (SurfaceFishBoostHandler.Instance.IsBoostActive)
		{
			num /= 2f;
		}
		if (FHelper.HasSecondsPassed(num, ref this.timer, true))
		{
			this.Spawn();
		}
	}

	protected override void OnFishSpawned(FishBehaviour fish)
	{
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
		float currentTotalValueFor = SkillManager.Instance.GetCurrentTotalValueFor<Skills.GiantismChance>();
		if (FHelper.DidRollWithChance(currentTotalValueFor))
		{
			fish.GiantismValueMultiplier = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.GiantismValueMultiplier>();
		}
		else
		{
			fish.GiantismValueMultiplier = 1;
		}
	}

	protected override void OnReset(FishBehaviour fish)
	{
	}

	protected override FishBehaviour GetFishToSpawn(out bool didCreate)
	{
		return FishPoolManager.Instance.GetFishAtDW(FishSpawnHelper.GetRandomFishDWLvl(), out didCreate);
	}

	private float timer = 4f;

	[SerializeField]
	private bool isHorisontalScreenArea;

	[SerializeField]
	private bool isVerticalScreenArea;

	[SerializeField]
	private bool isAreaOffset;
}
