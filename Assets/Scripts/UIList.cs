using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIList : MonoBehaviour
{
	public void SetItems(List<IListItemContent> items)
	{
		this.items = items;
		this.Init();
	}

	public void AddItem(IListItemContent item)
	{
		this.items.Add(item);
		int num = this.items.Count - 1;
		int num2 = num - this.topIndex;
		if (num2 < this.totalInstantiatedListItems)
		{
			UIListItem @object = this.pooledListItems.GetObject<UIListItem>(item.GetPrefab());
			@object.transform.SetParent(this.content.transform, false);
			@object.transform.SetAsLastSibling();
			@object.gameObject.SetActive(true);
			@object.OnUpdateUI(item);
			this.bottomIndex++;
		}
	}

	public void Remove(IListItemContent item)
	{
		int num = -1;
		for (int i = 0; i < this.items.Count; i++)
		{
			if (item == this.items[i])
			{
				num = i;
				break;
			}
		}
		IListItemContent listItemContent = this.items[num];
		if (num >= this.topIndex && num <= this.bottomIndex)
		{
			int num2 = num - this.topIndex;
			if (num2 < this.content.childCount)
			{
				UIListItem component = this.content.GetChild(num2).GetComponent<UIListItem>();
				this.ReturnObject(component);
				this.bottomIndex--;
			}
		}
		this.items.Remove(item);
	}

	public void Clear()
	{
		this.topIndex = 0;
		this.bottomIndex = 0;
		UIListItem[] componentsInChildren = this.content.GetComponentsInChildren<UIListItem>();
		foreach (UIListItem item in componentsInChildren)
		{
			this.ReturnObject(item);
		}
		this.items.Clear();
	}

	protected abstract List<UIListItem> GetPrefabs(params UIListItem[] prefabs);

	public void Init()
	{
		List<UIListItem> prefabs = this.GetPrefabs(new UIListItem[0]);
		this.scrollRect = base.GetComponent<InfinityScrollRect>();
		this.content = this.scrollRect.content;
		this.cache = new GameObject(base.GetType() + "ReuseableObjects").transform;
		this.pooledListItems = new DynamicObjectPool<UIListItem>(this.cache, prefabs.ToArray());
		this.itemHeight = prefabs[0].GetComponent<RectTransform>().rect.height;
		this.visibleItemsCount = (int)(this.scrollRect.viewport.rect.height / this.itemHeight);
		this.totalInstantiatedListItems = this.visibleItemsCount + UIList.ITEM_AMOUNT_BEYOND_VISIBLE;
		RectTransform viewport = this.scrollRect.viewport;
		Vector2 max = viewport.rect.max;
		this.viewportZero = viewport.TransformPoint(max).y;
		max.y += (float)UIList.ITEM_AMOUNT_BEYOND_VISIBLE * this.itemHeight;
		this.viewportZeroPlusExtraItemWidths = viewport.TransformPoint(max).y;
		this.itemHeightTimesVisibleItemsCount = this.itemHeight * (float)UIList.ITEM_AMOUNT_BEYOND_VISIBLE;
	}

	private void LateUpdate()
	{
		if (this.content == null)
		{
			return;
		}
		if (this.content.localPosition.y > this.itemHeightTimesVisibleItemsCount)
		{
			this.OnResetContentPosition(OutDirection.Top, ref this.bottomIndex, ref this.topIndex);
		}
		else if (this.content.localPosition.y < 0f)
		{
			this.OnResetContentPosition(OutDirection.Bottom, ref this.topIndex, ref this.bottomIndex);
		}
	}

	private void OnResetContentPosition(OutDirection direction, ref int index, ref int otherIndex)
	{
		if (direction == OutDirection.Top && index == this.items.Count)
		{
			return;
		}
		if (direction == OutDirection.Bottom && index == 0)
		{
			return;
		}
		int num = (direction != OutDirection.Top) ? (index - 1) : index;
		bool isFirstItemAboutToShow = num == 0;
		bool isLastItemAboutToShow = num == this.items.Count - 1;
		this.scrollRect.ResetContentPosition(direction, isFirstItemAboutToShow, isLastItemAboutToShow);
		if (num >= 0 && num < this.items.Count)
		{
			for (int i = 0; i < UIList.ITEM_AMOUNT_BEYOND_VISIBLE; i++)
			{
				if (direction == OutDirection.Top && num >= this.items.Count)
				{
					return;
				}
				if (direction == OutDirection.Bottom && num - i < 0)
				{
					return;
				}
				int index2 = (direction != OutDirection.Top) ? (this.content.childCount - 1) : 0;
				Transform child = this.content.transform.GetChild(index2);
				UIListItem component = child.GetComponent<UIListItem>();
				this.pooledListItems.ReturnObject(component);
				child.gameObject.SetActive(false);
				child.SetParent(this.cache, false);
				IListItemContent listItemContent = this.items[num];
				UIListItem @object = this.pooledListItems.GetObject<UIListItem>(listItemContent.GetPrefab());
				@object.transform.SetParent(this.content.transform, false);
				@object.OnUpdateUI(listItemContent);
				@object.gameObject.SetActive(true);
				if (direction == OutDirection.Top)
				{
					@object.transform.SetAsLastSibling();
					index++;
					otherIndex++;
				}
				else if (direction == OutDirection.Bottom)
				{
					@object.transform.SetAsFirstSibling();
					index--;
					otherIndex--;
				}
				num = ((direction != OutDirection.Top) ? (index - 1) : index);
			}
		}
	}

	private void ReturnObject(UIListItem item)
	{
		this.pooledListItems.ReturnObject(item);
		item.transform.SetParent(this.cache, false);
		item.gameObject.SetActive(false);
	}

	private UIListItem InstantiateCallback(object prefab)
	{
		UIListItem uilistItem = UnityEngine.Object.Instantiate<UIListItem>((UIListItem)prefab, this.content.transform, false);
		uilistItem.transform.SetAsFirstSibling();
		return uilistItem;
	}

	private static readonly int ITEM_AMOUNT_BEYOND_VISIBLE = 2;

	private RectTransform content;

	private Transform cache;

	private DynamicObjectPool<UIListItem> pooledListItems;

	private int bottomIndex;

	private int topIndex;

	private List<IListItemContent> items = new List<IListItemContent>();

	private List<IListItemContent> newlyAddedItems = new List<IListItemContent>();

	private InfinityScrollRect scrollRect;

	private float itemHeight;

	private float itemHeightTimesVisibleItemsCount;

	private int visibleItemsCount;

	private int totalInstantiatedListItems;

	private float viewportZero;

	private float viewportZeroPlusExtraItemWidths;
}
