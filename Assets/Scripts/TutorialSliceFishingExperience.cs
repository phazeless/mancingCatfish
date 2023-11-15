using System;

public class TutorialSliceFishingExperience : TutorialSliceBase
{
	private void Instance_OnCompleted()
	{
		this.Enter();
		TutorialManager.Instance.SetGraphicRaycaster(true);
		this.RunAfterDelay(0.5f, delegate()
		{
			this.hasdelayed = true;
		});
	}

	public void ClickNext()
	{
		if (this.hasdelayed)
		{
			base.Exit(true);
		}
	}

	protected override void Setup()
	{
		base.Setup();
		if (TutorialSliceDeepWater.Instance != null)
		{
			TutorialSliceDeepWater.Instance.OnCompleted += this.Instance_OnCompleted;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (TutorialSliceDeepWater.Instance != null)
		{
			TutorialSliceDeepWater.Instance.OnCompleted -= this.Instance_OnCompleted;
		}
	}

	private bool hasdelayed;
}
