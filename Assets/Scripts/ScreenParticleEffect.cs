using System;
using DG.Tweening;
using UnityEngine;

public class ScreenParticleEffect : MonoBehaviour
{
	private void Start()
	{
		ScreenParticleEffect.Instance = this;
	}

	public void StarSwipe(float duration = 1f, float revolutions = 1f)
	{
		this.starEmitter.Play();
		int revs = (int)revolutions;
		float dur = duration * 0.25f / revolutions;
		this.edgeHolder.DOAnchorPosX(this.rectTransform.rect.width, dur, false).SetEase(Ease.Linear).OnComplete(delegate
		{
			this.edgeHolder.DOAnchorPosY(this.rectTransform.rect.height, dur, false).SetEase(Ease.Linear).OnComplete(delegate
			{
				this.edgeHolder.DOAnchorPosX(0f, dur, false).SetEase(Ease.Linear).OnComplete(delegate
				{
					this.edgeHolder.DOAnchorPosY(0f, dur, false).SetEase(Ease.Linear).OnComplete(delegate
					{
						if (revs > 1)
						{
							revs--;
							this.StarSwipe(dur * 4f, (float)revs);
						}
						else
						{
							this.starEmitter.Stop();
						}
					});
				});
			});
		});
	}

	private void OnDestroy()
	{
		this.edgeHolder.DOKill(false);
	}

	public static ScreenParticleEffect Instance;

	[SerializeField]
	private RectTransform edgeHolder;

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private ParticleSystem starEmitter;
}
