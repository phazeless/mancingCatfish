using System;
using System.Collections.Generic;

namespace Soomla
{
	public class SequenceReward : Reward
	{
		public SequenceReward(string id, string name, List<Reward> rewards) : base(id, name)
		{
			if (rewards == null || rewards.Count == 0)
			{
				SoomlaUtils.LogError(SequenceReward.TAG, "This reward doesn't make sense without items");
			}
			this.Rewards = rewards;
		}

		public SequenceReward(JSONObject jsonReward) : base(jsonReward)
		{
			List<JSONObject> list = jsonReward["rewards"].list;
			if (list == null || list.Count == 0)
			{
				SoomlaUtils.LogWarning(SequenceReward.TAG, "Reward has no meaning without children");
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

		public Reward GetLastGivenReward()
		{
			int lastSeqIdxGiven = RewardStorage.GetLastSeqIdxGiven(this);
			if (lastSeqIdxGiven < 0)
			{
				return null;
			}
			return this.Rewards[lastSeqIdxGiven];
		}

		public bool HasMoreToGive()
		{
			return RewardStorage.GetLastSeqIdxGiven(this) < this.Rewards.Count;
		}

		public bool ForceNextRewardToGive(Reward reward)
		{
			for (int i = 0; i < this.Rewards.Count; i++)
			{
				if (this.Rewards[i].ID == reward.ID)
				{
					RewardStorage.SetLastSeqIdxGiven(this, i - 1);
					return true;
				}
			}
			return false;
		}

		protected override bool giveInner()
		{
			int lastSeqIdxGiven = RewardStorage.GetLastSeqIdxGiven(this);
			if (lastSeqIdxGiven >= this.Rewards.Count)
			{
				return false;
			}
			RewardStorage.SetLastSeqIdxGiven(this, lastSeqIdxGiven + 1);
			return true;
		}

		protected override bool takeInner()
		{
			int lastSeqIdxGiven = RewardStorage.GetLastSeqIdxGiven(this);
			if (lastSeqIdxGiven <= 0)
			{
				return false;
			}
			RewardStorage.SetLastSeqIdxGiven(this, lastSeqIdxGiven - 1);
			return true;
		}

		private static string TAG = "SOOMLA SequenceReward";

		public List<Reward> Rewards;
	}
}
