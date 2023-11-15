using System;
using Newtonsoft.Json.Linq;

namespace Soomla.Store
{
	public class GoogleReceiptAndSignature
	{
		public GoogleReceiptAndSignature(string unityReceiptAsJson)
		{
			JObject jobject = JObject.Parse(unityReceiptAsJson);
			JObject jobject2 = JObject.Parse(jobject["Payload"].Value<string>());
			this.receipt = jobject2["json"].Value<string>();
			this.signature = jobject2["signature"].Value<string>();
		}

		public string receipt;

		public string signature;
	}
}
