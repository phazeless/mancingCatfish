using System;

namespace Soomla.Store
{
	public class MarketItem
	{
		public MarketItem(string productId, double price)
		{
			this.ProductId = productId;
			this.Price = price;
		}

		public MarketItem(JSONObject jsonObject)
		{
			string text = string.Empty;
			text = "androidId";
			if (!string.IsNullOrEmpty(text) && jsonObject.HasField(text))
			{
				this.ProductId = jsonObject[text].str;
			}
			else
			{
				this.ProductId = jsonObject["productId"].str;
			}
			this.Price = jsonObject["price"].n;
			if (jsonObject["marketPrice"])
			{
				this.MarketPriceAndCurrency = jsonObject["marketPrice"].str;
			}
			else
			{
				this.MarketPriceAndCurrency = string.Empty;
			}
			if (jsonObject["marketTitle"])
			{
				this.MarketTitle = jsonObject["marketTitle"].str;
			}
			else
			{
				this.MarketTitle = string.Empty;
			}
			if (jsonObject["marketDesc"])
			{
				this.MarketDescription = jsonObject["marketDesc"].str;
			}
			else
			{
				this.MarketDescription = string.Empty;
			}
			if (jsonObject["marketCurrencyCode"])
			{
				this.MarketCurrencyCode = jsonObject["marketCurrencyCode"].str;
			}
			else
			{
				this.MarketCurrencyCode = string.Empty;
			}
			if (jsonObject["marketPriceMicros"])
			{
				this.MarketPriceMicros = Convert.ToInt64(jsonObject["marketPriceMicros"].n);
			}
			else
			{
				this.MarketPriceMicros = 0L;
			}
		}

		public JSONObject toJSONObject()
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject.AddField("className", SoomlaUtils.GetClassName(this));
			jsonobject.AddField("productId", this.ProductId);
			jsonobject.AddField("price", (float)this.Price);
			jsonobject.AddField("marketPrice", this.MarketPriceAndCurrency);
			jsonobject.AddField("marketTitle", this.MarketTitle);
			jsonobject.AddField("marketDesc", this.MarketDescription);
			jsonobject.AddField("marketCurrencyCode", this.MarketCurrencyCode);
			jsonobject.AddField("marketPriceMicros", (float)this.MarketPriceMicros);
			return jsonobject;
		}

		public string ProductId;

		public double Price;

		public string MarketPriceAndCurrency;

		public string MarketTitle;

		public string MarketDescription;

		public string MarketCurrencyCode;

		public long MarketPriceMicros;
	}
}
