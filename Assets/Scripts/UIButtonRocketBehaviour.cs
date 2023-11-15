using System;
using DG.Tweening;
using UnityEngine;

public class UIButtonRocketBehaviour : MonoBehaviour
{
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		this.startPosition = this.rectTransform.anchoredPosition;
		this.startRotation = base.transform.localEulerAngles;
	}

	public void Shoot()
	{
		this.isShooting = true;
		this.TranslationTween(0.07f);
		base.transform.DORotate(new Vector3(0f, 0f, base.transform.localScale.x * UnityEngine.Random.Range(-40f, 5f)), 0.3f, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
		this.particleSystem.Play();
	}

	public void ReturnToOrigin()
	{
		this.isShooting = false;
		this.TweenKiller();
		this.rectTransform.anchoredPosition = this.startPosition;
		base.transform.localEulerAngles = this.startRotation;
		this.particleSystem.Stop();
	}

	private void TranslationTween(float speed = 0.07f)
	{
		if (!this.isShooting)
		{
			return;
		}
		this.rectTransform.DOMove(base.transform.GetChild(0).transform.position, 0.1f, false).SetEase(Ease.Linear).OnComplete(delegate
		{
			this.TranslationTween(0.1f);
		});
	}

	private void TweenKiller()
	{
		this.rectTransform.DOKill(false);
		base.transform.DOKill(false);
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	private Vector2 startPosition;

	private Vector3 startRotation;

	private RectTransform rectTransform;

	[SerializeField]
	private ParticleSystem particleSystem;

	private bool isShooting;
}
