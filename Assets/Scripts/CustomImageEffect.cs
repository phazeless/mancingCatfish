using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

[ExecuteInEditMode]
public class CustomImageEffect : MonoBehaviour
{
	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		Graphics.Blit(src, dst, this.effectMaterial);
	}

	public void AnimateMagnitude()
	{
		DOTween.To(() => this.magnitude, delegate(float x)
		{
			this.magnitude = x;
		}, 0.15f, 0.8f).SetEase(Ease.InOutBack).OnUpdate(delegate
		{
			this.effectMaterial.SetFloat("_Magnitude", this.magnitude);
		}).OnComplete(delegate
		{
			DOTween.To(() => this.magnitude, delegate(float x)
			{
				this.magnitude = x;
			}, 0f, 0.5f).SetEase(Ease.OutElastic).OnUpdate(delegate
			{
				this.effectMaterial.SetFloat("_Magnitude", this.magnitude);
			}).OnComplete(delegate
			{
				base.enabled = false;
			});
		});
	}

	[SerializeField]
	private Material effectMaterial;

	private float magnitude;
}
