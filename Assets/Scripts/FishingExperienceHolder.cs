using System;
using System.Diagnostics;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingExperienceHolder : MonoBehaviour
{
	public static FishingExperienceHolder Instance { get; private set; }

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<long, long> OnCollectedFishingExp;

	private void Awake()
	{
		SkillManager.Instance.CollectStarsSkill.OnSkillLevelUp += this.CollectStarsSkill_OnSkillLevelUp;
		this.prevPos = this.rectTransform.anchoredPosition;
		FishingExperienceHolder.Instance = this;
	}

	private void CollectStarsSkill_OnSkillLevelUp(Skill arg1, LevelChange arg2)
	{
		this.totalStars0.SetVariableText(new string[]
		{
			arg1.CurrentLevel.ToString()
		});
		this.totalStars1.SetVariableText(new string[]
		{
			arg1.CurrentLevel.ToString()
		});
	}

	private void Start()
	{
		this.SetCacheValues(this.totalTierSkillUpgrades);
		this.SetCacheValues(this.totalCollectedFishes);
		this.totalFishingXP.SetVariableText(new string[]
		{
			this.prestigeSkill.CurrentLevelAsLong.ToString()
		});
		this.toBeCollectedFishingXP.SetVariableText(new string[]
		{
			this.TotalFishingExp.ToString()
		});
		this.totalFishValue.SetVariableText(new string[]
		{
			this.TotalFishValue.ToString()
		});
		SkillManager.Instance.OnSkillLevelChanged += this.Instance_OnSkillLevelUp;
		ScreenManager.Instance.OnScreenTransitionStarted += this.Instance_OnScreenTransitionStarted;
	}

	private void Instance_OnScreenTransitionStarted(ScreenManager.Screen to, ScreenManager.Screen from)
	{
		if (this.isInfo)
		{
			this.ToggleInfo();
		}
	}

	private void Instance_OnSkillLevelUp(Skill skillThatLeveledUp, LevelChange change)
	{
		if (change == LevelChange.Initialization || change == LevelChange.HardReset)
		{
			return;
		}
		if (this.SetCacheValues(skillThatLeveledUp))
		{
			this.toBeCollectedFishingXP.SetVariableText(new string[]
			{
				this.TotalFishingExp.ToString()
			});
		}
		else if (skillThatLeveledUp == this.collectedStarsSkill)
		{
			this.totalFishValue.SetVariableText(new string[]
			{
				this.TotalFishValue.ToString()
			});
		}
	}

	private bool SetCacheValues(Skill skillThatLeveledUp)
	{
		if (skillThatLeveledUp == this.totalCollectedFishes)
		{
			this.cacheTotalCollectedFishes = (float)this.totalCollectedFishes.CurrentLevel * (0.008f + (float)SkillManager.Instance.TierSkill.CurrentLevel * 0.001f + 0.001f * Mathf.Pow((float)this.prestigeSkill.CurrentLevelAsLong, 0.25f));
			return true;
		}
		return skillThatLeveledUp == this.dailyCatchFishingExp;
	}

	public void ToggleInfo()
	{
		if (!this.isInfo)
		{
			this.OpenInfo();
		}
		else
		{
			this.CloseInfo();
		}
		this.isInfo = !this.isInfo;
		AudioManager.Instance.MenuWhoosh();
	}

	public void Claim()
	{
		if (this.TotalFishingExp <= 0L)
		{
			this.SpawnNotEnoughFishExpToast();
		}
		else if (this.isInfo)
		{
			if (this.isclaimingAnimation)
			{
				return;
			}
			this.dialogCloseBG.interactable = false;
			this.isclaimingAnimation = true;
			base.transform.DOKill(false);
			this.animation.Stop();
			this.animation.clip = this.animationClaim;
			this.animation.Play();
			this.whiteShade.gameObject.SetActive(true);
			AudioManager.Instance.OneShooter(this.prestiegeClip, 1.5f);
			AudioManager.Instance.MenuClick();
			this.whiteShade.DOFade(1f, 0.2f).SetDelay(1.8f).OnComplete(delegate
			{
				this.ToggleInfo();
				ScreenManager.Instance.GoToScreen(0);
				this.Prestige();
				this.whiteShade.DOFade(0f, 0.2f).SetDelay(0.3f).OnComplete(delegate
				{
					SkillManager.Instance.TierSkill.SetCurrentLevel(1, LevelChange.LevelUp);
					this.whiteShade.gameObject.SetActive(false);
					this.isclaimingAnimation = false;
				});
			});
			this.RunAfterDelay(1.1f, delegate()
			{
				AudioManager.Instance.OneShooter(this.splash1, 0.5f);
				AudioManager.Instance.OneShooter(this.splash2, 1.5f);
			});
		}
		else
		{
			this.ToggleInfo();
		}
	}

	public void PopUI1()
	{
		AudioManager.Instance.OneShooter(this.popClip, 1f);
		this.totalFishingXP.SetVariableText(new string[]
		{
			(this.prestigeSkill.CurrentLevelAsLong + this.TotalFishingExp).ToString()
		});
	}

	public void PopUI2()
	{
		AudioManager.Instance.OneShooter(this.popClip, 1f);
		this.totalFishValue.SetVariableText(new string[]
		{
			((this.prestigeSkill.CurrentLevelAsLong + this.TotalFishingExp) * (long)this.collectedStarsSkill.CurrentLevel).ToString()
		});
	}

	public void OpenInfo()
	{
		base.transform.DOKill(false);
		this.listScrollRect.enabled = false;
		this.dialogCloseBG.interactable = true;
		this.dialogCloseBG.gameObject.SetActive(true);
		this.animation.Stop();
		this.animation.clip = this.animationOpen;
		this.fishExpToCollectInOpenDialog.text = "+" + this.TotalFishingExp;
		this.ImprovementPercentageInOpenDialog.text = "<b><color=#E66144FF>" + Mathf.Floor((float)this.TotalFishingExp / Mathf.Max(100f, (float)this.prestigeSkill.CurrentLevelAsLong) * 100f) + "% </color></b>better <b><color=#E66144FF>CASH</color></b> gain and <b><color=#E66144FF>EPIC</color></b> fish catch rate";
		base.transform.DOMove(new UnityEngine.Vector3(0f, 0.54f * CameraMovement.Instance.Zoom, 90f), 0.3f, false).SetEase(Ease.OutBack).OnComplete(delegate
		{
		});
		this.animation.Play();
	}

	public void CloseInfo()
	{
		base.transform.DOKill(false);
		this.listScrollRect.enabled = true;
		this.dialogCloseBG.gameObject.SetActive(false);
		this.animation.Stop();
		this.animation.clip = this.animationClose;
		this.rectTransform.DOAnchorPos(this.prevPos, 0.3f, false).SetEase(Ease.OutBack);
		this.animation.Play();
	}

	public void GainBonusExperience()
	{
		this.totalBonusExp.TryLevelUp();
	}

	public long TotalFishingExp
	{
		get
		{
			long num = (long)(this.cacheTotalTierSkillsUpgrades + this.cacheTotalCollectedFishes + (float)this.totalBonusExp.CurrentLevel);
			long num2 = (long)((float)num * SkillManager.Instance.GetCurrentTotalValueFor<Skills.FishingExperienceGainMultiplier>());
			return (long)((float)(num + num2) * SkillManager.Instance.GetCurrentTotalValueFor<Skills.FishingExperienceGain>()) + (long)this.dailyCatchFishingExp.CurrentLevel;
		}
	}

	public BigInteger TotalFishValue
	{
		get
		{
			return this.prestigeSkill.CurrentLevelAsLong * (long)SkillManager.Instance.CollectStarsSkill.CurrentLevel;
		}
	}

	public void OnPrestigeClick()
	{
		this.prestiegeDialog.Open();
		this.prestiegeDialog.UpdateUI();
	}

	public void Prestige()
	{
		this.oldFishingExp = this.prestigeSkill.CurrentLevelAsLong;
		this.toCollectFishingExp = this.TotalFishingExp;
		long level = this.oldFishingExp + this.toCollectFishingExp;
		this.prestigeSkill.SetCurrentLevelAsLong(level, LevelChange.LevelUp);
		this.totalFishingXP.SetVariableText(new string[]
		{
			this.prestigeSkill.CurrentLevelAsLong.ToString()
		});
		this.totalTierSkillUpgrades.SetCurrentLevel(0, LevelChange.SoftReset);
		this.toBeCollectedFishingXP.SetVariableText(new string[]
		{
			"0"
		});
		this.totalFishValue.SetVariableText(new string[]
		{
			this.TotalFishValue.ToString()
		});
		SkillManager.Instance.ResetSkills();
		this.cacheTotalTierSkillsUpgrades = 0f;
		this.cacheTotalCollectedFishes = 0f;
		AudioManager.Instance.BossTimeEnd();
		CameraMovement.bossTime = false;
		Time.timeScale = 1f;
		this.listScrollRect.verticalNormalizedPosition = 0f;
		this.queuedBoatLevelUp = true;
		if (this.OnCollectedFishingExp != null)
		{
			this.OnCollectedFishingExp(this.toCollectFishingExp, this.oldFishingExp);
		}
	}

	public void UpdatePrestigeTextUI()
	{
		this.totalFishingXP.SetVariableText(new string[]
		{
			this.prestigeSkill.CurrentLevelAsLong.ToString()
		});
	}

	private void SpawnNotEnoughFishExpToast()
	{
		TextMeshProUGUI upgradeInfoFeedbackInstance = UnityEngine.Object.Instantiate<TextMeshProUGUI>(this.upgradeInfoFeedbackLabel, base.transform.parent, false);
		upgradeInfoFeedbackInstance.transform.position = base.transform.position;
		upgradeInfoFeedbackInstance.fontSize = 45f;
		upgradeInfoFeedbackInstance.text = "Get more Fishing Exp!";
		upgradeInfoFeedbackInstance.transform.DOPunchScale(new UnityEngine.Vector2(0.2f, 0.1f), 0.6f, 10, 1f);
		upgradeInfoFeedbackInstance.transform.DOMove(new UnityEngine.Vector3(upgradeInfoFeedbackInstance.transform.position.x, upgradeInfoFeedbackInstance.transform.position.y + 0.05f * CameraMovement.Instance.Zoom, upgradeInfoFeedbackInstance.transform.position.z), 1.2f, false).SetEase(Ease.OutCirc).OnComplete(delegate
		{
			upgradeInfoFeedbackInstance.transform.DOKill(false);
			upgradeInfoFeedbackInstance.DOKill(false);
			UnityEngine.Object.Destroy(upgradeInfoFeedbackInstance.transform.gameObject);
		});
		upgradeInfoFeedbackInstance.DOFade(0f, 1.2f).SetEase(Ease.InCirc);
	}

	private const float COLLECT_FISH_MODIFIER = 0.008f;

	[SerializeField]
	private Skill prestigeSkill;

	[SerializeField]
	private Skill totalTierSkillUpgrades;

	[SerializeField]
	private Skill totalCollectedFishes;

	[SerializeField]
	private Skill collectedStarsSkill;

	[SerializeField]
	private Skill totalBonusExp;

	[SerializeField]
	private Skill dailyCatchFishingExp;

	[SerializeField]
	private TextMeshProUGUI totalFishingXP;

	[SerializeField]
	private TextMeshProUGUI totalStars0;

	[SerializeField]
	private TextMeshProUGUI totalStars1;

	[SerializeField]
	private TextMeshProUGUI totalFishValue;

	[SerializeField]
	private TextMeshProUGUI toBeCollectedFishingXP;

	[SerializeField]
	private TextMeshProUGUI explainTotalFishValueGained;

	[SerializeField]
	private TextMeshProUGUI fishExpToCollectInOpenDialog;

	[SerializeField]
	private TextMeshProUGUI ImprovementPercentageInOpenDialog;

	[SerializeField]
	private PrestiegeDialog prestiegeDialog;

	[SerializeField]
	private Animation animation;

	[SerializeField]
	private AnimationClip animationOpen;

	[SerializeField]
	private AnimationClip animationClose;

	[SerializeField]
	private AnimationClip animationClaim;

	[SerializeField]
	private Image whiteShade;

	[SerializeField]
	private AudioClip splash1;

	[SerializeField]
	private AudioClip splash2;

	[SerializeField]
	private AudioClip prestiegeClip;

	[SerializeField]
	private AudioClip popClip;

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private RectTransform buttonRect;

	[SerializeField]
	private RectTransform BgShadowShineRect;

	[SerializeField]
	private Image BgShadowShineImage;

	[SerializeField]
	private ScrollRect listScrollRect;

	[SerializeField]
	private TextMeshProUGUI upgradeInfoFeedbackLabel;

	[SerializeField]
	private PressReleaseButton dialogCloseBG;

	private UnityEngine.Vector2 prevPos;

	private float cacheTotalTierSkillsUpgrades;

	private float cacheTotalCollectedFishes;

	private bool isInfo;

	private bool isclaimingAnimation;

	private bool tweening;

	public bool queuedBoatLevelUp;

	public long oldFishingExp;

	public long toCollectFishingExp;
}
