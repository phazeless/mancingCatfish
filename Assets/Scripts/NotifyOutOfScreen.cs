using System;
using UnityEngine;

public class NotifyOutOfScreen : MonoBehaviour
{
	private void Awake()
	{
		this.myTransform = base.transform;
	}

	private void Start()
	{
		this.objectThatListens = base.GetComponent<INotifyOutOfScreen>();
		if (this.objectThatListens == null)
		{
			UnityEngine.Debug.LogError("NotifyOutOfScreen-component must be on the same object that has an INotifyOutOfScreen-component!");
		}
	}

	private void Update()
	{
		if (this.objectThatListens == null)
		{
			return;
		}
		NotifyOutOfScreen.OutOfScreenMethod? outOfScreenMethod = null;
		if (this.listenerMode == NotifyOutOfScreen.ListenerMode.BeyondScreen)
		{
			if (this.myTransform.position.y > CameraMovement.TopY + this.outOfScreenMargin + this.extraMarginOutOfScreenMargin)
			{
				outOfScreenMethod = new NotifyOutOfScreen.OutOfScreenMethod?(NotifyOutOfScreen.OutOfScreenMethod.Top);
			}
			else if (this.myTransform.position.y < CameraMovement.BottomY - this.outOfScreenMargin - this.extraMarginOutOfScreenMargin)
			{
				outOfScreenMethod = new NotifyOutOfScreen.OutOfScreenMethod?(NotifyOutOfScreen.OutOfScreenMethod.Bottom);
			}
			else if (this.myTransform.position.x > CameraMovement.RightX + this.outOfScreenMargin + this.extraMarginOutOfScreenMargin)
			{
				outOfScreenMethod = new NotifyOutOfScreen.OutOfScreenMethod?(NotifyOutOfScreen.OutOfScreenMethod.Right);
			}
			else if (this.myTransform.position.x < CameraMovement.LeftX - this.outOfScreenMargin - this.extraMarginOutOfScreenMargin)
			{
				outOfScreenMethod = new NotifyOutOfScreen.OutOfScreenMethod?(NotifyOutOfScreen.OutOfScreenMethod.Left);
			}
			if (outOfScreenMethod != null)
			{
				this.objectThatListens.OnOutOfScreen(outOfScreenMethod.Value, this.listenerMode, base.gameObject);
			}
		}
	}

	public void ForceOutOfScreen()
	{
		if (this.objectThatListens != null)
		{
			this.objectThatListens.OnOutOfScreen(NotifyOutOfScreen.OutOfScreenMethod.Forced, NotifyOutOfScreen.ListenerMode.Forced, base.gameObject);
		}
	}

	private Transform myTransform;

	[SerializeField]
	private NotifyOutOfScreen.ListenerMode listenerMode = NotifyOutOfScreen.ListenerMode.BeyondScreen;

	[SerializeField]
	private float outOfScreenMargin;

	private float extraMarginOutOfScreenMargin = 0.5f;

	private INotifyOutOfScreen objectThatListens;

	public enum ListenerMode
	{
		Forced,
		BeyondScreen
	}

	public enum OutOfScreenMethod
	{
		Forced,
		Top,
		Right,
		Bottom,
		Left
	}
}
