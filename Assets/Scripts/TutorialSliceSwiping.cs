using System;
using System.Diagnostics;
using UnityEngine;

public class TutorialSliceSwiping : TutorialSliceBase
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnCompleted;

	private void Instance_OnCompleted()
	{
		SwipeCatcher.Instance.OnFishSwiped += this.SwipeCatcher_OnFishSwiped;
		TutorialManager.Instance.SetGraphicRaycaster(true);
		base.Invoke("DelayedEnter", 2f);
	}

	private void DelayedEnter()
	{
		if (this.collectedFishSkill.LifetimeLevel < 5)
		{
			this.Enter();
			if (this.swipeAnimator != null)
			{
				this.swipeAnimator.enabled = true;
			}
		}
		else
		{
			base.Exit(true);
		}
	}

	private void SwipeCatcher_OnFishSwiped()
	{
		this.swipes++;
		if (this.swipes >= this.swipesToComplete)
		{
			base.CancelInvoke("DelayedEnter");
			if (this.swipeAnimator.isInitialized)
			{
				this.swipeAnimator.SetTrigger("exit");
			}
			base.Exit(true);
			SwipeCatcher.Instance.OnFishSwiped -= this.SwipeCatcher_OnFishSwiped;
		}
	}

	protected override void Entered()
	{
	}

	protected override void Setup()
	{
		base.Setup();
		if (TutorialSliceFishing.Instance != null)
		{
			TutorialSliceFishing.Instance.OnCompleted += this.Instance_OnCompleted;
		}
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
		SwipeCatcher.Instance.OnFishSwiped -= this.SwipeCatcher_OnFishSwiped;
		if (TutorialSliceFishing.Instance != null)
		{
			TutorialSliceFishing.Instance.OnCompleted -= this.Instance_OnCompleted;
		}
	}

	[SerializeField]
	private Skill collectedFishSkill;

	[SerializeField]
	private Animator swipeAnimator;

	public static TutorialSliceFishing Instance;

	private int swipesToComplete = 3;

	private int swipes;
}
