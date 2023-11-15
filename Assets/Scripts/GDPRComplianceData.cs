using System;
using System.Diagnostics;
using UnityEngine;

public static class GDPRComplianceData
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action OnConsentChanged;

	public static bool HasAdConsent
	{
		get
		{
			return PlayerPrefs.GetInt("KEY_HAS_AD_CONSENT", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("KEY_HAS_AD_CONSENT", (!value) ? 0 : 1);
		}
	}

	public static bool HasAnalyticConsent
	{
		get
		{
			return PlayerPrefs.GetInt("KEY_HAS_ANALYTICS_CONSENT", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("KEY_HAS_ANALYTICS_CONSENT", (!value) ? 0 : 1);
		}
	}

	public static bool HasSeenConsentDialogBefore
	{
		get
		{
			return PlayerPrefs.GetInt("KEY_HAS_SEEN_CONSENT_DIALOG_BEFORE", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("KEY_HAS_SEEN_CONSENT_DIALOG_BEFORE", (!value) ? 0 : 1);
		}
	}

	public static void SetAdConsent(bool consent)
	{
		GDPRComplianceData.HasAdConsent = consent;
	}

	public static void SetAnalyticConsent(bool consent)
	{
		GDPRComplianceData.HasAnalyticConsent = consent;
	}

	public static void Apply(bool acceptAll)
	{
		if (acceptAll)
		{
			GDPRComplianceData.HasAdConsent = true;
			GDPRComplianceData.HasAnalyticConsent = true;
		}
		if (GDPRComplianceData.HasAdConsent)
		{
			UnityEngine.Debug.Log("ABC: Granting ad consent...");
			
		}
		else
		{
			UnityEngine.Debug.Log("ABC: Revoking ad consent...");
			
		}
		if (GDPRComplianceData.HasAnalyticConsent)
		{
			UnityEngine.Debug.Log("ABC: Granting analytics consent...");

			UnityEngine.Debug.Log("ABC: Analytics consent is now: " + GDPRComplianceData.HasAnalyticConsent);
		}
		else
		{
			UnityEngine.Debug.Log("ABC: Revoking analytics consent...");

			UnityEngine.Debug.Log("ABC: Analytics consent is now: " + GDPRComplianceData.HasAnalyticConsent);
		}
		if (GDPRComplianceData.OnConsentChanged != null)
		{
			GDPRComplianceData.OnConsentChanged();
		}
	}

	private const string KEY_HAS_ANALYTICS_CONSENT = "KEY_HAS_ANALYTICS_CONSENT";

	private const string KEY_HAS_AD_CONSENT = "KEY_HAS_AD_CONSENT";

	private const string KEY_HAS_SEEN_CONSENT_DIALOG_BEFORE = "KEY_HAS_SEEN_CONSENT_DIALOG_BEFORE";
}
