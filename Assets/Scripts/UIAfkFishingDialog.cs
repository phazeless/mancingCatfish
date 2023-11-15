using System;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAfkFishingDialog : UpgradeDialogTween
{
	protected override void Awake()
	{
		base.Awake();
		
	}

	private new void Start()
	{
		this.afkTimeSkill.OnSkillLevelUp += this.AfkTimeSkill_OnSkillLevelUp;
		this.x2StartYPosition = this.x2.transform.position.y;
	}

	private void AfkTimeSkill_OnSkillLevelUp(Skill skill, LevelChange arg2)
	{
		float num = this.afkTime / 60f / 60f;
		this.maxAfkTime += 1f;
		if (num <= this.maxAfkTime)
		{
			this.awayTimeLabel.SetVariableText(new string[]
			{
				num.ToString("F1"),
				this.maxAfkTime.ToString("F1")
			});
		}
		else
		{
			this.awayTimeLabel.SetText(string.Concat(new string[]
			{
				"You were away for over <color=#BC280FFF><b>",
				this.maxAfkTime.ToString("F1"),
				" / ",
				this.maxAfkTime.ToString("F1"),
				" h</b></color>"
			}));
		}
	}

	public void SetEarned(BigInteger earned)
	{
		this.earnedText.SetVariableText(new string[]
		{
			CashFormatter.SimpleToCashRepresentation(earned, 1, false, false)
		});
	}

	private void OnAdAvailable(string adUnitId)
	{
		
	}

	public int GetIdleDoubleUpMultiplier()
	{
		int num = 2;
		float currentTotalValueFor = SkillManager.Instance.GetCurrentTotalValueFor<Skills.IdleDoubleUpMultiplierBonus>();
		float num2 = (float)num + currentTotalValueFor;
		return (int)num2;
	}

	private string GetIdleDoubleUpButtonText()
	{
		int idleDoubleUpMultiplier = this.GetIdleDoubleUpMultiplier();
		if (idleDoubleUpMultiplier == 3)
		{
			return "Triple It!";
		}
		if (idleDoubleUpMultiplier == 4)
		{
			return "Quadruple!";
		}
		if (idleDoubleUpMultiplier > 4)
		{
			return "Multiply by " + idleDoubleUpMultiplier + "!";
		}
		return "Double Up!";
	}

	private string GetIdleDoubleUpSuccessButtonText()
	{
		int idleDoubleUpMultiplier = this.GetIdleDoubleUpMultiplier();
		if (idleDoubleUpMultiplier == 3)
		{
			return "tripled It";
		}
		if (idleDoubleUpMultiplier == 4)
		{
			return "quadrupled";
		}
		if (idleDoubleUpMultiplier > 4)
		{
			return "multiplied by " + idleDoubleUpMultiplier + string.Empty;
		}
		return "doubled Up";
	}

	public void SetUpDialog(BigInteger earned, float afkTime, float maxAfkTime)
	{
		this.earned = earned;
		this.afkTime = afkTime;
		this.maxAfkTime = maxAfkTime;
		DialogInteractionHandler.Instance.DisableCloseByClickingShade = true;
		this.doubleUpButton.gameObject.SetActive(true);
		this.successOnDouble.localScale = UnityEngine.Vector3.zero;
		this.x2.transform.localScale = UnityEngine.Vector3.zero;
		this.multiplyAmountLabel.SetText(this.GetIdleDoubleUpButtonText());
		this.successOnDoubleLabel.SetVariableText(new string[]
		{
			this.GetIdleDoubleUpSuccessButtonText()
		});
		
		{
			this.doubleUpButton.interactable = false;
			this.loadingAdsText.SetText("(LOADING AD...)");
		}
		this.didDoubleUp = false;
		float num = afkTime / 60f / 60f;
		if (num <= maxAfkTime)
		{
			this.awayTimeLabel.SetVariableText(new string[]
			{
				num.ToString("F1"),
				maxAfkTime.ToString("F1")
			});
		}
		else
		{
			this.awayTimeLabel.SetText(string.Concat(new string[]
			{
				"You were away for over <color=#BC280FFF><b>",
				maxAfkTime.ToString("F1"),
				" / ",
				maxAfkTime.ToString("F1"),
				" h</b></color>"
			}));
		}
		string text = CashFormatter.SimpleToCashRepresentation(earned, 3, false, true);
		this.earnedText.SetText(text);
	}

	public void OnDoubleUpClicked()
	{
		UIIAPPendingBlocker.Instance.Show();
		//MoPubMono.ShowVideo(AdPlacement.DoubleUp, delegate(bool shouldReward)
		//{
		//	this.didDoubleUp = shouldReward;
		//	if (this.didDoubleUp)
		//	{
		//		this.doubleUpButton.gameObject.SetActive(false);
		//		this.doubleUpButton.interactable = false;
		//		if (this.didDoubleUp)
		//		{
		//			this.x2.SetText("x" + this.GetIdleDoubleUpMultiplier());
		//			this.x2.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
		//			this.x2.transform.DOMoveY(this.earnedText.transform.position.y, 0.5f, false).SetEase(Ease.InBack).OnComplete(delegate
		//			{
		//				this.x2.transform.localScale = UnityEngine.Vector3.zero;
		//				this.earnedText.transform.DOPunchScale(UnityEngine.Vector2.one * 0.2f, 0.6f, 10, 1f).OnStart(delegate
		//				{
		//					this.SetEarned(this.earned * this.GetIdleDoubleUpMultiplier());
		//				});
		//			});
		//			this.successOnDouble.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f);
		//		}
		//	}
		//}, null);
	}

	public override void Close(bool destroyOnFinish = false)
	{
		base.Close(destroyOnFinish);
		AudioManager.Instance.Cacthing();
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate<ParticleSystem>(this.moneyParticlePrefab, base.transform, true);
		particleSystem.transform.SetParent(base.transform.parent, true);
		particleSystem.transform.localScale = UnityEngine.Vector3.one * 4f;
		particleSystem.transform.position = UnityEngine.Vector3.zero;
		DialogInteractionHandler.Instance.DisableCloseByClickingShade = false;
	}

	public void OnIncreaseMaximumAwayTime()
	{
		UIInGameNotificationItem uiinGameNotificationItemFor = this.crewMemberList.GetUIInGameNotificationItemFor(this.afkTimeSkill);
		if (uiinGameNotificationItemFor != null)
		{
			uiinGameNotificationItemFor.OnListItemClick();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.earnedText.DOKill(false);
		this.x2.transform.DOKill(false);
		this.successOnDouble.DOKill(false);
		//MoPubMono.UnregisterOnAdAvailableListener(new Action<string>(this.OnAdAvailable));
	}

	[SerializeField]
	private TextMeshProUGUI earnedText;

	[SerializeField]
	private TextMeshProUGUI awayTimeLabel;

	[SerializeField]
	private TextMeshProUGUI loadingAdsText;

	[SerializeField]
	private TextMeshProUGUI x2;

	[SerializeField]
	private Transform successOnDouble;

	[SerializeField]
	private TextMeshProUGUI successOnDoubleLabel;

	[SerializeField]
	private TextMeshProUGUI multiplyAmountLabel;

	[SerializeField]
	private ParticleSystem moneyParticlePrefab;

	[SerializeField]
	private ParticleSystem doubleEffect;

	[SerializeField]
	private Button increaseAwayTimeButton;

	[SerializeField]
	private Skill afkTimeSkill;

	[SerializeField]
	private Button doubleUpButton;

	[SerializeField]
	private UIActiveCrewMembersList crewMemberList;

	public bool didDoubleUp;

	private BigInteger earned = 0;

	private float afkTime;

	private float maxAfkTime;

	private float x2StartYPosition;
}
