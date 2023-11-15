using System;

namespace CloudOnce.Internal
{
	public abstract class PersistentValue<T> : IPersistent
	{
		protected PersistentValue(string key, PersistenceType type, T value, T defaultValue, PersistentValue<T>.ValueLoaderDelegate valueLoader, PersistentValue<T>.ValueSetterDelegate valueSetter)
		{
			this.Key = key;
			this.Value = value;
			this.PersistenceType = type;
			this.DefaultValue = defaultValue;
			this.ValueLoader = valueLoader;
			this.ValueSetter = valueSetter;
			DataManager.CloudPrefs[key] = this;
			DataManager.InitDataManager();
		}

		public string Key { get; private set; }

		public T Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.IsValidSet(value))
				{
					this.value = value;
				}
			}
		}

		public PersistenceType PersistenceType { get; private set; }

		public T DefaultValue { get; private set; }

		private PersistentValue<T>.ValueLoaderDelegate ValueLoader { get; set; }

		private PersistentValue<T>.ValueSetterDelegate ValueSetter { get; set; }

		public void Load()
		{
			if (this.ValueLoader != null)
			{
				this.Value = this.ValueLoader(this.Key);
			}
		}

		public void Flush()
		{
			if (this.ValueSetter != null)
			{
				this.ValueSetter(this.Key, this.Value);
			}
		}

		public void Reset()
		{
			this.value = this.DefaultValue;
			this.Flush();
		}

		private bool IsValidSet(T newValue)
		{
			if (this.PersistenceType == PersistenceType.Latest)
			{
				return true;
			}
			if (newValue is DateTime)
			{
				DateTime t = (DateTime)((object)newValue);
				DateTime t2 = (DateTime)((object)this.value);
				return (this.PersistenceType != PersistenceType.Highest) ? (t < t2) : (t.Ticks > t2.Ticks);
			}
			if (newValue is long)
			{
				long num = long.Parse(newValue.ToString());
				long num2 = long.Parse(this.value.ToString());
				return (this.PersistenceType != PersistenceType.Highest) ? (num < num2) : (num > num2);
			}
			if (newValue is decimal)
			{
				decimal d = decimal.Parse(newValue.ToString());
				decimal d2 = decimal.Parse(this.value.ToString());
				return (this.PersistenceType != PersistenceType.Highest) ? (d < d2) : (d > d2);
			}
			if (!(newValue is bool) && !(newValue is string))
			{
				double num3 = double.Parse(newValue.ToString());
				double num4 = double.Parse(this.value.ToString());
				return (this.PersistenceType != PersistenceType.Highest) ? (num3 < num4) : (num3 > num4);
			}
			if (!(newValue is string))
			{
				bool flag = bool.Parse(newValue.ToString());
				bool flag2 = bool.Parse(this.value.ToString());
				return (this.PersistenceType != PersistenceType.Highest) ? (!flag && flag2) : (flag && !flag2);
			}
			int length = newValue.ToString().Length;
			int length2 = this.value.ToString().Length;
			return (this.PersistenceType != PersistenceType.Highest) ? (length < length2) : (length > length2);
		}

		private T value;

		protected delegate T ValueLoaderDelegate(string key);

		protected delegate void ValueSetterDelegate(string key, T value);
	}
}
