using System;

public interface ILAnalyticsReceiptData
{
	decimal PriceAsDecimal { get; set; }

	string Currency { get; set; }

	string ProductIdentifier { get; set; }
}
