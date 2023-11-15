using System;
using System.Collections.Generic;

public class LAnalytics
{
	public LAnalytics(params ILAnalyticsProvider[] providers)
	{
		this.providers = new List<ILAnalyticsProvider>(providers);
	}

	public void AddDefaultParamFunc(Func<KeyValuePair<string, object>> func)
	{
		this.defaultParamsAsFuncs.Add(func);
	}

	public void LogEvent<T>(string eventName, Dictionary<string, object> eventValues) where T : ILAnalyticsProvider
	{
		ILAnalyticsProvider p = this.providers.Find((ILAnalyticsProvider x) => x.GetType() == typeof(T));
		if (p != null)
		{
			Func<bool> func = () => p.LogEvent(eventName, this.MergeWithDefaultParameters(eventValues));
			if (!func())
			{
				this.pendingEvents.Add(func);
			}
		}
	}

	public void LogEvent(string eventName, string eventValue)
	{
		using (List<ILAnalyticsProvider>.Enumerator enumerator = this.providers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ILAnalyticsProvider p = enumerator.Current;
				Func<bool> func = () => p.LogEvent(eventName, eventValue);
				if (!func())
				{
					this.pendingEvents.Add(func);
				}
			}
		}
	}

	public void LogEvent(string eventName, Dictionary<string, object> eventValues)
	{
		using (List<ILAnalyticsProvider>.Enumerator enumerator = this.providers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ILAnalyticsProvider p = enumerator.Current;
				Func<bool> func = () => p.LogEvent(eventName, this.MergeWithDefaultParameters(eventValues));
				if (!func())
				{
					this.pendingEvents.Add(func);
				}
			}
		}
	}

	public void LogPurchase(ILAnalyticsReceiptData receiptData)
	{
		foreach (ILAnalyticsProvider ilanalyticsProvider in this.providers)
		{
			ilanalyticsProvider.LogPurchase(receiptData);
		}
	}

	public void SetDebug(bool useDebug)
	{
		foreach (ILAnalyticsProvider ilanalyticsProvider in this.providers)
		{
			ilanalyticsProvider.SetDebug(useDebug);
		}
	}

	public void TrySendQueuedEvent()
	{
		if (this.pendingEvents.Count == 0)
		{
			return;
		}
		List<Func<bool>> toBeRemoved = new List<Func<bool>>();
		for (int i = 0; i < this.pendingEvents.Count; i++)
		{
			Func<bool> func = this.pendingEvents[i];
			if (func())
			{
				toBeRemoved.Add(func);
			}
		}
		this.pendingEvents.RemoveAll((Func<bool> x) => toBeRemoved.Contains(x));
	}

	private Dictionary<string, object> MergeWithDefaultParameters(Dictionary<string, object> existingParams)
	{
		foreach (Func<KeyValuePair<string, object>> func in this.defaultParamsAsFuncs)
		{
			KeyValuePair<string, object> keyValuePair = func();
			if (!existingParams.ContainsKey(keyValuePair.Key))
			{
				existingParams.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}
		return existingParams;
	}

	private List<ILAnalyticsProvider> providers = new List<ILAnalyticsProvider>();

	private List<Func<bool>> pendingEvents = new List<Func<bool>>();

	private List<Func<KeyValuePair<string, object>>> defaultParamsAsFuncs = new List<Func<KeyValuePair<string, object>>>();
}
