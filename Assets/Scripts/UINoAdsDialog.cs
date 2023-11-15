using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class UINoAdsDialog : UpgradeDialogTween
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action<bool> onShowFinished;

	public void Show(UINoAdsDialog.Reason reason, Action<bool> onShowFinished)
	{
		base.Open();
		this.gemCostLabel.SetVariableText(new string[]
		{
			this.gemCost.ToString()
		});
		this.onShowFinished = onShowFinished;
		this.textTitle.text = this.titleText;
		if (reason == UINoAdsDialog.Reason.NoAds)
		{
			this.useGemsButtonHolder.SetActive(true);
			this.textDescription.text = this.noAdsText;
		}
		else if (reason == UINoAdsDialog.Reason.NoInternet)
		{
			this.useGemsButtonHolder.SetActive(false);
			this.textDescription.text = this.noInternetText;
		}
	}

	public override void Close(bool destroyOnFinish = false)
	{
		base.Close(destroyOnFinish);
		if (this.onShowFinished != null)
		{
			this.onShowFinished(this.didUseGems);
		}
		this.onShowFinished = null;
		this.didUseGems = false;
	}

	public void OnUseGems()
	{
		this.didUseGems = ResourceManager.Instance.TakeResource(ResourceType.Gems, this.gemCost);
		this.Close(false);
	}

	[SerializeField]
	private int gemCost;

	[SerializeField]
	private TextMeshProUGUI textTitle;

	[SerializeField]
	private TextMeshProUGUI textDescription;

	[SerializeField]
	private string titleText;

	[SerializeField]
	private string noAdsText;

	[SerializeField]
	private string noInternetText;

	[SerializeField]
	private TextMeshProUGUI gemCostLabel;

	[SerializeField]
	private GameObject useGemsButtonHolder;

	private bool didUseGems;

	public enum Reason
	{
		NoAds,
		NoInternet
	}
}
