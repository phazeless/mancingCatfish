using System;

public class CrownExpGrantedByLocation
{
	public void Increase()
	{
		this.Count++;
		this.LastGranted = DateTime.Now;
	}

	public GranterLocation Location;

	public int Count;

	public DateTime LastGranted = DateTime.Now;
}
