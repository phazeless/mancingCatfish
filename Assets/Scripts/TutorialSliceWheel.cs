using System;

public class TutorialSliceWheel : TutorialSliceBase
{
	private void Start()
	{
		ScreenManager.Instance.OnScreenTransitionStarted += this.Instance_OnScreenTransitionStarted;
	}

	private void Instance_OnScreenTransitionStarted(ScreenManager.Screen to, ScreenManager.Screen from)
	{
		if (to == ScreenManager.Screen.Shop)
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

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ScreenManager.Instance.OnScreenTransitionStarted -= this.Instance_OnScreenTransitionStarted;
	}

	private bool hasdelayed;
}
