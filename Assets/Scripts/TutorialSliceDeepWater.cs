using System;
using System.Diagnostics;

public class TutorialSliceDeepWater : TutorialSliceBase
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnCompleted;

	protected override void Awake()
	{
		base.Awake();
		TutorialSliceDeepWater.Instance = this;
	}

	private void Start()
	{
		ScreenManager.Instance.OnScreenTransitionStarted += this.Instance_OnScreenTransitionStarted;
	}

	private void Instance_OnScreenTransitionStarted(ScreenManager.Screen to, ScreenManager.Screen from)
	{
		if (to == ScreenManager.Screen.Map)
		{
			ScreenManager.Instance.OnScreenTransitionStarted -= this.Instance_OnScreenTransitionStarted;
			this.RunAfterDelay(0.2f, delegate()
			{
				this.Enter();
				TutorialManager.Instance.SetGraphicRaycaster(true);
				this.RunAfterDelay(0.5f, delegate()
				{
					this.hasdelayed = true;
				});
			});
		}
	}

	public void ClickNext()
	{
		if (this.hasdelayed)
		{
			base.Exit(true);
		}
	}

	protected override void Entered()
	{
	}

	protected override void Exited()
	{
		if (this.OnCompleted != null)
		{
			this.OnCompleted();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ScreenManager.Instance.OnScreenTransitionStarted -= this.Instance_OnScreenTransitionStarted;
	}

	public static TutorialSliceDeepWater Instance;

	private bool hasdelayed;
}
