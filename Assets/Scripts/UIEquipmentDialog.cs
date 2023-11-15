using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentDialog : UpgradeDialogTween
{
	public void Show(UIEquipmentItem uiItem, Action<ItemEquipState> onClose)
	{
		base.Open();
		this.uiItem = uiItem;
		this.onCloseCallback = onClose;
		if (TutorialSliceUpgradeItem.Instance != null && (uiItem.Item.CurrentItemAmount > 0 || uiItem.Item.CurrentLevel > 0))
		{
			if (!uiItem.Item.HasEnoughGemsToLevelUp)
			{
				BigInteger bigInteger = uiItem.Item.TotalGemsRequiredForNextLevel - ResourceManager.Instance.GetResourceAmount(ResourceType.Gems);
				ResourceChangeData gemChangeData = new ResourceChangeData("contentId_itemTutorial", "Tutorial Item Upgrade", (int)bigInteger, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.TutorialItemUpgrade);
				ResourceManager.Instance.GiveGems(bigInteger, gemChangeData);
			}
			TutorialSliceUpgradeItem.Instance.FirstTimeUpgrade();
		}
		this.equipmentIconHolder = this.lvlUpObject.transform.parent;
		this.UpdateUI();
	}

	public override void Close(bool destroyOnFinish = false)
	{
		base.Close(destroyOnFinish);
		this.uiItem = null;
		this.onCloseCallback = null;
	}

	public void Upgrade()
	{
		if (this.uiItem != null && this.uiItem.Item != null)
		{
			this.uiItem.Item.TryLevelUp(delegate(Item item, bool didSucceed)
			{
				if (didSucceed)
				{
					this.LevelUpEffect();
					if (TutorialSliceUpgradeItem.Instance != null)
					{
						TutorialSliceUpgradeItem.Instance.LeveledUp();
					}
				}
				this.UpdateUI();
			});
		}
	}

	public void EquipOrUnequip()
	{
		if (this.onCloseCallback != null && this.uiItem != null && this.uiItem.Item != null)
		{
			this.onCloseCallback((!this.uiItem.Item.IsEquipped) ? ItemEquipState.Equipped : ItemEquipState.Unequipped);
		}
		this.Close(false);
	}

	private void LevelUpEffect()
	{
		if (this.equipmentIconHolder != null)
		{
			this.equipmentIconHolder.DOKill(true);
			this.equipmentIconHolder.DOPunchScale(UnityEngine.Vector3.one * 0.4f, 0.4f, 3, 1f);
			this.upgradeButton.transform.DOKill(true);
			this.upgradeButton.transform.DOPunchScale(UnityEngine.Vector3.one * 0.3f, 0.5f, 10, 1f);
			this.icon.transform.DOKill(true);
			this.icon.transform.DOPunchScale(UnityEngine.Vector3.one * 0.6f, 0.55f, 10, 1f);
			this.rarityBannerMiddle.transform.parent.transform.DOKill(true);
			this.rarityBannerMiddle.transform.parent.transform.DOPunchScale(new UnityEngine.Vector3(1f, 0f, 0f) * 0.3f, 0.3f, 2, 1f);
			this.valueAndAttribute[0].transform.parent.transform.DOKill(true);
			this.valueAndAttribute[0].transform.parent.transform.DOPunchScale(new UnityEngine.Vector3(1f, 0.5f, 0f) * 0.15f, 0.3f, 2, 1f).SetDelay(0.1f);
			this.iconBackground.color = Color.white;
			this.iconBackground.DOColor(new Color(0.604f, 0.941f, 0.871f), 0.5f).SetEase(Ease.InCubic);
			this.lvlupSplash.transform.localScale = UnityEngine.Vector3.one * 4f * (CameraMovement.Instance.Zoom / 6f);
			this.lvlupSplash.Play();
		}
	}

	private void UpdateUI()
	{
		if (this.uiItem == null)
		{
			return;
		}
		Item item = this.uiItem.Item;
		int rarity = (int)item.Rarity;
		this.icon.sprite = item.Icon;
		float x = Mathf.Min(1f, (float)item.CurrentItemAmount / (float)item.TotalItemAmountRequiredForNextLevel);
		this.lvlBar.transform.localScale = new UnityEngine.Vector3(x, 1f, 1f);
		this.icon.color = ((!item.IsUnlocked && item.CurrentItemAmount <= 0) ? new Color(0f, 0f, 0f, 0.4f) : Color.white);
		this.lvlUpObject.SetActive(item.HasEnoughItemAmountToLevelUp && !item.IsMaxLevel);
		this.lvlLabel.SetVariableText(new string[]
		{
			item.CurrentLevel.ToString()
		});
		this.titleLabel.color = this.rarityColors[rarity];
		this.titleLabel.SetText(item.Title);
		this.descriptionLabel.SetText(item.Description);
		this.upgradeButtonNeedMoreLabel.gameObject.SetActive(!item.HasEnoughItemAmountToLevelUp);
		this.upgradeButtonCostLabel.gameObject.SetActive(item.HasEnoughItemAmountToLevelUp && !item.IsMaxLevel);
		if (!item.IsMaxLevel)
		{
			this.upgradeButtonNeedMoreLabel.SetVariableText(new string[]
			{
				(item.TotalItemAmountRequiredForNextLevel - item.CurrentItemAmount).ToString()
			});
			this.currentAndMaxAmountLabel.SetVariableText(new string[]
			{
				item.CurrentItemAmount.ToString(),
				item.TotalItemAmountRequiredForNextLevel.ToString()
			});
		}
		else
		{
			this.upgradeButtonNeedMoreLabel.SetText("Maxed");
			this.currentAndMaxAmountLabel.SetText("Maxed");
		}
		this.upgradeButtonCostLabel.SetVariableText(new string[]
		{
			item.TotalGemsRequiredForNextLevel.ToString()
		});
		this.upgradeButton.interactable = (item.HasEnoughGemsToLevelUp && item.HasEnoughItemAmountToLevelUp && !item.IsMaxLevel);
		this.equipButton.interactable = item.IsUnlocked;
		this.equipButtonLabel.SetText((!item.IsEquipped) ? "Equip" : "Unequip");
		ColorBlock colors = this.upgradeButton.colors;
		colors.normalColor = this.rarityColors[rarity];
		colors.highlightedColor = this.rarityColors[rarity];
		colors.pressedColor = this.rarityColors[rarity];
		this.upgradeButton.colors = colors;
		this.equipButton.colors = colors;
		if (item.IsEquipped)
		{
			Color color = new Color(0.2f, 0.2f, 0.2f, 1f);
			colors.normalColor = color;
			colors.highlightedColor = color;
			colors.pressedColor = color;
			this.equipButton.colors = colors;
		}
		this.rarityBannerDark1.color = new Color(this.rarityColors[rarity].r - 0.078f, this.rarityColors[rarity].g - 0.078f, this.rarityColors[rarity].b - 0.078f);
		this.rarityBannerDark2.color = new Color(this.rarityColors[rarity].r - 0.078f, this.rarityColors[rarity].g - 0.078f, this.rarityColors[rarity].b - 0.078f);
		this.rarityBannerMiddle.color = this.rarityColors[rarity];
		this.lvlCircle.color = this.rarityColors[rarity];
		this.lvlBar.color = this.rarityColors[rarity];
		this.UpdateTextForSkillBehaviours();
	}

	private void UpdateTextForSkillBehaviours()
	{
		Item item = this.uiItem.Item;
		List<string> list = new List<string>();
		for (int i = 0; i < this.valueAndAttribute.Count; i++)
		{
			this.valueAndAttribute[i].enabled = false;
		}
		for (int j = 0; j < item.SkillBehaviours.Count; j++)
		{
			if (j < this.valueAndAttribute.Count)
			{
				this.valueAndAttribute[j].enabled = true;
				SkillBehaviour skillBehaviour = item.SkillBehaviours[j];
				this.valueAndAttribute[j].transform.parent.gameObject.SetActive(true);
				float valueAtLevel = skillBehaviour.GetValueAtLevel(item.NextLevel);
				string text = (valueAtLevel <= 0f) ? string.Empty : "+";
				float totalValueAtLevel = skillBehaviour.GetTotalValueAtLevel(item.CurrentLevel);
				string text2 = (totalValueAtLevel <= 0f) ? string.Empty : "+";
				string text3 = FHelper.FindBracketAndReplace(skillBehaviour.Description, new string[]
				{
					string.Concat(new object[]
					{
						"<b>",
						text,
						valueAtLevel,
						skillBehaviour.PostFixCharacter,
						"</b>"
					})
				});
				this.valueAndAttribute[j].SetVariableText(new string[]
				{
					string.Empty,
					text3,
					string.Concat(new object[]
					{
						" (",
						text2,
						skillBehaviour.GetTotalValueAtLevel(item.CurrentLevel),
						skillBehaviour.PostFixCharacter,
						")"
					})
				});
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.equipmentIconHolder != null)
		{
			this.equipmentIconHolder.DOKill(true);
		}
		this.iconBackground.DOKill(false);
		this.upgradeButton.transform.DOKill(false);
		this.upgradeButton.transform.DOKill(true);
		this.icon.transform.DOKill(true);
		this.rarityBannerMiddle.transform.parent.transform.DOKill(true);
		this.valueAndAttribute[0].transform.parent.transform.DOKill(true);
	}

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image iconBackground;

	[SerializeField]
	private GameObject lvlUpObject;

	[SerializeField]
	private TextMeshProUGUI lvlLabel;

	[SerializeField]
	private TextMeshProUGUI currentAndMaxAmountLabel;

	[SerializeField]
	private TextMeshProUGUI titleLabel;

	[SerializeField]
	private TextMeshProUGUI upgradeButtonCostLabel;

	[SerializeField]
	private TextMeshProUGUI upgradeButtonNeedMoreLabel;

	[SerializeField]
	private Button upgradeButton;

	[SerializeField]
	private Button equipButton;

	[SerializeField]
	private TextMeshProUGUI equipButtonLabel;

	[SerializeField]
	private TextMeshProUGUI descriptionLabel;

	[SerializeField]
	private ParticleSystem lvlupSplash;

	private Transform equipmentIconHolder;

	[SerializeField]
	private Image rarityLevelUpButton;

	[SerializeField]
	private Image rarityBannerDark1;

	[SerializeField]
	private Image rarityBannerDark2;

	[SerializeField]
	private Image rarityBannerMiddle;

	[SerializeField]
	private Image lvlCircle;

	[SerializeField]
	private Image lvlBar;

	[SerializeField]
	private List<TextMeshProUGUI> valueAndAttribute = new List<TextMeshProUGUI>();

	private UIEquipmentItem uiItem;

	private Action<ItemEquipState> onCloseCallback;

	private Color[] rarityColors = new Color[]
	{
		new Color(0.376f, 0.722f, 0.773f),
		new Color(0.325f, 0.345f, 0.714f),
		new Color(0.729f, 0.259f, 0.463f)
	};

	private const string contentId_itemTutorial = "contentId_itemTutorial";

	private const string contentName_itemTutorial = "Tutorial Item Upgrade";
}
