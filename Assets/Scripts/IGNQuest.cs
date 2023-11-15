using System;
using Newtonsoft.Json;

public class IGNQuest : InGameNotification
{
	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.Quest;
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

	[JsonIgnore]
	public Quest Quest;
}
