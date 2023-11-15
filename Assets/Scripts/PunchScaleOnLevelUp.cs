using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class PunchScaleOnLevelUp : ILevelUpListener
{
	public void OnLevelUp(GameObject caller, Skill skill)
	{
		for (int i = 0; i < this.transforms.Length; i++)
		{
			this.startingScale = this.transforms[i].localScale;
			this.transforms[i].DOScale(new Vector3(this.startingScale.x + 0.1f, this.startingScale.y + 0.1f, 1f), 0.2f);
		}
	}

	[SerializeField]
	private Transform[] transforms;

	private Vector3 startingScale;
}
