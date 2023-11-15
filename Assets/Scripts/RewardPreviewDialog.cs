using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RewardPreviewDialog : MonoBehaviour
{
	public void SetColorTheme(Color color)
	{
		if (this.contentHolderBg != null)
		{
			this.contentHolderBg.color = color;
		}
		if (this.contentHolderArrow != null)
		{
			this.contentHolderArrow.color = color;
		}
	}

	public void Init(params RewardBox[] instances)
	{
		for (int i = 0; i < instances.Length; i++)
		{
			instances[i].transform.SetParent(this.rewardsHolder, false);
		}
	}

	public void PeekReward()
	{
		base.transform.DOKill(true);
		base.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);
		base.transform.localEulerAngles = new Vector3(0f, 0f, 45f);
		base.transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f, RotateMode.Fast).SetEase(Ease.OutBack);
	}

	public void CloseDialog()
	{
		base.transform.DOKill(true);
		base.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
		base.transform.DORotate(new Vector3(0f, 0f, -10f), 0.3f, RotateMode.Fast).SetEase(Ease.InBack);
	}

	private void OnDestroy()
	{
		base.transform.DOKill(true);
	}

	[SerializeField]
	private RewardBox rewardBoxPrefab;

	[SerializeField]
	private Transform rewardsHolder;

	[SerializeField]
	private Image contentHolderBg;

	[SerializeField]
	private Image contentHolderArrow;
}
