using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FireworkTriggerButton : MonoBehaviour
{
	public bool HasRockets
	{
		get
		{
			return this.lastKnownFireworkAmount > 0;
		}
	}

	public void Init(EventContent eventContent, BaseConsumable fireworkConsumable)
	{
		this.eventContent = eventContent;
		this.fireworkConsumable = fireworkConsumable;
	}

	public void UpdateUI()
	{
		this.lastKnownFireworkAmount = ConsumableManager.Instance.GetAmount(this.fireworkConsumable);
		this.isEventActive = (TimeManager.Instance.IsInitializedWithInternetTime && this.eventContent.IsWithinEventPeriod && this.eventContent.HasCompletedRequiredQuest);
		this.badgeAmountLbl.SetVariableText(new string[]
		{
			this.lastKnownFireworkAmount.ToString()
		});
		this.TweenKiller();
		if (TournamentManager.Instance.IsInsideTournament)
		{
			base.gameObject.SetActive(false);
		}
		else if (this.isOnCooldown && this.HasRockets)
		{
			base.gameObject.SetActive(true);
			this.buttonBg.rectTransform.DOSizeDelta(new Vector2(140f, 100f), 0.4f, false).SetEase(Ease.InOutBack);
			this.buttonBg.rectTransform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.4f, 5, 0.5f);
			this.imageMain.color = this.cooldownColorMain;
			this.imageDrop.color = this.cooldownColorDrop;
			this.imageEdge.color = this.cooldownColoredge;
			this.shine.enabled = false;
			this.hoverTween.enabled = false;
			this.idleStarParticles.Stop();
			this.RunAfterDelay(1f, delegate()
			{
				this.leftRockets.DOScale(0f, 0.4f).SetEase(Ease.InBack);
				this.rightRockets.DOScale(0f, 0.4f).SetEase(Ease.InBack);
			});
			this.shadow.DOAnchorPosY(-5f, 0.3f, false).SetEase(Ease.InOutCirc);
		}
		else if (!this.HasRockets)
		{
			if (!this.isEventActive)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.SetActive(true);
				this.buttonBg.rectTransform.DOSizeDelta(new Vector2(190f, 112f), 0.4f, false).SetEase(Ease.InOutBack);
				this.shine.enabled = true;
				this.isOnCooldown = false;
				this.hoverTween.enabled = false;
				this.imageMain.color = this.cooldownColorMain;
				this.imageDrop.color = this.cooldownColorDrop;
				this.imageEdge.color = this.cooldownColoredge;
				this.idleStarParticles.Stop();
				this.mainLabel.SetText("Get More");
				this.leftRockets.DOScale(1f, 0.6f).SetEase(Ease.InOutBack);
				this.rightRockets.DOScale(0f, 0.6f).SetEase(Ease.InOutBack);
				this.buttonRocket.DOScale(0f, 0.2f).SetEase(Ease.InOutBack);
				this.mainLabel.color = new Color(0.2f, 0.62f, 0.87f, 1f);
				this.shadow.DOAnchorPosY(-20f, 0.3f, false).SetEase(Ease.InOutCirc);
				this.RunAfterDelay(2f, delegate()
				{
					foreach (UIButtonRocketBehaviour uibuttonRocketBehaviour in this.rockets)
					{
						uibuttonRocketBehaviour.ReturnToOrigin();
					}
				});
			}
		}
		else
		{
			base.gameObject.SetActive(true);
			this.shine.enabled = true;
			this.isOnCooldown = false;
			this.hoverTween.enabled = true;
			this.buttonBg.rectTransform.DOSizeDelta(new Vector2(150f, 112f), 0.4f, false).SetEase(Ease.InOutBack);
			this.mainLabel.color = new Color(1f, 1f, 1f, 1f);
			this.idleStarParticles.Play();
			this.imageMain.color = this.fireColorMain;
			this.imageDrop.color = this.fireColorDrop;
			this.imageEdge.color = this.fireColoredge;
			this.mainLabel.SetText(" ");
			this.leftRockets.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
			this.rightRockets.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
			this.buttonRocket.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
			this.buttonBg.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.4f, 5, 0.5f);
			this.shadow.DOAnchorPosY(-20f, 0.3f, false).SetEase(Ease.InOutCirc);
		}
	}

	public void SetButtonToCooldown()
	{
		this.isOnCooldown = true;
		this.cooldownTimer = 0f;
		this.UpdateUI();
		foreach (UIButtonRocketBehaviour uibuttonRocketBehaviour in this.rockets)
		{
			uibuttonRocketBehaviour.Shoot();
		}
	}

	public void ButtonPressed()
	{
		if (ConsumableManager.Instance.GetAmount(FireworkFishingManager.Instance.Firework) <= 0)
		{
			this.moreFireworkDialog.gameObject.SetActive(true);
		}
		else if (!this.isOnCooldown)
		{
			FireworkFishingManager.Instance.UseFirework();
			this.SetButtonToCooldown();
		}
	}

	private void Update()
	{
		if (this.isOnCooldown)
		{
			if (FHelper.HasSecondsPassed(6f, ref this.cooldownTimer, true))
			{
				this.isOnCooldown = false;
				foreach (UIButtonRocketBehaviour uibuttonRocketBehaviour in this.rockets)
				{
					uibuttonRocketBehaviour.ReturnToOrigin();
				}
				this.UpdateUI();
			}
			else
			{
				int num = Mathf.CeilToInt(6f - this.cooldownTimer);
				this.mainLabel.SetText(num.ToString() + "s");
			}
		}
	}

	private void TweenKiller()
	{
		this.buttonBg.rectTransform.DOKill(true);
		this.leftRockets.DOKill(true);
		this.rightRockets.DOKill(true);
		this.buttonRocket.DOKill(true);
		this.shadow.DOKill(true);
		this.buttonBg.transform.DOKill(true);
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	[SerializeField]
	private Image buttonBg;

	[SerializeField]
	private TextMeshProUGUI badgeAmountLbl;

	[SerializeField]
	private TextMeshProUGUI mainLabel;

	[SerializeField]
	private RectTransform leftRockets;

	[SerializeField]
	private RectTransform rightRockets;

	[SerializeField]
	private RectTransform buttonRocket;

	[SerializeField]
	private RectTransform shadow;

	[SerializeField]
	private UIButtonRocketBehaviour[] rockets;

	[SerializeField]
	private ParticleSystem idleStarParticles;

	[SerializeField]
	private RecurringButtonShine shine;

	[SerializeField]
	private HoveringTween hoverTween;

	[SerializeField]
	private Image imageMain;

	[SerializeField]
	private Image imageDrop;

	[SerializeField]
	private Image imageEdge;

	[SerializeField]
	private Color fireColorMain;

	[SerializeField]
	private Color fireColorDrop;

	[SerializeField]
	private Color fireColoredge;

	[SerializeField]
	private Color cooldownColorMain;

	[SerializeField]
	private Color cooldownColorDrop;

	[SerializeField]
	private Color cooldownColoredge;

	[SerializeField]
	private MoreFireworkDialogTween moreFireworkDialog;

	private bool isEventActive;

	private bool isOnCooldown;

	private EventContent eventContent;

	private BaseConsumable fireworkConsumable;

	private int lastKnownFireworkAmount;

	private float cooldownTimer;

	private const int FIREWORK_COOLDOWN = 6;
}
