using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPreview : MonoBehaviour
{
	public void SetPreview(Item item, int min, int max)
	{
		this.sprite.sprite = item.Icon;
		this.label.SetText(min + "-" + max);
		this.bg.color = this.rarityColors[(int)item.Rarity];
		this.label.color = this.rarityColors[(int)item.Rarity];
	}

	[SerializeField]
	private Image sprite;

	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private Image bg;

	[SerializeField]
	private Color[] rarityColors;
}
