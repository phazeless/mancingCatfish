using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfinityScrollRect : ScrollRect
{
	public void ResetContentPosition(OutDirection direction, bool isFirstItemAboutToShow, bool isLastItemAboutToShow)
	{
		if (isFirstItemAboutToShow || isLastItemAboutToShow)
		{
			this.outOfViewHeight = Mathf.Abs(base.content.rect.min.y - base.viewport.rect.min.y) / 2f;
		}
		else if (direction == OutDirection.Top)
		{
			this.outOfViewHeight = 0f;
		}
		else if (direction == OutDirection.Bottom)
		{
			this.outOfViewHeight = Mathf.Abs(base.content.rect.min.y - base.viewport.rect.min.y);
		}
		base.content.localPosition = new Vector2(base.content.localPosition.x, this.outOfViewHeight);
		this.isResetted = true;
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		this.isResetted = false;
		this.pointerStartLocalCursor = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(base.viewRect, eventData.position, eventData.pressEventCamera, out this.pointerStartLocalCursor);
		this.imaginaryStartPos = this.imaginaryPosition;
		this.isDragging = true;
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		this.isDragging = false;
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if (this.isResetted)
		{
			Vector2 a;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(base.viewRect, eventData.position, eventData.pressEventCamera, out a))
			{
				return;
			}
			float num = this.pointerStartLocalCursor.y - a.y;
			this.m_ContentStartPosition = new Vector2(0f, num + this.outOfViewHeight);
			Vector2 b = a - this.pointerStartLocalCursor;
			this.imaginaryPosition = this.imaginaryStartPos + b;
			this.isResetted = false;
		}
		base.OnDrag(eventData);
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (this.isDragging && base.inertia)
		{
			Vector3 b = (this.imaginaryPosition - this.prevImaginaryPosition) / Time.unscaledDeltaTime;
			base.velocity = Vector3.Lerp(base.velocity, b, Time.unscaledDeltaTime * 10f);
			base.velocity = new Vector2(0f, base.velocity.y);
		}
		this.prevImaginaryPosition = this.imaginaryPosition;
	}

	private bool isDragging;

	private bool isResetted;

	private Vector2 pointerStartLocalCursor = Vector2.zero;

	private Vector2 imaginaryStartPos = Vector2.zero;

	private Vector2 imaginaryPosition = Vector2.zero;

	private Vector2 prevImaginaryPosition = Vector2.zero;

	private float outOfViewHeight;
}
