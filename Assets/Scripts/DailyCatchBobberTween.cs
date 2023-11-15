using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyCatchBobberTween : MonoBehaviour
{
	private void Start()
	{
	}

	public void SetBobber(int currentDay)
	{
		DailyGiftContentPossibilities dailyGiftContentPossibilitiesForStreak = DailyGiftManager.Instance.GetDailyGiftContentPossibilitiesForStreak(currentDay);
		this.bgImage.color = dailyGiftContentPossibilitiesForStreak.Visuals.color;
		this.coloredWaterPartImage.color = dailyGiftContentPossibilitiesForStreak.Visuals.color;
		this.splasher.SetColor(dailyGiftContentPossibilitiesForStreak.Visuals.color);
		this.bob.GetComponent<Image>().sprite = dailyGiftContentPossibilitiesForStreak.Visuals.bobblerIcon;
	}

	public void IdleBobbing()
	{
		this.bgImage.color = this.grey;
		this.coloredWaterPartImage.color = this.grey;
		this.bob.localEulerAngles = new Vector3(0f, 0f, -8f);
		this.bob.DOAnchorPosY(this.bob.anchoredPosition.y - 10f, 1f, false).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
		this.bob.DORotate(new Vector3(0f, 0f, 8f), 2f, RotateMode.Fast).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).SetDelay(0.5f);
		this.edge.DOScale(0.96f, 0.5f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
		base.InvokeRepeating("AnimateIdleRing", 0.5f, 2f);
	}

	private void AnimateIdleRing()
	{
		this.ringHolder.localScale = Vector3.zero;
		this.waterRingPart.localScale = Vector3.zero;
		this.whiteWaterPartImage.color = this.semiTransparent;
		this.ringHolder.DOScale(1f, 1.5f).SetEase(Ease.OutCubic);
		this.waterRingPart.DOScale(1f, 1.5f).SetEase(Ease.OutQuad);
		this.whiteWaterPartImage.DOFade(0f, 1f).SetDelay(0.4f);
	}

	public void CatchBobbing()
	{
		this.softEdgeFixer.gameObject.SetActive(false);
		this.bgImage.raycastTarget = true;
		base.transform.DOScale(0.98f, 1f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
		this.tapToPullLabel.color = new Color(1f, 1f, 1f, 0f);
		this.tapToPullLabel.DOFade(1f, 0.5f).SetDelay(1f).OnComplete(delegate
		{
			this.tapToPullLabel.DOFade(0f, 2f).SetLoops(-1, LoopType.Yoyo);
		});
		this.Bob();
	}

	private void Bob()
	{
		this.AnimateCatchRing();
		this.edge.DOScale(0.8f, 0.4f).SetEase(Ease.InOutCubic);
		this.splashParticle.Play();
		this.mask.DOAnchorPosY(this.mask.anchoredPosition.y + 10f, 0.4f, false).SetEase(Ease.InOutQuad).OnComplete(delegate
		{
			this.mask.DOAnchorPosY(this.mask.anchoredPosition.y - 10f, 0.5f, false).SetEase(this.catchbob).OnComplete(delegate
			{
			});
		});
		this.bob.DOAnchorPosY(this.bob.anchoredPosition.y - 50f, 0.3f, false).OnComplete(delegate
		{
			this.edge.DOScale(1f, 0.6f).SetEase(this.catchbob);
			int num = UnityEngine.Random.Range(0, 2);
			float z = 2f;
			if (num == 0)
			{
				z = -2f;
			}
			this.bob.DORotate(new Vector3(0f, 0f, z), 0.3f, RotateMode.Fast).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo);
			base.Invoke("AnimateCatchRing2", 0.3f);
			this.bob.DOAnchorPosY(this.bob.anchoredPosition.y + 50f, 0.9f, false).SetEase(this.catchbob).OnComplete(delegate
			{
				this.bob.DORotate(new Vector3(0f, 0f, 0f), 0.3f, RotateMode.Fast).SetEase(Ease.InOutQuad);
				this.Bob();
			});
		});
	}

	private void AnimateCatchRing()
	{
		this.ringHolder.localScale = Vector3.zero;
		this.waterRingPart.localScale = Vector3.zero;
		this.whiteWaterPartImage.color = Color.white;
		this.ringHolder.DOScale(1f, 0.7f).SetEase(Ease.OutCubic);
		this.waterRingPart.DOScale(1f, 0.7f).SetEase(Ease.OutQuad);
		this.whiteWaterPartImage.DOFade(0f, 0.4f).SetDelay(0.2f).OnStart(delegate
		{
		});
	}

	private void AnimateCatchRing2()
	{
		this.ringHolder.DOKill(false);
		this.waterRingPart.DOKill(false);
		this.whiteWaterPartImage.DOKill(false);
		this.ringHolder.localScale = Vector3.zero;
		this.waterRingPart.localScale = Vector3.zero;
		this.whiteWaterPartImage.color = this.semiTransparent;
		this.ringHolder.DOScale(0.8f, 0.5f).SetEase(Ease.OutCubic);
		this.waterRingPart.DOScale(0.8f, 0.5f).SetEase(Ease.OutQuad);
		this.whiteWaterPartImage.DOFade(0f, 0.3f).SetDelay(0.2f).OnStart(delegate
		{
		});
	}

	public void Pull()
	{
		this.TweenKiller();
		this.bigSplashParticle.Play();
		this.splasher.Splash();
		AudioManager.Instance.OneShooter(this.splashSoundClip, 1f);
		this.ringHolder.localScale = Vector3.zero;
		this.canvas.overrideSorting = true;
		this.canvas.sortingOrder = 15;
		this.ringHolder.DOScale(2.5f, 0.6f).SetEase(Ease.OutQuad);
		this.waterRingPart.DOScale(0f, 0.4f).SetEase(Ease.OutQuad);
		this.edge.DOScale(0f, 0.5f).SetEase(Ease.OutQuad);
		this.whiteWaterPartImage.color = Color.white;
		this.whiteWaterPartImage.DOFade(0f, 0.5f).SetDelay(0.3f);
		base.transform.DOPunchScale(Vector3.one * 0.1f, 0.8f, 10, 1f);
		this.bgImage.DOColor(this.grey, 0.2f).SetDelay(0.35f);
		this.bgImage.raycastTarget = false;
		this.mask.DOScale(2f, 0.2f).SetEase(Ease.OutQuad).OnComplete(delegate
		{
			this.mask.DOScale(0f, 0.2f).SetEase(Ease.InOutQuad);
		});
		this.bob.DOAnchorPosY(this.bob.anchoredPosition.y + 80f, 0.3f, false).OnComplete(delegate
		{
		});
		this.mask.DOAnchorPosY(this.mask.anchoredPosition.y + 300f, 0.3f, false).OnComplete(delegate
		{
		});
	}

	private void TweenKiller()
	{
		base.CancelInvoke("AnimateIdleRing");
		base.CancelInvoke("AnimateCatchRing2");
		this.ringHolder.DOKill(false);
		this.waterRingPart.DOKill(false);
		this.whiteWaterPartImage.DOKill(false);
		this.edge.DOKill(false);
		this.mask.DOKill(false);
		this.bob.DOKill(false);
		base.transform.DOKill(false);
		this.tapToPullLabel.DOKill(false);
		this.bgImage.DOKill(false);
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	[SerializeField]
	private RectTransform bob;

	[SerializeField]
	private RectTransform ringHolder;

	[SerializeField]
	private RectTransform waterRingPart;

	[SerializeField]
	private RectTransform edge;

	[SerializeField]
	private RectTransform softEdgeFixer;

	[SerializeField]
	private RectTransform mask;

	[SerializeField]
	private Image whiteWaterPartImage;

	[SerializeField]
	private Image coloredWaterPartImage;

	[SerializeField]
	private Image bgImage;

	[SerializeField]
	private DailyCatchSplash splasher;

	[SerializeField]
	private AudioClip splashSoundClip;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private TextMeshProUGUI tapToPullLabel;

	[SerializeField]
	private ParticleSystem splashParticle;

	[SerializeField]
	private ParticleSystem bigSplashParticle;

	[SerializeField]
	private AnimationCurve catchbob;

	private Color semiTransparent = new Color(1f, 1f, 1f, 0.5f);

	private Color grey = new Color(0.85f, 0.85f, 0.85f, 1f);

	private Color pink = new Color(0.929f, 0.404f, 0.478f, 1f);

	private Color blue = new Color(0.537f, 0.8f, 0.792f, 1f);

	private Color lightBlue = new Color(0.637f, 0.85f, 0.892f, 1f);
}
