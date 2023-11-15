using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuitDialog : UpgradeDialogTween
{
	public void Quit()
	{
		this.exitBtn.interactable = false;
		this.exitLbl.SetText("Shutting down...");
		if (TournamentManager.Instance != null && !TournamentManager.Instance.IsInsideTournament)
		{
			CloudOnceManager.Instance.SaveDataToCache();
		}
		this.RunAfterDelay(2f, delegate()
		{
			Application.Quit();
		});
	}

	[SerializeField]
	private Button exitBtn;

	[SerializeField]
	private TextMeshProUGUI exitLbl;
}
