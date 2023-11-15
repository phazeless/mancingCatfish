using System;
using UnityEngine;

namespace MoreMountains.NiceVibrations
{
	public static class MMVibrationManager
	{
		public static bool Android()
		{
			return true;
		}

		public static bool iOS()
		{
			return false;
		}

		public static void Vibrate()
		{
			if (MMVibrationManager.Android())
			{
				MMVibrationManager.AndroidVibrate(MMVibrationManager.MediumDuration);
			}
			else if (MMVibrationManager.iOS())
			{
				MMVibrationManager.iOSTriggerHaptics(HapticTypes.MediumImpact);
			}
		}

		public static void Haptic(HapticTypes type)
		{
			if (MMVibrationManager.Android())
			{
				switch (type)
				{
				case HapticTypes.Selection:
					MMVibrationManager.AndroidVibrate(MMVibrationManager.LightDuration, MMVibrationManager.LightAmplitude);
					break;
				case HapticTypes.Success:
					MMVibrationManager.AndroidVibrate(MMVibrationManager._successPattern, MMVibrationManager._successPatternAmplitude, -1);
					break;
				case HapticTypes.Warning:
					MMVibrationManager.AndroidVibrate(MMVibrationManager._warningPattern, MMVibrationManager._warningPatternAmplitude, -1);
					break;
				case HapticTypes.Failure:
					MMVibrationManager.AndroidVibrate(MMVibrationManager._failurePattern, MMVibrationManager._failurePatternAmplitude, -1);
					break;
				case HapticTypes.LightImpact:
					MMVibrationManager.AndroidVibrate(MMVibrationManager.LightDuration, MMVibrationManager.LightAmplitude);
					break;
				case HapticTypes.MediumImpact:
					MMVibrationManager.AndroidVibrate(MMVibrationManager.MediumDuration, MMVibrationManager.MediumAmplitude);
					break;
				case HapticTypes.HeavyImpact:
					MMVibrationManager.AndroidVibrate(MMVibrationManager.HeavyDuration, MMVibrationManager.HeavyAmplitude);
					break;
				}
			}
			else if (MMVibrationManager.iOS())
			{
				MMVibrationManager.iOSTriggerHaptics(type);
			}
		}

		public static void AndroidVibrate(long milliseconds)
		{
			if (!MMVibrationManager.Android())
			{
				return;
			}
			
		}

		public static void AndroidVibrate(long milliseconds, int amplitude)
		{
			if (!MMVibrationManager.Android())
			{
				return;
			}
			if (MMVibrationManager.AndroidSDKVersion() < 26)
			{
				MMVibrationManager.AndroidVibrate(milliseconds);
			}
			else
			{
				MMVibrationManager.VibrationEffectClassInitialization();
				
			}
		}

		public static void AndroidVibrate(long[] pattern, int repeat)
		{
			if (!MMVibrationManager.Android())
			{
				return;
			}
			if (MMVibrationManager.AndroidSDKVersion() < 26)
			{
				
			}
			else
			{
				MMVibrationManager.VibrationEffectClassInitialization();
				
			}
		}

		public static void AndroidVibrate(long[] pattern, int[] amplitudes, int repeat)
		{
			if (!MMVibrationManager.Android())
			{
				return;
			}
			if (MMVibrationManager.AndroidSDKVersion() < 26)
			{
				
			}
			else
			{
				MMVibrationManager.VibrationEffectClassInitialization();
				
			}
		}

		public static void AndroidCancelVibrations()
		{
			if (!MMVibrationManager.Android())
			{
				return;
			}
			
		}

		private static void VibrationEffectClassInitialization()
		{
			
		}

		public static int AndroidSDKVersion()
		{
			if (MMVibrationManager._sdkVersion == -1)
			{
				try
				{
					int num = int.Parse(SystemInfo.operatingSystem.Substring(SystemInfo.operatingSystem.IndexOf("-") + 1, 3));
					MMVibrationManager._sdkVersion = num;
					return num;
				}
				catch
				{
					return MMVibrationManager._sdkVersion = 24;
				}
			}
			return MMVibrationManager._sdkVersion;
		}

		private static void InstantiateFeedbackGenerators()
		{
		}

		private static void ReleaseFeedbackGenerators()
		{
		}

		private static void SelectionHaptic()
		{
		}

		private static void SuccessHaptic()
		{
		}

		private static void WarningHaptic()
		{
		}

		private static void FailureHaptic()
		{
		}

		private static void LightImpactHaptic()
		{
		}

		private static void MediumImpactHaptic()
		{
		}

		private static void HeavyImpactHaptic()
		{
		}

		public static void iOSInitializeHaptics()
		{
			if (!MMVibrationManager.iOS())
			{
				return;
			}
			MMVibrationManager.InstantiateFeedbackGenerators();
			MMVibrationManager.iOSHapticsInitialized = true;
		}

		public static void iOSReleaseHaptics()
		{
			if (!MMVibrationManager.iOS())
			{
				return;
			}
			MMVibrationManager.ReleaseFeedbackGenerators();
		}

		public static bool HapticsSupported()
		{
			return false;
		}

		public static void iOSTriggerHaptics(HapticTypes type)
		{
			if (!MMVibrationManager.iOS())
			{
				return;
			}
			if (!MMVibrationManager.iOSHapticsInitialized)
			{
				MMVibrationManager.iOSInitializeHaptics();
			}
			if (MMVibrationManager.HapticsSupported())
			{
				switch (type)
				{
				case HapticTypes.Selection:
					MMVibrationManager.SelectionHaptic();
					break;
				case HapticTypes.Success:
					MMVibrationManager.SuccessHaptic();
					break;
				case HapticTypes.Warning:
					MMVibrationManager.WarningHaptic();
					break;
				case HapticTypes.Failure:
					MMVibrationManager.FailureHaptic();
					break;
				case HapticTypes.LightImpact:
					MMVibrationManager.LightImpactHaptic();
					break;
				case HapticTypes.MediumImpact:
					MMVibrationManager.MediumImpactHaptic();
					break;
				case HapticTypes.HeavyImpact:
					MMVibrationManager.HeavyImpactHaptic();
					break;
				}
			}
		}

		public static string iOSSDKVersion()
		{
			return null;
		}

		public static long LightDuration = 20L;

		public static long MediumDuration = 40L;

		public static long HeavyDuration = 80L;

		public static int LightAmplitude = 40;

		public static int MediumAmplitude = 120;

		public static int HeavyAmplitude = 255;

		private static int _sdkVersion = -1;

		private static long[] _successPattern = new long[]
		{
			0L,
			MMVibrationManager.LightDuration,
			MMVibrationManager.LightDuration,
			MMVibrationManager.HeavyDuration
		};

		private static int[] _successPatternAmplitude = new int[]
		{
			0,
			MMVibrationManager.LightAmplitude,
			0,
			MMVibrationManager.HeavyAmplitude
		};

		private static long[] _warningPattern = new long[]
		{
			0L,
			MMVibrationManager.HeavyDuration,
			MMVibrationManager.LightDuration,
			MMVibrationManager.MediumDuration
		};

		private static int[] _warningPatternAmplitude = new int[]
		{
			0,
			MMVibrationManager.HeavyAmplitude,
			0,
			MMVibrationManager.MediumAmplitude
		};

		private static long[] _failurePattern = new long[]
		{
			0L,
			MMVibrationManager.MediumDuration,
			MMVibrationManager.LightDuration,
			MMVibrationManager.MediumDuration,
			MMVibrationManager.LightDuration,
			MMVibrationManager.HeavyDuration,
			MMVibrationManager.LightDuration,
			MMVibrationManager.LightDuration
		};

		private static int[] _failurePatternAmplitude = new int[]
		{
			0,
			MMVibrationManager.MediumAmplitude,
			0,
			MMVibrationManager.MediumAmplitude,
			0,
			MMVibrationManager.HeavyAmplitude,
			0,
			MMVibrationManager.LightAmplitude
		};

		


		private static int DefaultAmplitude;

		private static bool iOSHapticsInitialized = false;
	}
}
