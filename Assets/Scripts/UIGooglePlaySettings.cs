using System;
using UnityEngine;

public class UIGooglePlaySettings : UpgradeDialogTween
{
	public void Open(Action<GooglePlayGamesChoice> onChoiceMade)
	{
		base.Open();
		this.choice = GooglePlayGamesChoice.None;
		this.OnChoiceMade = onChoiceMade;
	}

	public void SetUi(bool isConfirmation)
	{
		this.isConfirmation = isConfirmation;
		foreach (GameObject gameObject in this.confirmation)
		{
			gameObject.SetActive(isConfirmation);
		}
		foreach (GameObject gameObject2 in this.decision)
		{
			gameObject2.SetActive(!isConfirmation);
		}
	}

	public void ConfirmDisconnection()
	{
		this.choice = GooglePlayGamesChoice.ConfirmDisconnect;
		this.Close(false);
	}

	public void LeaveAndLoadPreviousProgress()
	{
		this.choice = GooglePlayGamesChoice.LeaveAndLoadPreviousProgress;
		this.Close(false);
	}

	public void StayAndSaveCurrentProgress()
	{
		this.choice = GooglePlayGamesChoice.StayAndSaveCurrentProgress;
		this.Close(false);
	}

	public override void Close(bool destroyOnFinish = false)
	{
		base.Close(destroyOnFinish);
		if (this.OnChoiceMade != null)
		{
			this.OnChoiceMade(this.choice);
		}
	}

	[SerializeField]
	private GameObject[] confirmation;

	[SerializeField]
	private GameObject[] decision;

	private Action<GooglePlayGamesChoice> OnChoiceMade;

	private GooglePlayGamesChoice choice = GooglePlayGamesChoice.None;

	private bool isConfirmation = true;
}
