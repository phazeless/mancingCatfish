using System;
using System.Collections.Generic;

namespace Soomla.Store
{
	public class VirtualCategory
	{
		public VirtualCategory(string name, List<string> goodItemIds)
		{
			this.Name = name;
			this.GoodItemIds = goodItemIds;
		}

		public VirtualCategory(JSONObject jsonItem)
		{
			this.Name = jsonItem["name"].str;
			JSONObject jsonobject = jsonItem["goods_itemIds"];
			foreach (JSONObject jsonobject2 in jsonobject.list)
			{
				this.GoodItemIds.Add(jsonobject2.str);
			}
		}

		public JSONObject toJSONObject()
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject.AddField("className", SoomlaUtils.GetClassName(this));
			jsonobject.AddField("name", this.Name);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (string str in this.GoodItemIds)
			{
				jsonobject2.Add(str);
			}
			jsonobject.AddField("goods_itemIds", jsonobject2);
			return jsonobject;
		}

		private const string TAG = "SOOMLA VirtualCategory";

		public string Name;

		public List<string> GoodItemIds = new List<string>();
	}
}
