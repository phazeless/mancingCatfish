using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardItem : MonoBehaviour
{
	public void SetValues(Sprite sprite, Color color, string count, bool isInfo = false, float specialScale = 1f)
	{
		this.countLabel.SetText(count);
		this.countLabel.color = color;
		this.bgColorImage.color = color;
		this.icon.sprite = sprite;
		this.icon.transform.localScale = Vector3.one * specialScale;
		if (isInfo)
		{
			this.showAsInfo = true;
		}
		if (!this.showAsInfo)
		{
			this.Reveal();
		}
		else
		{
			this.SetInfo();
		}
	}

	private void Reveal()
	{
		base.transform.localScale = Vector3.zero;
		this.countRect.localScale = Vector3.zero;
		base.transform.localEulerAngles = new Vector3(0f, 0f, 15f);
		this.countRect.localEulerAngles = new Vector3(0f, 0f, 15f);
		AudioManager.Instance.OneShooter(this.itemGetSoundClip, 1f);
		base.transform.DOScale(1f, 0.6f).SetEase(Ease.OutElastic).OnComplete(delegate
		{
			base.transform.DOScale(1.05f, 1f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
		});
		this.countRect.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.4f);
		base.transform.DORotate(Vector3.zero, 0.5f, RotateMode.Fast).SetEase(Ease.OutBack);
		this.countRect.DORotate(Vector3.zero, 0.5f, RotateMode.Fast).SetEase(Ease.OutBack).SetDelay(0.5f);
		this.shine.DOAnchorPosX(150f, 0.3f, false).SetDelay(0.5f);
	}

	public void Remove()
	{
		base.transform.DOKill(false);
		this.countRect.DOKill(false);
		this.shine.DOKill(false);
		base.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack).OnComplete(delegate
		{
			if (base.gameObject != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		});
	}

	private void SetInfo()
	{
		base.transform.localScale = Vector3.zero;
		this.countRect.localScale = Vector3.zero;
		base.transform.localEulerAngles = new Vector3(0f, 0f, 15f);
		this.countRect.localEulerAngles = new Vector3(0f, 0f, 15f);
		base.transform.DOScale(1f, 0.6f).SetEase(Ease.OutElastic);
		this.countRect.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
		base.transform.DORotate(Vector3.zero, 0.5f, RotateMode.Fast).SetEase(Ease.OutBack);
		this.countRect.DORotate(Vector3.zero, 0.5f, RotateMode.Fast).SetEase(Ease.OutBack);
		this.shine.DOAnchorPosX(150f, 0.3f, false).SetDelay(0.5f);
	}

	private void OnDestroy()
	{
		base.transform.DOKill(false);
		this.countRect.DOKill(false);
		this.shine.DOKill(false);
	}

	[SerializeField]
	private RectTransform countRect;

	[SerializeField]
	private RectTransform shine;

	[SerializeField]
	private TextMeshProUGUI countLabel;

	[SerializeField]
	private Image bgColorImage;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private AudioClip itemGetSoundClip;

	private bool showAsInfo;
}
