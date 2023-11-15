using System;

namespace Soomla
{
	public class RewardStorage
	{
		private static RewardStorage instance
		{
			get
			{
				return RewardStorage._instance = ((RewardStorage._instance != null) ? RewardStorage._instance : new RewardStorageUnity());
			}
		}

		public static void SetRewardStatus(Reward reward, bool give)
		{
			RewardStorage.SetRewardStatus(reward, give, true);
		}

		public static void SetRewardStatus(Reward reward, bool give, bool notify)
		{
			RewardStorage.instance._setTimesGiven(reward, give, notify);
		}

		public static bool IsRewardGiven(Reward reward)
		{
			return RewardStorage.GetTimesGiven(reward) > 0;
		}

		public static int GetTimesGiven(Reward reward)
		{
			return RewardStorage.instance._getTimesGiven(reward);
		}

		public static DateTime GetLastGivenTime(Reward reward)
		{
			return RewardStorage.instance._getLastGivenTime(reward);
		}

		public static int GetLastSeqIdxGiven(SequenceReward reward)
		{
			return RewardStorage.instance._getLastSeqIdxGiven(reward);
		}

		public static void SetLastSeqIdxGiven(SequenceReward reward, int idx)
		{
			RewardStorage.instance._setLastSeqIdxGiven(reward, idx);
		}

		protected virtual int _getLastSeqIdxGiven(SequenceReward seqReward)
		{
			return 0;
		}

		protected virtual void _setLastSeqIdxGiven(SequenceReward seqReward, int idx)
		{
		}

		protected virtual void _setTimesGiven(Reward reward, bool up, bool notify)
		{
		}

		protected virtual int _getTimesGiven(Reward reward)
		{
			return 0;
		}

		protected virtual DateTime _getLastGivenTime(Reward reward)
		{
			return default(DateTime);
		}

		protected const string TAG = "SOOMLA RewardStorage";

		private static RewardStorage _instance;
	}
}
