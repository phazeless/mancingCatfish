using System;
using System.Numerics;

public class TournamentUserData
{
	public TournamentUserData()
	{
	}

	public TournamentUserData(string username, BigInteger score)
	{
		this.Username = username;
		this.Score = score;
	}

	public string ToJSONFormatForDB()
	{
		JSONObject jsonobject = new JSONObject();
		jsonobject.AddField("username", this.Username);
		jsonobject.AddField("score", this.Score.ToString());
		return jsonobject.ToString();
	}

	public string Id;

	public int Placement;

	public string Username = "Anonymous";

	public BigInteger Score = 0;
}
