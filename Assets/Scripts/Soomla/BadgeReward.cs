using System;

namespace Soomla
{
	public class BadgeReward : Reward
	{
		public BadgeReward(string id, string name) : base(id, name)
		{
		}

		public BadgeReward(string id, string name, string iconUrl) : base(id, name)
		{
			this.IconUrl = iconUrl;
		}

		public BadgeReward(JSONObject jsonReward) : base(jsonReward)
		{
			this.IconUrl = jsonReward["iconUrl"].str;
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jsonobject = base.toJSONObject();
			jsonobject.AddField("iconUrl", this.IconUrl);
			return jsonobject;
		}

		protected override bool giveInner()
		{
			return true;
		}

		protected override bool takeInner()
		{
			return true;
		}

		public string IconUrl;
	}
}
