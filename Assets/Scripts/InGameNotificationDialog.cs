using System;
using UnityEngine;

public abstract class InGameNotificationDialog : UpgradeDialogTween
{
	public abstract void SetInGameNotification(InGameNotification ign);

	public abstract InGameNotification GetInGameNotification();

	public void SetInGameNotificationManager(InGameNotificationManager ignManager)
	{
		this.ignManager = ignManager;
	}

	public void SetDialogCanvas(Transform dialogCanvas)
	{
		this.dialogCanvas = dialogCanvas;
	}

	protected InGameNotificationManager ignManager;

	protected Transform dialogCanvas;
}
