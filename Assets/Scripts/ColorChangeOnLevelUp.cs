using System;
using UnityEngine;

[Serializable]
public class ColorChangeOnLevelUp : ILevelUpListener
{
	public void OnLevelUp(GameObject caller, Skill skill)
	{
		for (int i = 0; i < this.spriteRenderer.Length; i++)
		{
			this.spriteRenderer[i].color = this.changeToColor;
		}
	}

	[SerializeField]
	private Color changeToColor;

	[SerializeField]
	private SpriteRenderer[] spriteRenderer;
}
