using System;
using UnityEngine;

public class GrantableCrew : BaseGrantable
{
	public Skill Crew
	{
		get
		{
			return this.crew;
		}
	}

	public override int Amount
	{
		get
		{
			return 1;
		}
	}

	public override void Grant(string contentIdForAnalytics, ResourceChangeReason resourceChangeReason)
	{
		PurchaseCrewMemberHandler.Instance.GetCrewMember(this.crew, resourceChangeReason, 0);
	}

	[SerializeField]
	private Skill crew;
}
