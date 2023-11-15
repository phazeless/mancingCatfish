using System;
using UnityEngine;

[Serializable]
public class ActivateOnLevelUp : ILevelUpListener
{
	public void OnLevelUp(GameObject caller, Skill skill)
	{
		for (int i = 0; i < this.transforms.Length; i++)
		{
			this.transforms[i].gameObject.SetActive(true);
		}
	}

	[SerializeField]
	private Transform[] transforms;
}
