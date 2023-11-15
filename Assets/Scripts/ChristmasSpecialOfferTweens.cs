using System;
using DG.Tweening;
using UnityEngine;

public class ChristmasSpecialOfferTweens : MonoBehaviour
{
	private void Start()
	{
		this.IntroTween();
	}

	private void IntroTween()
	{
		int num = 0;
		Transform[] array = this.itemstoPop;
		for (int i = 0; i < array.Length; i++)
		{
			Transform item = array[i];
			item.localPosition = new Vector3(item.localPosition.x, item.localPosition.y - 15f, item.localPosition.z);
			item.DOLocalMoveY(item.localPosition.y + 15f, 0.4f, false).SetDelay((float)num * this.popSpeed).SetEase(Ease.OutBack);
			item.localEulerAngles = new Vector3(0f, 0f, -item.localEulerAngles.z);
			item.DORotate(new Vector3(0f, 0f, -item.localEulerAngles.z), 0.4f, RotateMode.Fast).SetDelay((float)num * this.popSpeed).SetEase(Ease.OutBack);
			item.localScale = Vector3.zero;
			item.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay((float)num * this.popSpeed).OnComplete(delegate
			{
				item.DOScale(1.05f, 2f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
			});
			num++;
		}
	}

	private void OnDestroy()
	{
		foreach (Transform target in this.itemstoPop)
		{
			target.DOKill(false);
		}
	}

	[SerializeField]
	private Transform[] itemstoPop;

	[SerializeField]
	private float popSpeed = 0.4f;
}
