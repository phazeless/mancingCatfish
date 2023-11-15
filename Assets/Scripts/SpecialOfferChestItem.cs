using System;
using Newtonsoft.Json;

[Serializable]
public class SpecialOfferChestItem
{
	public int SecondsUntilExpiration
	{
		get
		{
			int num = (int)(DateTime.Now - this.Received).TotalSeconds;
			return this.DurationInSeconds - num;
		}
	}

	[JsonIgnore]
	public ItemChest ItemChest;

	public string IapSKU;

	public int DurationInSeconds = 86400;

	public DateTime Received = DateTime.Now;

	public bool WasPurchased;
}
