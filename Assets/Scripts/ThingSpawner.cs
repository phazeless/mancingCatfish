using System;
using System.Collections.Generic;
using UnityEngine;

public class ThingSpawner : MonoBehaviour
{
	private void Start()
	{
		this.fishAdManager = FishAdManager.Instance;
	}

	private void Update()
	{
		if (this.fishAdManager != null && !this.fishAdManager.IsAdActive)
		{
			for (int i = 0; i < this.spawnSettings.Count; i++)
			{
				SpawnSettings spawnSettings = this.spawnSettings[i];
				if (spawnSettings.ShouldSpawn() && !this.hasRecentlyComeBack)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(spawnSettings.GetPrefabToSpawn(), base.transform, false);
					gameObject.GetComponent<IHasSpawnSettings>().SetSpawnSettings(spawnSettings);
					gameObject.transform.position = this.GetRandomTopCoords();
					gameObject.AddComponent<SwimBehaviourMono>().AddSwimBehaviours(spawnSettings.GetSwimBehaviours());
				}
			}
		}
	}

	private Vector2 GetRandomTopCoords()
	{
		return this.mainCamera.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.15f, 0.85f), 1.01f, 90f));
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			this.hasRecentlyComeBack = true;
			this.RunAfterDelay(1f, delegate()
			{
				this.hasRecentlyComeBack = false;
			});
		}
	}

	[SerializeField]
	private List<SpawnSettings> spawnSettings = new List<SpawnSettings>();

	[SerializeField]
	private Camera mainCamera;

	private FishAdManager fishAdManager;

	private bool hasRecentlyComeBack;
}
