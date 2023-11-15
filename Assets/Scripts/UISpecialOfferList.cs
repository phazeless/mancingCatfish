using System;
using System.Collections.Generic;
using UnityEngine;

public class UISpecialOfferList : MonoBehaviour
{
	private void Awake()
	{
		SpecialOfferManager.Instance.OnSpecialOfferAvailable += this.Instance_OnSpecialOfferAvailable;
		SpecialOfferManager.Instance.OnSpecialOfferDurationEnd += this.Instance_OnSpecialOfferDurationEnd;
	}

	private void Instance_OnSpecialOfferDurationEnd(SpecialOffer specialOffer)
	{
		UISpecialOfferItem uispecialOfferItem = this.activeSpecialOfferItems.Find((UISpecialOfferItem x) => x.SpecialOffer == specialOffer);
		if (uispecialOfferItem != null)
		{
			this.activeSpecialOfferItems.Remove(uispecialOfferItem);
			UnityEngine.Object.Destroy(uispecialOfferItem.gameObject);
		}
	}

	private void Instance_OnSpecialOfferAvailable(SpecialOffer specialOffer, bool isNew)
	{
		UISpecialOfferItem uispecialOfferItem = UnityEngine.Object.Instantiate<UISpecialOfferItem>(this.prefabSpecialOffer, base.transform, false);
		uispecialOfferItem.SetSpecialOffer(specialOffer);
		uispecialOfferItem.transform.SetSiblingIndex(this.specialOfferPosition.GetSiblingIndex());
		this.activeSpecialOfferItems.Add(uispecialOfferItem);
	}

	[SerializeField]
	private UISpecialOfferItem prefabSpecialOffer;

	[SerializeField]
	private Transform specialOfferPosition;

	private List<UISpecialOfferItem> activeSpecialOfferItems = new List<UISpecialOfferItem>();
}
