using System;
using System.Collections.Generic;

[Serializable]
public class EasterEggContent
{
	public bool ContainsGems
	{
		get
		{
			return this.GemAmount > 0;
		}
	}

	public bool ContainsItems
	{
		get
		{
			return this.Items.Count > 0;
		}
	}

	public int GemAmount;

	public List<Item> Items = new List<Item>();
}
