using System;

public interface ILAnalyticsProvider<T> : ILAnalyticsProvider where T : ILAnalyticsSettings
{
	T Settings { get; }
}
