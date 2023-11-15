using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemGrid : MonoBehaviour
{
	public Transform EquippedItems
	{
		get
		{
			return this.equippedItems.transform;
		}
	}

	private void Start()
	{
		ItemManager.Instance.OnItemUnlocked += this.Instance_OnItemUnlocked;
		this.allItems = ItemManager.Instance.AllItems;
		foreach (Item item in this.allItems)
		{
			if (item.IsEquipped)
			{
				this.AddItem(item, ItemEquipState.Equipped, true);
				this.currentSlots++;
			}
			else
			{
				this.AddItem(item, ItemEquipState.Unequipped, true);
			}
		}
		this.SortChildernByRarity(this.unequippedItems.transform);
		this.GenerateEmptySlots();
	}

	private void GenerateEmptySlots()
	{
		foreach (Transform transform in this.emptySlots)
		{
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		this.emptySlots.Clear();
		for (int i = 0; i < this.avaliableSlots - this.currentSlots; i++)
		{
			Transform transform2 = UnityEngine.Object.Instantiate<Transform>(this.emptySlot);
			transform2.transform.SetParent(this.equippedItems.transform, false);
			this.emptySlots.Add(transform2);
		}
	}

	private void Instance_OnItemUnlocked(Item item)
	{
	}

	public void AddItem(Item item, ItemEquipState state, bool createNew)
	{
		UIEquipmentItem uiequipmentItem = (!createNew) ? this.equipmentByItemRef[item] : UnityEngine.Object.Instantiate<UIEquipmentItem>(this.uiEquipmentItemPrefab);
		if (state == ItemEquipState.Equipped)
		{
			uiequipmentItem.transform.SetParent(this.equippedItems.transform, false);
			item.Equip();
		}
		else if (state == ItemEquipState.Unequipped)
		{
			uiequipmentItem.transform.SetParent(this.unequippedItems.transform, false);
			item.Unequip();
		}
		uiequipmentItem.Grid = this;
		uiequipmentItem.SetItem(item);
		if (!this.equipmentByItemRef.ContainsKey(item))
		{
			this.equipmentByItemRef.Add(item, uiequipmentItem);
		}
		if (createNew)
		{
			this.counter++;
			uiequipmentItem.name = "item-" + this.counter;
		}
	}

	public void EquipItem(Item item, int slotId)
	{
		this.AddItem(item, ItemEquipState.Equipped, false);
		if (slotId < this.equippedItems.transform.childCount)
		{
			this.equipmentByItemRef[item].transform.SetSiblingIndex(slotId);
		}
		this.currentSlots++;
		this.GenerateEmptySlots();
	}

	private void SortChildernByRarity(Transform parent)
	{
		List<UIEquipmentItem> list = new List<UIEquipmentItem>();
		foreach (UIEquipmentItem item in parent.GetComponentsInChildren<UIEquipmentItem>())
		{
			list.Add(item);
		}
		list.Sort((UIEquipmentItem y, UIEquipmentItem x) => x.Item.Rarity.CompareTo(y.Item.Rarity));
		foreach (UIEquipmentItem uiequipmentItem in list)
		{
			uiequipmentItem.transform.SetAsFirstSibling();
		}
	}

	public void UnequipItem(Item item)
	{
		this.AddItem(item, ItemEquipState.Unequipped, false);
		this.SortChildernByRarity(this.unequippedItems.transform);
		this.currentSlots--;
		this.GenerateEmptySlots();
	}

	private bool IsEquippingItem
	{
		get
		{
			return this.itemToBeEquipped != null;
		}
	}

	public void OnEquipmentClicked(UIEquipmentItem itemClicked)
	{
		if (this.IsEquippingItem)
		{
			if (itemClicked.Item.IsEquipped)
			{
				int slotIndexOf = this.GetSlotIndexOf(itemClicked.Item);
				this.UnequipItem(itemClicked.Item);
				this.EquipItem(this.itemToBeEquipped.Item, slotIndexOf);
				this.itemToBeEquipped = null;
				this.selectBlocker.gameObject.SetActive(false);
			}
		}
		else
		{
			this.uiEquipmentDialog.Show(itemClicked, delegate(ItemEquipState equipState)
			{
				if (equipState == ItemEquipState.Equipped)
				{
					if (!ItemManager.Instance.HasEquippedMaxAmount)
					{
						this.EquipItem(itemClicked.Item, 999999);
					}
					else
					{
						this.scrollRect.verticalNormalizedPosition = 1f;
						this.itemToBeEquipped = itemClicked;
						this.selectBlocker.gameObject.SetActive(true);
					}
				}
				else if (equipState == ItemEquipState.Unequipped)
				{
					this.UnequipItem(itemClicked.Item);
				}
			});
		}
	}

	public void CancelSelection()
	{
		this.selectBlocker.gameObject.SetActive(false);
		this.itemToBeEquipped = null;
	}

	public int GetSlotIndexOf(Item item)
	{
		UIEquipmentItem uiequipmentItem = this.equipmentByItemRef[item];
		Transform parent = uiequipmentItem.transform.parent;
		for (int i = 0; i < parent.childCount; i++)
		{
			if (uiequipmentItem.transform == parent.GetChild(i))
			{
				return i;
			}
		}
		return 999999;
	}

	[SerializeField]
	private int avaliableSlots = 4;

	[SerializeField]
	private GridLayoutGroup equippedItems;

	[SerializeField]
	private GridLayoutGroup unequippedItems;

	[SerializeField]
	private UIEquipmentItem uiEquipmentItemPrefab;

	[SerializeField]
	private UIEquipmentDialog uiEquipmentDialog;

	[SerializeField]
	private Button selectBlocker;

	[SerializeField]
	private Transform emptySlot;

	[SerializeField]
	private ScrollRect scrollRect;

	private List<Transform> emptySlots = new List<Transform>();

	private int currentSlots;

	private List<Item> allItems = new List<Item>();

	private Dictionary<Item, UIEquipmentItem> equipmentByItemRef = new Dictionary<Item, UIEquipmentItem>();

	private int counter;

	private UIEquipmentItem itemToBeEquipped;
}
