using System;
using DG.Tweening;
using UnityEngine;

public class BoatUpgradeBodyExpansion : MonoBehaviour
{
	private void Start()
	{
		base.transform.localScale = Vector2.zero;
		this.spriteRenderer.DOFade(0f, this.AnimationTime);
		base.transform.DOScale(10f, this.AnimationTime).OnComplete(delegate
		{
			this.spriteRenderer.DOKill(false);
			UnityEngine.Object.Destroy(base.gameObject);
		});
	}

	private void OnDestroy()
	{
		if (base.transform != null)
		{
			base.transform.DOKill(false);
		}
		if (this.spriteRenderer != null)
		{
			this.spriteRenderer.DOKill(false);
		}
	}

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private float AnimationTime;
}
