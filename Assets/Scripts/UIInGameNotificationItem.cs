using System;
using DG.Tweening;
using FullInspector;
using UnityEngine;

public class UIInGameNotificationItem : BaseBehavior
{
	public InGameNotificationDialog Dialog
	{
		get
		{
			return this.dialog;
		}
	}

	public InGameNotification.IGN IGNType
	{
		get
		{
			return this.ignType;
		}
	}

	public InGameNotification InGameNotification
	{
		get
		{
			return this.inGameNotification;
		}
	}

	public InGameNotificationDialog SetupAndGetDialogReferences()
	{
		if (this.dialog == null)
		{
			this.dialog = base.transform.GetChild(0).GetComponent<InGameNotificationDialog>();
		}
		return this.dialog;
	}

	protected override void Awake()
	{
		this.SetupAndGetDialogReferences();
	}

	public void Init(InGameNotification inGameNotification, InGameNotificationManager ignManager, Transform dialogCanvas)
	{
		this.inGameNotification = inGameNotification;
		this.dialog.SetInGameNotification(inGameNotification);
		this.dialog.SetDialogCanvas(dialogCanvas);
		this.dialog.SetInGameNotificationManager(ignManager);
	}

	public void OnListItemClick()
	{
		this.dialog.Open();
	}

	private void OnDestroy()
	{
		base.transform.DOKill(false);
	}

	[SerializeField]
	private InGameNotification.IGN ignType;

	private InGameNotification inGameNotification;

	private InGameNotificationDialog dialog;
}
