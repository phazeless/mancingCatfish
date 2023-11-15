using System;
using UnityEngine;

public class GDPRWarningButton : MonoBehaviour
{
	private void Start()
	{
		GDPRComplianceData.OnConsentChanged += this.GDPRComplianceData_OnConsentChanged;
		this.CheckConsent();
	}

	private void GDPRComplianceData_OnConsentChanged()
	{
		this.CheckConsent();
	}

	public void ActivateGdprDialog()
	{
		this.gdprHandler.StartGdprSequence(2);
	}

	private void CheckConsent()
	{
		if (GDPRComplianceData.HasAdConsent && GDPRComplianceData.HasAnalyticConsent)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.SetActive(true);
		}
	}

	private void OnDestroy()
	{
		GDPRComplianceData.OnConsentChanged -= this.GDPRComplianceData_OnConsentChanged;
	}

	[SerializeField]
	private GDPRHandler gdprHandler;
}
