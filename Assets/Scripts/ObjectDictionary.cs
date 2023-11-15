using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectDictionary : UnityDictionary<string>
{
	protected override List<UnityKeyValuePair<string, string>> KeyValuePairs
	{
		get
		{
			if (this.values == null)
			{
				this.values = new List<ObjectKvp>();
			}
			List<UnityKeyValuePair<string, string>> list = new List<UnityKeyValuePair<string, string>>();
			foreach (ObjectKvp okvp in this.values)
			{
				list.Add(this.ConvertOkvp(okvp));
			}
			return list;
		}
		set
		{
			if (value == null)
			{
				this.values = new List<ObjectKvp>();
				return;
			}
			foreach (UnityKeyValuePair<string, string> ukvp in value)
			{
				this.values.Add(this.ConvertUkvp(ukvp));
			}
		}
	}

	public new ObjectKvp ConvertUkvp(UnityKeyValuePair<string, string> ukvp)
	{
		return new ObjectKvp(ukvp.Key, ukvp.Value);
	}

	public UnityKeyValuePair<string, string> ConvertOkvp(ObjectKvp okvp)
	{
		return new UnityKeyValuePair<string, string>(okvp.Key, okvp.Value);
	}

	protected override void SetKeyValuePair(string k, string v)
	{
		int num = this.values.FindIndex((ObjectKvp x) => x.Key == k);
		if (num == -1)
		{
			this.values.Add(new ObjectKvp(k, v));
			return;
		}
		if (v == null)
		{
			this.values.RemoveAt(num);
			return;
		}
		this.values[num] = new ObjectKvp(k, v);
	}

	public List<ObjectKvp> values;
}
