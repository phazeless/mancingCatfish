using System;
using UnityEngine;

namespace Soomla
{
	public class RewardStorageAndroid : RewardStorage
	{
		protected override int _getLastSeqIdxGiven(SequenceReward reward)
		{
			int result = -1;
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.data.RewardStorage"))
			{
				result = androidJavaClass.CallStatic<int>("getLastSeqIdxGiven", new object[]
				{
					reward.ID
				});
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return result;
		}

		protected override void _setLastSeqIdxGiven(SequenceReward reward, int idx)
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.data.RewardStorage"))
			{
				androidJavaClass.CallStatic("setLastSeqIdxGiven", new object[]
				{
					reward.ID,
					idx
				});
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _setTimesGiven(Reward reward, bool up, bool notify)
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.data.RewardStorage"))
			{
				androidJavaClass.CallStatic("setTimesGiven", new object[]
				{
					reward.ID,
					up,
					notify
				});
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override int _getTimesGiven(Reward reward)
		{
			int result = 0;
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.data.RewardStorage"))
			{
				result = androidJavaClass.CallStatic<int>("getTimesGiven", new object[]
				{
					reward.ID
				});
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return result;
		}

		protected override DateTime _getLastGivenTime(Reward reward)
		{
			long num = 0L;
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.data.RewardStorage"))
			{
				num = androidJavaClass.CallStatic<long>("getLastGivenTimeMillis", new object[]
				{
					reward.ID
				});
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return new DateTime(TimeSpan.FromMilliseconds((double)num).Ticks);
		}
	}
}
