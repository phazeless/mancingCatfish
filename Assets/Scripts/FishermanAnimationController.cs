using System;
using UnityEngine;

public class FishermanAnimationController : MonoBehaviour
{
	private void Start()
	{
		this.rodCatcher.OnFishHooked += this.RodCatcher_OnFishHooked;
	}

	private void RodCatcher_OnFishHooked(FishBehaviour obj)
	{
		this.SetCatchAnimation();
	}

	private void SetCatchAnimation()
	{
		int num = UnityEngine.Random.Range(0, 3);
		this.animator.SetTrigger("catch" + num);
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private RodCatcher rodCatcher;
}
