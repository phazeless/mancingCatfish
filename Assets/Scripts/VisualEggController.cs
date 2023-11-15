using System;
using DG.Tweening;
using UnityEngine;

public class VisualEggController : MonoBehaviour
{
	private void OnEnable()
	{
		base.transform.DOPunchRotation(new Vector3(0f, 0f, 20f), 3f, 6, 0.6f);
		base.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 1f, 5, 0.6f);
		AudioManager.Instance.OneShooter(this.appearSound, 1f);
	}

	private void OnDestroy()
	{
		base.transform.DOKill(false);
	}

	[SerializeField]
	private AudioClip appearSound;
}
