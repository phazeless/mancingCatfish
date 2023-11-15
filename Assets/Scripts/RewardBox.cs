using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardBox : MonoBehaviour
{
	public void SetContent(Sprite iconSprite, Color bgColor, int amountCount = 1, float scaleMultiplier = 1f)
	{
		this.rectTransform.sizeDelta = new Vector2(this.rectTransform.sizeDelta.x * scaleMultiplier, this.rectTransform.sizeDelta.y * scaleMultiplier);
		base.transform.localScale *= scaleMultiplier;
		this.bgImage.color = bgColor;
		if (iconSprite != null)
		{
			this.icon.sprite = iconSprite;
		}
		this.count.SetVariableText(new string[]
		{
			amountCount.ToString()
		});
	}

	public void SetContentAsGems(int amountCount = 1, float scaleMultiplier = 1f)
	{
		this.rectTransform.sizeDelta = new Vector2(this.rectTransform.sizeDelta.x * scaleMultiplier, this.rectTransform.sizeDelta.y * scaleMultiplier);
		base.transform.localScale *= scaleMultiplier;
		this.bgImage.color = HookedColors.Purple;
		if (this.gemsSprite != null)
		{
			this.icon.sprite = this.gemsSprite;
		}
		this.count.SetVariableText(new string[]
		{
			amountCount.ToString()
		});
	}

	[SerializeField]
	private Image bgImage;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private TextMeshProUGUI count;

	[SerializeField]
	private Transform countBg;

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private Sprite gemsSprite;
}
