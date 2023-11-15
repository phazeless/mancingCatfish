using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

internal class PressReleaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
		if (!this.interactable)
		{
			return;
		}
		if (this.onPressed != null)
		{
			this.onPressed.Invoke();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!this.interactable)
		{
			return;
		}
		if (this.onRelease != null)
		{
			this.onRelease.Invoke();
		}
	}

	[SerializeField]
	private UnityEvent onPressed;

	[SerializeField]
	private UnityEvent onRelease;

	public bool interactable = true;
}
