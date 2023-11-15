using System;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeDialogTween : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnOpen;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnClose;

	protected virtual void Awake()
	{
		this.cachedRectTransform = (RectTransform)base.transform;
	}

	protected virtual void Start()
	{
	}

	public void Toggle()
	{
		if (this.isOpen)
		{
			this.Close(false);
		}
		else
		{
			this.Open();
		}
	}

	public bool IsOpen
	{
		get
		{
			return this.isOpen;
		}
	}

	public virtual void Open()
	{
		this.TweenKiller();
		DialogInteractionHandler.Instance.NewActiveDialog(base.transform);
		AudioManager.Instance.MenuWhoosh();
		this.isOpen = true;
		base.gameObject.SetActive(true);
		base.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetId("UpgradeDialogScaleOpen").OnComplete(delegate
		{
			this.dialogTweenFinishedListeners.Invoke();
		});
		this.cachedRectTransform.DOAnchorPos(new Vector2(0f, (float)this.YPositionAdjuster), 0.25f, false).SetId("UpgradeDialogPositionOpen").SetEase(Ease.OutBack);
		if (this.OnOpen != null)
		{
			this.OnOpen();
		}
	}

	public virtual void Close(bool destroyOnFinish = false)
	{
		if (!this.isOpen)
		{
			return;
		}
		this.TweenKiller();
		this.isOpen = false;
		AudioManager.Instance.MenuWhoosh();
		DialogInteractionHandler.Instance.DialogClosed(base.transform);
		this.cachedRectTransform.DOAnchorPos(new Vector2(0f, -800f), 0.25f, false).SetId("UpgradeDialogPositionClose").SetEase(Ease.InBack);
		base.transform.DOScale(0f, 0.25f).SetEase(Ease.InBack).SetId("UpgradeDialogScaleClose").OnComplete(delegate
		{
			if (destroyOnFinish)
			{
				UnityEngine.Object.Destroy(this.gameObject);
			}
			else
			{
				this.gameObject.SetActive(false);
			}
		});
		if (this.OnClose != null)
		{
			this.OnClose();
		}
	}

	private void TweenKiller()
	{
		DOTween.Kill(base.transform, false);
		this.cachedRectTransform.DOKill(false);
	}

	protected virtual void OnDestroy()
	{
		this.TweenKiller();
	}

	[SerializeField]
	protected int YPositionAdjuster = 50;

	private bool isOpen;

	private RectTransform cachedRectTransform;

	[SerializeField]
	private UnityEvent dialogTweenFinishedListeners = new UnityEvent();
}
