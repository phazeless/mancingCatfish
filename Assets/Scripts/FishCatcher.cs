using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class FishCatcher : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<Action<FishBehaviour>, Queue<FishCatcher.FishProps>> OnFishToBeCollected;

	private void Awake()
	{
		this.txtCapacity.SetVariableText(new string[]
		{
			this.caughtFishes.Count.ToString(),
			this.maxCapacity.ToString()
		});
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (this.caughtFishes.Count >= this.maxCapacity)
		{
			return;
		}
		FishBehaviour componentInParent = collider.GetComponentInParent<FishBehaviour>();
		componentInParent.OnCaught(1f);
		this.caughtFishes.Enqueue(new FishCatcher.FishProps(componentInParent.DeepWaterLvl, componentInParent.FishInfo.Rarity));
		this.txtCapacity.SetVariableText(new string[]
		{
			this.caughtFishes.Count.ToString(),
			this.maxCapacity.ToString()
		});
		if (this.autoCollect)
		{
			this.OnCollectFishClick();
		}
	}

	public void OnCollectFishClick()
	{
		if (!this.collider2D.enabled)
		{
			return;
		}
		if (FishCatcher.OnFishToBeCollected != null)
		{
			FishCatcher.OnFishToBeCollected(new Action<FishBehaviour>(this.OnFishCollected), this.caughtFishes);
		}
		if (!this.autoCollect)
		{
			this.collider2D.enabled = false;
		}
	}

	private void OnFishCollected(FishBehaviour fish)
	{
		this.txtCapacity.SetVariableText(new string[]
		{
			this.caughtFishes.Count.ToString(),
			this.maxCapacity.ToString()
		});
		float num = UnityEngine.Random.Range(-0.3f, 0.3f);
		float num2 = UnityEngine.Random.Range(-0.3f, 0.3f);
		Vector3 position = base.transform.position;
		position.x += num;
		position.y += num2;
		fish.OnCollected(position);
		if (this.caughtFishes.Count == 0)
		{
			this.collider2D.enabled = true;
		}
	}

	[SerializeField]
	private TextMeshPro txtCapacity;

	[SerializeField]
	private Collider2D collider2D;

	[SerializeField]
	private int maxCapacity = 1;

	[SerializeField]
	private bool autoCollect = true;

	private Queue<FishCatcher.FishProps> caughtFishes = new Queue<FishCatcher.FishProps>();

	public class FishProps
	{
		public FishProps(int deepWaterLvl, int rarityIndex)
		{
			this.DeepWaterLvl = deepWaterLvl;
			this.RarityIndex = rarityIndex;
		}

		public int DeepWaterLvl;

		public int RarityIndex;
	}
}
