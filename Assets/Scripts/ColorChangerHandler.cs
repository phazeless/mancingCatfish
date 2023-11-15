using System;
using DG.Tweening;
using UnityEngine;

public class ColorChangerHandler : MonoBehaviour
{
	private void Awake()
	{
		ColorChangerHandler.mainCamera = base.GetComponent<Camera>();
	}

	public static void TweenToColor(Color color, float time = 3f)
	{
		ColorChangerHandler.TweenKiller(false);
		ColorChangerHandler.targetColor = color;
		ColorChangerHandler.mainCamera.DOColor(ColorChangerHandler.targetColor, time).SetDelay(2f).SetId("CameraColorTween");
	}

	public static void SetColor(Color color)
	{
		ColorChangerHandler.TweenKiller(true);
		ColorChangerHandler.targetColor = color;
		ColorChangerHandler.mainCamera.backgroundColor = ColorChangerHandler.targetColor;
	}

	public static void BossModeColorStart()
	{
		ColorChangerHandler.TweenKiller(false);
		ColorChangerHandler.mainCamera.DOColor(ColorChangerHandler.bossColor, 2f).SetId("CameraBossColorTween0").OnComplete(delegate
		{
			ColorChangerHandler.mainCamera.backgroundColor = Color.white;
			ColorChangerHandler.mainCamera.DOColor(ColorChangerHandler.bossColor, 0.2f).SetId("CameraBossColorTween1").OnComplete(delegate
			{
				ColorChangerHandler.mainCamera.DOColor(ColorChangerHandler.bossColor * 1.25f, 2f).SetLoops(-1, LoopType.Yoyo).SetId("CameraBossColorTween2");
			});
		});
	}

	public static void BossmodeColorEnd()
	{
		ColorChangerHandler.TweenKiller(false);
		ColorChangerHandler.mainCamera.DOColor(ColorChangerHandler.targetColor, 0.5f).SetId("CameraBossColorTweenEnd");
	}

	public static void FireworkModeColorStart()
	{
		if (CameraMovement.bossTime)
		{
			return;
		}
		ColorChangerHandler.TweenKiller(false);
		ColorChangerHandler.mainCamera.DOColor(ColorChangerHandler.fireworkColor, 1f).SetId("CameraFireworkColorStart").OnComplete(delegate
		{
		});
	}

	public static void FireworkModeColorEnd()
	{
		if (CameraMovement.bossTime)
		{
			return;
		}
		ColorChangerHandler.TweenKiller(false);
		ColorChangerHandler.mainCamera.DOColor(ColorChangerHandler.targetColor, 2f).SetDelay(2f).SetId("CameraFireworkColorEnd").SetDelay(1f);
	}

	private static void TweenKiller(bool complete)
	{
		DOTween.Kill("CameraColorTween", complete);
		DOTween.Kill("CameraBossColorTween0", complete);
		DOTween.Kill("CameraBossColorTween1", complete);
		DOTween.Kill("CameraBossColorTween2", complete);
		DOTween.Kill("CameraBossColorTweenEnd", complete);
		DOTween.Kill("CameraFireworkColorStart", complete);
		DOTween.Kill("CameraFireworkColorEnd", complete);
	}

	private static Color targetColor;

	private static Color bossColor = new Color(0.27f, 0.3f, 0.33f, 1f);

	private static Color fireworkColor = new Color(0.192f, 0.455f, 0.706f, 1f);

	private static int lightnings;

	private static Camera mainCamera = null;
}
