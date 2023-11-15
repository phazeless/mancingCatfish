using System;
using MoreMountains.NiceVibrations;

public static class HookedVibration
{
	public static HapticsState CurrentHapticsState
	{
		get
		{
			return HookedVibration.currentHapticsState;
		}
	}

	public static void SetHapticsState(HapticsState hapticsState)
	{
		HookedVibration.currentHapticsState = hapticsState;
	}

	public static HapticsState ToggleHaptics()
	{
		int num = (int)(HookedVibration.currentHapticsState + 1);
		int num2 = Enum.GetNames(typeof(HapticsState)).Length;
		HookedVibration.currentHapticsState = (HapticsState)(num % num2);
		return HookedVibration.currentHapticsState;
	}

	public static void NormalFishCaughtHaptic()
	{
		if (!HookedVibration.IsCapable())
		{
			return;
		}
		if (HookedVibration.currentHapticsState == HapticsState.OnForAllFishes)
		{
			MMVibrationManager.Haptic(HapticTypes.LightImpact);
		}
	}

	public static void NewFishHaptic()
	{
		if (!HookedVibration.IsCapable())
		{
			return;
		}
		if (HookedVibration.currentHapticsState == HapticsState.OnForAllFishes || HookedVibration.currentHapticsState == HapticsState.OnForBossAndNewFishes)
		{
			MMVibrationManager.Haptic(HapticTypes.MediumImpact);
		}
	}

	public static void BossFishSwipeHaptic()
	{
		if (!HookedVibration.IsCapable())
		{
			return;
		}
		if (HookedVibration.currentHapticsState == HapticsState.OnForAllFishes || HookedVibration.currentHapticsState == HapticsState.OnForBossAndNewFishes)
		{
			MMVibrationManager.Haptic(HapticTypes.MediumImpact);
		}
	}

	public static void BossFishCaughtHaptic()
	{
		if (!HookedVibration.IsCapable())
		{
			return;
		}
		if (HookedVibration.currentHapticsState == HapticsState.OnForAllFishes || HookedVibration.currentHapticsState == HapticsState.OnForBossAndNewFishes)
		{
			MMVibrationManager.HeavyDuration = 1300L;
			MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
		}
	}

	private static bool IsCapable()
	{
		bool? flag = HookedVibration.isCapable;
		if (flag == null)
		{
			HookedVibration.isCapable = new bool?(MMVibrationManager.AndroidSDKVersion() >= 25);
		}
		bool? flag2 = HookedVibration.isCapable;
		return flag2.Value;
	}

	private static bool? isCapable;

	private static HapticsState currentHapticsState = HapticsState.OnForBossAndNewFishes;
}
