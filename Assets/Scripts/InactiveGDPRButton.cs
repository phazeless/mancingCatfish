using System;
using UnityEngine;

public class InactiveGDPRButton : MonoBehaviour
{
	public void Toggle(int nr)
	{
		this.CheckIfDisabled();
	}

	private void Start()
	{
		this.CheckIfDisabled();
	}

	private void CheckIfDisabled()
	{
		if (GDPRComplianceData.HasAnalyticConsent && GDPRComplianceData.HasAdConsent)
		{
			base.gameObject.SetActive(false);
			this.objectToReplace.SetActive(true);
		}
		else
		{
			base.gameObject.SetActive(true);
			this.objectToReplace.SetActive(false);
		}
	}

	[SerializeField]
	private GameObject objectToReplace;
}
