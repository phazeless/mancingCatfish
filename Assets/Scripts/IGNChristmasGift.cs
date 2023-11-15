using System;
using System.Diagnostics;

public class IGNChristmasGift : InGameNotification
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnGiftClaimed;

	public void NotifyClaimed()
	{
		if (this.OnGiftClaimed != null)
		{
			this.OnGiftClaimed();
		}
	}

	public DateTime LocalTime { get; set; }

	public override bool IsClearable
	{
		get
		{
			return true;
		}
	}

	public override InGameNotification.IGN Type
	{
		get
		{
			return InGameNotification.IGN.ChristmasGift;
		}
	}

	public override bool RemoveOnReset
	{
		get
		{
			return true;
		}
	}

	public override bool RemoveOnExit
	{
		get
		{
			return true;
		}
	}
}
