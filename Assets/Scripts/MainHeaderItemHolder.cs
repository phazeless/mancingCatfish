using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainHeaderItemHolder : MonoBehaviour
{
	private void Awake()
	{
		ItemManager.Instance.OnItemEquipStateChanged += this.Instance_OnItemEquipStateChanged;
		foreach (GameObject gameObject in this.itemSlots)
		{
			this.itemIcons.Add(gameObject.transform.GetChild(0).GetComponent<Image>());
		}
		base.gameObject.SetActive(false);
	}

	private void Instance_OnItemEquipStateChanged(Item item, ItemEquipState state)
	{
		for (int i = 0; i < this.itemSlots.Count; i++)
		{
			this.itemSlots[i].SetActive(false);
		}
		List<Item> equippedItems = ItemManager.Instance.EquippedItems;
		if (equippedItems.Count > 0)
		{
			base.gameObject.SetActive(true);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		for (int j = 0; j < equippedItems.Count; j++)
		{
			if (j < this.itemSlots.Count)
			{
				this.itemSlots[j].SetActive(true);
			}
			if (j < this.itemIcons.Count)
			{
				this.itemIcons[j].sprite = equippedItems[j].Icon;
			}
		}
	}

	[SerializeField]
	private List<GameObject> itemSlots = new List<GameObject>();

	private List<Image> itemIcons = new List<Image>();
}
