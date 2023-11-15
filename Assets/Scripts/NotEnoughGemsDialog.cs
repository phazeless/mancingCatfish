using System;
using ACE.IAPS;
using TMPro;
using UnityEngine;

public class NotEnoughGemsDialog : UpgradeDialogTween
{
	public static NotEnoughGemsDialog Instance { get; private set; }

	private void OnEnable()
	{
		ResourceManager.Instance.OnPurchaseResponse -= this.Instance_OnPurchaseResponse;
		ResourceManager.Instance.OnPurchaseResponse += this.Instance_OnPurchaseResponse;
	}

	private void OnDisable()
	{
		ResourceManager.Instance.OnPurchaseResponse -= this.Instance_OnPurchaseResponse;
	}

	private void Instance_OnPurchaseResponse(PurchaseResult result, string itemId, ResourceChangeReason reason)
	{
		if (result == PurchaseResult.ItemPurchased && reason == ResourceChangeReason.PurchaseGemPack)
		{
			this.Close(false);
		}
	}

	public void OpenWithMissingGems(int missingGems)
	{
		this.Open();
		this.gemsLeft.SetVariableText(new string[]
		{
			missingGems.ToString()
		});
	}

	[SerializeField]
	private TextMeshProUGUI gemsLeft;
}
