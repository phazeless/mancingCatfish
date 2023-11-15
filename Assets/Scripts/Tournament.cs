using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;

public class Tournament
{
	public Tournament.TournamentStatus Status
	{
		get
		{
			if (this.ScoreStatus == Tournament.SendScoreStatus.Failed)
			{
				return Tournament.TournamentStatus.UnsentScore;
			}
			if (this.HasEnded && !this.HasWinners)
			{
				return Tournament.TournamentStatus.PendingResults;
			}
			if (this.HasEnded && this.HasWinners)
			{
				return Tournament.TournamentStatus.Ended;
			}
			if (!this.HasEnded && this.HasWinners)
			{
				return Tournament.TournamentStatus.Ended;
			}
			if (!this.HasEnded && this.IsAllowedToJoin)
			{
				return Tournament.TournamentStatus.OnGoing;
			}
			if (!this.HasEnded && !this.IsAllowedToJoin)
			{
				return Tournament.TournamentStatus.PendingResults;
			}
			return Tournament.TournamentStatus.Unknown;
		}
	}

	public bool HasPlayedBefore(string userId)
	{
		return this.Users.ContainsKey(userId);
	}

	public bool HasWinners
	{
		get
		{
			return this.Winners != null && this.Winners.Count > 0;
		}
	}

	public bool HasEnded
	{
		get
		{
			return DateTime.UtcNow > this.EndTime;
		}
	}

	public bool IsAllowedToJoin
	{
		get
		{
			return DateTime.UtcNow < this.EndTime.AddMinutes(-10.0);
		}
	}

	public DateTime StartTime
	{
		get
		{
			return Tournament.DateTime1970.AddMilliseconds((double)long.Parse(this.Start));
		}
	}

	public DateTime EndTime
	{
		get
		{
			return Tournament.DateTime1970.AddMilliseconds((double)long.Parse(this.End));
		}
	}

	public List<Tournament.User> FinalWinners
	{
		get
		{
			List<Tournament.User> list = new List<Tournament.User>();
			foreach (KeyValuePair<string, object> keyValuePair in this.Winners)
			{
				string value = JsonConvert.SerializeObject(keyValuePair.Value);
				Tournament.User user = JsonConvert.DeserializeObject<Tournament.User>(value);
				user.Id = keyValuePair.Key;
				list.Add(user);
			}
			return list;
		}
	}

	public List<Tournament.User> Participants
	{
		get
		{
			List<Tournament.User> list = new List<Tournament.User>();
			foreach (KeyValuePair<string, object> keyValuePair in this.Users)
			{
				string value = JsonConvert.SerializeObject(keyValuePair.Value);
				Tournament.User user = JsonConvert.DeserializeObject<Tournament.User>(value);
				user.Id = keyValuePair.Key;
				list.Add(user);
			}
			return list;
		}
	}

	public void AddUser(Tournament.User user)
	{
		if (this.Users.ContainsKey(user.Id))
		{
			return;
		}
		this.Users.Add(user.Id, user);
	}

	public Tournament.SendScoreStatus ScoreStatus;

	public string Id;

	[JsonProperty]
	private string Start = "0";

	[JsonProperty]
	private string End = "0";

	[JsonProperty]
	public string Reward0 = "0";

	[JsonProperty]
	public string Reward1 = "1";

	[JsonProperty]
	public string Reward2 = "2";

	[JsonProperty]
	public string Reward3 = "3";

	[JsonProperty]
	public int CostAfterFirst;

	[JsonProperty]
	private Dictionary<string, object> Users = new Dictionary<string, object>();

	[JsonProperty]
	private Dictionary<string, object> Winners = new Dictionary<string, object>();

	private static readonly DateTime DateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public enum TournamentStatus
	{
		Unknown,
		OnGoing,
		PendingResults,
		Ended,
		UnsentScore
	}

	public enum SendScoreStatus
	{
		SuccessOrNotSent,
		Failed,
		TooLate
	}

	public class User
	{
		public User()
		{
		}

		public User(string id, string username, BigInteger score)
		{
			this.Id = id;
			this.Username = username;
			this.Score = score;
		}

		public string ToJSONFormatForDB(string sauth = null)
		{
			JSONObject jsonobject = new JSONObject();
			jsonobject.AddField("username", this.Username);
			jsonobject.AddField("score", this.Score.ToString());
			if (sauth != null)
			{
				jsonobject.AddField("sauth", sauth);
			}
			return jsonobject.ToString();
		}

		[JsonIgnore]
		public string Id;

		[JsonIgnore]
		public int Placement;

		[JsonIgnore]
		public bool IsLocalUser;

		[JsonConverter(typeof(BigIntegerToStringConverter))]
		public BigInteger Score = 0;

		public string Username;

		public int Reward;
	}
}
