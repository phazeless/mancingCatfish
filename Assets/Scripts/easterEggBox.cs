using System;
using UnityEngine;
using UnityEngine.UI;

public class easterEggBox : MonoBehaviour
{
	public void SetSprite(Sprite spriteToSet)
	{
		this.sprite.sprite = spriteToSet;
	}

	[SerializeField]
	private Image sprite;
}
