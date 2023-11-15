using System;
using UnityEngine;

public class DeactivatorByDate : MonoBehaviour
{
	private void Start()
	{
		if (!this.holidayOfferAvailability.IsAvailableAtThisTime)
		{
			base.gameObject.SetActive(false);
		}
	}

	[SerializeField]
	private HolidayOfferAvailability holidayOfferAvailability;
}
