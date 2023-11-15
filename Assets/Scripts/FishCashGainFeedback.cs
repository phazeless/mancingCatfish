using System;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FishCashGainFeedback : MonoBehaviour
{
	private void Start()
	{
	}

	private void ResourceManager_OnResourceChanged(ResourceType type, BigInteger amount, BigInteger TotalAmount)
	{
		if (type == ResourceType.Cash && TotalAmount > this.lastAmount)
		{
			this.SpawnUpgradeInfoFeedback(amount);
		}
		this.lastAmount = TotalAmount;
	}

	private void SpawnUpgradeInfoFeedback(BigInteger amount)
	{
		if (this.latestUpgradeInfoFeedbackInstance == null)
		{
			this.latestUpgradeInfoFeedbackInstance = UnityEngine.Object.Instantiate<TextMeshProUGUI>(this.upgradeInfoFeedbackLabel, this.upgradeInfoFeedbackLabelPositioner, false);
		}
		this.latestUpgradeInfoFeedbackInstance.transform.DOKill(false);
		this.latestUpgradeInfoFeedbackInstance.DOKill(false);
		this.latestUpgradeInfoFeedbackInstance.transform.position = this.upgradeInfoFeedbackLabelPositioner.position;
		this.latestUpgradeInfoFeedbackInstance.color = Color.white;
		this.latestUpgradeInfoFeedbackInstance.transform.localScale = UnityEngine.Vector2.one;
		this.latestUpgradeInfoFeedbackInstance.text = CashFormatter.SimpleToCashRepresentation(amount, 3, false, true);
		this.latestUpgradeInfoFeedbackInstance.transform.DOPunchScale(new UnityEngine.Vector2(0.2f, 0.1f), 0.6f, 10, 1f);
		this.latestUpgradeInfoFeedbackInstance.transform.DOMove(new UnityEngine.Vector3(this.latestUpgradeInfoFeedbackInstance.transform.position.x, this.latestUpgradeInfoFeedbackInstance.transform.position.y + 0.2f, this.latestUpgradeInfoFeedbackInstance.transform.position.z), 0.8f, false).SetEase(Ease.InCirc);
		this.latestUpgradeInfoFeedbackInstance.DOFade(0f, 0.8f).SetEase(Ease.InCirc);
	}

	[SerializeField]
	private TextMeshProUGUI upgradeInfoFeedbackLabel;

	[SerializeField]
	private Transform upgradeInfoFeedbackLabelPositioner;

	private BigInteger lastAmount;

	private TextMeshProUGUI latestUpgradeInfoFeedbackInstance;
}
