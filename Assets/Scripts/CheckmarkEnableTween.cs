using System;
using DG.Tweening;
using UnityEngine;

public class CheckmarkEnableTween : MonoBehaviour
{
	private void OnEnable()
	{
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].localScale = Vector3.zero;
			componentsInChildren[i].DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f + 0.2f * (float)i);
		}
	}

	private void OnDestroy()
	{
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DOKill(false);
		}
	}
}
