using System;
using UnityEngine;

public class GemGainVisual : MonoBehaviour
{
	private void Awake()
	{
		GemGainVisual.Instance = this;
	}

	public void GainGems(int amount, Vector2 startFrom, ResourceChangeData gemChangeData)
	{
		GemVisualSpawner gemVisualSpawner = UnityEngine.Object.Instantiate<GemVisualSpawner>(this.gemVisualSpawner, this.gemVisualSpawner.transform, false);
		gemVisualSpawner.transform.position = startFrom;
		gemVisualSpawner.Spawn(amount, this.targetPosition, gemChangeData);
		AudioManager.Instance.GemSpawn();
	}

	public static GemGainVisual Instance;

	[SerializeField]
	private GemVisualSpawner gemVisualSpawner;

	[SerializeField]
	private Transform targetPosition;
}
