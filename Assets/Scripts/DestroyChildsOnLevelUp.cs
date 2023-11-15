using System;
using UnityEngine;

[Serializable]
public class DestroyChildsOnLevelUp : ILevelUpListener
{
	public void OnLevelUp(GameObject caller, Skill skill)
	{
		for (int i = 0; i < caller.transform.childCount; i++)
		{
			UnityEngine.Object.Destroy(caller.transform.GetChild(0).gameObject);
		}
	}
}
