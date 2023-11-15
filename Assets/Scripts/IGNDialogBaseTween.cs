using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IGNDialogBaseTween : MonoBehaviour
{
	public virtual void Start()
	{
		this.SetDialogInitialState();
	}

	public virtual void SetDialogInitialState()
	{
		this.dialogActive = false;
		this.bgRect.sizeDelta = this.bgStartSize;
		this.contentRect.localScale = Vector2.zero;
	}

	public virtual void OpenDialogTween()
	{
		this.iconButton.interactable = false;
		this.HolderTweenKiller();
		this.dialogActive = true;
		this.holderRect.DOAnchorPosY(300f, 0.25f, false).SetEase(Ease.OutBack);
		this.contentRect.gameObject.SetActive(true);
		this.bgRect.gameObject.SetActive(true);
		this.bgRect.DOSizeDelta(this.bgTargetSize, 0.5f, false).SetEase(Ease.OutBack).SetDelay(0.15f);
		this.contentRect.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.15f);
	}

	public virtual void OpenDialogTween(Vector2 targetSize)
	{
		this.iconButton.interactable = false;
		this.HolderTweenKiller();
		this.dialogActive = true;
		this.holderRect.DOAnchorPosY(300f, 0.25f, false).SetEase(Ease.OutBack);
		this.contentRect.gameObject.SetActive(true);
		this.bgRect.gameObject.SetActive(true);
		this.bgRect.DOSizeDelta(targetSize, 0.5f, false).SetEase(Ease.OutBack).SetDelay(0.15f);
		this.contentRect.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.15f);
	}

	public virtual void CloseDialogTween()
	{
		this.iconButton.interactable = true;
		this.HolderTweenKiller();
		this.dialogActive = false;
		this.holderRect.DOAnchorPosY(0f, 0.25f, false);
		this.bgRect.DOSizeDelta(this.bgStartSize, 0.2f, false).OnComplete(delegate
		{
			this.contentRect.gameObject.SetActive(false);
			this.bgRect.gameObject.SetActive(false);
		});
		this.contentRect.DOScale(0f, 0.2f);
	}

	public virtual void ToggleDialog()
	{
		if (this.dialogActive)
		{
			this.CloseDialogTween();
		}
		else
		{
			this.OpenDialogTween();
		}
	}

	public virtual void HolderTweenKiller()
	{
		this.iconRect.DOKill(true);
		this.contentRect.DOKill(true);
		this.bgRect.DOKill(true);
		this.holderRect.DOKill(true);
	}

	protected virtual void OnDestroy()
	{
		this.HolderTweenKiller();
	}

	[SerializeField]
	[Header("Holders")]
	private RectTransform iconRect;

	[SerializeField]
	private RectTransform contentRect;

	[SerializeField]
	private RectTransform bgRect;

	[SerializeField]
	private RectTransform holderRect;

	[SerializeField]
	private Button iconButton;

	protected bool dialogActive = true;

	private Vector2 bgStartSize = new Vector2(80f, 25f);

	private Vector2 bgTargetSize = new Vector2(680f, 640f);
}
