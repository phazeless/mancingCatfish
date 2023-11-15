using System;
using System.Diagnostics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkilltreeSkill : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<Skill, Transform> OnPeekSkill;

	private Color skillColor
	{
		get
		{
			if (this.currentSkill != null)
			{
				return this.currentSkill.GetExtraInfo().IconBgColor;
			}
			return Color.white;
		}
	}

	private void Setup(bool isAlteredVisuals = false)
	{
		if (this.skillLineReference != null)
		{
			this.lineInstance = UnityEngine.Object.Instantiate<Transform>(this.skillLinePrefab, base.transform);
			this.lineInstance.SetParent(this.lineInstance.parent.parent);
			this.lineInstance.SetAsFirstSibling();
			this.lineInstance.right = this.skillLineReference.localPosition - this.lineInstance.localPosition - new Vector3(0f, 0.25f, 0f);
			this.lineInstance.GetComponent<RectTransform>().sizeDelta = new Vector2(10f + Mathf.Sqrt(Mathf.Pow((this.lineInstance.localPosition - this.skillLineReference.localPosition).x, 2f) + Mathf.Pow((this.lineInstance.localPosition - this.skillLineReference.localPosition).y, 2f)), 10f);
		}
		this.UpdateUI();
		if (isAlteredVisuals)
		{
			this.SetAlteredVisuals();
		}
		this.levelLabelHolderTransform.GetComponent<Image>().color = this.skillColor + new Color(0.1f, 0.1f, 0.1f, 0f);
		this.costLabel.SetVariableText(new string[]
		{
			this.currentSkill.GetExtraInfo().InitialUpgradeCost.stringValue
		});
		this.currentSkill.OnSkillAvailabilityChanged += this.CurrentSkill_OnSkillAvailabilityChanged;
		this.currentSkill.OnSkillLevelUp += this.CurrentSkill_OnSkillLevelUp;
	}

	private void CurrentSkill_OnSkillLevelUp(Skill arg1, LevelChange levelChange)
	{
		this.levelUpParticle.Play();
		this.UpdateUI();
		if (levelChange == LevelChange.LevelUp || levelChange == LevelChange.LevelUpFree)
		{
			this.AddLevelToLevelLabel(false);
		}
	}

	private void CurrentSkill_OnSkillAvailabilityChanged(Skill arg1, bool arg2)
	{
		this.UpdateUI();
	}

	public void Ini(Transform sRef, Skill skill, bool isAlteredVisuals = false)
	{
		this.skillLineReference = sRef;
		this.currentSkill = skill;
		this.icon.sprite = skill.GetExtraInfo().Icon;
		this.Setup(isAlteredVisuals);
	}

	private void SetAlteredVisuals()
	{
		this.mask2D.enabled = false;
		this.mask.enabled = true;
		this.shadowImagePart.sprite = this.alteredVisualsSprite;
		this.bottomImagePart.sprite = this.alteredVisualsSprite;
		this.borderImagePart.sprite = this.alteredVisualsSprite;
		this.centerImagePart.sprite = this.alteredVisualsSprite;
		this.shadowImagePart.rectTransform.sizeDelta = new Vector2(this.shadowImagePart.rectTransform.sizeDelta.x * 1.2f, this.shadowImagePart.rectTransform.sizeDelta.y);
		this.bottomImagePart.rectTransform.sizeDelta = new Vector2(this.bottomImagePart.rectTransform.sizeDelta.x * 1.2f, this.bottomImagePart.rectTransform.sizeDelta.y);
		this.borderImagePart.rectTransform.sizeDelta = new Vector2(this.borderImagePart.rectTransform.sizeDelta.x * 1.2f, this.borderImagePart.rectTransform.sizeDelta.y);
		this.centerImagePart.rectTransform.sizeDelta = new Vector2(this.centerImagePart.rectTransform.sizeDelta.x * 1.13f, this.centerImagePart.rectTransform.sizeDelta.y);
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.centerImagePart.gameObject, this.borderImagePart.transform);
		gameObject.transform.SetAsFirstSibling();
		gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.23f);
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x * 1.11f, gameObject.GetComponent<RectTransform>().sizeDelta.y);
	}

	private void SetLevelColor()
	{
		if (this.lineInstance != null && this.currentSkill.CurrentLevel <= 1)
		{
			this.lineInstance.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			this.lineInstance.localScale = new Vector3(1f, 0f, 1f);
			this.lineInstance.DOScale(1.2f, 0.4f).SetEase(Ease.OutBack);
		}
		this.costLabel.color = Color.white;
		this.centerImagePart.color = this.skillColor;
		this.bottomImagePart.color = this.skillColor - new Color(0.1f, 0.1f, 0.1f, 0f);
		this.borderImagePart.color = this.skillColor + new Color(0.1f, 0.1f, 0.1f, 0f);
		this.icon.color = this.skillColor + new Color(0.1f, 0.1f, 0.1f, 0f);
		this.costLabelHolderImage.color = this.skillColor + new Color(0.1f, 0.1f, 0.1f, 0f);
		this.unlockLabel.SetActive(false);
		this.topHolder.DOAnchorPosY(8f, 0.3f, false).SetEase(Ease.OutBack);
		base.transform.localScale = Vector3.one * 1.1f;
		base.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 10, 1f);
		this.AddLevelToLevelLabel(true);
	}

	private void SetUpgradableColor()
	{
		this.centerImagePart.color = (this.skillColor + new Color(0.1f, 0.1f, 0.1f, 0f)) * 0.7f + new Color(0f, 0f, 0f, 0.3f);
		this.bottomImagePart.color = (this.skillColor - new Color(0.1f, 0.1f, 0.1f, 0f)) * 0.7f + new Color(0f, 0f, 0f, 0.3f);
		this.borderImagePart.color = this.skillColor * 0.7f + new Color(0f, 0f, 0f, 0.3f);
		this.icon.color = this.borderImagePart.color;
		this.costLabelHolderImage.color = this.borderImagePart.color;
		this.unlockLabel.GetComponent<Image>().color = this.borderImagePart.color;
		this.costLabel.color = Color.white;
		this.unlockLabel.SetActive(true);
		this.topHolder.DOAnchorPosY(8f, 0.3f, false).SetEase(Ease.OutBack);
		base.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 10, 1f);
		this.levelLabelHolderTransform.gameObject.SetActive(false);
		if (this.lineInstance != null && this.currentSkill.CurrentLevel <= 1)
		{
			this.lineInstance.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.3f);
			this.lineInstance.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void SetUnavailableColor()
	{
		this.centerImagePart.color = this.skillColor * 0.2f + new Color(0.2f, 0.2f, 0.2f, 0.8f);
		this.bottomImagePart.color = (this.skillColor - new Color(0.1f, 0.1f, 0.1f, 0f)) * 0.2f + new Color(0.2f, 0.2f, 0.2f, 0.8f);
		this.borderImagePart.color = (this.skillColor + new Color(0.1f, 0.1f, 0.1f, 0f)) * 0.2f + new Color(0.25f, 0.25f, 0.25f, 0.8f);
		this.icon.color = this.borderImagePart.color;
		this.costLabelHolderImage.color = this.borderImagePart.color;
		this.topHolder.anchoredPosition = Vector2.zero;
		this.unlockLabel.SetActive(false);
		this.costLabel.color = new Color(1f, 1f, 1f, 0.5f);
		this.levelLabelHolderTransform.gameObject.SetActive(false);
		if (this.lineInstance != null && this.currentSkill.CurrentLevel <= 1)
		{
			this.lineInstance.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.1f);
			this.lineInstance.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void AddLevelToLevelLabel(bool isSetup = false)
	{
		this.levelLabelHolderTransform.gameObject.SetActive(true);
		if (!isSetup)
		{
			this.levelLabelHolderTransform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 5, 0.8f);
		}
		if (this.currentSkill.IsMaxLevel)
		{
			this.levelLabel.SetText("<b>MAX</b>");
		}
		else
		{
			this.levelLabel.SetVariableText(new string[]
			{
				this.currentSkill.CurrentLevel.ToString(),
				this.currentSkill.MaxLevel.ToString()
			});
		}
	}

	public void PeekSkill()
	{
		if (SkilltreeSkill.OnPeekSkill != null)
		{
			SkilltreeSkill.OnPeekSkill(this.currentSkill, base.transform);
		}
	}

	private void UpdateUI()
	{
		this.TweenKiller();
		if (this.currentSkill.CurrentLevel > 0)
		{
			this.SetLevelColor();
		}
		else if (this.currentSkill.IsAvailableForLevelUp && this.currentSkill.CurrentLevel == 0)
		{
			this.SetUpgradableColor();
		}
		else
		{
			this.SetUnavailableColor();
		}
	}

	private void TweenKiller()
	{
		this.levelLabelHolderTransform.DOKill(true);
		this.topHolder.DOKill(true);
		base.transform.DOKill(true);
	}

	private void OnDestroy()
	{
		this.TweenKiller();
		this.currentSkill.OnSkillAvailabilityChanged -= this.CurrentSkill_OnSkillAvailabilityChanged;
	}

	[SerializeField]
	[Header("References")]
	private Image shadowImagePart;

	[SerializeField]
	private Image bottomImagePart;

	[SerializeField]
	private Image borderImagePart;

	[SerializeField]
	private Image centerImagePart;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private GameObject shine;

	[SerializeField]
	private RectTransform topHolder;

	[SerializeField]
	private ParticleSystem levelUpParticle;

	[SerializeField]
	private Transform levelLabelHolderTransform;

	[SerializeField]
	private Image costLabelHolderImage;

	[SerializeField]
	private TextMeshProUGUI levelLabel;

	[SerializeField]
	private TextMeshProUGUI costLabel;

	[SerializeField]
	private Transform skillLinePrefab;

	[SerializeField]
	private GameObject unlockLabel;

	[SerializeField]
	private RectMask2D mask2D;

	[SerializeField]
	private Mask mask;

	[SerializeField]
	private Sprite alteredVisualsSprite;

	private Transform skillLineReference;

	private Transform lineInstance;

	private Skill currentSkill;
}
