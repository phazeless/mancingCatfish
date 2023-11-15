using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IGNNewFishDialog : InGameNotificationDialog<IGNNewFish>
{
	protected override void OnIGNHasBeenSet()
	{
	}

	protected override void OnAboutToOpen()
	{
		this.customSize = new Vector2?(new Vector2(732f, 822f));
		this.SetCaughtFishInfo();
	}

	protected override void OnAboutToReturn()
	{
	}

	public void ClaimButton()
	{
		this.Close(true);
	}

	protected override void OnOpened()
	{
	}

	protected override void OnRemovedFromList()
	{
		this.iconTween.IconTweenKiller();
		StarGainEffect starGainEffect = UnityEngine.Object.Instantiate<StarGainEffect>(this.starGainEffect);
		starGainEffect.GainStars(this.fishInfo.Stars);
	}

	protected override void OnReturned()
	{
	}

	private void SetCaughtFishInfo()
	{
		this.fishInfo = this.inGameNotification.FishInfo;
		if (this.fishInfo == null)
		{
			return;
		}
		this.fish.sprite = FishPoolManager.Instance.GetFishIcon(this.fishInfo.FishType);
		if (this.fishInfo.Rarity == -1)
		{
			this.fish.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}
		this.fishDecriptionLabel.SetText('"' + this.fishInfo.Description + '"');
		this.fishTitle.SetText(this.fishInfo.Name);
		this.starsLabel.SetVariableText(new string[]
		{
			this.fishInfo.Stars.ToString()
		});
	}

	private void Update()
	{
	}

	[SerializeField]
	private IGNPackageTween iconTween;

	[SerializeField]
	private Image fish;

	[SerializeField]
	private TextMeshProUGUI starsLabel;

	[SerializeField]
	private TextMeshProUGUI fishTitle;

	[SerializeField]
	private TextMeshProUGUI fishDecriptionLabel;

	[SerializeField]
	private StarGainEffect starGainEffect;

	private FishAttributes fishInfo;
}
