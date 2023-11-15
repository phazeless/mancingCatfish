using System;
using DG.Tweening;
using UnityEngine;

public class FloatTween : MonoBehaviour
{
	private void Start()
	{
		this.startScale = base.transform.localScale;
		base.transform.DOScale(this.startScale * 1.05f, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
		base.transform.DORotate(new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, 2f), 7f, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
	}

	private Vector3 startScale;
}
