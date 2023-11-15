using System;
using TMPro;
using UnityEngine;

public class UIItemBoxInfoDialog : UpgradeDialogTween
{
	public override void Open()
	{
		base.Open();
		for (int i = 0; i < this.infoLabels.Length; i++)
		{
			this.infoLabels[i].SetVariableText(new string[]
			{
				this.chests[i].MaxItemAmount.ToString(),
				this.chests[i].MinRare.ToString(),
				this.chests[i].MaxRare.ToString(),
				this.chests[i].MinEpic.ToString(),
				this.chests[i].MaxEpic.ToString(),
				this.chests[i].RareChance.ToString(),
				this.chests[i].EpicChance.ToString()
			});
		}
	}

	[SerializeField]
	private ItemChest[] chests;

	[SerializeField]
	private TextMeshProUGUI[] infoLabels;
}
