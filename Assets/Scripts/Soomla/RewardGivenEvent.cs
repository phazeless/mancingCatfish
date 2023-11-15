using System;
using UnityEngine;

namespace Soomla
{
	public class RewardGivenEvent : SoomlaEvent
	{
		public RewardGivenEvent(string rewardId) : this(rewardId, null)
		{
		}

		public RewardGivenEvent(Reward reward) : this(reward, null)
		{
		}

		public RewardGivenEvent(string rewardId, UnityEngine.Object sender) : base(sender)
		{
			this.Reward = Reward.GetReward(rewardId);
		}

		public RewardGivenEvent(Reward reward, UnityEngine.Object sender) : base(sender)
		{
			this.Reward = reward;
		}

		public readonly Reward Reward;
	}
}
