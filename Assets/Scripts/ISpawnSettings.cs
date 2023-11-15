using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnSettings
{
	GameObject GetPrefabToSpawn();

	InGameNotification GetIGN();

	float GetSpawnFrequencyCheckInSeconds();

	float GetChanceForSpawn();

	float GetSpeed();

	bool ShouldSpawn();

	List<ISwimBehaviour> GetSwimBehaviours();
}
