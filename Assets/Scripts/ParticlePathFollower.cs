using System;
using DG.Tweening;
using UnityEngine;

public class ParticlePathFollower : MonoBehaviour
{
	private void Start()
	{
		this.follower.DOKill(false);
		this.StartPath();
	}

	private void StartPath()
	{
		this.follower.DOMove(this.pathPoints[this.currentPathIndex % this.pathPoints.Length].position, this.speed, false).SetEase(Ease.Linear).OnComplete(delegate
		{
			this.currentPathIndex++;
			if (this.currentPathIndex >= this.pathPoints.Length)
			{
				this.currentPathIndex = 0;
			}
			this.StartPath();
		});
	}

	private void OnDestroy()
	{
		this.follower.DOKill(false);
	}

	[SerializeField]
	private Transform[] pathPoints;

	[SerializeField]
	private Transform follower;

	[SerializeField]
	private float speed = 0.5f;

	private int currentPathIndex;
}
