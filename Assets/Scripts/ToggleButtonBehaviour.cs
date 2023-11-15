using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonBehaviour : MonoBehaviour
{
	private void Start()
	{
	}

	public void ToggelActive()
	{
		if (this.isActive)
		{
			this.SetOff();
		}
		else
		{
			this.SetOn();
		}
	}

	public void SetOn()
	{
		this.knob.DOKill(false);
		this.knob.DOAnchorPosX(this.activeTarget.anchoredPosition.x, 0.2f, false);
		this.image.color = this.activeColor;
		this.isActive = true;
	}

	public void SetOff()
	{
		this.knob.DOKill(false);
		this.knob.DOAnchorPosX(this.inActiveTarget.anchoredPosition.x, 0.2f, false);
		this.image.color = this.inActiveColor;
		this.isActive = false;
	}

	private void OnDestroy()
	{
		this.knob.DOKill(false);
	}

	[SerializeField]
	private RectTransform knob;

	[SerializeField]
	private RectTransform inActiveTarget;

	[SerializeField]
	private RectTransform activeTarget;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Color activeColor;

	[SerializeField]
	private Color inActiveColor;

	public bool isActive = true;
}
