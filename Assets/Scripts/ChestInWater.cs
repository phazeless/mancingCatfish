using System;
using UnityEngine;

public class ChestInWater : StuffInWaterBase
{
	private void Start()
	{
		ChestSpawnSettings chestSpawnSettings = (ChestSpawnSettings)this.spawnSettings;
		int tier = chestSpawnSettings.Chest.Tier;
	}

	protected override void OnSwiped()
	{
		ChestSpawnSettings settings = (ChestSpawnSettings)this.spawnSettings;
		ChestManager.Instance.CreateChestFromSettings(settings);
		AudioManager.Instance.PickupStuffFromWater();
		Transform transform = UnityEngine.Object.Instantiate<Transform>(this.pickupSplash, base.transform.root, false);
		transform.position = base.transform.position;
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
