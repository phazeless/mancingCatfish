using System;
using System.Diagnostics;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<float, float> OnCameraOrthographicSizeChanged;

	public static CameraMovement Instance { get; private set; }

	public float PixelRelation { get; private set; }

	public static float LeftX { get; private set; }

	public static float RightX { get; private set; }

	public static float TopY { get; private set; }

	public static float BottomY { get; private set; }

	public float Zoom
	{
		get
		{
			return this.targetZoom;
		}
	}

	private void Awake()
	{
		CameraMovement.Instance = this;
		this.myCamera = base.GetComponent<Camera>();
		float pixelRelation = 0.0052f * (this.myCamera.orthographicSize / 5f);
		this.PixelRelation = pixelRelation;
		this.PixelRelation = pixelRelation;
		this.previousOrthographicSize = this.myCamera.orthographicSize;
		this.CalculateSideCoordinates();
	}

	private void Start()
	{
		if (this.targetZoom < 5f)
		{
			this.targetZoom = 5f;
		}
	}

	public void ZoomAddToCurrent(float zoom, float duration)
	{
		this.ZoomTo(this.myCamera.orthographicSize + zoom, duration);
	}

	public void ZoomTo(float zoom, float duration)
	{
		this.targetZoom = zoom;
		if (CameraMovement.bossTime)
		{
			return;
		}
		DOTween.Kill("CameraZoomTween", false);
		DOTween.Kill("CameraZoomTweenFirework", false);
		DOTween.Kill("CameraBossZoomTween", false);
		DOTween.Kill("CameraZoomTweenBGCONV", false);
		DOTween.To(() => this.myCamera.orthographicSize, delegate(float x)
		{
			this.myCamera.orthographicSize = x;
		}, this.targetZoom, duration).SetEase(Ease.InOutCubic).SetId("CameraZoomTween").OnUpdate(delegate
		{
			this.CalculateSideCoordinates();
			this.PixelRelation = 0.0052f * (this.myCamera.orthographicSize / 5f);
			if (this.OnCameraOrthographicSizeChanged != null)
			{
				this.OnCameraOrthographicSizeChanged(this.myCamera.orthographicSize, this.PixelRelation);
			}
		});
	}

	public void BossTimeZoomStart()
	{
		CameraMovement.bossTime = true;
		DOTween.Kill("CameraZoomTween", false);
		DOTween.Kill("CameraZoomTweenFirework", false);
		DOTween.Kill("CameraBossZoomTween", false);
		DOTween.Kill("CameraZoomTweenBGCONV", false);
		DOTween.To(() => this.myCamera.orthographicSize, delegate(float x)
		{
			this.myCamera.orthographicSize = x;
		}, this.targetZoom + 1f, 1f).SetEase(Ease.OutCubic).SetId("CameraBossZoomTween").OnUpdate(delegate
		{
			this.CalculateSideCoordinates();
			this.PixelRelation = 0.0052f * (this.myCamera.orthographicSize / 5f);
			if (this.OnCameraOrthographicSizeChanged != null)
			{
				this.OnCameraOrthographicSizeChanged(this.myCamera.orthographicSize, this.PixelRelation);
			}
		});
	}

	public void BossTimeZoomEnd()
	{
		CameraMovement.bossTime = false;
		this.ZoomTo(this.targetZoom, 1.5f);
	}

	public void BgIntroductionZoomStart()
	{
		DOTween.Kill("CameraZoomTween", false);
		DOTween.Kill("CameraBossZoomTween", false);
		DOTween.Kill("CameraZoomTweenFirework", false);
		DOTween.Kill("CameraZoomTweenBGCONV", false);
		DOTween.To(() => this.myCamera.orthographicSize, delegate(float x)
		{
			this.myCamera.orthographicSize = x;
		}, this.targetZoom, 1.5f).SetEase(Ease.InOutCubic).SetId("CameraZoomTweenBGCONV").OnUpdate(delegate
		{
			this.CalculateSideCoordinates();
			this.PixelRelation = 0.0052f * (this.myCamera.orthographicSize / 5f);
			if (this.OnCameraOrthographicSizeChanged != null)
			{
				this.OnCameraOrthographicSizeChanged(this.myCamera.orthographicSize, this.PixelRelation);
			}
		});
		base.transform.DOMoveX(-1.5f, 3f, false).SetEase(Ease.InOutCubic);
	}

	public void BgIntroductionZoomEnd()
	{
		this.ZoomTo(this.targetZoom, 1.5f);
		base.transform.DOMoveX(0f, 3f, false).SetEase(Ease.InOutCubic);
	}

	public void FireworkZoomStart()
	{
		DOTween.Kill("CameraZoomTween", false);
		DOTween.Kill("CameraBossZoomTween", false);
		DOTween.Kill("CameraZoomTweenBGCONV", false);
		DOTween.Kill("CameraZoomTweenFirework", false);
		DOTween.To(() => this.myCamera.orthographicSize, delegate(float x)
		{
			this.myCamera.orthographicSize = x;
		}, this.targetZoom + 1f, 1f).SetEase(Ease.OutBack).SetId("CameraZoomTweenFirework").OnUpdate(delegate
		{
			this.CalculateSideCoordinates();
			this.PixelRelation = 0.0052f * (this.myCamera.orthographicSize / 5f);
			if (this.OnCameraOrthographicSizeChanged != null)
			{
				this.OnCameraOrthographicSizeChanged(this.myCamera.orthographicSize, this.PixelRelation);
			}
		});
	}

	public void FireworkZoomEnd()
	{
		if (!CameraMovement.bossTime)
		{
			this.ZoomTo(this.targetZoom, 2.5f);
		}
	}

	private void CalculateSideCoordinates()
	{
		CameraMovement.LeftX = this.myCamera.ScreenToWorldPoint(Vector2.zero).x;
		CameraMovement.RightX = this.myCamera.ScreenToWorldPoint(new Vector2((float)Screen.width, 0f)).x;
		CameraMovement.TopY = this.myCamera.ScreenToWorldPoint(new Vector2(0f, (float)Screen.height)).y;
		CameraMovement.BottomY = this.myCamera.ScreenToWorldPoint(Vector2.zero).y;
	}

	private const float PIXEL_CONST = 0.0052f;

	[SerializeField]
	private Camera myCamera;

	private float targetZoom;

	public static bool bossTime;

	private float previousOrthographicSize;
}
