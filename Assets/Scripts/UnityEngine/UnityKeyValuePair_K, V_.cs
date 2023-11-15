using System;

namespace UnityEngine
{
	public class UnityKeyValuePair<K, V>
	{
		public UnityKeyValuePair()
		{
			this.Key = default(K);
			this.Value = default(V);
		}

		public UnityKeyValuePair(K key, V value)
		{
			this.Key = key;
			this.Value = value;
		}

		public virtual K Key { get; set; }

		public virtual V Value { get; set; }
	}
}
