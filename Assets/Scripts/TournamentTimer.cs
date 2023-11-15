using System;
using TMPro;
using UnityEngine;

public class TournamentTimer : MonoBehaviour
{
	private void Update()
	{
		if (TournamentManager.Instance.IsInsideTournament)
		{
			this.timerHolder.SetActive(true);
			this.timerLabel.SetText(FHelper.FromSecondsToHoursMinutesSecondsFormat(TournamentManager.Instance.TimeLeft));
		}
		else
		{
			this.timerHolder.SetActive(false);
		}
	}

	[SerializeField]
	private GameObject timerHolder;

	[SerializeField]
	private TextMeshProUGUI timerLabel;
}
