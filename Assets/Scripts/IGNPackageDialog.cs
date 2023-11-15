using System;
using System.Diagnostics;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class IGNPackageDialog : InGameNotificationDialog<IGNPackage>
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action OnPackageCollected;

	private BigInteger CashAmount
	{
		get
		{
			return this.inGameNotification.CashAmount * this.contentAdMultiplier;
		}
	}

	protected override void OnAboutToOpen()
	{
		this.cashAsString = CashFormatter.SimpleToCashRepresentation(this.CashAmount, 3, false, true);
		this.cashAmount.SetText(this.cashAsString);
		
		float currentTotalValueFor = SkillManager.Instance.GetCurrentTotalValueFor<Skills.PackageAdMultiplier>();
		this.adMultiplyLabel.SetVariableText(new string[]
		{
			currentTotalValueFor.ToString()
		});
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
	}

	protected override void OnOpened()
	{
	}

	protected override void OnRemovedFromList()
	{
		this.iconTween.IconTweenKiller();
		this.SpawnMoneyEffect();
	}

	protected override void OnReturned()
	{
	}

	public void MultiplyByWatchingAd()
	{
		
	}

	private void SpawnMoneyEffect()
	{
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate<ParticleSystem>(this.cashEffect);
		particleSystem.transform.position = UnityEngine.Vector3.zero;
		particleSystem.transform.localScale = UnityEngine.Vector3.one * CameraMovement.Instance.Zoom;
		TextMeshProUGUI labelInstance = TextObjectPool.Instance.TextMeshProPoolUGUI.GetObject();
		labelInstance.transform.SetParent(base.transform.parent, false);
		labelInstance.transform.position = this.cashAmount.transform.position;
		labelInstance.fontSize = 95f;
		labelInstance.gameObject.SetActive(true);
		labelInstance.alignment = TextAlignmentOptions.Center;
		labelInstance.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 800f);
		labelInstance.transform.localScale = UnityEngine.Vector3.one;
		labelInstance.color = Color.white;
		labelInstance.DOFade(0f, 1f).SetEase(Ease.InCubic);
		labelInstance.transform.DOScale(new UnityEngine.Vector3(0.4f, 0.4f, 0.4f), 1f).SetEase(Ease.InCubic);
		labelInstance.transform.DOMoveY(Camera.main.ViewportToWorldPoint(new UnityEngine.Vector3(0f, 0.85f, 0f)).y, 1f, false).SetEase(Ease.Linear).OnComplete(delegate
		{
			labelInstance.DOKill(false);
			labelInstance.transform.DOKill(false);
			labelInstance.gameObject.SetActive(false);
			TextObjectPool.Instance.TextMeshProPoolUGUI.ReturnObject(labelInstance);
		});
		ResourceManager.Instance.GiveResource(ResourceType.Cash, this.CashAmount);
		AudioManager.Instance.Cacthing();
		labelInstance.SetText("<b>" + this.cashAsString + "</b>");
		if (IGNPackageDialog.OnPackageCollected != null)
		{
			IGNPackageDialog.OnPackageCollected();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.cashAmount != null && this.cashAmount.transform != null)
		{
			this.cashAmount.transform.DOKill(false);
		}
	}

	[SerializeField]
	private IGNPackageTween iconTween;

	[SerializeField]
	private TextMeshProUGUI cashAmount;

	[SerializeField]
	private ParticleSystem cashEffect;

	[SerializeField]
	private ParticleSystem[] addDubbleEffects;

	[SerializeField]
	private TextMeshProUGUI adMultiplyLabel;

	[SerializeField]
	private AdBonusIncreaseButton watchAdToMultiply;

	private string cashAsString = "0";

	private int contentAdMultiplier = 1;
}
