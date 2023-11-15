using System;
using UnityEngine;

public class IGNItemChest : InGameNotification
{
	[HideInInspector]
	public RecievedChest Chest { get; set; }

	public override bool IsClearable
	{
		get
		{
			return false;
		}
	}

	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.ItemChest;
		}
	}

	public override bool RemoveOnReset
	{
		get
		{
			return false;
		}
	}

	public override bool RemoveOnExit
	{
		get
		{
			return true;
		}
	}
}
