using System;
using System.Diagnostics;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class IGNGemChestDialog : InGameNotificationDialog<IGNGemChest>
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action OnGemChestCollected;

	protected override void Awake()
	{
		base.Awake();

	}

	private bool HasReceivedAdBonus
	{
		get
		{
			return this.contentAdExtraGems > 0;
		}
	}

	private int GemAmount
	{
		get
		{
			return this.inGameNotification.GemAmount + this.contentAdExtraGems;
		}
	}

	private int CrownAmount
	{
		get
		{
			return CrownExpGranterManager.Instance.GetCrownExpAmountAtLocation(AdPlacement.GemChest);
		}
	}

	public void AddGemsByWatchingAd()
	{
		
	}

	protected override void OnAboutToOpen()
	{
		if (!this.hasUnlockedChest)
		{
			this.SetLockedUI();
		}
		else
		{
			this.SetUnlockedUI();
		}
		this.UpdateWatchAdButton(true);
	}

	private void OnAdAvailable(string adUnitId)
	{
		if (base.IsOpen && !this.HasReceivedAdBonus)
		{
			this.UpdateWatchAdButton(false);
		}
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
		this.fakeMinGemAmount = UnityEngine.Random.Range(1, 4);
	}

	protected override void OnOpened()
	{
	}

	protected override void OnRemovedFromList()
	{
		this.iconTween.IconTweenKiller();
		this.TweenKiller();
	}

	protected override void OnReturned()
	{
	}

	public void UnlockClicked()
	{
		if (!this.hasUnlockedChest)
		{
			this.allowCloseByPressingCross = false;
			UIIAPPendingBlocker.Instance.Show();
			
		}
		else
		{
			GameAnalyticsEvents.ResourceGemsIncreased(AnalyticsEvents.REType.Chest, AnalyticsEvents.RECategory.Gameplay, this.GemAmount);
			ResourceChangeData gemChangeData = new ResourceChangeData("contentId_gemChestOpen", null, this.GemAmount, ResourceType.Gems, ResourceChangeType.Earn, ResourceChangeReason.GemChestOpen);
			GemGainVisual.Instance.GainGems(this.GemAmount, new Vector2(0f, 2.5f), gemChangeData);
			ResourceChangeData changeData = new ResourceChangeData("contentId_gemChestOpen", null, 0, ResourceType.CrownExp, ResourceChangeType.Earn, ResourceChangeReason.GemChestOpen);
			CrownExpGranterManager.Instance.Grant(AdPlacement.GemChest, changeData);
			this.inGameNotification.OverrideClearable = true;
			if (IGNGemChestDialog.OnGemChestCollected != null)
			{
				IGNGemChestDialog.OnGemChestCollected();
			}
			this.Close(true);
		}
	}

	public void SetLockedUI()
	{
		this.gemAmountLable.SetVariableText(new string[]
		{
			this.fakeMinGemAmount.ToString(),
			(this.inGameNotification.MaxRandomGemAmount + this.fakeMinGemAmount).ToString()
		});
	}

	public void SetUnlockedUI()
	{
		if (this.gemAmountLable != null && this.infoLabel != null && this.gemAmountLable.transform != null && this.buttonText != null)
		{
			this.buttonText.SetText("Claim");
			this.gemAmountLable.SetText("<size=50>" + this.GemAmount + "<sprite=0></size>");
			this.CrownAmountLable.SetText("<size=50>" + this.CrownAmount + "<sprite=2></size>");
			this.infoLabel.SetText("Claim your treasure");
			this.gemAmountLable.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.4f, 10, 1f);
		}
	}

	private void UpdateWatchAdButton(bool hideButtonIfAlreadyGrantedAdBonus)
	{
		if (this.hasUnlockedChest)
		{
			if (this.HasReceivedAdBonus && hideButtonIfAlreadyGrantedAdBonus)
			{
				this.watchAdToGetExtraGemsButton.gameObject.SetActive(false);
			}
			else
			{
				this.watchAdToGetExtraGemsButton.gameObject.SetActive(true);
				this.watchAdToGetExtraGemsLabel.SetVariableText(new string[]
				{
					3.ToString()
				});
				
			}
		}
		else
		{
			this.watchAdToGetExtraGemsButton.gameObject.SetActive(false);
		}
	}

	private void TweenKiller()
	{
		if (this.gemAmountLable != null)
		{
			this.gemAmountLable.transform.DOKill(false);
		}
		if (this.CrownAmountLable != null)
		{
			this.CrownAmountLable.transform.DOKill(false);
		}
	}

	public void CancelDialog()
	{
		if (this.allowCloseByPressingCross)
		{
			this.inGameNotification.OverrideClearable = true;
			this.Close(true);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		
	}

	private const int EXTRA_GEMS_BY_WATCHING_ADS = 3;

	private const string contentId_gemChestOpen = "contentId_gemChestOpen";

	[SerializeField]
	private TextMeshProUGUI buttonText;

	[SerializeField]
	private TextMeshProUGUI gemAmountLable;

	[SerializeField]
	private TextMeshProUGUI CrownAmountLable;

	[SerializeField]
	private TextMeshProUGUI infoLabel;

	[SerializeField]
	private IGNPackageTween iconTween;

	[SerializeField]
	private TextMeshProUGUI watchAdToGetExtraGemsLabel;

	[SerializeField]
	private AdBonusIncreaseButton watchAdToGetExtraGemsButton;

	[SerializeField]
	private ParticleSystem addDubbleEffects;

	private bool hasUnlockedChest;

	private int contentAdExtraGems;

	private int fakeMinGemAmount = 1;

	private bool allowCloseByPressingCross = true;
}
