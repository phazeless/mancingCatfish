using System;
using System.Collections.Generic;


public class FacebookLAnalytics : LAnalyticsProvider<FacebookLAnalyticsSettings>
{
	public FacebookLAnalytics(FacebookLAnalyticsSettings settings) : base(settings)
	{
	}

	public override bool LogEvent(string eventName, string eventValue)
	{
		
		return true;
	}

	public override bool LogEvent(string eventName, Dictionary<string, object> eventValues)
	{
		
		return true;
	}

	public override void LogPurchase(ILAnalyticsReceiptData receiptData)
	{
	}

	public override void SetDebug(bool useDebug)
	{
	}
}
