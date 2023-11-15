using System;
using UnityEngine;

[Serializable]
public sealed class ObjectKvp : UnityNameValuePair<string>
{
	public ObjectKvp(string key, string value) : base(key, value)
	{
	}

	public override string Value
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	public string value;
}
