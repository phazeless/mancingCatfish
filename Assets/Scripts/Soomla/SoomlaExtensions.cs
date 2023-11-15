using System;
using System.Collections.Generic;

namespace Soomla
{
	public static class SoomlaExtensions
	{
		public static void AddOrUpdate<T>(this Dictionary<string, T> dict, string key, T val)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = val;
			}
			else
			{
				dict.Add(key, val);
			}
		}
	}
}
