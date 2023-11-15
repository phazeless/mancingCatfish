using System;
using UnityEngine;

public class SetItemChestVisualInWater : MonoBehaviour
{
	private void Start()
	{
		ChestSpawnSettings chestSpawnSettings = (ChestSpawnSettings)base.GetComponent<ChestInWater>().GetSpawnSettings();
		this.top.color = this.colorAlterations[chestSpawnSettings.Chest.Tier] * 0.3f + new Color(0.7f, 0.7f, 0.7f);
		this.sides.color = this.colorAlterations[chestSpawnSettings.Chest.Tier];
	}

	[SerializeField]
	private Color[] colorAlterations;

	[SerializeField]
	private SpriteRenderer top;

	[SerializeField]
	private SpriteRenderer sides;
}
