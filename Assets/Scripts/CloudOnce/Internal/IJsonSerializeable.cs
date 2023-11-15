using System;

namespace CloudOnce.Internal
{
	public interface IJsonSerializeable
	{
		JSONObject ToJSONObject();
	}
}
