using System;
using System.Collections.Generic;

namespace Soomla
{
	public abstract class Reward : SoomlaEntity<Reward>
	{
		public Reward(string id, string name) : base(id, name, string.Empty)
		{
			this.Schedule = Schedule.AnyTimeOnce();
			Reward.RewardsMap.AddOrUpdate(base.ID, this);
		}

		public Reward(JSONObject jsonReward) : base(jsonReward)
		{
			JSONObject jsonobject = jsonReward["schedule"];
			if (jsonobject)
			{
				this.Schedule = new Schedule(jsonobject);
			}
			else
			{
				this.Schedule = null;
			}
			Reward.RewardsMap.AddOrUpdate(base.ID, this);
		}

		public bool Owned
		{
			get
			{
				return RewardStorage.IsRewardGiven(this);
			}
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jsonobject = base.toJSONObject();
			if (this.Schedule != null)
			{
				jsonobject.AddField("schedule", this.Schedule.toJSONObject());
			}
			else
			{
				jsonobject.AddField("schedule", Schedule.AnyTimeOnce().toJSONObject());
			}
			return jsonobject;
		}

		public static Reward fromJSONObject(JSONObject rewardObj)
		{
			string str = rewardObj["className"].str;
			Reward reward = (Reward)Activator.CreateInstance(Type.GetType("Soomla." + str), new object[]
			{
				rewardObj
			});
			Reward.RewardsMap.AddOrUpdate(reward.ID, reward);
			return reward;
		}

		public bool Take()
		{
			if (!RewardStorage.IsRewardGiven(this))
			{
				SoomlaUtils.LogDebug(Reward.TAG, "Reward not given. id: " + this._id);
				return false;
			}
			if (this.takeInner())
			{
				RewardStorage.SetRewardStatus(this, false);
				return true;
			}
			return false;
		}

		public bool CanGive()
		{
			return this.Schedule.Approve(RewardStorage.GetTimesGiven(this));
		}

		public bool Give()
		{
			if (!this.CanGive())
			{
				SoomlaUtils.LogDebug(Reward.TAG, "(Give) Reward is not approved by Schedule. id: " + this._id);
				return false;
			}
			if (this.giveInner())
			{
				RewardStorage.SetRewardStatus(this, true);
				return true;
			}
			return false;
		}

		protected abstract bool giveInner();

		protected abstract bool takeInner();

		public static Reward GetReward(string rewardID)
		{
			Reward result = null;
			Reward.RewardsMap.TryGetValue(rewardID, out result);
			return result;
		}

		public static List<Reward> GetRewards()
		{
			List<Reward> list = new List<Reward>();
			foreach (Reward item in Reward.RewardsMap.Values)
			{
				list.Add(item);
			}
			return list;
		}

		private static string TAG = "SOOMLA Reward";

		public Schedule Schedule;

		private static Dictionary<string, Reward> RewardsMap = new Dictionary<string, Reward>();
	}
}
