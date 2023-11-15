using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentTabs : MonoBehaviour
{
	public void SetContent(bool isInfo)
	{
		if (isInfo)
		{
			this.infoTabBg.color = this.activeBGColor;
			this.infoBody.color = this.activeBodyColor;
			this.infoLabel.color = this.activeLabelColor;
			this.scoreTabBg.color = this.inactiveBGColor;
			this.scoreBody.color = this.inactiveBodyColor;
			this.scoreLabel.color = this.inactiveLabelColor;
			this.infoContentHolder.SetActive(true);
			this.highscoreContent.SetActive(false);
			this.scoreTab.transform.SetAsFirstSibling();
		}
		else
		{
			this.infoTabBg.color = this.inactiveBGColor;
			this.infoBody.color = this.inactiveBodyColor;
			this.infoLabel.color = this.inactiveLabelColor;
			this.scoreTabBg.color = this.activeBGColor;
			this.scoreBody.color = this.activeBodyColor;
			this.scoreLabel.color = this.activeLabelColor;
			this.infoContentHolder.SetActive(false);
			this.highscoreContent.SetActive(true);
			this.infoTab.transform.SetAsFirstSibling();
		}
	}

	[SerializeField]
	private GameObject infoContentHolder;

	[SerializeField]
	private GameObject highscoreContent;

	[SerializeField]
	private GameObject infoTab;

	[SerializeField]
	private GameObject scoreTab;

	[SerializeField]
	private Image infoTabBg;

	[SerializeField]
	private Image infoBody;

	[SerializeField]
	private TextMeshProUGUI infoLabel;

	[SerializeField]
	private Image scoreTabBg;

	[SerializeField]
	private Image scoreBody;

	[SerializeField]
	private TextMeshProUGUI scoreLabel;

	private Color activeBGColor = new Color(0.075f, 0.663f, 0.651f);

	private Color activeBodyColor = new Color(0.145f, 0.706f, 0.694f);

	private Color activeLabelColor = new Color(0.6f, 0.867f, 0.863f);

	private Color inactiveBGColor = new Color(0f, 0.565f, 0.541f);

	private Color inactiveBodyColor = new Color(0f, 0.604f, 0.576f);

	private Color inactiveLabelColor = new Color(0.012f, 0.42f, 0.4f);
}
