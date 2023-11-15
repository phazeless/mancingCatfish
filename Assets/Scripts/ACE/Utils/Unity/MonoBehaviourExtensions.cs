using System;
using System.Collections;
using UnityEngine;

namespace ACE.Utils.Unity
{
	public static class MonoBehaviourExtensions
	{
		public static void RunWithExponentialBackoff(this MonoBehaviour mono, int timeoutAfterTries, Func<bool> methodToRun, Action<bool, int, bool> resultCallback)
		{
			MonoBehaviourExtensions.InternalRunWithExponentialBackoff(mono, 0, 0, timeoutAfterTries, methodToRun, resultCallback);
		}

		public static void RunAfterDelay(this MonoBehaviour mono, float delay, Action methodToRun)
		{
			mono.StartCoroutine(MonoBehaviourExtensions.InternalRunAfterDelay(delay, methodToRun));
		}

		private static void InternalRunWithExponentialBackoff(MonoBehaviour mono, int delay, int count, int timeoutAfterTries, Func<bool> methodToRun, Action<bool, int, bool> resultCallback)
		{
			int newCount = count + 1;
			int newDelay = (int)(Mathf.Pow(2f, (float)count) - 1f);
			if (newCount > timeoutAfterTries)
			{
				return;
			}
			mono.StartCoroutine(MonoBehaviourExtensions.InternalRunAfterDelay((float)newDelay, delegate
			{
				bool flag = methodToRun();
				bool arg = newCount == timeoutAfterTries;
				resultCallback(flag, newCount, arg);
				if (!flag)
				{
					MonoBehaviourExtensions.InternalRunWithExponentialBackoff(mono, newDelay, newCount, timeoutAfterTries, methodToRun, resultCallback);
				}
			}));
		}

		private static IEnumerator InternalRunAfterDelay(float delay, Action methodToRun)
		{
			yield return new WaitForSeconds(delay);
			if (methodToRun != null)
			{
				methodToRun();
			}
			yield break;
		}
	}
}
