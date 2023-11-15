using System;
using DG.Tweening;
using UnityEngine;

public class FireworkBehaviour : MonoBehaviour
{
	private void Start()
	{
		base.transform.localScale = Vector3.one * (CameraMovement.Instance.Zoom * 0.3f);
		this.ps1 = this.explosions[UnityEngine.Random.Range(0, this.explosions.Length)];
		this.ps1.gameObject.SetActive(true);
		SkillManager.Instance.OnSkillAttributeValueChanged += this.Instance_OnSkillAttributeValueChanged;
		this.fishToSpawnFromFirework = (int)SkillManager.Instance.GetCurrentTotalValueFor<Skills.FireworkFishAmount>();
	}

	private void Instance_OnSkillAttributeValueChanged(ISkillAttribute attr, float val)
	{
		if (attr is Skills.FireworkFishAmount)
		{
			this.fishToSpawnFromFirework = (int)val;
		}
	}

	public void AnimateFirework(Vector2 target)
	{
		this.targetPosition = target;
		this.ps0.Play();
		AudioManager.Instance.OneShooter(this.launchSound[UnityEngine.Random.Range(0, this.launchSound.Length)], UnityEngine.Random.Range(0.2f, 0.8f));
		base.transform.DOMove(new Vector3(this.targetPosition.x, this.targetPosition.y, 90f), 0.5f, false).OnComplete(delegate
		{
			this.ps0.Stop();
			this.ps1.Play();
			AudioManager.Instance.OneShooter(this.bangSound[UnityEngine.Random.Range(0, this.launchSound.Length)], UnityEngine.Random.Range(0.2f, 0.8f));
			this.hasReachedDestination = true;
			this.fishCatcher.SetActive(true);
		});
	}

	private void Update()
	{
		if (!this.hasReachedDestination)
		{
			return;
		}
		if (this.currentDelay < this.delay)
		{
			this.currentDelay++;
			return;
		}
		if (this.currentSpawns < this.fishToSpawnFromFirework)
		{
			this.spawner.Spawn();
			this.currentSpawns++;
		}
		else if (!this.hasTriggerSelfDestruct)
		{
			this.hasTriggerSelfDestruct = true;
			this.RunAfterDelay(2f, delegate()
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
		}
	}

	private void OnDestroy()
	{
		SkillManager.Instance.OnSkillAttributeValueChanged -= this.Instance_OnSkillAttributeValueChanged;
		base.transform.DOKill(false);
	}

	[SerializeField]
	private ParticleSystem ps0;

	[SerializeField]
	private ParticleSystem ps1;

	[SerializeField]
	private ParticleSystem[] explosions;

	[SerializeField]
	private FireworkSpawner spawner;

	[SerializeField]
	private AudioClip[] launchSound;

	[SerializeField]
	private AudioClip[] bangSound;

	[SerializeField]
	private GameObject fishCatcher;

	private Vector2 targetPosition;

	[SerializeField]
	private int delay = 20;

	private int currentSpawns;

	private int currentDelay;

	private bool hasReachedDestination;

	private bool hasTriggerSelfDestruct;

	private int fishToSpawnFromFirework;
}
