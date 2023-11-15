using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RefreshBoatDialog : MonoBehaviour
{
	private void Start()
	{
		if (FishingExperienceHolder.Instance.queuedBoatLevelUp)
		{
			this.percantageIncrease.SetVariableText(new string[]
			{
				Mathf.Floor((float)FishingExperienceHolder.Instance.toCollectFishingExp / Mathf.Max(100f, (float)FishingExperienceHolder.Instance.oldFishingExp) * 100f) + "%"
			});
			this.AnimateTween();
			FishingExperienceHolder.Instance.queuedBoatLevelUp = false;
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void AnimateTween()
	{
		AudioManager.Instance.OneShooter(this.ahClip, 1f);
		base.transform.DORotate(new Vector3(0f, 0f, -4.619f), 2f, RotateMode.Fast).SetEase(Ease.OutElastic);
		base.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).OnComplete(delegate
		{
			base.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).SetDelay(2f).OnComplete(delegate
			{
				base.gameObject.SetActive(false);
			});
		});
	}

	private void TweenKiller()
	{
		base.transform.DOKill(false);
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	[SerializeField]
	private AudioClip ahClip;

	[SerializeField]
	private TextMeshPro percantageIncrease;
}
