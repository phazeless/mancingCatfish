using System;
using System.Collections.Generic;

public class ResourceChangeData
{
	public ResourceChangeData()
	{
		this.ContentId = "Unknown";
		this.ContentName = "Unknown";
		this.ResourceChangeReason = ResourceChangeReason.Unknown;
		this.ResourceChangeType = ResourceChangeType.Unknown;
	}

	public ResourceChangeData(string contentId, string contentName, int amount, ResourceType resourceType, ResourceChangeType resourceChangeType, ResourceChangeReason reason)
	{
		this.ContentId = contentId;
		this.ContentName = (contentName ?? Enum.GetName(reason.GetType(), reason));
		this.Amount = amount;
		this.ResourceType = resourceType;
		this.ResourceChangeReason = reason;
		this.ResourceChangeType = resourceChangeType;
	}

	public string GetAnalyticsEventName()
	{
		return this.ResourceChangeType + "_Resource";
	}

	public Dictionary<string, object> GetAnalyticsDictionary()
	{
		return new Dictionary<string, object>
		{
			{
				"contentId",
				this.ContentId
			},
			{
				"contentName",
				this.ContentName
			},
			{
				"amount",
				this.Amount
			},
			{
				"resourceType",
				(int)this.ResourceType
			},
			{
				"resourceChangeType",
				(int)this.ResourceChangeType
			},
			{
				"resourceChangeReason",
				(int)this.ResourceChangeReason
			}
		};
	}

	public string GetNormalizedAnalyticsEventName()
	{
		return this.ResourceChangeType.ToString().ToLower() + this.ResourceType.ToString();
	}

	public Dictionary<string, object> GetNormalizedAnalyticsDictionary()
	{
		return new Dictionary<string, object>
		{
			{
				"resourceAmount",
				this.Amount
			},
			{
				"evSource",
				this.ResourceChangeReason.ToString()
			},
			{
				"additionalId",
				this.ContentId
			},
			{
				"additionalInfo",
				this.ContentName
			}
		};
	}

	public string ContentId;

	public string ContentName;

	public int Amount;

	public ResourceChangeReason ResourceChangeReason = ResourceChangeReason.Unknown;

	public ResourceType ResourceType = ResourceType.Gems;

	public ResourceChangeType ResourceChangeType = ResourceChangeType.Unknown;
}
