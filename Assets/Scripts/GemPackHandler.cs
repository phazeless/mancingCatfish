using System;
using TMPro;
using UnityEngine;

public class GemPackHandler : MonoBehaviour
{
	private void Start()
	{
		if (!this.updateOnEnableInstead)
		{
			ScreenManager.Instance.OnScreenTransitionEnded += this.Instance_OnScreenTransitionEnded;
		}
	}

	private void OnEnable()
	{
		if (this.updateOnEnableInstead)
		{
			this.UpdateUI();
		}
	}

	private void Instance_OnScreenTransitionEnded(ScreenManager.Screen to, ScreenManager.Screen from)
	{
		if (to == ScreenManager.Screen.Shop)
		{
			this.UpdateUI();
		}
	}

	private void UpdateUI()
	{
		if (!ResourceManager.Instance.IsInitialized || this.hasUpdatedGemPackUI)
		{
			return;
		}
		this.gemPack1Amount.SetVariableText(new string[]
		{
			40.ToString()
		});
		this.gemPack1Cost.SetVariableText(new string[]
		{
			ResourceManager.Instance.GetMarketItemPriceAndCurrency("se.ace.gem_pack_1.sku")
		});
		int crownExpAmountAtLocation = CrownExpGranterManager.Instance.GetCrownExpAmountAtLocation(IAPPlacement.GemPack1);
		this.crownPack1Amount.SetVariableText(new string[]
		{
			crownExpAmountAtLocation.ToString()
		});
		this.gemPack2Amount.SetVariableText(new string[]
		{
			300.ToString()
		});
		this.gemPack2Cost.SetVariableText(new string[]
		{
			ResourceManager.Instance.GetMarketItemPriceAndCurrency("se.ace.gem_pack_2.sku")
		});
		crownExpAmountAtLocation = CrownExpGranterManager.Instance.GetCrownExpAmountAtLocation(IAPPlacement.GemPack2);
		this.crownPack2Amount.SetVariableText(new string[]
		{
			crownExpAmountAtLocation.ToString()
		});
		this.gemPack3Amount.SetVariableText(new string[]
		{
			800.ToString()
		});
		this.gemPack3Cost.SetVariableText(new string[]
		{
			ResourceManager.Instance.GetMarketItemPriceAndCurrency("se.ace.gem_pack_3.sku")
		});
		crownExpAmountAtLocation = CrownExpGranterManager.Instance.GetCrownExpAmountAtLocation(IAPPlacement.GemPack3);
		this.crownPack3Amount.SetVariableText(new string[]
		{
			crownExpAmountAtLocation.ToString()
		});
		this.gemPack4Amount.SetVariableText(new string[]
		{
			3000.ToString()
		});
		this.gemPack4Cost.SetVariableText(new string[]
		{
			ResourceManager.Instance.GetMarketItemPriceAndCurrency("se.ace.gem_pack_4.sku")
		});
		crownExpAmountAtLocation = CrownExpGranterManager.Instance.GetCrownExpAmountAtLocation(IAPPlacement.GemPackWhale);
		this.crownPack4Amount.SetVariableText(new string[]
		{
			crownExpAmountAtLocation.ToString()
		});
		this.hasUpdatedGemPackUI = true;
	}

	[SerializeField]
	private bool updateOnEnableInstead;

	[SerializeField]
	private TextMeshProUGUI gemPack1Amount;

	[SerializeField]
	private TextMeshProUGUI crownPack1Amount;

	[SerializeField]
	private TextMeshProUGUI gemPack1Cost;

	[SerializeField]
	private TextMeshProUGUI gemPack2Amount;

	[SerializeField]
	private TextMeshProUGUI crownPack2Amount;

	[SerializeField]
	private TextMeshProUGUI gemPack2Cost;

	[SerializeField]
	private TextMeshProUGUI gemPack3Amount;

	[SerializeField]
	private TextMeshProUGUI crownPack3Amount;

	[SerializeField]
	private TextMeshProUGUI gemPack3Cost;

	[SerializeField]
	private TextMeshProUGUI gemPack4Amount;

	[SerializeField]
	private TextMeshProUGUI crownPack4Amount;

	[SerializeField]
	private TextMeshProUGUI gemPack4Cost;

	private bool hasUpdatedGemPackUI;
}
