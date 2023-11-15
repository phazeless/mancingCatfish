using System;
using System.Collections.Generic;

public class LAnalyticsIAPReceiptDataIOS : ILAnalyticsReceiptData
{
	public LAnalyticsIAPReceiptDataIOS(string productIdentifier, string transactionId, decimal price, string currency, string receipt)
	{
		this.ProductIdentifier = productIdentifier;
		this.TransactionId = transactionId;
		this.PriceAsDecimal = price;
		this.Price = price.ToString();
		this.Currency = currency;
		this.Receipt = receipt;
	}

	public string ProductIdentifier { get; set; }

	public string Currency { get; set; }

	public decimal PriceAsDecimal { get; set; }

	public string TransactionId;

	public string Price;

	public string Receipt;

	public Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();
}
