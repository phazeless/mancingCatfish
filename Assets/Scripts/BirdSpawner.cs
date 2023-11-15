using System;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
	private void Start()
	{
		if (this.pooledBirds.Count < 3)
		{
			for (int i = 0; i < 3; i++)
			{
				Transform transform = UnityEngine.Object.Instantiate<Transform>(this.bird, base.transform, false);
				transform.GetComponent<BirdBehaviour>().spawner = this;
				this.pooledBirds.Add(transform);
			}
		}
		base.InvokeRepeating("TrySpawnBird", 5f, 25f);
	}

	public void PoolBird(Transform bird)
	{
		this.pooledBirds.Add(bird);
		bird.gameObject.SetActive(false);
	}

	private void SpawnBird()
	{
		if (this.pooledBirds.Count > 0)
		{
			Transform transform = this.pooledBirds[0];
			this.pooledBirds.RemoveAt(0);
			transform.gameObject.SetActive(true);
			transform.GetChild(0).gameObject.SetActive(true);
		}
	}

	private void TrySpawnBird()
	{
		int num = UnityEngine.Random.Range(0, 10);
		if (num < 5)
		{
			return;
		}
		if (num < 8)
		{
			this.SpawnBird();
			this.audioSource.PlayOneShot(this.seagullSingle);
		}
		else
		{
			this.SpawnBird();
			this.audioSource.PlayOneShot(this.SeagullMulti);
			this.RunAfterDelay(0.3f, delegate()
			{
				this.SpawnBird();
			});
			this.RunAfterDelay(0.6f, delegate()
			{
				this.SpawnBird();
			});
		}
	}

	[SerializeField]
	private Transform bird;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip seagullSingle;

	[SerializeField]
	private AudioClip SeagullMulti;

	private List<Transform> pooledBirds = new List<Transform>();
}
