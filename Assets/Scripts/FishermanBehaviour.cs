using System;
using DG.Tweening;
using UnityEngine;

public class FishermanBehaviour : MonoBehaviour
{
	public void Pull()
	{
		DOTween.Kill("fisherman", false);
		DOTween.Kill("fisherman1", false);
		DOTween.Kill("fisherman2", false);
		DOTween.Kill("fisherman3", false);
		this.rod.localEulerAngles = Vector3.zero;
		this.hat.localEulerAngles = Vector3.zero;
		this.rod.DOLocalRotate(new Vector3(0f, -45f, 0f), 0.3f, RotateMode.Fast).SetId("fisherman").OnComplete(delegate
		{
			this.rod.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f, RotateMode.Fast).SetId("fisherman1");
		});
		this.hat.DOLocalRotate(new Vector3(0f, 0f, -10f), 0.3f, RotateMode.Fast).SetId("fisherman2").OnComplete(delegate
		{
			this.hat.DOLocalRotate(new Vector3(0f, 0f, 0f), 1f, RotateMode.Fast).SetId("fisherman3");
		});
	}

	private void Start()
	{
		this.guy.DORotate(new Vector3(0f, 0f, 45f), 2f, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
	}

	private void Update()
	{
	}

	public Transform rod;

	public Transform hat;

	public Transform guy;
}
