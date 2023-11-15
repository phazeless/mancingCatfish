using System;

namespace Soomla
{
	public class RewardTakenEvent : SoomlaEvent
	{
		public RewardTakenEvent(string rewardId) : this(rewardId, null)
		{
		}

		public RewardTakenEvent(Reward reward) : this(reward, null)
		{
		}

		public RewardTakenEvent(string rewardId, object sender) : base(sender)
		{
			this.Reward = Reward.GetReward(rewardId);
		}

		public RewardTakenEvent(Reward reward, object sender) : base(sender)
		{
			this.Reward = reward;
		}

		public readonly Reward Reward;
	}
}
