using System;
using UnityEngine;

public class CrabCageHandler : MonoBehaviour
{
	private void Update()
	{
		float currentTotalValueFor = ItemAndSkillValues.GetCurrentTotalValueFor<Skills.SpawnCrabAfterSeconds>();
		bool flag = currentTotalValueFor != 0f;
		if (flag && FHelper.HasSecondsPassed(180f - currentTotalValueFor, ref this.timer, true))
		{
			if (InGameNotificationManager.Instance.IsAnyIGNActiveOfType<IGNCrab>())
			{
				return;
			}
			IGNCrab igncrab = new IGNCrab();
			igncrab.RanomizeContentInCage();
			InGameNotificationManager.Instance.Create<IGNCrab>(igncrab);
		}
	}

	private float timer;
}
