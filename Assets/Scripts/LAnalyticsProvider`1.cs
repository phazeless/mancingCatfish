using System;
using System.Collections.Generic;

public abstract class LAnalyticsProvider<T> : ILAnalyticsProvider<T>, ILAnalyticsProvider where T : ILAnalyticsSettings
{
	protected LAnalyticsProvider(T settings)
	{
		this.Settings = settings;
	}

	public T Settings { get; protected set; }

	public abstract bool LogEvent(string eventName, string eventValue);

	public abstract bool LogEvent(string eventName, Dictionary<string, object> eventValues);

	public abstract void LogPurchase(ILAnalyticsReceiptData receiptData);

	public abstract void SetDebug(bool useDebug);
}
