using System;
using DG.Tweening;
using UnityEngine;

public class FishShadowBehaviour : MonoBehaviour
{
	public void StartTween()
	{
		base.transform.gameObject.SetActive(true);
		Vector3 endValue = base.transform.position + BucketEffect.instance.transform.position + new Vector3(1f, -1f);
		base.transform.DOMove(endValue, 0.25f, false).OnComplete(delegate
		{
			base.transform.DOMove(BucketEffect.instance.transform.position, 0.25f, false).OnComplete(delegate
			{
				base.transform.position = Vector3.zero;
				base.transform.gameObject.SetActive(false);
			});
		});
	}
}
