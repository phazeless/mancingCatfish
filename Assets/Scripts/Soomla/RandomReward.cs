using System;
using System.Collections.Generic;

namespace Soomla
{
	public class RandomReward : Reward
	{
		public RandomReward(string id, string name, List<Reward> rewards) : base(id, name)
		{
			if (rewards == null || rewards.Count == 0)
			{
				SoomlaUtils.LogError(RandomReward.TAG, "This reward doesn't make sense without items");
			}
			this.Rewards = rewards;
		}

		public RandomReward(JSONObject jsonReward) : base(jsonReward)
		{
			List<JSONObject> list = jsonReward["rewards"].list;
			if (list == null || list.Count == 0)
			{
				SoomlaUtils.LogWarning(RandomReward.TAG, "Reward has no meaning without children");
				list = new List<JSONObject>();
			}
			this.Rewards = new List<Reward>();
			foreach (JSONObject rewardObj in list)
			{
				this.Rewards.Add(Reward.fromJSONObject(rewardObj));
			}
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jsonobject = base.toJSONObject();
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (Reward reward in this.Rewards)
			{
				jsonobject2.Add(reward.toJSONObject());
			}
			jsonobject.AddField("rewards", jsonobject2);
			return jsonobject;
		}

		protected override bool giveInner()
		{
			List<Reward> list = new List<Reward>();
			foreach (Reward reward in this.Rewards)
			{
				if (reward.CanGive())
				{
					list.Add(reward);
				}
			}
			if (list.Count == 0)
			{
				SoomlaUtils.LogDebug(RandomReward.TAG, "No more rewards to give in this Random Reward: " + base.ID);
				return false;
			}
			Random random = new Random();
			int index = random.Next(list.Count);
			Reward reward2 = list[index];
			reward2.Give();
			this.LastGivenReward = reward2;
			return true;
		}

		protected override bool takeInner()
		{
			if (this.LastGivenReward == null)
			{
				return false;
			}
			bool result = this.LastGivenReward.Take();
			this.LastGivenReward = null;
			return result;
		}

		private static string TAG = "SOOMLA RandomReward";

		public List<Reward> Rewards;

		public Reward LastGivenReward;
	}
}
