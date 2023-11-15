using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIInGameNotificationList : MonoBehaviour
{
	public void Init(int maxVisibleNotifications, Transform dialogCanvas, InGameNotificationManager ignManager)
	{
		this.maxVisibleNotifications = maxVisibleNotifications;
		this.ignManager = ignManager;
		this.dialogCanvas = dialogCanvas;
		this.listElementSpacing = base.GetComponent<VerticalLayoutGroup>().spacing;
	}

	public List<UIInGameNotificationItem> GetUIItem(InGameNotification.IGN ignType)
	{
		return this.items.FindAll((UIInGameNotificationItem x) => x.InGameNotification.Type == ignType);
	}

	public void OnInGameNotificationCreated(UIInGameNotificationItem uiInGameNotification, InGameNotification inGameNotification)
	{
		if (inGameNotification.HasExpired)
		{
			return;
		}
		if (this.items.Count < this.maxVisibleNotifications)
		{
			uiInGameNotification.transform.SetParent(base.transform, false);
			uiInGameNotification.gameObject.SetActive(true);
			uiInGameNotification.transform.localPosition = new Vector3(uiInGameNotification.transform.localPosition.x, -base.GetComponent<RectTransform>().rect.height, 0f);
			this.items.Add(uiInGameNotification);
			this.MoveItemToPositionSmoothly(uiInGameNotification, this.CalcYPos(this.items.Count - 1, uiInGameNotification), 1f);
		}
		else
		{
			uiInGameNotification.gameObject.SetActive(false);
			this.pendingItems.Enqueue(uiInGameNotification);
		}
	}

	public void OnInGameNotificationRemoved(InGameNotification inGameNotification)
	{
		int num = this.items.FindIndex((UIInGameNotificationItem x) => x.InGameNotification == inGameNotification);
		bool flag = num != -1;
		if (flag)
		{
			UIInGameNotificationItem uiinGameNotificationItem = this.items[num];
			this.items.RemoveAt(num);
			for (int i = num; i < this.items.Count; i++)
			{
				this.MoveItemToPositionSmoothly(this.items[i], this.CalcYPos(i, this.items[i]), 0.3f);
			}
			UnityEngine.Object.Destroy(uiinGameNotificationItem.gameObject);
		}
		int count = this.pendingItems.Count;
		if (count > 0)
		{
			int b = this.maxVisibleNotifications - this.items.Count;
			for (int j = 0; j < Mathf.Min(count, b); j++)
			{
				UIInGameNotificationItem uiinGameNotificationItem2 = this.pendingItems.Dequeue();
				this.OnInGameNotificationCreated(uiinGameNotificationItem2, uiinGameNotificationItem2.InGameNotification);
			}
		}
	}

	public List<InGameNotificationDialog> ActiveUIInGameNotificationDialogs
	{
		get
		{
			return (from x in this.items
			select x.Dialog).ToList<InGameNotificationDialog>();
		}
	}

	public List<InGameNotificationDialog> ActiveUIInGameNotificationDialogsOfType<T>() where T : InGameNotification
	{
		return (from x in this.items
		where x.InGameNotification.GetType() == typeof(T)
		select x.Dialog).ToList<InGameNotificationDialog>();
	}

	private void OnListItemCancel(InGameNotification item)
	{
		this.ignManager.Remove(item);
	}

	private void MoveItemToPositionSmoothly(UIInGameNotificationItem item, float yPos, float duration)
	{
		item.GetComponent<LayoutElement>().ignoreLayout = true;
		item.transform.DOKill(false);
		item.transform.DOLocalMoveY(yPos, duration, false).OnComplete(delegate
		{
			item.GetComponent<LayoutElement>().ignoreLayout = false;
		});
	}

	private float CalcYPos(int index, UIInGameNotificationItem item)
	{
		return -((float)index * (item.GetComponent<RectTransform>().rect.height + this.listElementSpacing));
	}

	private int maxVisibleNotifications;

	private InGameNotificationManager ignManager;

	[HideInInspector]
	public List<UIInGameNotificationItem> items = new List<UIInGameNotificationItem>();

	private Queue<UIInGameNotificationItem> pendingItems = new Queue<UIInGameNotificationItem>();

	private Transform dialogCanvas;

	private float listElementSpacing;
}
