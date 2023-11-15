using System;
using UnityEngine;

public class UISettingDialog : UpgradeDialogTween
{
	public override void Open()
	{
		base.Open();
		this.playServicesButton.UpdateUI(CloudOnceManagerHelper.IsLoggedIntoCloudService, CloudOnceManagerHelper.HasInitializedCloudService, false);
	}

	public void GooglePlayServicesClick()
	{
		if (CloudOnceManagerHelper.IsLoggedIntoCloudService)
		{
			this.uiGooglePlaySettingsDialog.SetUi(true);
			this.uiGooglePlaySettingsDialog.Open(delegate(GooglePlayGamesChoice choice)
			{
				if (choice == GooglePlayGamesChoice.ConfirmDisconnect)
				{
					UnityEngine.Debug.Log("ABC: Did Logout!");
					CloudOnceManagerHelper.Logout();
					this.playServicesButton.UpdateUI(CloudOnceManagerHelper.IsLoggedIntoCloudService, CloudOnceManagerHelper.HasInitializedCloudService, false);
				}
			});
		}
		else
		{
			CloudOnceManagerHelper.Login(delegate(bool loginSuccess, bool hasExistingCloudData)
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"ABC: DidLoginWithSuccess: ",
					loginSuccess,
					", hasExistingCloudData: ",
					hasExistingCloudData
				}));
				if (loginSuccess && hasExistingCloudData)
				{
					this.uiGooglePlaySettingsDialog.SetUi(false);
					this.uiGooglePlaySettingsDialog.Open(delegate(GooglePlayGamesChoice choice)
					{
						if (choice == GooglePlayGamesChoice.StayAndSaveCurrentProgress)
						{
							UnityEngine.Debug.Log("ABC: Chose to stay and save data to cloud");
							if (TournamentManager.Instance != null && !TournamentManager.Instance.IsInsideTournament)
							{
								CloudOnceManager.Instance.SaveDataToCache();
								CloudOnceManager.Instance.SaveDataToCloud();
							}
						}
						else if (choice == GooglePlayGamesChoice.LeaveAndLoadPreviousProgress)
						{
							UnityEngine.Debug.Log("ABC: Chose to leave and load previous cloud data");
							CloudOnceManagerHelper.ForcedLoadFromCloud = true;
							PlayerPrefs.Save();
							this.RunAfterDelay(1f, delegate()
							{
								Application.Quit();
							});
						}
						else if (choice == GooglePlayGamesChoice.None)
						{
							UnityEngine.Debug.Log("ABC: Didn't make a choice will Logout again!");
							CloudOnceManagerHelper.Logout();
						}
						this.playServicesButton.UpdateUI(CloudOnceManagerHelper.IsLoggedIntoCloudService, CloudOnceManagerHelper.HasInitializedCloudService, false);
					});
				}
				else
				{
					this.playServicesButton.UpdateUI(CloudOnceManagerHelper.IsLoggedIntoCloudService, CloudOnceManagerHelper.HasInitializedCloudService, false);
				}
			});
		}
	}

	[SerializeField]
	private UIFishGooglePlayGamesButton playServicesButton;

	[SerializeField]
	private UIGooglePlaySettings uiGooglePlaySettingsDialog;
}
