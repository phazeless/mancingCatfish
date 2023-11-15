using System;
using System.Collections.Generic;
using System.Numerics;
using SimpleFirebaseUnity;

public class TournamentDB
{
	public static bool CanRefreshHighscoreFromDB
	{
		get
		{
			return (DateTime.Now - TournamentDB.lastDownloadedHighscore).TotalSeconds > (double)TournamentDB.SECONDS_BEFORE_REDOWNLOAD_HIGHSCORE;
		}
	}

	public static void Init(User user)
	{
		TournamentDB.LocalUser = user;
		TournamentDB.LocalPlayer.IsLocalUser = true;
		TournamentDB.LocalPlayer.Id = user.LocalId;
		TournamentDB.LocalPlayer.Username = user.Username;
	}

	public static void SendScore(string tournamentId, Action<BigInteger, FirebaseError> onResponse)
	{
		v2TournamentDB.SendScore(TournamentDB.LocalPlayer, TournamentDB.LocalUser, tournamentId, onResponse);
	}

	public static void GetActiveTournament(Action<Tournament, FirebaseError> onResponse)
	{
		TournamentDB.GetTournament(null, onResponse);
	}

	public static void GetTournament(string tournamentId, Action<Tournament, FirebaseError> onResponse)
	{
		v2TournamentDB.GetTournament(TournamentDB.LocalUser, tournamentId, onResponse);
		TournamentDB.lastDownloadedHighscore = DateTime.Now;
	}

	public static void UpdateLists(Tournament tournament)
	{
		TournamentDB.LocalPlayer.Score = 0;
		TournamentDB.allScores.Clear();
		List<Tournament.User> participants = tournament.Participants;
		foreach (Tournament.User user in participants)
		{
			TournamentDB.allScores.Add(new Tournament.User(user.Id, user.Username, user.Score));
		}
		TournamentDB.UpdateAllScoresList();
		TournamentDB.UpdateLocalPlayerObject();
		TournamentDB.UpdateLocalPlayerPlacement();
		TournamentDB.UpdateFirstPlacementPlayerObject();
		TournamentDB.UpdateWindowedHighscoreList();
	}

	public static void UpdateWindowedHighscoreList()
	{
		TournamentDB.UpdateFirstPlacementPlayerObject();
		TournamentDB.LookUpAndAddSurroundingPlayers();
	}

	private static void UpdateLocalPlayerObject()
	{
		Tournament.User user = TournamentDB.allScores.Find((Tournament.User x) => x.Id == TournamentDB.LocalPlayer.Id);
		if (user != null)
		{
			TournamentDB.LocalPlayer = user;
			TournamentDB.LocalPlayer.IsLocalUser = true;
		}
		else
		{
			TournamentDB.allScores.Add(TournamentDB.LocalPlayer);
		}
	}

	public static void UpdateLocalPlayerScore(BigInteger score)
	{
		if (score > TournamentDB.LocalPlayer.Score)
		{
			TournamentDB.LocalPlayer.Score = score;
		}
		TournamentDB.UpdateLocalPlayerPlacement();
	}

	private static void UpdateLocalPlayerPlacement()
	{
		TournamentDB.UpdateAllScoresList();
		TournamentDB.LocalPlayer.Placement = TournamentDB.GetLocalPlayerPlacement();
	}

	private static int GetLocalPlayerPlacement()
	{
		return TournamentDB.allScores.FindIndex((Tournament.User x) => x.Id == TournamentDB.LocalPlayer.Id);
	}

	private static void UpdateFirstPlacementPlayerObject()
	{
		Tournament.User user = (TournamentDB.allScores.Count <= 0) ? null : TournamentDB.allScores[0];
		if (user != null)
		{
			TournamentDB.FirstPlayer = user;
		}
	}

	private static void UpdateAllScoresList()
	{
		TournamentDB.allScores.Sort((Tournament.User x, Tournament.User y) => y.Score.CompareTo(x.Score));
	}

	private static void LookUpAndAddSurroundingPlayers()
	{
		TournamentDB.SurroundingPlayers.Clear();
		int num = 0;
		int num2 = 0;
		int num3 = 3;
		for (int i = 1; i <= num3; i++)
		{
			if (!TournamentDB.CheckPos(TournamentDB.LocalPlayer.Placement, i))
			{
				num++;
			}
			if (!TournamentDB.CheckPos(TournamentDB.LocalPlayer.Placement, -i))
			{
				num2++;
			}
		}
		for (int j = 1; j <= num; j++)
		{
			TournamentDB.CheckPos(TournamentDB.LocalPlayer.Placement, -(num3 + j));
		}
		for (int k = 1; k <= num2; k++)
		{
			TournamentDB.CheckPos(TournamentDB.LocalPlayer.Placement, num3 + k);
		}
		TournamentDB.SurroundingPlayers.Add(TournamentDB.LocalPlayer);
		TournamentDB.SurroundingPlayers.Sort((Tournament.User x, Tournament.User y) => y.Score.CompareTo(x.Score));
	}

	private static bool CheckPos(int initialPos, int nextPos)
	{
		int num = initialPos + nextPos;
		if ((nextPos <= 0) ? (num > 0) : (num < TournamentDB.allScores.Count))
		{
			Tournament.User user = TournamentDB.allScores[num];
			user.Placement = num;
			TournamentDB.SurroundingPlayers.Add(user);
			return true;
		}
		return false;
	}

	private static User LocalUser = null;

	private static readonly string HOST = "fishinc-app.firebaseio.com";

	private static readonly string API_KEY = "AIzaSyBm9T8MScrMElaadDWev9JkiWWaQUt-qY0";

	public static Tournament.User FirstPlayer = new Tournament.User();

	public static Tournament.User LocalPlayer = new Tournament.User();

	public static List<Tournament.User> SurroundingPlayers = new List<Tournament.User>();

	private static List<Tournament.User> allScores = new List<Tournament.User>();

	private static readonly int SECONDS_BEFORE_REDOWNLOAD_HIGHSCORE = 3600;

	private static DateTime lastDownloadedHighscore = DateTime.Now;
}
