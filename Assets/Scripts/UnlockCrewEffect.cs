using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockCrewEffect : UpgradeDialogTween
{
	private void SetUpgradeItem(Skill skill)
	{
		this.icon.sprite = skill.GetExtraInfo().Icon;
		this.title.text = skill.GetExtraInfo().TitleText;
	}

	private void SetupAttributeValues(Skill updatedContent)
	{
		foreach (TextMeshProUGUI textMeshProUGUI in this.valueAndAttribute)
		{
			textMeshProUGUI.transform.parent.gameObject.SetActive(false);
		}
		List<string> list = new List<string>();
		for (int i = 0; i < updatedContent.SkillBehaviours.Count; i++)
		{
			SkillBehaviour skillBehaviour = updatedContent.SkillBehaviours[i];
			this.valueAndAttribute[i].transform.parent.gameObject.SetActive(true);
			float valueAtLevel = skillBehaviour.GetValueAtLevel(updatedContent.NextLevel);
			string text = (valueAtLevel <= 0f) ? string.Empty : "+";
			float totalValueAtLevel = skillBehaviour.GetTotalValueAtLevel(updatedContent.CurrentLevel);
			string text2 = (totalValueAtLevel <= 0f) ? string.Empty : "+";
			string text3 = FHelper.FindBracketAndReplace(skillBehaviour.Description, new string[]
			{
				string.Concat(new object[]
				{
					"<b>",
					text,
					valueAtLevel,
					skillBehaviour.PostFixCharacter,
					"</b>"
				})
			});
			this.valueAndAttribute[i].SetVariableText(new string[]
			{
				string.Empty,
				text3,
				string.Concat(new object[]
				{
					" (",
					text2,
					skillBehaviour.GetTotalValueAtLevel(updatedContent.NextLevel),
					skillBehaviour.PostFixCharacter,
					")"
				})
			});
			Canvas.ForceUpdateCanvases();
		}
		Canvas.ForceUpdateCanvases();
	}

	public void UnlockEffect(Skill skill, Action onUnlockEffectFinish = null)
	{
		this.onUnlockEffectFinish = onUnlockEffectFinish;
		this.isPlaying = true;
		DialogInteractionHandler.Instance.NewActiveDialog(base.transform);
		this.SetUpgradeItem(skill);
		this.SetupAttributeValues(skill);
		this.rectTransformCrew.DOAnchorPosY(100f, 0.5f, false).OnComplete(delegate
		{
			this.rectTransformCrew.DOAnchorPosY(this.rectTransformCrew.anchoredPosition.y + 10f, 2f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
		});
		for (int i = 0; i < this.TitleAndAttributes.Length; i++)
		{
			this.TitleAndAttributes[i].localScale = Vector2.zero;
		}
		this.InfoboxHolderTargetSize = this.InfoboxHolder.sizeDelta;
		this.BannerHolderTargetSize = this.BannerHolder.sizeDelta;
		this.InfoboxHolder.sizeDelta = new Vector2(0f, 0f);
		this.BannerHolder.sizeDelta = new Vector2(0f, 0f);
		this.BannerHolder.localScale = Vector2.zero;
		this.BannerHolder.DOScale(1f, 0.2f).SetDelay(0.8f);
		Vector2 anchoredPosition = this.rectTransformCrew.anchoredPosition;
		this.PortraitEtcHolder.anchoredPosition = new Vector2(this.rectTransformCrew.anchoredPosition.x, 1000f);
		this.lockAnimator.enabled = true;
		this.lockAnimator.SetTrigger("unlock");
		this.PortraitEtcHolder.DOAnchorPosY(anchoredPosition.y, 1.15f, false).SetEase(Ease.OutBack).OnComplete(delegate
		{
			this.lockAnimator.enabled = false;
			this.lockAnimator.gameObject.SetActive(false);
			this.PortraitEtcHolder.DOAnchorPosY(this.PortraitEtcHolder.anchoredPosition.y + 10f, 2f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
		});
		this.PortraitEtcHolder.DORotate(new Vector3(0f, 0f, 360f), 0.9f, RotateMode.FastBeyond360).OnComplete(delegate
		{
			this.purpleCoverOverPortrait.DOScale(3f, 0.5f);
			this.purpleCoverOverPortrait.GetComponent<Image>().DOFade(0f, 0.5f);
			this.PortraitEtcHolder.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f, 5, 0.5f);
			this.InfoboxHolder.DOSizeDelta(this.InfoboxHolderTargetSize, 0.5f, false).SetDelay(0.7f).SetEase(Ease.OutBack);
			this.BannerHolder.DOSizeDelta(this.BannerHolderTargetSize, 0.5f, false).SetEase(Ease.OutBack).OnComplete(delegate
			{
				for (int j = 0; j < this.TitleAndAttributes.Length; j++)
				{
					this.TitleAndAttributes[j].DOScale(1f, 0.3f).SetDelay(0.2f + 0.2f * (float)j).SetEase(Ease.OutBack).OnComplete(delegate
					{
					});
					this.isPlaying = false;
				}
			});
		});
	}

	public override void Close(bool destroyOnFinish)
	{
		this.CloseUnlockEffect();
	}

	public void CloseUnlockEffect()
	{
		if (this.isPlaying)
		{
			this.TweenKiller(true);
		}
		else
		{
			this.TweenKiller(false);
			DialogInteractionHandler.Instance.DialogClosed(base.transform);
			this.InfoboxHolder.DOScale(0f, 0.2f).SetEase(Ease.InBack);
			this.PortraitEtcHolder.DOScale(0f, 0.2f).SetEase(Ease.InBack).SetDelay(0.1f).OnComplete(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			this.BannerHolder.DOScale(0f, 0.2f).SetEase(Ease.InBack).SetDelay(0.05f);
		}
		if (this.onUnlockEffectFinish != null)
		{
			this.onUnlockEffectFinish();
			this.onUnlockEffectFinish = null;
		}
	}

	private void TweenKiller(bool isFinish)
	{
		if (this.rectTransformCrew != null)
		{
			this.rectTransformCrew.DOKill(isFinish);
		}
		if (this.TitleAndAttributes != null)
		{
			for (int i = 0; i < this.TitleAndAttributes.Length; i++)
			{
				this.TitleAndAttributes[i].DOKill(isFinish);
			}
		}
		if (this.BannerHolder != null)
		{
			this.BannerHolder.DOKill(isFinish);
		}
		if (this.PortraitEtcHolder != null)
		{
			this.PortraitEtcHolder.DOKill(isFinish);
		}
		if (this.purpleCoverOverPortrait != null)
		{
			this.purpleCoverOverPortrait.DOKill(isFinish);
		}
		if (this.InfoboxHolder != null)
		{
			this.InfoboxHolder.DOKill(isFinish);
		}
		if (this.purpleCoverOverPortrait != null && this.purpleCoverOverPortrait.GetComponent<Image>() != null)
		{
			this.purpleCoverOverPortrait.GetComponent<Image>().DOKill(isFinish);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.TweenKiller(false);
	}

	[SerializeField]
	private Image icon;

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private List<TextMeshProUGUI> valueAndAttribute = new List<TextMeshProUGUI>();

	[SerializeField]
	private Transform bgShine1;

	[SerializeField]
	private Transform bgShine2;

	[SerializeField]
	private Animator lockAnimator;

	[SerializeField]
	private RectTransform rectTransformCrew;

	[SerializeField]
	private Transform purpleCoverOverPortrait;

	[SerializeField]
	private RectTransform PortraitEtcHolder;

	[SerializeField]
	private RectTransform BannerHolder;

	[SerializeField]
	private RectTransform InfoboxHolder;

	[SerializeField]
	private Transform[] TitleAndAttributes;

	private Vector3 InfoboxHolderTargetSize;

	private Vector3 BannerHolderTargetSize;

	private bool isPlaying;

	private Action onUnlockEffectFinish;
}
