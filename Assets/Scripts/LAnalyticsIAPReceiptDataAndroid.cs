using System;
using System.Collections.Generic;

public class LAnalyticsIAPReceiptDataAndroid : ILAnalyticsReceiptData
{
	public LAnalyticsIAPReceiptDataAndroid(string productIdentifier, string publicKey, string purchaseData, string signature, decimal price, string currency)
	{
		this.ProductIdentifier = productIdentifier;
		this.PublicKey = publicKey;
		this.PurchaseData = purchaseData;
		this.Signature = signature;
		this.PriceAsDecimal = price;
		this.Price = price.ToString();
		this.Currency = currency;
	}

	public string ProductIdentifier { get; set; }

	public string Currency { get; set; }

	public decimal PriceAsDecimal { get; set; }

	public string PublicKey;

	public string PurchaseData;

	public string Signature;

	public string Price;

	public Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();
}
