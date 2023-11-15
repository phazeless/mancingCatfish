using System;

namespace TMPro
{
	public static class TMP_Math
	{
		public static bool Approximately(float a, float b)
		{
			return b - 0.0001f < a && a < b + 0.0001f;
		}

		public const float FLOAT_MAX = 32768f;

		public const float FLOAT_MIN = -32768f;

		public const int INT_MAX = 2147483647;

		public const int INT_MIN = -2147483647;
	}
}
