using System;

namespace UnityEngine
{
	public class UnityNameValuePair<V> : UnityKeyValuePair<string, V>
	{
		public UnityNameValuePair()
		{
		}

		public UnityNameValuePair(string key, V value) : base(key, value)
		{
		}

		public override string Key
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string name;
	}
}
