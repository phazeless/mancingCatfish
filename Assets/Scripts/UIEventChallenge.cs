using System;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEventChallenge : MonoBehaviour
{
	private BaseGoal CurrentGoal
	{
		get
		{
			return this.currentChallenge.CurrentGoal;
		}
	}

	private void Start()
	{
		this.descLabel.color = this.color_description;
		this.progressMeterText.color = this.color_meterText;
		this.borderImg.color = this.color_border;
		this.progressMeter.MeterImg.color = this.color_meter;
		this.progressMeter.MeterBgImg.color = this.color_meterBg;
		this.claimRewardButton.color = this.color_claimRewardButton;
		this.completedChallengeText.color = this.color_completedChallengeText;
		this.currentGoalLbl.color = this.color_currentGoalTxt;
		this.currentGoalBg.color = this.color_currentGoalBg;
		this.goalRewardAmountLbl.color = this.color_rewardAmonutTxt;
		this.rewardIconBg.color = this.color_rewardIconBg;
	}

	public void SetupChallenge(EventChallenge challenge)
	{
		this.currentChallenge = challenge;
		this.currentChallenge.OnGoalProgress += this.CurrentChallenge_OnProgress;
		this.currentChallenge.OnGoalCompleted += this.CurrentChallenge_OnGoalCompleted;
		this.currentChallenge.OnGoalClaimed += this.CurrentChallenge_OnGoalClaimed;
		this.currentChallenge.OnChallengeCompleted += this.CurrentChallenge_OnChallengeCompleted;
		this.UpdateUI();
	}

	private void CurrentChallenge_OnGoalClaimed(EventChallenge arg1, BaseGoal arg2)
	{
		this.UpdateUI();
	}

	private void CurrentChallenge_OnChallengeCompleted(EventChallenge obj)
	{
		this.UpdateUI();
	}

	public void ClaimGoalReward()
	{
		this.TweenKiller();
		base.transform.localScale = new UnityEngine.Vector3(1f, 1f, 1f);
		this.rewardIcon.transform.localScale = new UnityEngine.Vector3(1f, 1f, 1f);
		base.transform.DOPunchScale(new UnityEngine.Vector3(0.3f, 0.3f, 0.3f), 0.2f, 4, 0.5f);
		AudioManager.Instance.OneShooter(this.rewardSound, 1f);
		this.rewardIcon.transform.DOPunchScale(new UnityEngine.Vector3(1.3f, 1.3f, 1.3f), 0.3f, 4, 0.5f).OnComplete(delegate
		{
			this.CurrentGoal.CollectReward();
			base.transform.DOPunchScale(new UnityEngine.Vector3(-0.1f, -0.1f, -0.1f), 0.2f, 4, 0.5f);
		});
	}

	private void CurrentChallenge_OnProgress(EventChallenge arg1, BaseGoal arg2)
	{
		this.UpdateUI();
	}

	private void CurrentChallenge_OnGoalCompleted(EventChallenge arg1, BaseGoal arg2)
	{
		this.UpdateUI();
	}

	private void OnEnable()
	{
		this.UpdateUI();
	}

	private void UpdateUI()
	{
		this.rewardIcon.sprite = this.CurrentGoal.GoalReward.Icon;
		this.currentGoalLbl.SetText(string.Concat(new object[]
		{
			"(",
			this.currentChallenge.CurrentGoalIndex + 1,
			"/",
			this.currentChallenge.MaxGoals,
			") "
		}));
		this.descLabel.SetText(this.currentChallenge.CurrentGoalDescription);
		BigInteger totalRequiredTargetValue = this.currentChallenge.TotalRequiredTargetValue;
		BigInteger totalProgress = this.currentChallenge.TotalProgress;
		this.progressMeter.SetMax(totalRequiredTargetValue);
		this.progressMeter.SetCurrent(totalProgress);
		this.progressMeterText.SetVariableText(new string[]
		{
			this.GetTotalProgressValueFormatted(),
			this.GetTotalTargetValueFormatted()
		});
		this.claimRewardButton.gameObject.SetActive(this.currentChallenge.CurrentGoal.IsCompleted && !this.currentChallenge.CurrentGoal.IsClaimed);
		this.contentHolder.SetActive(!this.currentChallenge.IsCompleted);
		this.completedChallengeText.gameObject.SetActive(this.currentChallenge.IsCompleted);
		this.goalRewardAmountLbl.SetText("x" + this.currentChallenge.CurrentGoal.GoalReward.Amount.ToString());
	}

	protected virtual string GetTotalProgressValueFormatted()
	{
		if (this.currentChallenge.UseBigIntegerCashFormatting)
		{
			return CashFormatter.SimpleToCashRepresentation(this.currentChallenge.TotalProgress, 0, false, true);
		}
		return this.currentChallenge.TotalProgress.ToString();
	}

	protected virtual string GetTotalTargetValueFormatted()
	{
		if (this.currentChallenge.UseBigIntegerCashFormatting)
		{
			return CashFormatter.SimpleToCashRepresentation(this.currentChallenge.TotalRequiredTargetValue, 0, false, false);
		}
		return this.currentChallenge.TotalRequiredTargetValue.ToString();
	}

	private void TweenKiller()
	{
		this.rewardIcon.transform.DOKill(false);
		base.transform.DOKill(false);
	}

	private void OnDestroy()
	{
		this.TweenKiller();
		if (this.currentChallenge != null)
		{
			this.currentChallenge.OnGoalProgress -= this.CurrentChallenge_OnProgress;
			this.currentChallenge.OnGoalCompleted -= this.CurrentChallenge_OnGoalCompleted;
			this.currentChallenge.OnGoalClaimed -= this.CurrentChallenge_OnGoalClaimed;
			this.currentChallenge.OnChallengeCompleted -= this.CurrentChallenge_OnChallengeCompleted;
		}
	}

	[SerializeField]
	private Color color_description = Color.black;

	[SerializeField]
	private Color color_meterText = Color.black;

	[SerializeField]
	private Color color_meter = Color.black;

	[SerializeField]
	private Color color_meterBg = Color.black;

	[SerializeField]
	private Color color_border = Color.black;

	[SerializeField]
	private Color color_claimRewardButton = Color.black;

	[SerializeField]
	private Color color_completedChallengeText = Color.black;

	[SerializeField]
	private Color color_currentGoalBg = Color.black;

	[SerializeField]
	private Color color_currentGoalTxt = Color.black;

	[SerializeField]
	private Color color_rewardAmonutTxt = Color.black;

	[SerializeField]
	private Color color_rewardIconBg = Color.black;

	[SerializeField]
	private Color color_rewardCountBg = Color.black;

	[SerializeField]
	private GameObject contentHolder;

	[SerializeField]
	private TextMeshProUGUI descLabel;

	[SerializeField]
	private UIMeterBigInteger progressMeter;

	[SerializeField]
	private TextMeshProUGUI progressMeterText;

	[SerializeField]
	private Image rewardIcon;

	[SerializeField]
	private Image borderImg;

	[SerializeField]
	private Image claimRewardButton;

	[SerializeField]
	private TextMeshProUGUI completedChallengeText;

	[SerializeField]
	private TextMeshProUGUI currentGoalLbl;

	[SerializeField]
	private Image currentGoalBg;

	[SerializeField]
	private TextMeshProUGUI goalRewardAmountLbl;

	[SerializeField]
	private Image rewardIconBg;

	[SerializeField]
	private Image rewardCountBg;

	[SerializeField]
	private AudioClip rewardSound;

	[SerializeField]
	private EventChallenge currentChallenge;
}
