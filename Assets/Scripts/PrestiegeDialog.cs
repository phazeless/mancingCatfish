using System;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrestiegeDialog : UpgradeDialogTween
{
	public void UpdateUI()
	{
		long totalFishingExp = FishingExperienceHolder.Instance.TotalFishingExp;
		BigInteger left = SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong;
		int currentLevel = SkillManager.Instance.CollectStarsSkill.CurrentLevel;
		BigInteger bigInteger = left * currentLevel;
		this.fishingExperienceToCollectLabel.SetVariableText(new string[]
		{
			totalFishingExp.ToString()
		});
		this.currentFishingExperienceLabel.SetVariableText(new string[]
		{
			left.ToString()
		});
		this.currentStarsLabel.SetVariableText(new string[]
		{
			currentLevel.ToString()
		});
		this.currentEarningBonusLabel.SetVariableText(new string[]
		{
			bigInteger.ToString()
		});
		if (totalFishingExp > 0L)
		{
			this.buttonShine.GetComponent<Image>().color = this.activeShineColor;
			this.buttonOutline.color = this.activeOutlineColor;
			this.buttonMiddle.color = this.activeMiddleColor;
			this.collectminilabel.color = this.ActiveMinilabels;
			this.fishexpminilabel.color = this.ActiveMinilabels;
			this.circle1.color = this.activeShineColor;
			this.circle2.color = this.activeShineColor;
			this.button.interactable = true;
		}
		else
		{
			this.buttonShine.GetComponent<Image>().color = this.inActiveShineColor;
			this.buttonOutline.color = this.inActiveOutlineColor;
			this.buttonMiddle.color = this.inActiveMiddleColor;
			this.collectminilabel.color = Color.white;
			this.fishexpminilabel.color = Color.white;
			this.circle1.color = this.inActiveShineColor;
			this.circle2.color = this.inActiveShineColor;
			this.button.interactable = false;
		}
		this.TweenKiller2();
		this.SetButtonShineLoop();
	}

	private void SetButtonShineLoop()
	{
		int num = 715;
		this.buttonShine.anchoredPosition = new UnityEngine.Vector2((float)(-(float)num) * 0.5f, 0f);
		this.buttonShine.DOAnchorPosX((float)num * 1.5f, 0.5f, false).SetDelay(2f).OnComplete(delegate
		{
			this.SetButtonShineLoop();
		});
	}

	private void TweenKiller2()
	{
		this.buttonShine.DOKill(true);
		DOTween.Kill("PrestiegeDialogInstanceLabel", false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.TweenKiller2();
	}

	private void OnDisable()
	{
		this.TweenKiller2();
	}

	public void OnCollectClick()
	{
		this.AnimateExpGain();
		FishingExperienceHolder.Instance.Prestige();
	}

	private void AnimateExpGain()
	{
		long oldEarningsBonus = SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong * (long)SkillManager.Instance.CollectStarsSkill.CurrentLevel;
		long newEarningsBonus = (SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong + FishingExperienceHolder.Instance.TotalFishingExp) * (long)SkillManager.Instance.CollectStarsSkill.CurrentLevel;
		this.fishingExperienceToCollectLabel.SetVariableText(new string[]
		{
			0.ToString()
		});
		TextMeshProUGUI expLabelInstance = UnityEngine.Object.Instantiate<TextMeshProUGUI>(this.fishingExperienceToCollectLabel, this.fishingExperienceToCollectLabel.transform.parent.parent, true);
		expLabelInstance.transform.SetAsFirstSibling();
		expLabelInstance.SetText("+" + FishingExperienceHolder.Instance.TotalFishingExp);
		this.buttonShine.GetComponent<Image>().color = this.inActiveShineColor;
		this.buttonOutline.color = this.inActiveOutlineColor;
		this.buttonMiddle.color = this.inActiveMiddleColor;
		this.circle1.color = this.inActiveShineColor;
		this.circle2.color = this.inActiveShineColor;
		this.collectminilabel.color = Color.white;
		this.fishexpminilabel.color = Color.white;
		this.button.interactable = false;
		expLabelInstance.DOColor(new Color(0.894f, 0.4f, 0.29f), 0.3f).OnComplete(delegate
		{
			this.currentFishingExperienceLabel.transform.DOPunchScale(new UnityEngine.Vector3(0.1f, 0.1f, 0f), 0.3f, 10, 1f);
			this.currentFishingExperienceLabel.SetVariableText(new string[]
			{
				SkillManager.Instance.PrestigeSkill.CurrentLevelAsLong.ToString()
			});
			DOTween.To(() => oldEarningsBonus, delegate(long x)
			{
				oldEarningsBonus = x;
			}, newEarningsBonus, 0.5f).OnUpdate(delegate
			{
				this.currentEarningBonusLabel.SetVariableText(new string[]
				{
					oldEarningsBonus.ToString()
				});
			}).OnComplete(delegate
			{
				this.currentEarningBonusLabel.transform.DOPunchScale(new UnityEngine.Vector3(0.1f, 0.1f, 0f), 0.3f, 10, 1f);
			});
			expLabelInstance.DOKill(false);
			expLabelInstance.transform.DOKill(false);
			UnityEngine.Object.Destroy(expLabelInstance.gameObject);
		});
		expLabelInstance.transform.DOMoveY(this.currentFishingExperienceLabel.transform.position.y, 0.3f, false).SetId("PrestiegeDialogInstanceLabel").OnComplete(delegate
		{
		});
	}

	[SerializeField]
	private Button button;

	[SerializeField]
	private TextMeshProUGUI fishingExperienceToCollectLabel;

	[SerializeField]
	private TextMeshProUGUI currentFishingExperienceLabel;

	[SerializeField]
	private TextMeshProUGUI currentStarsLabel;

	[SerializeField]
	private TextMeshProUGUI currentEarningBonusLabel;

	[SerializeField]
	private RectTransform buttonShine;

	[SerializeField]
	private Image buttonOutline;

	[SerializeField]
	private Image buttonMiddle;

	[SerializeField]
	private Image circle1;

	[SerializeField]
	private Image circle2;

	[SerializeField]
	private TextMeshProUGUI collectminilabel;

	[SerializeField]
	private TextMeshProUGUI fishexpminilabel;

	[SerializeField]
	private Color activeOutlineColor;

	[SerializeField]
	private Color inActiveOutlineColor;

	[SerializeField]
	private Color activeMiddleColor;

	[SerializeField]
	private Color inActiveMiddleColor;

	[SerializeField]
	private Color activeShineColor;

	[SerializeField]
	private Color inActiveShineColor;

	[SerializeField]
	private Color ActiveMinilabels;
}
