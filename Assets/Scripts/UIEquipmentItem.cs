using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentItem : MonoBehaviour
{
	public Item Item { get; private set; }

	public UIItemGrid Grid { get; set; }

	public void SetItem(Item item)
	{
		this.Item = item;
		this.Item.OnItemLevelUp += this.Item_OnItemLevelUp;
		this.Item.OnItemUpgradeAvailabilityChanged += this.Item_OnItemUpgradeAvailabilityChanged;
		this.Item.OnItemAmountChanged += this.Item_OnItemAmountChanged;
		this.UpdateUI();
	}

	private void Item_OnItemAmountChanged(Item arg1, int arg2, int arg3, ResourceChangeReason reason)
	{
		this.UpdateUI();
	}

	private void Item_OnItemUpgradeAvailabilityChanged(Item arg1, bool arg2)
	{
		this.UpdateUI();
	}

	private void Item_OnItemLevelUp(Item arg1, LevelChange arg2, int itemAmountSpent, int gemCost)
	{
		this.UpdateUI();
	}

	public void UpdateUI()
	{
		if (this.Item.CurrentItemAmount <= 0 && this.Item.CurrentLevel == 0)
		{
			this.SetUnAquiredUI();
		}
		else
		{
			this.SetAquiredUI();
		}
	}

	private void SetUnAquiredUI()
	{
		int rarity = (int)this.Item.Rarity;
		this.rarityBackground.color = Color.white * 0.9f;
		this.colorBackground.color = Color.white * 0.95f;
		this.lvlObjectBg.color = Color.white * 0.9f;
		if (!this.Item.IsMaxLevel)
		{
			this.currentAndMaxAmountLabel.SetVariableText(new string[]
			{
				this.Item.CurrentItemAmount.ToString(),
				this.Item.TotalItemAmountRequiredForNextLevel.ToString()
			});
		}
		else
		{
			this.currentAndMaxAmountLabel.SetText("Maxed");
		}
		this.lvlUpObject.SetActive(this.Item.HasEnoughItemAmountToLevelUp);
		if (this.Item.HasEnoughItemAmountToLevelUp || this.Item.IsMaxLevel)
		{
			this.currentAndMaxAmountLabel.fontStyle = FontStyles.Bold;
			this.currentAndMaxAmountLabel.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			this.currentAndMaxAmountLabel.fontStyle = FontStyles.Normal;
			this.currentAndMaxAmountLabel.color = new Color(1f, 1f, 1f, 0.58f);
		}
		this.icon.sprite = this.Item.Icon;
		this.icon.color = new Color(0f, 0f, 0f, 0.2f);
	}

	private void SetAquiredUI()
	{
		int rarity = (int)this.Item.Rarity;
		this.rarityBackground.color = this.rarityColors[rarity];
		this.colorBackground.color = this.Item.IconBgColor;
		this.lvlObjectBg.color = this.rarityBackground.color;
		if (!this.Item.IsMaxLevel)
		{
			this.currentAndMaxAmountLabel.SetVariableText(new string[]
			{
				this.Item.CurrentItemAmount.ToString(),
				this.Item.TotalItemAmountRequiredForNextLevel.ToString()
			});
		}
		else
		{
			this.currentAndMaxAmountLabel.SetText("Maxed");
		}
		this.lvlLabel.SetText("Lv" + this.Item.CurrentLevel);
		this.lvlUpObject.SetActive(this.Item.HasEnoughItemAmountToLevelUp);
		if (this.Item.HasEnoughItemAmountToLevelUp || this.Item.IsMaxLevel)
		{
			this.currentAndMaxAmountLabel.fontStyle = FontStyles.Bold;
			this.currentAndMaxAmountLabel.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			this.currentAndMaxAmountLabel.fontStyle = FontStyles.Normal;
			this.currentAndMaxAmountLabel.color = new Color(1f, 1f, 1f, 0.58f);
		}
		this.icon.sprite = this.Item.Icon;
		this.icon.color = new Color(1f, 1f, 1f, 1f);
	}

	public void OnItemClicked()
	{
		this.Grid.OnEquipmentClicked(this);
	}

	private void OnEnable()
	{
		if (this.Item != null && this.Grid != null)
		{
			this.UpdateUI();
		}
	}

	private void OnDestroy()
	{
		if (this.Item != null)
		{
			this.Item.OnItemLevelUp -= this.Item_OnItemLevelUp;
			this.Item.OnItemUpgradeAvailabilityChanged -= this.Item_OnItemUpgradeAvailabilityChanged;
			this.Item.OnItemAmountChanged -= this.Item_OnItemAmountChanged;
		}
	}

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image colorBackground;

	[SerializeField]
	private Image rarityBackground;

	[SerializeField]
	private TextMeshProUGUI currentAndMaxAmountLabel;

	[SerializeField]
	private TextMeshProUGUI lvlLabel;

	[SerializeField]
	private Image lvlObjectBg;

	[SerializeField]
	private GameObject lvlUpObject;

	private Color[] rarityColors = new Color[]
	{
		new Color(0.376f, 0.722f, 0.773f),
		new Color(0.325f, 0.345f, 0.714f),
		new Color(0.729f, 0.259f, 0.463f)
	};
}
