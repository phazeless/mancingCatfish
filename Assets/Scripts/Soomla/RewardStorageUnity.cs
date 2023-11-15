using System;

namespace Soomla
{
	public class RewardStorageUnity : RewardStorage
	{
		protected override int _getLastSeqIdxGiven(SequenceReward seqReward)
		{
			string key = RewardStorageUnity.keyRewardIdxSeqGiven(seqReward.ID);
			string @string = EncryptedPlayerPrefs.GetString(key);
			if (string.IsNullOrEmpty(@string))
			{
				return -1;
			}
			return int.Parse(@string);
		}

		protected override void _setLastSeqIdxGiven(SequenceReward seqReward, int idx)
		{
			string key = RewardStorageUnity.keyRewardIdxSeqGiven(seqReward.ID);
			EncryptedPlayerPrefs.SetString(key, idx.ToString(), true);
		}

		protected override void _setTimesGiven(Reward reward, bool up, bool notify)
		{
			int num = this._getTimesGiven(reward) + ((!up) ? -1 : 1);
			if (num < 0)
			{
				num = 0;
			}
			string key = RewardStorageUnity.keyRewardTimesGiven(reward.ID);
			EncryptedPlayerPrefs.SetString(key, num.ToString(), true);
			if (up)
			{
				key = RewardStorageUnity.keyRewardLastGiven(reward.ID);
				EncryptedPlayerPrefs.SetString(key, (DateTime.Now.Ticks / 10000L).ToString(), true);
			}
			if (notify)
			{
				if (up)
				{
					CoreEvents.OnRewardGiven(reward);
				}
				else
				{
					CoreEvents.OnRewardTaken(reward);
				}
			}
		}

		protected override int _getTimesGiven(Reward reward)
		{
			string key = RewardStorageUnity.keyRewardTimesGiven(reward.ID);
			string @string = EncryptedPlayerPrefs.GetString(key);
			if (string.IsNullOrEmpty(@string))
			{
				return 0;
			}
			return int.Parse(@string);
		}

		protected override DateTime _getLastGivenTime(Reward reward)
		{
			string key = RewardStorageUnity.keyRewardLastGiven(reward.ID);
			string @string = EncryptedPlayerPrefs.GetString(key);
			if (string.IsNullOrEmpty(@string))
			{
				return default(DateTime);
			}
			long num = Convert.ToInt64(@string);
			return new DateTime(TimeSpan.FromMilliseconds((double)num).Ticks);
		}

		private static string keyRewards(string rewardId, string postfix)
		{
			return "soomla.rewards." + rewardId + "." + postfix;
		}

		private static string keyRewardIdxSeqGiven(string rewardId)
		{
			return RewardStorageUnity.keyRewards(rewardId, "seq.idx");
		}

		private static string keyRewardTimesGiven(string rewardId)
		{
			return RewardStorageUnity.keyRewards(rewardId, "timesGiven");
		}

		private static string keyRewardLastGiven(string rewardId)
		{
			return RewardStorageUnity.keyRewards(rewardId, "lastGiven");
		}
	}
}
