using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EasterEggOpening : MonoBehaviour
{
	public void Init(BaseEasterEgg egg, Action<BaseEasterEgg> onAboutToClose)
	{
		this.egg = egg;
		this.onAboutToClose = onAboutToClose;
		this.rewarHolder.localScale = Vector3.zero;
		this.startingShadeColor = this.shade.color;
		for (int i = 0; i < EasterManager.Instance.MaxEggsCount; i++)
		{
			List<int> eggsFoundIndexes = EasterManager.Instance.GetEggsFoundIndexes();
			easterEggBox easterEggBox = UnityEngine.Object.Instantiate<easterEggBox>(this.easterEggBoxPrefab, this.boxHolder);
			if (eggsFoundIndexes.Contains(i))
			{
				easterEggBox.SetSprite(EasterManager.Instance.GetEggPrefab(i).EggSprite);
			}
			easterEggBox.transform.localScale = Vector3.zero;
		}
		Transform transform = UnityEngine.Object.Instantiate<Transform>(egg.BigEgg, this.bigEggHolder);
		transform.name = "eggHolder";
		this.title.transform.localScale = Vector3.zero;
		this.instructions.transform.localScale = Vector3.zero;
		this.info.transform.localScale = Vector3.zero;
		this.StartEggOpeningDialog();
	}

	public void StartEggOpeningDialog()
	{
		this.shade.color = new Color(this.startingShadeColor.r, this.startingShadeColor.g, this.startingShadeColor.b, 0f);
		AudioManager.Instance.OneShooter(this.startSound, 1f);
		this.shade.DOFade(0.95f, 0.2f).OnComplete(delegate
		{
			this.RunAfterDelay(0.1f, delegate()
			{
				this.title.transform.localScale = Vector3.one;
				this.titleModifier.enabled = true;
				AudioManager.Instance.OneShooter(this.woshSound, 1f);
				this.eggAnimation.Play();
				this.eggAnimation.PlayQueued("easterEggIdle", QueueMode.CompleteOthers);
				this.RunAfterDelay(0.5f, delegate()
				{
					this.instructions.transform.localScale = Vector3.one;
					this.instructionsModifier.enabled = true;
					this.RunAfterDelay(0.5f, delegate()
					{
						this.info.transform.localScale = Vector3.one;
						this.infoModifier.enabled = true;
						this.hatchButton.interactable = true;
						this.tapToHatch.DOFade(0.5f, 1f);
					});
				});
			});
		});
		this.RunAfterDelay(1.5f, delegate()
		{
			for (int i = 0; i < this.boxHolder.childCount; i++)
			{
				this.boxHolder.GetChild(i).localScale = Vector3.zero;
				this.boxHolder.GetChild(i).DOScale(1f, 0.4f).SetDelay((float)i * 0.15f).SetEase(Ease.OutBack);
				this.boxHolder.GetChild(i).localEulerAngles = new Vector3(0f, 0f, 25f);
				this.boxHolder.GetChild(i).DORotate(Vector3.zero, 0.4f, RotateMode.Fast).SetDelay((float)i * 0.15f + 0.1f).SetEase(Ease.OutBack);
			}
		});
	}

	public void HatchEgg()
	{
		if (!this.eggHatched)
		{
			this.eggHatched = true;
			this.AnimateReward();
			AudioManager.Instance.OneShooter(this.eggCrackSound, 1f);
			this.eggAnimation.PlayQueued("eggHolderOpen", QueueMode.PlayNow);
			this.tapToHatch.DOFade(0f, 0.3f).OnComplete(delegate
			{
				AudioManager.Instance.OneShooter(this.eggRewardSound, 1f);
				this.tapToHatch.DOFade(0.5f, 0.3f);
				this.tapToHatch.SetText("Get back tomorrow for another Egg");
				this.readyToClose = true;
			});
		}
		else if (this.readyToClose)
		{
			this.CloseDialog();
		}
	}

	private void TweenKiller()
	{
		this.title.transform.DOKill(false);
		this.instructions.transform.DOKill(false);
		this.info.transform.DOKill(false);
		this.tapToHatch.transform.DOKill(false);
		this.title.DOKill(false);
		this.instructions.DOKill(false);
		this.info.DOKill(false);
		this.tapToHatch.DOKill(false);
		this.shade.DOKill(false);
		this.boxHolder.DOKill(false);
		this.bigEggHolder.DOKill(false);
		this.rewarHolder.DOKill(false);
		this.rewarHolder.GetComponent<RectTransform>().DOKill(false);
	}

	private void CloseDialog()
	{
		this.shade.DOFade(0f, 0.3f).OnComplete(delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		});
		this.bigEggHolder.DOScale(0f, 0.2f);
		this.boxHolder.DOScale(0f, 0.2f);
		this.rewarHolder.DOScale(0f, 0.2f).SetEase(Ease.InBack);
		this.title.transform.DOScale(0f, 0.2f);
		this.instructions.transform.DOScale(0f, 0.2f);
		this.info.transform.DOScale(0f, 0.2f);
		this.tapToHatch.transform.DOScale(0f, 0.2f);
		for (int i = 0; i < this.boxHolder.childCount; i++)
		{
			this.boxHolder.GetChild(i).DOKill(false);
		}
	}

	private void AnimateReward()
	{
		if (this.egg.Content.ContainsItems)
		{
			this.rewardContentIcon.sprite = this.egg.Content.Items[0].Icon;
			if (this.egg.Content.Items[0].Rarity == Rarity.Common)
			{
				this.rewardContentBg.color = HookedColors.ItemCommon;
			}
			else if (this.egg.Content.Items[0].Rarity == Rarity.Rare)
			{
				this.rewardContentBg.color = HookedColors.ItemRare;
			}
			else if (this.egg.Content.Items[0].Rarity == Rarity.Epic)
			{
				this.rewardContentBg.color = HookedColors.ItemEpic;
			}
			this.rewardContentAmount.SetVariableText(new string[]
			{
				this.egg.Content.Items.Count.ToString()
			});
		}
		if (this.egg.Content.ContainsGems)
		{
			this.rewardContentIcon.sprite = this.gemSprite;
			this.rewardContentBg.color = HookedColors.Purple;
			this.rewardContentAmount.SetVariableText(new string[]
			{
				this.egg.Content.GemAmount.ToString()
			});
		}
		this.rewarHolder.DOScale(1f, 0.6f).SetEase(Ease.OutBack);
		this.rewarHolder.DORotate(new Vector3(0f, 0f, 360f), 1.2f, RotateMode.LocalAxisAdd).SetEase(Ease.OutElastic);
		this.rewarHolder.GetComponent<RectTransform>().DOAnchorPosY(200f, 0.8f, false).SetEase(Ease.OutBack);
	}

	private void OnDestroy()
	{
		if (this.onAboutToClose != null)
		{
			this.onAboutToClose(this.egg);
		}
		this.TweenKiller();
	}

	[SerializeField]
	private Image shade;

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private TextMeshProUGUI instructions;

	[SerializeField]
	private TextMeshProUGUI info;

	[SerializeField]
	private TextMeshProUGUI tapToHatch;

	[SerializeField]
	private VertexAttributeModifier titleModifier;

	[SerializeField]
	private VertexAttributeModifier instructionsModifier;

	[SerializeField]
	private VertexAttributeModifier infoModifier;

	[SerializeField]
	private easterEggBox easterEggBoxPrefab;

	[SerializeField]
	private AudioClip eggCrackSound;

	[SerializeField]
	private AudioClip eggRewardSound;

	[SerializeField]
	private AudioClip startSound;

	[SerializeField]
	private AudioClip woshSound;

	[SerializeField]
	private Animation eggAnimation;

	[SerializeField]
	private Button hatchButton;

	[SerializeField]
	private Transform boxHolder;

	[SerializeField]
	private Transform bigEggHolder;

	[SerializeField]
	private Transform rewarHolder;

	[SerializeField]
	private Image rewardContentIcon;

	[SerializeField]
	private Image rewardContentBg;

	[SerializeField]
	private Sprite gemSprite;

	[SerializeField]
	private TextMeshProUGUI rewardContentAmount;

	private List<BaseEasterEgg> baseEasterEggList = new List<BaseEasterEgg>();

	private Action<BaseEasterEgg> onAboutToClose;

	private BaseEasterEgg egg;

	private Color startingShadeColor;

	private bool eggHatched;

	private bool readyToClose;
}
