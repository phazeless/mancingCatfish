using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using Newtonsoft.Json;
using SimpleFirebaseUnity;
using UnityEngine;

public static class v2TournamentDB
{
	public static void SendScore(Tournament.User localPlayer, User localUser, string tournamentId, Action<BigInteger, FirebaseError> onResponse)
	{
		if (localUser == null)
		{
			UnityEngine.Debug.LogWarning("Can't send score because LocalUser is null (probably no Internet upon startup)");
			return;
		}
		Firebase firebase = Firebase.CreateNew(v2TournamentDB.HOST, localUser.IdToken).Child("v2/tournaments", false).Child(tournamentId, false).Child("users", false).Child(localPlayer.Id, false);
		Firebase firebase2 = firebase;
		firebase2.OnSetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase2.OnSetFailed, new Action<Firebase, FirebaseError>(delegate(Firebase a, FirebaseError error)
		{
			if (onResponse != null)
			{
				onResponse(0, error);
			}
		}));
		Firebase firebase3 = firebase;
		firebase3.OnSetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnSetSuccess, new Action<Firebase, DataSnapshot>(delegate(Firebase a, DataSnapshot snapshot)
		{
			if (onResponse != null)
			{
				onResponse(localPlayer.Score, null);
			}
		}));
		firebase.SetValue(localPlayer.ToJSONFormatForDB(TournamentAlph.GetAlphScore(localPlayer.Score.ToString())), true, string.Empty);
	}

	public static void GetTournament(User localUser, string tournamentId, Action<Tournament, FirebaseError> onResponse)
	{
		bool flag = localUser != null;
		bool flag2 = tournamentId != null;
		if (!flag)
		{
			UnityEngine.Debug.LogWarning("Can't GetTournament because LocalUser is null (probably no Internet upon startup)");
			return;
		}
		Firebase firebase = Firebase.CreateNew(v2TournamentDB.HOST, localUser.IdToken).Child("v2/tournaments", false);
		string param;
		if (flag2)
		{
			param = string.Empty;
			firebase = firebase.Child(tournamentId, false);
		}
		else
		{
			param = "orderBy=\"status\"&limitToFirst=1&equalTo=" + 0;
		}
		Firebase firebase2 = firebase;
		firebase2.OnGetFailed = (Action<Firebase, FirebaseError>)Delegate.Combine(firebase2.OnGetFailed, new Action<Firebase, FirebaseError>(delegate(Firebase a, FirebaseError error)
		{
			if (onResponse != null)
			{
				onResponse(null, error);
			}
		}));
		Firebase firebase3 = firebase;
		firebase3.OnGetSuccess = (Action<Firebase, DataSnapshot>)Delegate.Combine(firebase3.OnGetSuccess, new Action<Firebase, DataSnapshot>(delegate(Firebase a, DataSnapshot snapshot)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)snapshot.RawValue;
			if (dictionary != null)
			{
				Tournament tournament = null;
				if (tournamentId == null)
				{
					foreach (KeyValuePair<string, object> keyValuePair in dictionary)
					{
						tournament = JsonConvert.DeserializeObject<Tournament>(JsonConvert.SerializeObject(keyValuePair.Value));
						tournament.Id = keyValuePair.Key;
					}
				}
				else
				{
					tournament = JsonConvert.DeserializeObject<Tournament>(JsonConvert.SerializeObject(dictionary));
					tournament.Id = tournamentId;
				}
				if (onResponse != null)
				{
					onResponse(tournament, null);
				}
			}
			else if (onResponse != null)
			{
				onResponse(null, new FirebaseError(HttpStatusCode.Gone, "No tournament available!"));
			}
		}));
		firebase.GetValue(param);
	}

	private static readonly string HOST = "fishinc-app.firebaseio.com";

	private static readonly string API_KEY = "AIzaSyBm9T8MScrMElaadDWev9JkiWWaQUt-qY0";

	public enum v2TournamentStatus
	{
		NotFull,
		Full,
		Finished
	}
}
