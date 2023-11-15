using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	public class ScrollSnapScrollbarHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IEventSystemHandler
	{
		public void OnBeginDrag(PointerEventData eventData)
		{
			this.OnScrollBarDown();
		}

		public void OnDrag(PointerEventData eventData)
		{
			this.ss.CurrentPage();
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			this.OnScrollBarUp();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			this.OnScrollBarDown();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			this.OnScrollBarUp();
		}

		private void OnScrollBarDown()
		{
			if (this.ss != null)
			{
				this.ss.SetLerp(false);
				this.ss.StartScreenChange();
			}
		}

		private void OnScrollBarUp()
		{
			this.ss.SetLerp(true);
			this.ss.ChangePage(this.ss.CurrentPage());
		}

		internal IScrollSnap ss;
	}
}
