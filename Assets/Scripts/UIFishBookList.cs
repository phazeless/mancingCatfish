using System;
using System.Collections.Generic;
using UnityEngine;

public class UIFishBookList : UIListNormal
{
	protected override void Awake()
	{
		base.Awake();
		this.fishBook.OnFishBookChanged += this.FishBook_OnFishBookChanged;
		List<FishAttributes> list = (this.listType != UIFishBookList.FishListType.Common) ? ((this.listType != UIFishBookList.FishListType.Boss) ? this.fishBook.SpecialFishes : this.fishBook.BossFishes) : this.fishBook.TierFishes;
		foreach (FishAttributes fishAttributes in list)
		{
			fishAttributes.SetPrefab(this.prefabFishListItem);
			UIListItem uilistItem = base.AddItem(fishAttributes, false);
			((UIFishBookItem)uilistItem).SetColor(this.caughtFishbgColor, this.caughtItemBgColor, this.caughtTextColor, this.listType == UIFishBookList.FishListType.Common);
			this.uiListItemByFishInfoAsKey.Add(fishAttributes, uilistItem);
			uilistItem.OnUpdateUI(fishAttributes);
		}
	}

	private void FishBook_OnFishBookChanged(FishAttributes fishInfo)
	{
		if (this.uiListItemByFishInfoAsKey.ContainsKey(fishInfo))
		{
			this.uiListItemByFishInfoAsKey[fishInfo].OnUpdateUI(fishInfo);
		}
	}

	[SerializeField]
	private UIFishBookList.FishListType listType;

	[SerializeField]
	private UIFishBookItem prefabFishListItem;

	[SerializeField]
	private FishBook fishBook;

	[SerializeField]
	private Color caughtItemBgColor;

	[SerializeField]
	private Color caughtTextColor;

	[SerializeField]
	private Color caughtFishbgColor;

	private List<UIFishBookItem> fishItems = new List<UIFishBookItem>();

	private Dictionary<FishAttributes, UIListItem> uiListItemByFishInfoAsKey = new Dictionary<FishAttributes, UIListItem>();

	public enum FishListType
	{
		Common,
		Boss,
		Special
	}
}
