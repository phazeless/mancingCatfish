using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IGNTournamentDialog : InGameNotificationDialog<IGNTournament>
{
	protected override void Start()
	{
		base.Start();
		this.selectCrewPopUp.OnCrewHasBeenSelected += this.SelectCrewPopUp_OnCrewHasBeenSelected;
		this.selectCrewPopUp.RefreshAllAvailableCrewMembers();
	}

	private void SelectCrewPopUp_OnCrewHasBeenSelected(int index, Skill crew)
	{
		if (crew != null)
		{
			this.selectedCrewMemers[index].sprite = crew.GetExtraInfo().Icon;
			this.selectedCrewMemers[index].color = Color.white;
			this.UpdateUI(false);
		}
	}

	public void OnAddCrewMemberClick(int index)
	{
		this.selectCrewPopUp.Show(index);
	}

	protected override void OnAboutToOpen()
	{
		this.dialogContentHolder.SetActive(true);
		this.UpdateUI(true);
		TournamentManager.Instance.UpdateCurrentTournament(delegate
		{
			if (this != null)
			{
				this.UpdateUI(true);
			}
		});
	}

	private void UpdateUI(bool setPreviouslySelectedCrew = true)
	{
		if (this == null)
		{
			return;
		}
		this.TweenKiller();
		if (setPreviouslySelectedCrew)
		{
			for (int i = 0; i < TournamentManager.Instance.PreviouslyChosenCrewMembers.Count; i++)
			{
				this.selectCrewPopUp.SetSelected(i, TournamentManager.Instance.PreviouslyChosenCrewMembers[i]);
			}
		}
		this.tournamentTabs.SetContent(!TournamentManager.Instance.IsInsideTournament);
		this.prizesLabels[0].SetText(TournamentManager.Instance.CurrentTournament.Reward0);
		this.prizesLabels[1].SetText(TournamentManager.Instance.CurrentTournament.Reward1);
		this.prizesLabels[2].SetText(TournamentManager.Instance.CurrentTournament.Reward2);
		this.prizesLabels[3].SetText(TournamentManager.Instance.CurrentTournament.Reward3);
		this.iconSecondTweenHolder.DOAnchorPosY(350f, 0.2f, false).OnComplete(delegate
		{
			this.dialogBackgroundHolder.gameObject.SetActive(true);
		});
		if (TournamentManager.Instance.IsInsideTournament)
		{
			BigInteger resourceAmount = ResourceManager.Instance.GetResourceAmount(ResourceType.Cash);
			TournamentDB.UpdateLocalPlayerScore(resourceAmount);
			this.UpdateUIInsideTournament();
		}
		else
		{
			this.UpdateUIOutsideTournament();
		}
		TournamentDB.UpdateWindowedHighscoreList();
		this.highscoreFirstPlaceItem.UpdateRow(TournamentDB.FirstPlayer);
		for (int j = 0; j < TournamentDB.SurroundingPlayers.Count; j++)
		{
			this.highscoreListItems[j].UpdateRow(TournamentDB.SurroundingPlayers[j]);
		}
	}

	private void UpdateUIInsideTournament()
	{
		TournamentManager instance = TournamentManager.Instance;
		Tournament.TournamentStatus status = instance.CurrentTournament.Status;
		Tournament.SendScoreStatus scoreStatus = instance.CurrentTournament.ScoreStatus;
		this.tournamentButtonBehaviour.SetButtonInteractability(true);
		this.helperSelectedAtleastOneCrew.SetActive(false);
		if (status == Tournament.TournamentStatus.Ended)
		{
			int reward = instance.GetReward();
			if (reward > 0)
			{
				this.buttonTextJoinLeave.SetVariableText(new string[]
				{
					"Claim",
					"Reward: ",
					reward.ToString()
				});
			}
			else
			{
				this.buttonTextJoinLeave.SetText("Close");
			}
		}
		else if (status == Tournament.TournamentStatus.OnGoing)
		{
			this.buttonTextJoinLeave.SetText("Send Score");
		}
		else if (status == Tournament.TournamentStatus.PendingResults)
		{
			this.buttonTextJoinLeave.SetText("Close");
		}
	}

	private void UpdateUIOutsideTournament()
	{
		TournamentManager instance = TournamentManager.Instance;
		Tournament.TournamentStatus status = instance.CurrentTournament.Status;
		this.tournamentButtonBehaviour.SetButtonInteractability(true);
		this.helperSelectedAtleastOneCrew.SetActive(false);
		if (status == Tournament.TournamentStatus.UnsentScore)
		{
			this.tournamentTabs.SetContent(false);
			this.buttonTextJoinLeave.SetVariableText(new string[]
			{
				"Send Again",
				"(Failed previously)",
				string.Empty
			});
		}
		else if (status == Tournament.TournamentStatus.Ended)
		{
			int reward = instance.GetReward();
			if (reward > 0)
			{
				this.buttonTextJoinLeave.SetVariableText(new string[]
				{
					"Claim",
					"Reward: ",
					reward.ToString()
				});
			}
			else
			{
				this.buttonTextJoinLeave.SetText("Close");
			}
		}
		else if (status == Tournament.TournamentStatus.OnGoing)
		{
			int costAfterFirst = instance.CurrentTournament.CostAfterFirst;
			bool flag = instance.CurrentTournament.HasPlayedBefore(UserManager.Instance.LocalUser.LocalId);
			bool flag2 = !flag || ResourceManager.Instance.GetResourceAmount(ResourceType.Gems) >= (long)costAfterFirst;
			bool flag3 = this.selectCrewPopUp.SelectedCrewMembers.Count > 0;
			if (flag)
			{
				this.buttonTextJoinLeave.SetVariableText(new string[]
				{
					"Join",
					"Cost: ",
					costAfterFirst.ToString()
				});
			}
			else
			{
				this.buttonTextJoinLeave.SetText("Join");
			}
			this.tournamentButtonBehaviour.SetButtonInteractability(flag2 && flag3);
			this.helperSelectedAtleastOneCrew.SetActive(!flag3);
		}
		else if (status == Tournament.TournamentStatus.PendingResults)
		{
			this.buttonTextJoinLeave.SetText("Close");
		}
	}

	protected override void OnAboutToReturn()
	{
		this.TweenKiller();
		this.iconSecondTweenHolder.DOAnchorPosY(0f, 0.4f, false).OnComplete(delegate
		{
			this.dialogBackgroundHolder.gameObject.SetActive(false);
		});
	}

	protected override void OnIGNHasBeenSet()
	{
		this.customSize = new UnityEngine.Vector2?(this.dialogBackgroundHolder.rect.size);
	}

	protected override void OnOpened()
	{
		this.dialogBackgroundHolder.gameObject.SetActive(false);
	}

	protected override void OnRemovedFromList()
	{
	}

	protected override void OnReturned()
	{
	}

	public void RemoveDialog()
	{
		this.inGameNotification.OverrideClearable = true;
		this.Close(true);
	}

	private void TweenKiller()
	{
		this.iconSecondTweenHolder.DOKill(true);
	}

	public void SwapTab(bool isInfo)
	{
	}

	public void JoinOrLeave()
	{
		TournamentManager instance = TournamentManager.Instance;
		if (TournamentManager.Instance.HasTournament)
		{
			Tournament.TournamentStatus status = instance.CurrentTournament.Status;
			if (instance.IsInsideTournament)
			{
				if (status == Tournament.TournamentStatus.Ended)
				{
					if (instance.CurrentTournament.HasWinners)
					{
						this.inGameNotification.OverrideClearable = true;
					}
					instance.Claim(this.buttonJoinLeave.transform.position);
				}
				else if (status == Tournament.TournamentStatus.OnGoing)
				{
					instance.SendScore(false);
				}
				else if (status == Tournament.TournamentStatus.PendingResults)
				{
				}
			}
			else if (status == Tournament.TournamentStatus.Ended)
			{
				if (instance.CurrentTournament.HasWinners)
				{
					this.inGameNotification.OverrideClearable = true;
				}
				instance.Claim(this.buttonJoinLeave.transform.position);
			}
			else if (status == Tournament.TournamentStatus.OnGoing)
			{
				instance.Join(this.selectCrewPopUp.SelectedCrewMembers);
			}
			else if (status != Tournament.TournamentStatus.PendingResults)
			{
				if (status == Tournament.TournamentStatus.UnsentScore)
				{
					instance.SendScore(true);
				}
			}
		}
		else
		{
			this.inGameNotification.OverrideClearable = true;
			UnityEngine.Debug.LogWarning("CurrentTournament has somehow become null when dialog is open!");
		}
		this.Close(false);
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			if (TournamentManager.Instance.CurrentTournament.Status == Tournament.TournamentStatus.OnGoing)
			{
				float seconds = (float)(TournamentManager.Instance.CurrentTournament.EndTime - DateTime.UtcNow).TotalSeconds;
				this.textExpirationLabel.SetVariableText(new string[]
				{
					FHelper.FromSecondsToHoursMinutesSecondsFormat(seconds)
				});
			}
			else if (TournamentManager.Instance.CurrentTournament.Status == Tournament.TournamentStatus.PendingResults)
			{
				this.textExpirationLabel.SetText("Calculating winners...");
			}
			else if (TournamentManager.Instance.CurrentTournament.Status == Tournament.TournamentStatus.Ended)
			{
				this.textExpirationLabel.SetText("TOURNAMENT ENDED");
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.TweenKiller();
	}

	[SerializeField]
	private RectTransform dialogBackgroundHolder;

	[SerializeField]
	private GameObject dialogContentHolder;

	[SerializeField]
	private RectTransform iconSecondTweenHolder;

	[SerializeField]
	private Button buttonJoinLeave;

	[SerializeField]
	private TextMeshProUGUI buttonTextJoinLeave;

	[SerializeField]
	private TextMeshProUGUI textExpirationLabel;

	[SerializeField]
	private TournamentTabs tournamentTabs;

	[SerializeField]
	private UIHighscoreListItem highscoreFirstPlaceItem;

	[SerializeField]
	private List<UIHighscoreListItem> highscoreListItems = new List<UIHighscoreListItem>();

	[SerializeField]
	private List<TextMeshProUGUI> prizesLabels = new List<TextMeshProUGUI>();

	[SerializeField]
	private List<Image> selectedCrewMemers = new List<Image>();

	[SerializeField]
	private UISelectCrewMemberPopup selectCrewPopUp;

	[SerializeField]
	private TournamentButtonBehaviour tournamentButtonBehaviour;

	[SerializeField]
	private GameObject helperSelectedAtleastOneCrew;
}
