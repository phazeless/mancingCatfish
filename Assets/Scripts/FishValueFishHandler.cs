using System;
using UnityEngine;

public class FishValueFishHandler : MonoBehaviour
{
	public static FishValueFishHandler Instance { get; private set; }

	public bool IsBoostActive
	{
		get
		{
			DateTime? dateTime = this.lastActivated;
			if (dateTime == null || !this.bigFishHookItem.IsEquipped)
			{
				return false;
			}
			DateTime now = DateTime.Now;
			DateTime? dateTime2 = this.lastActivated;
			bool flag = (now - dateTime2.Value).TotalSeconds < 10.0;
			if (!flag)
			{
				this.lastActivated = null;
			}
			return flag;
		}
	}

	private void Awake()
	{
		FishValueFishHandler.Instance = this;
	}

	private void Start()
	{
		BaseCatcher.OnFishCollected += this.RodCatcher_OnFishCollected;
	}

	private void RodCatcher_OnFishCollected(FishBehaviour fish)
	{
		if (fish.FishInfo.FishType == FishBehaviour.FishType.Special6 && this.bigFishHookItem.IsEquipped)
		{
			this.lastActivated = new DateTime?(DateTime.Now);
		}
	}

	[SerializeField]
	private Item bigFishHookItem;

	private DateTime? lastActivated;
}
