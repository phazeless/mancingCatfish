using System;
using System.Collections.Generic;
using System.Linq;
using FullInspector;
using UnityEngine;

public class BoatCatcherHolder : BaseBehavior
{
	[InspectorButton]
	public void FindAndAssignBoatCatchers()
	{
		this.boatCatchers = base.transform.GetComponentsInChildren<RodCatcher>(true).ToList<RodCatcher>();
	}

	[SerializeField]
	[InspectorDisabled]
	[ShowInInspector]
	private List<RodCatcher> boatCatchers = new List<RodCatcher>();
}
