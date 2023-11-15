using System;
using System.Collections.Generic;
using UnityEngine;

namespace CloudOnce.Internal
{
	public class PersistentCurrency : IPersistent
	{
		protected PersistentCurrency(string key, float defaultValue, bool allowNegative)
		{
			this.Key = key;
			this.DefaultValue = defaultValue;
			this.AllowNegative = allowNegative;
			DataManager.CloudPrefs[key] = this;
			DataManager.InitDataManager();
		}

		public string Key { get; private set; }

		public float Additions
		{
			get
			{
				float num = this.thisDeviceCurrencyValue.Additions;
				if (this.deviceCurrencyValues != null)
				{
					foreach (KeyValuePair<string, CurrencyValue> keyValuePair in this.deviceCurrencyValues)
					{
						if (!(keyValuePair.Key == PersistentCurrency.DeviceID))
						{
							num += keyValuePair.Value.Additions;
						}
					}
				}
				return num;
			}
		}

		public float Subtractions
		{
			get
			{
				float num = this.thisDeviceCurrencyValue.Subtractions;
				if (this.deviceCurrencyValues != null)
				{
					foreach (KeyValuePair<string, CurrencyValue> keyValuePair in this.deviceCurrencyValues)
					{
						if (!(keyValuePair.Key == PersistentCurrency.DeviceID))
						{
							num += keyValuePair.Value.Subtractions;
						}
					}
				}
				return num;
			}
		}

		public float Value
		{
			get
			{
				float num = this.thisDeviceCurrencyValue.Value + this.DefaultValue;
				if (this.deviceCurrencyValues != null)
				{
					foreach (KeyValuePair<string, CurrencyValue> keyValuePair in this.deviceCurrencyValues)
					{
						if (!(keyValuePair.Key == PersistentCurrency.DeviceID))
						{
							num += keyValuePair.Value.Value;
						}
					}
				}
				if (!this.AllowNegative && num < 0f)
				{
					this.Value = 0f;
					return 0f;
				}
				return num;
			}
			set
			{
				if (this.AllowNegative || value >= 0f)
				{
					this.thisDeviceCurrencyValue.Value = value - this.otherDevicesValueCache - this.DefaultValue;
				}
				else
				{
					this.thisDeviceCurrencyValue.Value = -this.otherDevicesValueCache - this.DefaultValue;
				}
			}
		}

		public float DefaultValue { get; private set; }

		public bool AllowNegative { get; private set; }

		private static string DeviceID
		{
			get
			{
				if (!string.IsNullOrEmpty(PersistentCurrency.s_deviceIdCache))
				{
					return PersistentCurrency.s_deviceIdCache;
				}
				if (PlayerPrefs.HasKey("CloudOnceDeviceID"))
				{
					PersistentCurrency.s_deviceIdCache = PlayerPrefs.GetString("CloudOnceDeviceID");
					return PersistentCurrency.s_deviceIdCache;
				}
				PersistentCurrency.s_deviceIdCache = Guid.NewGuid().ToString();
				PlayerPrefs.SetString("CloudOnceDeviceID", PersistentCurrency.s_deviceIdCache);
				PlayerPrefs.Save();
				return PersistentCurrency.s_deviceIdCache;
			}
		}

		public void Flush()
		{
			if (this.deviceCurrencyValues == null)
			{
				this.deviceCurrencyValues = new Dictionary<string, CurrencyValue>();
			}
			this.deviceCurrencyValues[PersistentCurrency.DeviceID] = this.thisDeviceCurrencyValue;
			DataManager.SetCurrencyValues(this.Key, this.deviceCurrencyValues);
		}

		public void Load()
		{
			this.deviceCurrencyValues = DataManager.GetCurrencyValues(this.Key);
			if (this.deviceCurrencyValues != null)
			{
				this.thisDeviceCurrencyValue = ((!this.deviceCurrencyValues.ContainsKey(PersistentCurrency.DeviceID)) ? new CurrencyValue() : this.deviceCurrencyValues[PersistentCurrency.DeviceID]);
				this.CacheValueFromOtherDevices();
			}
			else
			{
				this.thisDeviceCurrencyValue = new CurrencyValue();
			}
		}

		public void Reset()
		{
			DataManager.ResetSyncableCurrency(this.Key);
			this.Load();
		}

		private void CacheValueFromOtherDevices()
		{
			this.otherDevicesValueCache = 0f;
			foreach (KeyValuePair<string, CurrencyValue> keyValuePair in this.deviceCurrencyValues)
			{
				if (!(keyValuePair.Key == PersistentCurrency.DeviceID))
				{
					this.otherDevicesValueCache += keyValuePair.Value.Value;
				}
			}
		}

		private const string deviceIdKey = "CloudOnceDeviceID";

		private static string s_deviceIdCache;

		private Dictionary<string, CurrencyValue> deviceCurrencyValues;

		private CurrencyValue thisDeviceCurrencyValue;

		private float otherDevicesValueCache;
	}
}
