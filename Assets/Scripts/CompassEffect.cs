using System;
using DG.Tweening;
using UnityEngine;

public class CompassEffect : MonoBehaviour
{
	private void OnEnable()
	{
		this.pointer.DOKill(false);
		this.pointer.localEulerAngles = new Vector3(0f, 0f, 25f);
		this.pointer.DORotate(new Vector3(0f, 0f, -25f), 3f, RotateMode.Fast).SetEase(Ease.OutElastic);
	}

	public void OnTapped()
	{
		this.pointer.DOKill(true);
		base.transform.DOKill(true);
		base.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.5f, 7, 0.8f);
		this.pointer.DOPunchRotation(new Vector3(0f, 0f, 5f), 1f, 5, 0.7f);
		base.transform.DORotate(new Vector3(5f, 10f, 0f), 0.1f, RotateMode.Fast).SetEase(Ease.Linear).OnComplete(delegate
		{
			base.transform.DORotate(new Vector3(-5f, 5f, 0f), 0.2f, RotateMode.Fast).SetEase(Ease.Linear).OnComplete(delegate
			{
				base.transform.DORotate(new Vector3(2f, -2f, 0f), 0.2f, RotateMode.Fast).SetEase(Ease.Linear).OnComplete(delegate
				{
					base.transform.DORotate(new Vector3(0f, 0f, 0f), 0.2f, RotateMode.Fast).SetEase(Ease.Linear).OnComplete(delegate
					{
					});
				});
			});
		});
	}

	private void OnDestroy()
	{
		this.pointer.DOKill(true);
		base.transform.DOKill(true);
	}

	[SerializeField]
	private Transform pointer;
}
