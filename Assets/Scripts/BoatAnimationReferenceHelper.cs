using System;
using UnityEngine;

public class BoatAnimationReferenceHelper : MonoBehaviour
{
	private void Awake()
	{
		BoatAnimationReferenceHelper.Instance = this;
		this.animator = base.GetComponent<Animator>();
	}

	public void BossCaughtBoatAnimation()
	{
		this.animator.SetTrigger("BossCatch");
	}

	public static BoatAnimationReferenceHelper Instance;

	private Animator animator;
}
