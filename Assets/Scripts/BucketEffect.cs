using System;
using DG.Tweening;
using UnityEngine;

public class BucketEffect : MonoBehaviour
{
	private void Start()
	{
		BucketEffect.instance = this;
		this.bucketScalePunch = base.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 10, 1f).SetId("Bucket").SetAutoKill(false);
		DOTween.Pause("Bucket");
	}

	public void Splash()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.waterSplash, base.transform);
		gameObject.transform.localPosition = new Vector3(-0.02f, 0.1f, -1f);
		DOTween.Restart("Bucket", true, -1f);
	}

	private void OnDestroy()
	{
		if (this.bucketScalePunch != null)
		{
			this.bucketScalePunch.Kill(false);
		}
	}

	[SerializeField]
	private GameObject waterSplash;

	private Tween bucketScalePunch;

	public static BucketEffect instance;
}
