using System;
using UnityEngine;

[Serializable]
public class DestroyOnLevelUp : ILevelUpListener
{
	public void OnLevelUp(GameObject caller, Skill skill)
	{
		foreach (GameObject obj in this.gameObjects)
		{
			UnityEngine.Object.Destroy(obj);
		}
	}

	[SerializeField]
	private GameObject[] gameObjects;
}
