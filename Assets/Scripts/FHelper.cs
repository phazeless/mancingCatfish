using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FHelper
{
	public static void GetTime(bool fromInternet, Action<bool, DateTime> onCheckComplete)
	{
		FHelperMono.Instance.StartCoroutine(FHelper.InternalGetTime(fromInternet, onCheckComplete));
	}

	private static IEnumerator InternalGetTime(bool fromInternet, Action<bool, DateTime> onCheckComplete)
	{
		bool didSucceed = false;
		DateTime realTime = DateTime.Now;
		if (fromInternet)
		{
			WWW req = new WWW("https://google.com");
			yield return req;
			string timeAsString = null;
			bool hasInternet = req.error == null;
			if (hasInternet && req.responseHeaders.TryGetValue("Date", out timeAsString))
			{
				realTime = DateTime.Parse(timeAsString);
				didSucceed = true;
			}
		}
		if (onCheckComplete != null)
		{
			onCheckComplete(didSucceed, realTime);
		}
		yield break;
	}

	public static void HasInternet(Action<bool> onCheckComplete)
	{
		FHelperMono.Instance.StartCoroutine(FHelper.InternalHasInternet(onCheckComplete));
	}

	private static IEnumerator InternalHasInternet(Action<bool> onCheckComplete)
	{
		WWW req = new WWW("https://google.com");
		yield return req;
		bool hasInternet = req.error == null;
		if (onCheckComplete != null)
		{
			onCheckComplete(hasInternet);
		}
		yield break;
	}

	public static string FindBracketAndReplace(string originalString, params string[] replaceWithStrings)
	{
		return string.Format(originalString, replaceWithStrings).TrimEnd(new char[]
		{
			'\r',
			'\n'
		});
	}

	public static string FromSecondsToHoursMinutesSecondsFormat(float seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)seconds);
		string result = string.Empty;
		if (timeSpan.Hours > 0)
		{
			result = string.Format("{0:D2}h {1:D2}m {2:D2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
		else if (timeSpan.Minutes > 0)
		{
			result = string.Format("{1:D2}m {2:D2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
		else if (timeSpan.Seconds > 0)
		{
			result = string.Format("{2:D2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
		return result;
	}

	public static string FromSecondsToDaysHoursMinutesSecondsFormatMaxTwo(float seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)seconds);
		string result = string.Empty;
		if (timeSpan.Days > 0)
		{
			result = string.Format("{0:D1}d {0:D1}h", new object[]
			{
				timeSpan.Days,
				timeSpan.Hours,
				timeSpan.Minutes,
				timeSpan.Seconds
			});
		}
		else if (timeSpan.Hours > 0)
		{
			result = string.Format("{0:D1}h {1:D1}m", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
		else if (timeSpan.Minutes > 0)
		{
			result = string.Format("{1:D1}m {2:D1}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
		else if (timeSpan.Seconds > 0)
		{
			result = string.Format("{2:D1}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
		return result;
	}

	public static int GetValueWithChance(int[] values, int[] chances)
	{
		if (values.Length != chances.Length)
		{
			throw new InvalidCastException("The amount of values must be the exact same amount as chances!");
		}
		List<int> list = new List<int>();
		for (int i = 0; i < values.Length; i++)
		{
			for (int j = 0; j < chances[i]; j++)
			{
				list.Add(values[i]);
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static bool DidRollWithChance(float chance)
	{
		return FHelper.DidRollSide(chance, 100f);
	}

	public static bool DidRollSide(float sideToRoll, float sides)
	{
		float num = UnityEngine.Random.Range(0f, 1f);
		float num2 = sideToRoll / sides;
		return num <= num2;
	}

	public static int RollDice(int sides)
	{
		return UnityEngine.Random.Range(1, sides + 1);
	}

	public static float RoundToDecimals(float value)
	{
		return (float)Math.Round((double)value, 2);
	}

	public static bool HasSecondsPassed(float seconds, ref float timer, bool resetValue = true)
	{
		if (timer > seconds)
		{
			if (resetValue)
			{
				timer = 0f;
			}
			return true;
		}
		timer += Time.deltaTime;
		return false;
	}

	public static bool HasSecondsPassedSince(float seconds, DateTime since, bool useRealTime = false)
	{
		DateTime d = (!useRealTime) ? DateTime.Now : TimeManager.Instance.RealNow;
		return (d - since).TotalSeconds > (double)seconds;
	}

	public static Vector2 GetRandomEdgeScreenCoordinate()
	{
		float num = UnityEngine.Random.Range(0f, 1f);
		float num2 = UnityEngine.Random.Range(0f, 1f);
		float num3 = (num <= num2) ? (1f / num2) : (1f / num);
		float num4 = num * num3 - (float)UnityEngine.Random.Range(0, 1 + (int)(num * num3));
		float num5 = num2 * num3 - (float)UnityEngine.Random.Range(0, 1 + (int)(num2 * num3));
		return new Vector2(num4 * (float)Screen.width, num5 * (float)Screen.height);
	}
}
