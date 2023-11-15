using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
	public abstract class UnityDictionary<K, V> : IDictionary<K, V>, IEnumerable, ICollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>
	{
		protected abstract List<UnityKeyValuePair<K, V>> KeyValuePairs { get; set; }

		protected abstract void SetKeyValuePair(K k, V v);

		public virtual V this[K key]
		{
			get
			{
				UnityKeyValuePair<K, V> unityKeyValuePair = this.KeyValuePairs.Find(delegate(UnityKeyValuePair<K, V> x)
				{
					K key2 = x.Key;
					return key2.Equals(key);
				});
				if (unityKeyValuePair == null)
				{
					return default(V);
				}
				return unityKeyValuePair.Value;
			}
			set
			{
				if (key == null)
				{
					return;
				}
				this.SetKeyValuePair(key, value);
			}
		}

		public void Add(K key, V value)
		{
			this[key] = value;
		}

		public void Add(KeyValuePair<K, V> kvp)
		{
			this[kvp.Key] = kvp.Value;
		}

		public bool TryGetValue(K key, out V value)
		{
			if (!this.ContainsKey(key))
			{
				value = default(V);
				return false;
			}
			value = this[key];
			return true;
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			return this.Remove(item.Key);
		}

		public bool Remove(K key)
		{
			List<UnityKeyValuePair<K, V>> keyValuePairs = this.KeyValuePairs;
			int num = keyValuePairs.FindIndex(delegate(UnityKeyValuePair<K, V> x)
			{
				K key2 = x.Key;
				return key2.Equals(key);
			});
			if (num == -1)
			{
				return false;
			}
			keyValuePairs.RemoveAt(num);
			this.KeyValuePairs = keyValuePairs;
			return true;
		}

		public void Clear()
		{
			List<UnityKeyValuePair<K, V>> keyValuePairs = this.KeyValuePairs;
			keyValuePairs.Clear();
			this.KeyValuePairs = keyValuePairs;
		}

		public bool ContainsKey(K key)
		{
			return this.KeyValuePairs.FindIndex(delegate(UnityKeyValuePair<K, V> x)
			{
				K key2 = x.Key;
				return key2.Equals(key);
			}) != -1;
		}

		public bool Contains(KeyValuePair<K, V> kvp)
		{
			V v = this[kvp.Key];
			return v.Equals(kvp.Value);
		}

		public int Count
		{
			get
			{
				return this.KeyValuePairs.Count;
			}
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int index)
		{
			List<KeyValuePair<K, V>> list = new List<KeyValuePair<K, V>>();
			for (int i = 0; i < this.KeyValuePairs.Count; i++)
			{
				list[i] = this.ConvertUkvp(this.KeyValuePairs[i]);
			}
			list.CopyTo(array, index);
		}

		public KeyValuePair<K, V> ConvertUkvp(UnityKeyValuePair<K, V> ukvp)
		{
			return new KeyValuePair<K, V>(ukvp.Key, ukvp.Value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return new UnityDictionary<K, V>.UnityDictionaryEnumerator(this);
		}

		public ICollection<K> Keys
		{
			get
			{
				ICollection<K> collection = new List<K>();
				foreach (UnityKeyValuePair<K, V> unityKeyValuePair in this.KeyValuePairs)
				{
					collection.Add(unityKeyValuePair.Key);
				}
				return collection;
			}
		}

		public ICollection<V> Values
		{
			get
			{
				ICollection<V> collection = new List<V>();
				foreach (UnityKeyValuePair<K, V> unityKeyValuePair in this.KeyValuePairs)
				{
					collection.Add(unityKeyValuePair.Value);
				}
				return collection;
			}
		}

		public ICollection<KeyValuePair<K, V>> Items
		{
			get
			{
				List<KeyValuePair<K, V>> list = new List<KeyValuePair<K, V>>();
				foreach (UnityKeyValuePair<K, V> unityKeyValuePair in this.KeyValuePairs)
				{
					list.Add(new KeyValuePair<K, V>(unityKeyValuePair.Key, unityKeyValuePair.Value));
				}
				return list;
			}
		}

		public V SyncRoot
		{
			get
			{
				return default(V);
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		internal sealed class UnityDictionaryEnumerator : IEnumerator<KeyValuePair<K, V>>, IEnumerator, IDisposable
		{
			internal UnityDictionaryEnumerator()
			{
			}

			internal UnityDictionaryEnumerator(UnityDictionary<K, V> ud)
			{
				this.items = new KeyValuePair<K, V>[ud.Count];
				ud.CopyTo(this.items, 0);
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public KeyValuePair<K, V> Current
			{
				get
				{
					this.ValidateIndex();
					return this.items[this.index];
				}
			}

			public KeyValuePair<K, V> Entry
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
				this.index = -1;
				this.items = null;
			}

			public K Key
			{
				get
				{
					this.ValidateIndex();
					return this.items[this.index].Key;
				}
			}

			public V Value
			{
				get
				{
					this.ValidateIndex();
					return this.items[this.index].Value;
				}
			}

			public bool MoveNext()
			{
				if (this.index < this.items.Length - 1)
				{
					this.index++;
					return true;
				}
				return false;
			}

			private void ValidateIndex()
			{
				if (this.index < 0 || this.index >= this.items.Length)
				{
					throw new InvalidOperationException("Enumerator is before or after the collection.");
				}
			}

			public void Reset()
			{
				this.index = -1;
			}

			private KeyValuePair<K, V>[] items;

			private int index = -1;
		}
	}
}
