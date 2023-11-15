using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CounterBehaviour : MonoBehaviour
{
	private void Start()
	{
		CounterBehaviour.instance = this;
	}

	public void UpdateCount(float addedValue)
	{
		DOTween.Kill(base.transform, false);
		base.transform.localScale = Vector2.one;
		this.count += addedValue;
		string text = "$" + this.count + ".00";
		this.countLabel.text = text;
		base.transform.DOPunchScale(new Vector3(0.1f, 0.3f, 0f), 0.2f, 10, 1f);
	}

	private void OnDestroy()
	{
		if (base.transform != null)
		{
			base.transform.DOKill(false);
		}
	}

	public static CounterBehaviour instance;

	public Text countLabel;

	public float count;
}
