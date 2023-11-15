using System;

[Serializable]
public class RecievedChest
{
	public int GetElapsedSecondsSinceReceived()
	{
		return (int)(DateTime.Now - this.RecievedDate).TotalSeconds;
	}

	public DateTime RecievedDate = DateTime.Now;

	public string ChestId;
}
