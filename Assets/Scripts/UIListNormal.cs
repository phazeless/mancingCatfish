using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIListNormal : MonoBehaviour
{
	protected virtual void Awake()
	{
		this.content = base.GetComponent<ScrollRect>().content;
	}

	public void Remove(IListItemContent listItemToRemove)
	{
		int index = this.items.FindIndex((IListItemContent x) => x == listItemToRemove);
		UIListItem uilistItem = this.instantiatedItems[index];
		this.instantiatedItems.RemoveAt(index);
		this.items.RemoveAt(index);
		UnityEngine.Object.Destroy(uilistItem.gameObject);
	}

	public void Remove(List<IListItemContent> listItemsToRemove)
	{
		List<IListItemContent> list = new List<IListItemContent>(listItemsToRemove);
		for (int i = 0; i < list.Count; i++)
		{
			this.Remove(list[i]);
		}
	}

	public void Clear()
	{
		this.Remove(this.items);
	}

	public UIListItem AddItem(IListItemContent listItemContent, bool updateUi = true)
	{
		this.items.Add(listItemContent);
		UIListItem prefab = listItemContent.GetPrefab();
		UIListItem uilistItem = UnityEngine.Object.Instantiate<UIListItem>(prefab, this.content.transform, false);
		uilistItem.transform.SetAsLastSibling();
		if (updateUi)
		{
			uilistItem.OnUpdateUI(listItemContent);
		}
		uilistItem.gameObject.SetActive(true);
		this.instantiatedItems.Add(uilistItem);
		return uilistItem;
	}

	public void OnListPositioningFinished()
	{
		foreach (UIListItem uilistItem in this.instantiatedItems)
		{
			if (uilistItem.gameObject.activeInHierarchy)
			{
				uilistItem.SetSizes(CameraMovement.Instance.PixelRelation);
			}
		}
	}

	private Transform content;

	protected List<IListItemContent> items = new List<IListItemContent>();

	protected List<UIListItem> instantiatedItems = new List<UIListItem>();
}
