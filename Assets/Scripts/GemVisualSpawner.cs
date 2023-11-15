using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GemVisualSpawner : MonoBehaviour
{
	public void Spawn(int count, Transform targetPosition, ResourceChangeData gemChangeData)
	{
		base.transform.DOMove(targetPosition.position, 1f, false).SetEase(Ease.InBack).OnComplete(delegate
		{
			targetPosition.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.3f, 10, 1f);
			ResourceManager.Instance.GiveGems(count, gemChangeData);
			this.TweenKiller();
			UnityEngine.Object.Destroy(this.gameObject);
		});
		int num = 0;
		while (num < count && num < 20)
		{
			GameObject gemInstance = UnityEngine.Object.Instantiate<GameObject>(this.gemPrefab, base.gameObject.transform, false);
			gemInstance.transform.localPosition = Vector2.zero;
			gemInstance.transform.DOPunchScale(new Vector3(1f, 1f, 0.2f), 1f, 10, 1f);
			gemInstance.transform.DORotate(new Vector3(0f, 0f, (float)UnityEngine.Random.Range(0, 360)), 1f, RotateMode.Fast).SetEase(Ease.OutBack).OnComplete(delegate
			{
				gemInstance.transform.DOKill(false);
				UnityEngine.Object.Destroy(gemInstance.gameObject);
			});
			Image circleInstance = UnityEngine.Object.Instantiate<Image>(this.circlePrefabImage, base.gameObject.transform, false);
			circleInstance.transform.localScale = Vector3.zero;
			circleInstance.transform.DOScale(Vector3.one, 0.2f);
			circleInstance.DOFade(0f, 0.2f).SetEase(Ease.Linear).OnComplete(delegate
			{
				circleInstance.transform.DOKill(false);
				UnityEngine.Object.Destroy(circleInstance.gameObject);
			});
			gemInstance.transform.DOLocalMove(new Vector3(UnityEngine.Random.Range(-200f, 200f), UnityEngine.Random.Range(-200f, 200f), 0f), 0.5f, false).SetLoops(2, LoopType.Yoyo);
			this.tweeningTransforms.Add(gemInstance.transform);
			num++;
		}
	}

	public void TweenKiller()
	{
		for (int i = 0; i < this.tweeningTransforms.Count; i++)
		{
			this.tweeningTransforms[i].DOKill(true);
		}
		base.transform.DOKill(true);
	}

	[SerializeField]
	private GameObject gemPrefab;

	[SerializeField]
	private Image circlePrefabImage;

	private List<Transform> tweeningTransforms = new List<Transform>();
}
