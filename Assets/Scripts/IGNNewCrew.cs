using System;
using Newtonsoft.Json;

public class IGNNewCrew : InGameNotification
{
	public IGNNewCrew(Skill crew, bool isNewCrew = true)
	{
		this.Skill = crew;
		this.IsNewCrew = isNewCrew;
		base.SetExpiration(150f);
	}

	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.NewCrew;
		}
	}

	public override bool IsClearable
	{
		get
		{
			return false;
		}
	}

	public override bool RemoveOnReset
	{
		get
		{
			return false;
		}
	}

	public override bool RemoveOnExit
	{
		get
		{
			return true;
		}
	}

	public bool IsNewCrew { get; private set; }

	[JsonIgnore]
	public Skill Skill { get; private set; }

	private const int EXPIRATION_IN_SECONDS = 150;
}
