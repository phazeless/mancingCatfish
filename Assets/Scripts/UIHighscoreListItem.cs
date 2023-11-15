using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHighscoreListItem : MonoBehaviour
{
	private void Awake()
	{
		this.colorOfRow = this.imageRowBG.color;
	}

	public void UpdateRow(Tournament.User user)
	{
		this.labelPlacement.SetVariableText(new string[]
		{
			(user.Placement + 1).ToString()
		});
		this.labelName.SetText(user.Username);
		this.labelScore.SetText(CashFormatter.SimpleToCashRepresentation(user.Score, 3, false, true));
		this.imageRowBG.color = ((!user.IsLocalUser) ? this.colorOfRow : this.colorWhenUser);
	}

	[SerializeField]
	private Color colorWhenUser = Color.cyan;

	[SerializeField]
	private TextMeshProUGUI labelPlacement;

	[SerializeField]
	private TextMeshProUGUI labelName;

	[SerializeField]
	private TextMeshProUGUI labelScore;

	[SerializeField]
	private Image imagePlacementBG;

	[SerializeField]
	private Image imageRowBG;

	private Color colorOfRow = Color.gray;
}
