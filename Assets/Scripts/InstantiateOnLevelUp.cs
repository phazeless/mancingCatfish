using System;
using UnityEngine;

[Serializable]
public class InstantiateOnLevelUp : ILevelUpListener
{
	public void OnLevelUp(GameObject caller, Skill skill)
	{
		foreach (Transform original in this.transforms)
		{
			UnityEngine.Object.Instantiate<Transform>(original, caller.transform, false);
		}
	}

	[SerializeField]
	private Transform[] transforms;
}
