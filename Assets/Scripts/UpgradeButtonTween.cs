using System;
using DG.Tweening;
using UnityEngine;

public class UpgradeButtonTween : MonoBehaviour
{
	private void Start()
	{
		this.rectTransform = (RectTransform)base.transform;
		this.startingPosition = this.rectTransform.anchoredPosition;
	}

	private void ToggleActiveButton()
	{
		if (this.activeButton.activeInHierarchy)
		{
			this.KillAllTweens();
			this.inactiveButton.SetActive(true);
			this.isActiveButton = false;
			base.transform.DOScale(0f, 0.2f);
			this.shadowRectTransform.DOScale(0f, 0.2f).OnComplete(delegate
			{
				this.activeButton.SetActive(false);
			});
		}
		else
		{
			this.KillAllTweens();
			this.isActiveButton = true;
			this.activeButton.SetActive(true);
			base.transform.localScale = Vector2.zero;
			this.Idle();
			this.inactiveButton.SetActive(false);
		}
	}

	public void Idle()
	{
	}

	public void Press()
	{
	}

	private void KillAllTweens()
	{
	}

	[SerializeField]
	private RectTransform shadowRectTransform;

	[SerializeField]
	private GameObject activeButton;

	[SerializeField]
	private GameObject inactiveButton;

	private Vector2 startingPosition;

	private RectTransform rectTransform;

	private bool isActiveButton;
}
