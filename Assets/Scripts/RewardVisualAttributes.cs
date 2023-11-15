using System;
using UnityEngine;

public class RewardVisualAttributes : MonoBehaviour
{
	public Color GetItemColor(int rarity)
	{
		if (this.itemColor != null && rarity < this.itemColor.Length)
		{
			return this.itemColor[rarity];
		}
		return Color.gray;
	}

	[Header("Reward Rarity Colors")]
	public Color[] rewardRarityColor;

	[Header("Reward Specifics Colors")]
	public Color gemRewardBgColor;

	public Color itemBoxBgColor;

	public Color[] itemColor;

	public Color fishExpColor;

	public Color crownExpColor;

	public Color fishColor;

	public Color freeSpinColor;

	[Header("Reward Specifics Icons")]
	public Sprite generalItemIcon;

	public Sprite gemRewardIcon;

	public Sprite fishExpIcon;

	public Sprite crownExpIcon;

	public Sprite freeSpinIcon;
}
