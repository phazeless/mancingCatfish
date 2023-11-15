using System;
using UnityEngine;

[Serializable]
public class PlaySoundOnLevelUp : ILevelUpListener
{
	public void OnLevelUp(GameObject caller, Skill skill)
	{
		AudioManager.Instance.PlaySpecial(this.clip);
	}

	[SerializeField]
	private AudioClip clip;
}
