using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFishBookItem : UIListItem<FishAttributes>
{
	public override void OnUpdateUI(FishAttributes fishInfo)
	{
		if (fishInfo == null)
		{
			return;
		}
		this.FishType = fishInfo.FishType;
		bool flag = CultureInfo.CurrentCulture.Name.Equals("en-US");
		this.fishIcon.sprite = FishPoolManager.Instance.GetFishIcon(fishInfo.FishType);
		this.fishName.SetText(fishInfo.Name);
		this.fishDescription.SetText(fishInfo.Description);
		this.biggestCatch.SetVariableText(new string[]
		{
			fishInfo.BiggestCatch.ToString(),
			(!flag) ? "kg" : "lb"
		});
		this.baseValue.SetVariableText(new string[]
		{
			CashFormatter.SimpleToCashRepresentation(fishInfo.BaseValue.Value, 1, false, false)
		});
		this.stars.SetText(fishInfo.Stars.ToString());
		if (this.isTierFish)
		{
			this.fishIcon.GetComponent<RectTransform>().anchoredPosition = Vector2.right * 16f;
		}
		if (fishInfo.IsCaught)
		{
			this.SetCaughtUI();
		}
	}

	public void SetColor(Color fishBg, Color itemBg, Color text, bool isTier)
	{
		this.caughtFishbgColor = fishBg;
		this.caughtItemBgColor = itemBg;
		this.caughtTextColor = text;
		this.isTierFish = isTier;
	}

	public void SetCaughtUI()
	{
		this.fishIconBg.color = this.caughtFishbgColor;
		this.fishItemBg.color = this.caughtItemBgColor;
		this.fakeMask.color = this.caughtItemBgColor;
		this.fishName.color = this.caughtTextColor;
		this.fishDescription.color = this.caughtTextColor;
		this.starImage.color = this.caughtStarColor;
		this.fishIcon.color = Color.white;
		this.stars.color = Color.white;
		this.baseValue.color = Color.black * 0.8f;
		this.biggestCatch.color = Color.black * 0.8f;
		Image[] componentsInChildren = this.bubbleHolder.GetComponentsInChildren<Image>();
		foreach (Image image in componentsInChildren)
		{
			image.color = this.caughtFishbgColor * 1.1f;
			RectTransform component = image.GetComponent<RectTransform>();
			component.anchoredPosition += new Vector2((float)UnityEngine.Random.Range(-7, 7), (float)UnityEngine.Random.Range(-7, 7));
		}
	}

	public override void OnShouldRegisterListeners()
	{
	}

	public override void OnShouldUnregisterListeners()
	{
	}

	[SerializeField]
	private Image fishIcon;

	[SerializeField]
	private Image fishIconBg;

	[SerializeField]
	private TextMeshProUGUI fishName;

	[SerializeField]
	private TextMeshProUGUI fishDescription;

	[SerializeField]
	private TextMeshProUGUI biggestCatch;

	[SerializeField]
	private TextMeshProUGUI baseValue;

	[SerializeField]
	private TextMeshProUGUI stars;

	[SerializeField]
	private Image fishItemBg;

	[SerializeField]
	private Image fakeMask;

	[SerializeField]
	private Image starImage;

	private Color caughtItemBgColor;

	private Color caughtTextColor;

	private Color caughtFishbgColor;

	[SerializeField]
	private Color caughtStarColor;

	[SerializeField]
	private GameObject bubbleHolder;

	private bool isTierFish;

	private Color grayish = new Color(0.9f, 0.9f, 0.9f, 1f);

	public FishBehaviour.FishType FishType;
}
