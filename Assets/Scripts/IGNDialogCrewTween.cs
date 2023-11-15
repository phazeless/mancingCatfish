using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IGNDialogCrewTween : IGNDialogBaseTween
{
	public override void Start()
	{
		this.SetDialogInitialState();
		this.crewDialog = base.GetComponent<IGNNewCrewDialog>();
		this.startingIconScale = this.iconHolder.localScale;
		this.startingIconLocalPosition = this.iconHolder.anchoredPosition;
		this.startingIconLocalRotation = this.iconHolder.localEulerAngles;
		this.startingContentHolderPosition = this.contentHolder.anchoredPosition;
		this.startingContentHolderRotation = this.contentHolder.localEulerAngles;
		this.startingContentHolderScale = this.contentHolder.localScale;
		this.iconHolder.localScale = Vector3.one * 0.33f;
		this.iconHolder.anchoredPosition = Vector3.zero;
		this.iconHolder.localEulerAngles = Vector3.zero;
		this.contentHolder.localScale = Vector3.zero;
		this.contentHolder.anchoredPosition = Vector3.zero;
		this.contentHolder.localEulerAngles = Vector3.zero;
		this.iconShadow.localScale = Vector2.zero;
		this.SetPassiveIconTween();
	}

	public override void SetDialogInitialState()
	{
		this.dialogActive = false;
	}

	public override void OpenDialogTween()
	{
		this.crewIconButton.interactable = false;
		this.HolderTweenKiller();
		this.dialogActive = true;
		this.OpenTween();
	}

	public override void OpenDialogTween(Vector2 targetSize)
	{
		this.crewIconButton.interactable = false;
		this.HolderTweenKiller();
		this.dialogActive = true;
	}

	private void OpenTween()
	{
		this.iconHolder.DOScale(this.startingIconScale, 0.4f).SetEase(Ease.OutBack);
		this.iconHolder.DORotate(this.startingIconLocalRotation, 0.7f, RotateMode.Fast).SetEase(Ease.InOutBack).SetDelay(0.3f);
		this.iconHolder.DOAnchorPos(this.startingIconLocalPosition, 0.7f, false).SetEase(Ease.OutBack).SetDelay(0.1f);
		this.contentHolder.DOAnchorPos(this.startingContentHolderPosition, 0.6f, false).SetEase(Ease.InOutBack).SetDelay(0.1f).OnComplete(delegate
		{
			this.holderHolder.DOAnchorPos(new Vector2(this.holderHolder.anchoredPosition.x, this.holderHolder.anchoredPosition.y + 15f), 2f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
		});
		this.contentHolder.DOScale(this.startingContentHolderScale, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f);
		this.contentHolder.DORotate(this.startingContentHolderRotation, 0.4f, RotateMode.Fast).SetEase(Ease.OutBack).SetDelay(0.2f);
		this.iconShadow.DOScale(Vector2.one, 0.5f);
	}

	public override void CloseDialogTween()
	{
		this.crewIconButton.interactable = true;
		this.HolderTweenKiller();
		this.dialogActive = false;
		this.iconHolder.DOScale(Vector3.one * 0.33f, 0.3f).SetEase(Ease.OutCirc);
		this.iconHolder.DORotate(Vector3.zero, 0.3f, RotateMode.Fast);
		this.iconHolder.DOAnchorPos(Vector3.zero, 0.3f, false);
		this.contentHolder.DOAnchorPos(Vector3.zero, 0.3f, false);
		this.contentHolder.DOScale(Vector3.zero, 0.1f);
		this.contentHolder.DORotate(Vector3.zero, 0.3f, RotateMode.Fast);
		this.iconShadow.DOScale(Vector2.zero, 0.5f).OnComplete(delegate
		{
			this.SetPassiveIconTween();
		});
	}

	public void SetPassiveIconTween()
	{
		if (!this.crewDialog.IsNewCrewDialog)
		{
			return;
		}
		this.passiveIconSequence = DOTween.Sequence();
		Tween t = this.iconHolder.DORotate(new Vector3(0f, 0f, -10f), 0.4f, RotateMode.Fast).SetEase(Ease.InOutBack).OnStart(delegate
		{
			this.iconHolder.localEulerAngles = new Vector3(0f, 0f, 0f);
		});
		Tween t2 = this.iconHolder.DORotate(new Vector3(0f, 0f, 10f), 0.2f, RotateMode.Fast).SetEase(Ease.OutBack);
		Tween t3 = this.iconHolder.DORotate(new Vector3(0f, 0f, 0f), 0.2f, RotateMode.Fast).SetEase(Ease.OutBack);
		this.passiveIconSequence.AppendInterval(3f).Append(t).Append(t2).Append(t3).SetLoops(-1, LoopType.Restart);
	}

	public override void ToggleDialog()
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

	public override void HolderTweenKiller()
	{
		this.iconHolder.DOKill(true);
		this.contentHolder.DOKill(true);
		this.iconShadow.DOKill(true);
		this.holderHolder.DOKill(true);
		this.holderHolder.anchoredPosition = Vector2.zero;
		this.passiveIconSequence.Kill(true);
	}

	protected override void OnDestroy()
	{
		this.HolderTweenKiller();
	}

	[SerializeField]
	[Header("CrewSpecific")]
	private RectTransform iconHolder;

	[SerializeField]
	private RectTransform iconShadow;

	[SerializeField]
	private RectTransform holderHolder;

	[SerializeField]
	private RectTransform contentHolder;

	[SerializeField]
	private Button crewIconButton;

	private Vector3 startingIconScale;

	private Vector3 startingIconLocalPosition;

	private Vector3 startingIconLocalRotation;

	private Vector3 startingContentHolderRotation;

	private Vector3 startingContentHolderScale;

	private Vector3 startingContentHolderPosition;

	private Sequence passiveIconSequence;

	private IGNNewCrewDialog crewDialog;
}
