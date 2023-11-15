using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChristmasGiftDayBehaviour : MonoBehaviour
{
	public void SetDay(int dayOfItem, int currentDay, int stars)
	{
		this.dayLabel.SetText(dayOfItem.ToString());
		if (dayOfItem < currentDay)
		{
			this.bg.color = this.pastColor;
		}
		else if (dayOfItem > currentDay)
		{
			this.bg.color = this.futureColor;
		}
		else
		{
			base.transform.DOKill(false);
			base.transform.localScale = Vector3.one;
			base.transform.DOScale(1.15f, 1f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
			this.button.enabled = true;
		}
		for (int i = 0; i < stars; i++)
		{
			this.stars[i].SetActive(true);
		}
	}

	public void FadeOut()
	{
		this.bg.DOFade(0f, 0.2f).SetDelay(0.2f);
	}

	public void Expand()
	{
		if (this.isOpened)
		{
			return;
		}
		this.isOpened = true;
		LayoutElement layoutElement = base.gameObject.AddComponent(typeof(LayoutElement)) as LayoutElement;
		layoutElement.ignoreLayout = true;
		base.transform.DOKill(false);
		base.transform.SetParent(base.transform.parent.parent.parent, true);
		this.bg.DOColor(this.openColor, 0.2f);
		base.transform.DOScale(30f, 0.4f);
		this.dayLabel.DOFade(0f, 0.5f);
		foreach (GameObject gameObject in this.stars)
		{
			gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		base.transform.DOKill(false);
		this.bg.DOKill(false);
		this.dayLabel.DOKill(false);
	}

	[SerializeField]
	private TextMeshProUGUI dayLabel;

	[SerializeField]
	private Image bg;

	[SerializeField]
	private Color pastColor;

	[SerializeField]
	private Color futureColor;

	[SerializeField]
	private Color openColor;

	[SerializeField]
	private Button button;

	[SerializeField]
	private GameObject[] stars;

	private bool isOpened;
}
