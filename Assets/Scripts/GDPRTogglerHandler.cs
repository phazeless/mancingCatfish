using System;
using UnityEngine;

public class GDPRTogglerHandler : MonoBehaviour
{
	private void Start()
	{
		this.UpdateToggles();
	}

	public void ToggleCompliance(bool isAd)
	{
		if (isAd)
		{
			GDPRComplianceData.SetAdConsent(!GDPRComplianceData.HasAdConsent);
		}
		if (!isAd)
		{
			GDPRComplianceData.SetAnalyticConsent(!GDPRComplianceData.HasAnalyticConsent);
		}
		this.UpdateToggles();
	}

	private void OnEnable()
	{
		this.UpdateToggles();
	}

	private void UpdateToggles()
	{
		if (GDPRComplianceData.HasAnalyticConsent)
		{
			this.analyticsConsent.SetOn();
		}
		else
		{
			this.analyticsConsent.SetOff();
		}
		if (GDPRComplianceData.HasAdConsent)
		{
			this.adConsent.SetOn();
		}
		else
		{
			this.adConsent.SetOff();
		}
		this.buttonSwitch.Toggle(0);
	}

	[SerializeField]
	private ToggleButtonBehaviour analyticsConsent;

	[SerializeField]
	private ToggleButtonBehaviour adConsent;

	[SerializeField]
	private InactiveGDPRButton buttonSwitch;
}
