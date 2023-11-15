using System;
using System.Numerics;
using ACE.IAPS;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RushTimeHandler : MonoBehaviour
{
	private void Awake()
	{
		this.gemCost.SetVariableText(new string[]
		{
			5.ToString()
		});
		this.rushTimeSkill.OnSkillActivation += this.RushTimeSkill_OnSkillActivation;
		this.rushTimeSkill.OnSkillCooldownZero += this.RushTimeSkill_OnSkillCooldownZero;
		StoreManager storeManager = ResourceManager.StoreManager;
		storeManager.OnGoodBalanceChanged = (Action<string, int, int>)Delegate.Combine(storeManager.OnGoodBalanceChanged, new Action<string, int, int>(this.Instance_OnGoodBalanceChanged));
		SkillManager.Instance.OnSkillAttributeValueChanged += this.Instance_OnSkillAttributeValueChanged;
	}

	private void Instance_OnSkillAttributeValueChanged(ISkillAttribute attr, float val)
	{
		if (attr is Skills.RushTimeBonus)
		{
			this.UpdateUI();
		}
	}

	private void Start()
	{
		this.rushTimeSkill.ResetCooldown();
		this.UpdateUI();
	}

	private void UpdateUI()
	{
		float num = this.GetRushTimeDuration() / 60f;
		this.rushTimeDurationLabel.SetVariableText(new string[]
		{
			num.ToString()
		});
	}

	private void Instance_OnGoodBalanceChanged(string itemId, int balance, int amountAdded)
	{
		if ("se.ace.boost_time_0" == itemId && amountAdded == 1)
		{
			this.rushTimeSkill.Activate();
		}
	}

	private void RushTimeSkill_OnSkillCooldownZero(Skill skill)
	{
		this.buyBlocker.SetActive(false);
	}

	private void RushTimeSkill_OnSkillActivation(Skill skill)
	{
		ResourceManager.StoreManager.TakeItem("se.ace.boost_time_0", 1);
		this.buyBlocker.SetActive(true);
		this.RushTimeEffect();
	}

	private void RushTimeEffect()
	{
		this.customImageEffect.enabled = true;
		this.customImageEffect.AnimateMagnitude();
		this.rushtimeAudio.Play();
		this.RunAfterDelay(0.5f, delegate()
		{
			float currentTotalValueFor = SkillManager.Instance.GetCurrentTotalValueFor<Skills.AfkCollectedValueIncrease>();
			int value = (int)(this.GetRushTimeDuration() * currentTotalValueFor);
			BigInteger bigInteger = AFKManager.Instance.CurrentGPM * value;
			string text = CashFormatter.SimpleToCashRepresentation(bigInteger, 3, false, true);
			ResourceManager.Instance.GiveResource(ResourceType.Cash, bigInteger);
			TextMeshProUGUI labelInstance = UnityEngine.Object.Instantiate<TextMeshProUGUI>(this.cashGainedLabel, this.mainHeader, false);
			RectTransform labelRect = labelInstance.GetComponent<RectTransform>();
			labelRect.anchoredPosition = new UnityEngine.Vector2(labelRect.anchoredPosition.x, labelRect.anchoredPosition.y - 700f);
			labelRect.localScale = UnityEngine.Vector3.one * 2f;
			labelRect.DOAnchorPosY(labelRect.anchoredPosition.y + 200f, 0.5f, false).SetEase(Ease.OutCirc).OnComplete(delegate
			{
				labelInstance.DOFade(0f, 0.5f).SetEase(Ease.InCirc);
				labelRect.DOAnchorPosY(labelRect.anchoredPosition.y - 100f, 0.5f, false).SetEase(Ease.InCirc).OnComplete(delegate
				{
					labelInstance.DOKill(false);
					labelRect.DOKill(false);
					UnityEngine.Object.Destroy(labelInstance.gameObject);
				});
			});
			labelInstance.SetText(text);
			SkillManager.Instance.FastForward((float)this.rushTimeSkill.Duration);
		});
	}

	private float GetRushTimeDuration()
	{
		float num = SkillManager.Instance.GetCurrentTotalValueFor<Skills.RushTimeBonus>() * 60f;
		return (float)this.rushTimeSkill.Duration / 60f + num;
	}

	private void Update()
	{
		if (this.timeLeft.gameObject.activeInHierarchy)
		{
			this.timeLeft.SetVariableText(new string[]
			{
				Mathf.CeilToInt(this.rushTimeSkill.GetTotalSecondsLeftOnCooldown()).ToString()
			});
		}
	}

	private void OnDestroy()
	{
		StoreManager storeManager = ResourceManager.StoreManager;
		storeManager.OnGoodBalanceChanged = (Action<string, int, int>)Delegate.Remove(storeManager.OnGoodBalanceChanged, new Action<string, int, int>(this.Instance_OnGoodBalanceChanged));
		SkillManager.Instance.OnSkillAttributeValueChanged -= this.Instance_OnSkillAttributeValueChanged;
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && this.rushTimeSkill.IsOnCooldown)
		{
			this.rushTimeSkill.ResetCooldown();
		}
	}

	[SerializeField]
	private Skill rushTimeSkill;

	[SerializeField]
	private GameObject buyBlocker;

	[SerializeField]
	private TextMeshProUGUI timeLeft;

	[SerializeField]
	private Animator screenAnimator;

	[SerializeField]
	private TextMeshProUGUI gemCost;

	[SerializeField]
	private CustomImageEffect customImageEffect;

	[SerializeField]
	private TextMeshProUGUI cashGainedLabel;

	[SerializeField]
	private RectTransform mainHeader;

	[SerializeField]
	private AudioSource rushtimeAudio;

	[SerializeField]
	private TextMeshProUGUI rushTimeDurationLabel;
}
