using System;
using UnityEngine;

public class EasterEgg_TapToUnlock : BaseEasterEgg
{
	private void Start()
	{
		if (this.tapsToUnlock == 0)
		{
			this.ActivateVisualHolder();
		}
	}

	public void IncreaseTapCounter()
	{
		this.counter++;
		if (this.counter == this.tapsToUnlock)
		{
			this.ActivateVisualHolder();
		}
	}

	public override void OnEggCollected()
	{
		base.gameObject.SetActive(false);
	}

	[SerializeField]
	private int tapsToUnlock;

	private int counter;
}
