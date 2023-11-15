using System;
using ACE.Notifications;
using UnityEngine;

public class PushwooshPushProviderSettings : MonoBehaviour, IPushProviderSettings
{
	public string ApplicationCode;

	public string GcmProjectNumber;
}
