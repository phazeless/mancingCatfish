using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GoToDialogTweens : MonoBehaviour
{
	public void Enter()
	{
		base.transform.DOKill(true);
		base.gameObject.SetActive(true);
		base.transform.localScale = Vector2.zero;
		base.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetDelay(1f);
	}

	public void SetAvaliable()
	{
		this.button.interactable = true;
		this.shine.SetActive(true);
	}

	public void SetUnaviable()
	{
		this.button.interactable = false;
		this.shine.SetActive(false);
	}

	public void Exit()
	{
		base.transform.DOKill(true);
		base.transform.DOScale(0f, 0.25f).SetEase(Ease.InBack).OnComplete(delegate
		{
			base.gameObject.SetActive(false);
		});
	}

	[SerializeField]
	private Button button;

	[SerializeField]
	private GameObject shine;
}
