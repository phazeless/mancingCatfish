using System;
using UnityEngine;

public interface IFishSpawner
{
	void Spawn();

	void SetSpawnedFishParent(Transform parent);
}
