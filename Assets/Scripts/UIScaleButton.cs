using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScaleButton : Selectable, IPointerClickHandler, IEventSystemHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.onClick != null)
		{
			this.onClick.Invoke();
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		this.objectToScale.DOKill(false);
		this.objectToScale.DOScale(0.9f, 0.2f);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		this.objectToScale.DOKill(false);
		this.objectToScale.DOScale(1f, 0.2f);
	}

	protected override void OnDestroy()
	{
		if (this.objectToScale != null)
		{
			this.objectToScale.DOKill(false);
		}
	}

	[SerializeField]
	private Transform objectToScale;

	[SerializeField]
	private UnityEvent onClick;
}
