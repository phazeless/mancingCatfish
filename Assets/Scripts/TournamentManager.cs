using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Numerics;
using SimpleFirebaseUnity;
using UnityEngine;

public class TournamentManager : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<string> OnJoinTournament;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<string, int, string> OnLeftTournament;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<string, int, string> OnFinishedTournament;

	public static TournamentManager Instance { get; private set; }

	public List<Skill> PreviouslyChosenCrewMembers
	{
		get
		{
			return this.previouslyChosenCrewMembers;
		}
	}

	public bool IsInsideTournament { get; private set; }

	public Skill TournamentDWLevelSkill
	{
		get
		{
			return this.tournamentDWLevelSkill;
		}
	}

	public float TimeLeft
	{
		get
		{
			return (float)this.durationInSeconds - this.tournamentTimer;
		}
	}

	private void Awake()
	{
		TournamentManager.Instance = this;
		this.increaseDWLevelAfterSeconds = (float)(this.durationInSeconds / this.increaseDWLevelInterval);
		UserManager.Instance.OnUserInitialized += this.Instance_OnUserInitialized;
		this.tournamentIsAvailableAfterQuest.OnQuestCompleted += this.TournamentIsAvailableAfterQuest_OnQuestCompleted;
	}

	private void TournamentIsAvailableAfterQuest_OnQuestCompleted(Quest quest)
	{
		this.CheckForTournamentUpdates();
	}

	private void Instance_OnUserInitialized(User user)
	{
		if (user != null)
		{
			TournamentDB.Init(user);
			if (this.tournamentIsAvailableAfterQuest.IsCompleted)
			{
				this.LoadTournament();
				this.CheckForTournamentUpdates();
			}
		}
	}

	public void CheckForTournamentUpdates()
	{
		if (this.HasTournament)
		{
			UnityEngine.Debug.Log("ABC: Getting tournament: " + this.currentTournament.Id);
			TournamentDB.GetTournament(this.currentTournament.Id, delegate(Tournament tournament, FirebaseError error)
			{
				if (error == null)
				{
					this.currentTournament = tournament;
					TournamentDB.UpdateLists(this.currentTournament);
					this.previousSentScore = TournamentDB.LocalPlayer.Score;
					if (!InGameNotificationManager.Instance.IsAnyIGNActiveOfType<IGNTournament>())
					{
						InGameNotificationManager.Instance.Create<IGNTournament>(new IGNTournament());
					}
				}
				else if (error.Status == HttpStatusCode.Gone)
				{
					this.currentTournament = null;
					this.CheckForTournamentUpdates();
					UnityEngine.Debug.Log("The requested tournament is no longer available! Setting current saved tournament to null.");
				}
				else
				{
					UnityEngine.Debug.Log("Failed 'GetTournament'. Error: " + error.Message);
				}
			});
		}
		else
		{
			TournamentDB.GetActiveTournament(delegate(Tournament tournament, FirebaseError error)
			{
				if (error == null)
				{
					this.currentTournament = tournament;
					TournamentDB.UpdateLists(this.currentTournament);
					this.previousSentScore = TournamentDB.LocalPlayer.Score;
					if (!InGameNotificationManager.Instance.IsAnyIGNActiveOfType<IGNTournament>())
					{
						InGameNotificationManager.Instance.Create<IGNTournament>(new IGNTournament());
					}
				}
				else if (error.Status == HttpStatusCode.Gone)
				{
					UnityEngine.Debug.Log("Currently there's no active tournaments...");
				}
				else
				{
					UnityEngine.Debug.Log("Failed 'GetTournament'. Error: " + error.Message);
				}
			});
		}
	}

	public bool HasUserJoinedCurrentTournament
	{
		get
		{
			if (this.HasTournament)
			{
				List<Tournament.User> participants = this.currentTournament.Participants;
				return participants.Find((Tournament.User x) => x.Id == UserManager.Instance.LocalUser.LocalId) != null;
			}
			return false;
		}
	}

	public bool HasTournament
	{
		get
		{
			return this.currentTournament != null;
		}
	}

	public Tournament CurrentTournament
	{
		get
		{
			return this.currentTournament;
		}
	}

	public int GetReward()
	{
		if (this.HasTournament)
		{
			List<Tournament.User> finalWinners = this.currentTournament.FinalWinners;
			Tournament.User user = finalWinners.Find((Tournament.User x) => x.Id == UserManager.Instance.LocalUser.LocalId);
			if (user != null)
			{
				return user.Reward;
			}
		}
		return 0;
	}

	public void Join(List<Skill> selectedCrewMembers)
	{
		this.tournamentTimer = 0f;
		bool flag = true;
		bool flag2 = this.currentTournament.HasPlayedBefore(UserManager.Instance.LocalUser.LocalId);
		this.previouslyChosenCrewMembers = selectedCrewMembers;
		if (flag2)
		{
			ResourceChangeData changeData = new ResourceChangeData("tournamentEntryFeeId", "Tournament Entry Fee", this.currentTournament.CostAfterFirst, ResourceType.Gems, ResourceChangeType.Spend, ResourceChangeReason.PurchaseTournamentEntry);
			flag = ResourceManager.Instance.TakeGems(this.currentTournament.CostAfterFirst, changeData);
		}
		if (flag)
		{
			if (selectedCrewMembers.Count < 1)
			{
				return;
			}
			this.IsInsideTournament = true;
			if (this.OnJoinTournament != null)
			{
				this.OnJoinTournament(this.currentTournament.Id);
			}
			this.tournamentDWLevelSkill.SetCurrentLevel(0, LevelChange.Initialization);
			SkillManager.Instance.SaveCurrentStateOfSkills(true);
			InGameNotificationManager.Instance.SaveIGNs();
			ResourceManager.Instance.SaveCash();
			ChestManager.Instance.SaveChests();
			ConsumableManager.Instance.Save();
			SkillManager.Instance.ResetCrewMembers();
			SkillManager.Instance.ResetSkills();
			SkillManager.Instance.ResetPrestige();
			SkillManager.Instance.ResetWheelBonus();
			SkillManager.Instance.ResetStars();
			SkillManager.Instance.TierSkill.TryLevelUp();
			foreach (Skill skill in selectedCrewMembers)
			{
				SkillManager.Instance.LoadSkill(skill, 0);
			}
			InGameNotificationManager.Instance.ClearIGNs();
			InGameNotificationManager.Instance.Create<IGNTournament>(new IGNTournament());
			GoldFishingHandler.Instance.DisableGoldFishing();
			ColorChangerHandler.TweenToColor(this.endColor, this.TimeLeft);
			ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Tournament);
			this.crownExpButton.SetActive(false);
			this.settingsButton.SetActive(false);
			this.CheckForTournamentUpdates();
		}
		else
		{
			UnityEngine.Debug.Log("Not enough Gems to play this tournament again...");
		}
	}

	public void SendScore(bool retryAttempt = false)
	{
		if (TournamentDB.LocalPlayer.Score > this.previousSentScore)
		{
			TournamentDB.SendScore(this.currentTournament.Id, delegate(BigInteger score, FirebaseError error)
			{
				if (error == null)
				{
					this.previousSentScore = score;
					this.currentTournament.AddUser(TournamentDB.LocalPlayer);
					this.currentTournament.ScoreStatus = Tournament.SendScoreStatus.SuccessOrNotSent;
				}
				else
				{
					if (error.Status == HttpStatusCode.Unauthorized)
					{
						this.currentTournament.ScoreStatus = Tournament.SendScoreStatus.TooLate;
					}
					else
					{
						this.currentTournament.ScoreStatus = Tournament.SendScoreStatus.Failed;
					}
					UnityEngine.Debug.LogWarning(string.Concat(new object[]
					{
						"Failed to send Tournament score to the Database. Error code: ",
						error.Status,
						", msg: ",
						error.Message
					}));
				}
			});
		}
		if (!retryAttempt)
		{
			int arg = 999;
			string arg2 = "0";
			try
			{
				arg = TournamentDB.LocalPlayer.Placement;
				arg2 = TournamentDB.LocalPlayer.Score.ToString();
			}
			catch
			{
			}
			this.GoBackToStateBeforeTournament();
			if (this.currentTournament != null && this.OnLeftTournament != null)
			{
				this.OnLeftTournament(this.currentTournament.Id, arg, arg2);
			}
		}
	}

	private void GoBackToStateBeforeTournament()
	{
		this.IsInsideTournament = false;
		SkillManager.Instance.ResetSkills();
		SkillManager.Instance.LoadSavedSkillLevels();
		ResourceManager.Instance.LoadSavedCash(true);
		InGameNotificationManager.Instance.ClearIGNs();
		InGameNotificationManager.Instance.LoadSavedIGNs();
		InGameNotificationManager.Instance.Create<IGNTournament>(new IGNTournament());
		QuestManager.Instance.LoadQuests();
		ChestManager.Instance.CreateChestIGNs();
		ConsumableManager.Instance.Load();
		InGameNotificationManager.Instance.CheckForAndCreateTimePeriodSpecificIGNs(false);
		DialogInteractionHandler.Instance.DisableCloseByClickingShade = false;
		this.hasTournamentEnded = false;
		this.tournamentTimer = 0f;
		this.increaseDWLevelTimer = 0f;
		ScreenManager.Instance.GoToScreen(ScreenManager.Screen.Main);
		this.crownExpButton.SetActive(true);
		this.settingsButton.SetActive(true);
	}

	public void Claim(UnityEngine.Vector2 claimButtonPosition)
	{
		string text = (this.currentTournament == null) ? null : this.currentTournament.Id;
		int num = (TournamentDB.LocalPlayer == null) ? -1 : TournamentDB.LocalPlayer.Placement;
		string text2 = (TournamentDB.LocalPlayer == null) ? null : TournamentDB.LocalPlayer.Score.ToString();
		int reward = this.GetReward();
		ResourceChangeData gemChangeData = new ResourceChangeData("contentId_tournamentReward", null, reward, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.TournamentReward);
		GemGainVisual.Instance.GainGems(reward, claimButtonPosition, gemChangeData);
		this.currentTournament = null;
		this.SaveTournament();
		this.CheckForTournamentUpdates();
		if (text != null && num >= 0 && text2 != null && this.OnFinishedTournament != null)
		{
			this.OnFinishedTournament(text, num, text2);
		}
	}

	public void UpdateCurrentTournament(Action onResponse)
	{
		if (this.HasTournament && !this.isUpdatingTournament)
		{
			if (!this.currentTournament.HasWinners)
			{
				if (DateTime.UtcNow > this.currentTournament.EndTime || TournamentDB.CanRefreshHighscoreFromDB)
				{
					TournamentDB.GetTournament(this.currentTournament.Id, delegate(Tournament tournament, FirebaseError error)
					{
						if (error == null)
						{
							this.currentTournament = tournament;
							TournamentDB.UpdateLists(this.currentTournament);
							this.isUpdatingTournament = false;
							if (onResponse != null)
							{
								onResponse();
							}
						}
						else
						{
							UnityEngine.Debug.LogWarning("Failed to get Active Tournaments. Error: " + error.Message);
						}
					});
				}
			}
			else if (!this.currentTournament.HasEnded)
			{
			}
		}
	}

	private void Update()
	{
		if (!this.IsInsideTournament)
		{
			return;
		}
		if (FHelper.HasSecondsPassed((float)this.durationInSeconds, ref this.tournamentTimer, false) && !this.hasTournamentEnded)
		{
			this.hasTournamentEnded = true;
			InGameNotificationManager.Instance.OpenFirstOccurrenceOfIGN<IGNTournament>();
			DialogInteractionHandler.Instance.DisableCloseByClickingShade = true;
		}
		if (!this.hasTournamentEnded && FHelper.HasSecondsPassed(this.increaseDWLevelAfterSeconds, ref this.increaseDWLevelTimer, true))
		{
			this.tournamentDWLevelSkill.LevelUpForFree();
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			this.SaveTournament();
			this.timeWhenLeavingApp = DateTime.Now;
		}
		else if (DateTime.Now < this.timeWhenLeavingApp)
		{
			UnityEngine.Debug.LogWarning("Some cheating going on here...");
			this.tournamentTimer = (float)this.durationInSeconds;
		}
		else
		{
			this.awayTimeInSeconds = (float)(DateTime.Now - this.timeWhenLeavingApp).TotalSeconds;
			this.tournamentTimer += this.awayTimeInSeconds;
		}
	}

	private void LoadTournament()
	{
		string text = EncryptedPlayerPrefs.GetString(TournamentManager.KEY_CURRENT_TOURNAMENT, null);
		if (!string.IsNullOrEmpty(text))
		{
			bool flag = text.StartsWith("{") || text.Length > 60;
			if (flag)
			{
				text = null;
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			this.currentTournament = new Tournament();
			this.currentTournament.Id = text;
		}
		this.hasLoadedTournament = true;
	}

	private void SaveTournament()
	{
		if (this.currentTournament != null)
		{
			EncryptedPlayerPrefs.SetString(TournamentManager.KEY_CURRENT_TOURNAMENT, this.currentTournament.Id, true);
		}
		else if (this.hasLoadedTournament)
		{
			EncryptedPlayerPrefs.DeleteKey(TournamentManager.KEY_CURRENT_TOURNAMENT);
		}
	}

	private const string contentId_tournamentReward = "contentId_tournamentReward";

	private static readonly string KEY_CURRENT_TOURNAMENT = "KEY_CURRENT_TOURNAMENT";

	[SerializeField]
	private Skill tournamentDWLevelSkill;

	[SerializeField]
	private int durationInSeconds;

	[SerializeField]
	private int increaseDWLevelInterval;

	[SerializeField]
	private Color endColor;

	[SerializeField]
	private Quest tournamentIsAvailableAfterQuest;

	[SerializeField]
	private GameObject settingsButton;

	[SerializeField]
	private GameObject crownExpButton;

	private float increaseDWLevelAfterSeconds;

	private float tournamentTimer;

	private float increaseDWLevelTimer;

	private bool hasTournamentEnded;

	private bool hasLoadedTournament;

	private BigInteger previousSentScore = 0;

	private Tournament currentTournament;

	private List<Skill> previouslyChosenCrewMembers = new List<Skill>();

	private DateTime timeWhenLeavingApp = DateTime.Now;

	private float awayTimeInSeconds;

	private bool isUpdatingTournament;
}
