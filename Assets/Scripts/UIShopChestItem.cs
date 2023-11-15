using System;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopChestItem : MonoBehaviour
{
	public int CostToPurchase
	{
		get
		{
			return this.itemChest.CostToPurchase;
		}
	}

	private void Awake()
	{
		this.costLabel.SetVariableText(new string[]
		{
			this.itemChest.CostToPurchase.ToString()
		});
	}

	private void Start()
	{
		if (ResourceManager.Instance != null)
		{
			ResourceManager.Instance.OnResourceChanged += this.Instance_OnResourceChanged;
			if (ResourceManager.Instance.IsInitialized)
			{
				this.Instance_OnResourceChanged(ResourceType.Gems, 2, ResourceManager.Instance.GetResourceAmount(ResourceType.Gems));
			}
		}
	}

	private void Instance_OnResourceChanged(ResourceType resourceType, BigInteger amountAdded, BigInteger totalAmount)
	{
		if (resourceType != ResourceType.Gems)
		{
			return;
		}
		if (totalAmount >= (long)this.itemChest.CostToPurchase)
		{
			this.UpdateUIActive();
			this.button.interactable = true;
		}
		else
		{
			this.UpdateUIDisabled();
			this.button.interactable = false;
		}
	}

	private void UpdateUIActive()
	{
		if (!this.buyable)
		{
			this.buyable = true;
			for (int i = 0; i < this.images.Length; i++)
			{
				this.images[i].color = this.activeColors[i];
			}
		}
	}

	private void UpdateUIDisabled()
	{
		if (this.buyable)
		{
			this.buyable = false;
			for (int i = 0; i < this.images.Length; i++)
			{
				this.images[i].color = this.inactiveColors[i];
			}
		}
	}

	public void Purchase()
	{
		ResourceChangeData changeData = new ResourceChangeData(this.itemChest.Id, this.itemChest.ChestName, this.itemChest.CostToPurchase, ResourceType.Gems, ResourceChangeType.Spend, ResourceChangeReason.PurchaseItemChest);
		if (ResourceManager.Instance.TakeGems(this.itemChest.CostToPurchase, changeData))
		{
			ChestManager.Instance.OpenChest(this.itemChest);
			base.transform.DOKill(true);
			base.transform.DOPunchScale(UnityEngine.Vector3.one * 0.3f, 0.5f, 10, 1f);
		}
	}

	public void OnDestroy()
	{
		ResourceManager.Instance.OnResourceChanged -= this.Instance_OnResourceChanged;
		base.transform.DOKill(true);
	}

	[SerializeField]
	private ItemChest itemChest;

	[SerializeField]
	private TextMeshProUGUI costLabel;

	[SerializeField]
	private Image[] images;

	[SerializeField]
	private Color[] activeColors;

	[SerializeField]
	private Color[] inactiveColors;

	[SerializeField]
	private Button button;

	private bool buyable = true;
}
