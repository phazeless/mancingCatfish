using System;
using System.Collections.Generic;

public interface ILAnalyticsProvider
{
	void SetDebug(bool useDebug);

	bool LogEvent(string eventName, string eventValue);

	bool LogEvent(string eventName, Dictionary<string, object> eventValues);

	void LogPurchase(ILAnalyticsReceiptData receiptData);
}
