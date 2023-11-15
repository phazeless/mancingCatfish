using System;
using DG.Tweening;
using UnityEngine;

public abstract class InGameNotificationDialog<T> : InGameNotificationDialog where T : InGameNotification
{
	protected abstract void OnAboutToOpen();

	protected abstract void OnOpened();

	protected abstract void OnAboutToReturn();

	protected abstract void OnReturned();

	protected abstract void OnRemovedFromList();

	protected abstract void OnIGNHasBeenSet();

	protected override void Awake()
	{
		base.Awake();
		this.tween = base.GetComponent<IGNDialogBaseTween>();
	}

	public override void SetInGameNotification(InGameNotification ign)
	{
		this.inGameNotification = (T)((object)ign);
		this.OnIGNHasBeenSet();
	}

	public override InGameNotification GetInGameNotification()
	{
		return this.inGameNotification;
	}

	public override void Close(bool destroyOnFinish)
	{
		base.Close(true);
		if (this.inGameNotification.IsClearable)
		{
			if (this.ignManager != null)
			{
				this.ignManager.Remove(this.inGameNotification);
			}
			this.OnRemovedFromList();
		}
		else if (!this.inGameNotification.OverrideClearable)
		{
			this.ReturnToPreviousPosition();
		}
		else
		{
			if (this.ignManager != null)
			{
				this.ignManager.Remove(this.inGameNotification);
			}
			this.OnRemovedFromList();
		}
	}

	public override void Open()
	{
		this.OnAboutToOpen();
		Vector2? vector = this.customSize;
		if (vector == null)
		{
			this.tween.OpenDialogTween();
		}
		else
		{
			IGNDialogBaseTween igndialogBaseTween = this.tween;
			Vector2? vector2 = this.customSize;
			igndialogBaseTween.OpenDialogTween(vector2.Value);
		}
		this.originalParent = base.transform.parent;
		this.originalLocalPosition = base.transform.localPosition;
		base.transform.SetParent(this.dialogCanvas);
		base.Open();
		this.OnOpened();
		if (this.ignManager != null)
		{
			this.ignManager.NotifyIGNOpened(this.inGameNotification);
		}
	}

	private void ReturnToPreviousPosition()
	{
		this.KillTweens();
		this.tween.CloseDialogTween();
		this.OnAboutToReturn();
		if (this.originalParent != null)
		{
			base.transform.SetParent(this.originalParent);
			base.transform.localScale = Vector3.one;
			base.transform.DOLocalMove(this.originalLocalPosition, 0.3f, false);
			this.OnReturned();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void KillTweens()
	{
		base.transform.DOKill(false);
		this.tween.DOKill(false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.KillTweens();
	}

	private IGNDialogBaseTween tween;

	protected Vector2? customSize;

	protected T inGameNotification;

	private Transform originalParent;

	private Vector2 originalLocalPosition = Vector2.zero;
}
