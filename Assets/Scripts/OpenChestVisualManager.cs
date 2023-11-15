using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class OpenChestVisualManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnChestOpeningFinished;

	private void Awake()
	{
		OpenChestVisualManager.Instance = this;
	}

	private void Start()
	{
		ChestManager.Instance.OnChestOpened += this.Instance_OnChestOpened;
	}

	private void Instance_OnChestOpened(ItemChest chest, List<Item> items)
	{
		ItemBoxTweener itemBoxTweener = UnityEngine.Object.Instantiate<ItemBoxTweener>(this.chests[chest.Tier], this.canvasTarget);
		itemBoxTweener.Setup(chest, items);
	}

	public void ChestOpeningFinished()
	{
		if (this.OnChestOpeningFinished != null)
		{
			this.OnChestOpeningFinished();
		}
	}

	public static OpenChestVisualManager Instance;

	[SerializeField]
	private ItemBoxTweener[] chests;

	[SerializeField]
	private Transform canvasTarget;
}
