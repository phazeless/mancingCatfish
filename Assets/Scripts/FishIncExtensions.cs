using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public static class FishIncExtensions
{
	public static Dictionary<T, int> DistinctAndCount<T>(this List<T> items)
	{
		Dictionary<T, int> dictionary = new Dictionary<T, int>();
		foreach (T t in items)
		{
			if (dictionary.ContainsKey(t))
			{
				Dictionary<T, int> dictionary2 = dictionary;
				T key=t;
				(dictionary2 = dictionary)[key = t] = dictionary2[key] + 1;
			}
			else
			{
				dictionary.Add(t, 1);
			}
		}
		return dictionary;
	}

	public static BigInteger MultiplyFloat(this BigInteger bigInteger, float value)
	{
		if (value == 0f)
		{
			return BigInteger.Zero;
		}
		bool flag = value > 9999f;
		if (flag)
		{
			BigInteger right = new BigInteger(Mathf.RoundToInt(value)) * 10000;
			return bigInteger * right / 10000;
		}
		int value2 = (int)(value * 10000f);
		return bigInteger * value2 / 10000;
	}

	public static float UnitsPerPixel(this Camera cam)
	{
        UnityEngine.Vector3 a = cam.ScreenToWorldPoint(UnityEngine.Vector2.zero);
        UnityEngine.Vector3 b = cam.ScreenToWorldPoint(UnityEngine.Vector2.up);
		return UnityEngine.Vector3.Distance(a, b);
	}

	public static float PixelsPerUnit(this Camera cam)
	{
		return 1f / cam.UnitsPerPixel();
	}

	public static void LookAt2D(this Transform transform, UnityEngine.Vector3 target)
	{
        UnityEngine.Vector3 vector = target - transform.position;
		vector.Normalize();
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		transform.rotation = UnityEngine.Quaternion.Euler(0f, 0f, num - 90f);
	}

	public static void RunAfterDelay(this MonoBehaviour go, float runAfterSeconds, Action callbackToRunAfterDelay)
	{
		if (go.isActiveAndEnabled)
		{
			go.StartCoroutine(FishIncExtensions.RunAfterDelay(runAfterSeconds, callbackToRunAfterDelay));
		}
	}

	private static IEnumerator RunAfterDelay(float afterSeconds, Action callback)
	{
		yield return new WaitForSeconds(afterSeconds);
		if (callback != null)
		{
			callback();
		}
		yield break;
	}
}
