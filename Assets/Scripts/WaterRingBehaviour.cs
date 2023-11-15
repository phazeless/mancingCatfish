using System;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;

public class WaterRingBehaviour : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnTweenFinished;

	private void Awake()
	{
		this.spriteRender = base.GetComponent<SpriteRenderer>();
		this.mainCamera = Camera.main;
	}

	private void Start()
	{
		base.transform.localScale = Vector2.zero;
		CameraColorChangeListener.OnCameraColorChange += this.CameraColorChangeListener_OnCameraColorChange;
		this.spriteRender.color = this.GetOffsetColor(this.mainCamera.backgroundColor, 0.0784313753f);
		this.middleCircleTransform.GetComponent<SpriteRenderer>().color = this.GetOffsetColor(this.mainCamera.backgroundColor, 0f);
	}

	public void Go(int order = 0)
	{
		if (order > 0)
		{
			this.spriteRender.sortingOrder = this.spriteRender.sortingOrder + order;
			this.middleCircleTransform.GetComponent<SpriteRenderer>().sortingOrder = this.middleCircleTransform.GetComponent<SpriteRenderer>().sortingOrder + order;
		}
		base.Invoke("Splash", this.startDelay);
	}

	private Color GetOffsetColor(Color color, float offset)
	{
		Color result = color;
		result.r += offset;
		result.g += offset;
		result.b += offset;
		return result;
	}

	private void CameraColorChangeListener_OnCameraColorChange(Color newColor, Color oldColor)
	{
		this.spriteRender.color = this.GetOffsetColor(newColor, 0.0784313753f);
		this.middleCircleTransform.GetComponent<SpriteRenderer>().color = this.GetOffsetColor(newColor, 0f);
	}

	private void Splash()
	{
		base.transform.localScale = Vector2.zero;
		this.middleCircleTransform.localScale = Vector2.zero;
		this.middleCircleTransform.DOScale(1f, 1f).SetEase(this.curve);
		base.transform.DOScale(this.targetScale, 1f).OnComplete(delegate
		{
			if (this.isDestroyOnFinish)
			{
				this.DelayedDestroy();
			}
			if (this.OnTweenFinished != null)
			{
				this.OnTweenFinished();
			}
		});
	}

	private void DelayedDestroy()
	{
		if (base.gameObject != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		CameraColorChangeListener.OnCameraColorChange -= this.CameraColorChangeListener_OnCameraColorChange;
		DOTween.Kill(base.transform, false);
		this.middleCircleTransform.DOKill(false);
		base.transform.DOKill(false);
	}

	public float startDelay = 1f;

	public float targetScale = 1.5f;

	public bool isDestroyOnFinish;

	private SpriteRenderer spriteRender;

	[SerializeField]
	private Transform middleCircleTransform;

	[SerializeField]
	private AnimationCurve curve;

	private Camera mainCamera;
}
