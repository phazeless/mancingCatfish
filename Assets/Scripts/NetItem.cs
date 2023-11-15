using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetItem : MonoBehaviour
{
	public void SetNet(int amount)
	{
		this.collectedAmount = amount;
		if (amount >= 100)
		{
			this.label.SetText("Full");
		}
		else
		{
			this.label.SetText(amount + "%");
		}
		if (amount > 0)
		{
			this.label.color = this.activeNetColor;
			this.backGround.color = this.activeBgColor;
			this.netImage.color = this.activeNetColor;
		}
	}

	public void Refresh(int delay)
	{
		this.backGround.DOColor(this.unactiveBgColor, 0.4f).SetDelay((float)delay * 0.2f);
		this.netImage.DOColor(this.unactiveNetColor, 0.4f).SetDelay((float)delay * 0.2f).OnUpdate(delegate
		{
			this.collectedAmount = Mathf.Clamp(this.collectedAmount - 5, 0, 100);
			this.label.SetText(this.collectedAmount + "%");
		}).OnComplete(delegate
		{
			this.label.color = this.unactiveNetColor;
			this.label.SetText("0%");
		});
	}

	[SerializeField]
	private Image backGround;

	[SerializeField]
	private Image netImage;

	[SerializeField]
	private TextMeshProUGUI label;

	private Color activeBgColor = new Color(0.851f, 0.957f, 0.953f, 1f);

	private Color activeNetColor = new Color(0.259f, 0.8f, 0.788f, 1f);

	private Color unactiveBgColor = new Color(0.933f, 0.933f, 0.933f);

	private Color unactiveNetColor = new Color(0.831f, 0.831f, 0.831f);

	private int collectedAmount;

	private int currentAmount;
}
