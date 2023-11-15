using System;
using ACE.IAPS;
using DG.Tweening;
using UnityEngine;

public class PurchaseItemButton : MonoBehaviour
{
	public string Id
	{
		get
		{
			return this.id;
		}
	}

	public void OnClicked()
	{
		foreach (Transform target in this.transformToTween)
		{
			target.DOKill(true);
			target.DOPunchScale(new Vector3(0.05f, 0.1f, 0f), 0.5f, 10, 1f);
		}
		if (!string.IsNullOrEmpty(this.id))
		{
			if (ResourceManager.Instance.IsMarketItem(this.id))
			{
				UIIAPPendingBlocker.Instance.Show();
			}
			ResourceManager.Instance.Buy(this.id, delegate(PurchaseResult resp, string b)
			{
				if (resp == PurchaseResult.ItemPurchased)
				{
					if (ResourceManager.Instance.IsMarketItem(this.id))
					{
						UnityEngine.Debug.LogWarning("User just Purchased: " + this.id);
					}
				}
				else if (resp == PurchaseResult.InsufficientFunds)
				{
				}
				UIIAPPendingBlocker.Instance.Hide();
			});
		}
	}

	private void TweenKiller()
	{
		foreach (Transform target in this.transformToTween)
		{
			target.DOKill(true);
		}
	}

	private void OnDestroy()
	{
		this.TweenKiller();
	}

	[SerializeField]
	private string id;

	[SerializeField]
	private Transform[] transformToTween;
}
