using System;
using UnityEngine;

public class GrantableFish : BaseGrantable
{
	public override int Amount
	{
		get
		{
			return 1;
		}
	}

	public override void Grant(string contentIdForAnalytics, ResourceChangeReason resourceChangeReason)
	{
		FishBook.Instance.TryAddToBook(this.fish);
	}

	[SerializeField]
	private FishBehaviour fish;
}
