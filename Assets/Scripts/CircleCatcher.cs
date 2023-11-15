using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class CircleCatcher : BaseCatcher
{
	protected virtual void Start()
	{
		this.col2D = base.GetComponent<CircleCollider2D>();
	}

	protected override FishBehaviour OnCheckForCatch(bool isSimulation)
	{
		return this.caughtFish;
	}

	protected override void OnFishCaught(FishBehaviour fish)
	{
		fish.OnCaught(1f);
		fish.OnCollected(base.transform.position);
		this.caughtFish = null;
	}

	private void OnFishSwiped(FishBehaviour swipedFish)
	{
		swipedFish.OnSwiped(this);
		if (swipedFish.IsCaught())
		{
			this.caughtFish = swipedFish;
		}
	}

	protected virtual void OnTriggerEnter2D(Collider2D collider)
	{
		FishBehaviour componentInParent = collider.GetComponentInParent<FishBehaviour>();
		if (componentInParent == null)
		{
			return;
		}
		this.OnFishSwiped(componentInParent);
	}

	protected FishBehaviour caughtFish;

	protected FishBehaviour swipedFish;

	protected CircleCollider2D col2D;
}
