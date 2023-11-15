using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class IGNCrabDialog : InGameNotificationDialog<IGNCrab>
{
	protected override void OnAboutToOpen()
	{
		this.cashAsString = CashFormatter.SimpleToCashRepresentation(this.inGameNotification.CashAmount, 3, false, true);
		this.cashAmount.SetText(this.cashAsString);
		for (int i = 0; i < this.catchType.Length; i++)
		{
			this.catchType[i].SetActive(false);
		}
		IGNCrab.CrabCageContent content = this.inGameNotification.Content;
		this.catchType[(int)content].SetActive(true);
		switch (content)
		{
		case IGNCrab.CrabCageContent.None:
			this.title.SetText("Empty...");
			break;
		case IGNCrab.CrabCageContent.Small:
			this.title.SetText("Small Catch!");
			break;
		case IGNCrab.CrabCageContent.Great:
			this.title.SetText("Great Catch!!");
			break;
		case IGNCrab.CrabCageContent.Huge:
			this.title.SetText("Huge Success!!!");
			break;
		}
		this.customSize = new Vector2?(new Vector2(889f, 925f));
	}

	protected override void OnAboutToReturn()
	{
	}

	protected override void OnIGNHasBeenSet()
	{
	}

	protected override void OnOpened()
	{
	}

	protected override void OnRemovedFromList()
	{
		this.iconTween.IconTweenKiller();
		ResourceManager.Instance.GiveResource(ResourceType.Cash, this.inGameNotification.CashAmount);
		if (this.inGameNotification.Content != IGNCrab.CrabCageContent.None)
		{
			this.SpawnMoneyEffect();
		}
	}

	protected override void OnReturned()
	{
	}

	private void SpawnMoneyEffect()
	{
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate<ParticleSystem>(this.cashEffect);
		particleSystem.transform.position = Vector3.zero;
		particleSystem.transform.localScale = Vector3.one * CameraMovement.Instance.Zoom;
		TextMeshProUGUI labelInstance = TextObjectPool.Instance.TextMeshProPoolUGUI.GetObject();
		labelInstance.transform.SetParent(base.transform.parent, false);
		labelInstance.transform.position = this.cashAmount.transform.position;
		labelInstance.fontSize = 95f;
		labelInstance.gameObject.SetActive(true);
		labelInstance.alignment = TextAlignmentOptions.Center;
		labelInstance.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 800f);
		labelInstance.transform.localScale = Vector3.one;
		labelInstance.color = Color.white;
		labelInstance.DOFade(0f, 1f).SetEase(Ease.InCubic);
		labelInstance.transform.DOScale(new Vector3(0.4f, 0.4f, 0.4f), 1f).SetEase(Ease.InCubic);
		labelInstance.transform.DOMoveY(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.85f, 0f)).y, 1f, false).SetEase(Ease.Linear).OnComplete(delegate
		{
			labelInstance.DOKill(false);
			labelInstance.transform.DOKill(false);
			labelInstance.gameObject.SetActive(false);
			TextObjectPool.Instance.TextMeshProPoolUGUI.ReturnObject(labelInstance);
		});
		AudioManager.Instance.Cacthing();
		labelInstance.SetText("<b>" + this.cashAsString + "</b>");
	}

	[SerializeField]
	private IGNPackageTween iconTween;

	[SerializeField]
	private TextMeshProUGUI cashAmount;

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private GameObject[] catchType;

	[SerializeField]
	private ParticleSystem cashEffect;

	private string cashAsString = string.Empty;
}
