using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pushwoosh : MonoBehaviour
{
	protected Pushwoosh()
	{
	}

	public static string ApplicationCode { get; set; }

	public static string GcmProjectNumber { get; set; }

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Pushwoosh.RegistrationSuccessHandler OnRegisteredForPushNotifications;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Pushwoosh.RegistrationErrorHandler OnFailedToRegisteredForPushNotifications;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Pushwoosh.NotificationHandler OnPushNotificationsReceived;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Pushwoosh.NotificationHandler OnPushNotificationsOpened;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Pushwoosh.GdprSuccessHandler OnSetCommunicationEnable;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Pushwoosh.GdprErrorHandler OnFailedSetCommunicationEnable;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Pushwoosh.GdprSuccessHandler OnRemoveAllData;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Pushwoosh.GdprErrorHandler OnFailedRemoveAllData;



	public virtual string HWID
	{
		get
		{
			UnityEngine.Debug.Log("[Pushwoosh] Error: HWID is not supported on this platform");
			return "Unsupported platform";
		}
	}

	public virtual string PushToken
	{
		get
		{
			UnityEngine.Debug.Log("[Pushwoosh] Error: PushToken is not supported on this platform");
			return "Unsupported platform";
		}
	}

	public virtual void RegisterForPushNotifications()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: RegisterForPushNotifications is not supported on this platform");
	}

	public virtual void UnregisterForPushNotifications()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: UnregisterForPushNotifications is not supported on this platform");
	}

	public virtual void StartTrackingGeoPushes()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: StartTrackingGeoPushes is not supported on this platform");
	}

	public virtual void StopTrackingGeoPushes()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: StopTrackingGeoPushes is not supported on this platform");
	}

	public virtual void SetIntTag(string tagName, int tagValue)
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: SetIntTag is not supported on this platform");
	}

	public virtual void SetStringTag(string tagName, string tagValue)
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: SetStringTag is not supported on this platform");
	}

	public virtual void SetListTag(string tagName, List<object> tagValues)
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: SetListTag is not supported on this platform");
	}

	public virtual void GetTags(Pushwoosh.GetTagsHandler handler)
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: GetTags() is not supported on this platform");
	}

	public virtual void ClearNotificationCenter()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: ClearNotificationCenter is not supported on this platform");
	}

	public virtual void SetBadgeNumber(int number)
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: SetBadgeNumber is not supported on this platform");
	}

	public virtual void AddBadgeNumber(int deltaBadge)
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: AddBadgeNumber is not supported on this platform");
	}

	public virtual void SetUserId(string userId)
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: SetUserId is not supported on this platform");
	}

	public virtual void PostEvent(string eventId, IDictionary<string, object> attributes)
	{
		string attributes2 = PushwooshUtils.DictionaryToJson(attributes);
		this.PostEventInternal(eventId, attributes2);
	}

	public virtual void SendPurchase(string productId, double price, string currency)
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: SendPurchase is not supported on this platform");
	}

	protected virtual void PostEventInternal(string eventId, string attributes)
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: PostEvent is not supported on this platform");
	}

	protected void RegisteredForPushNotifications(string token)
	{
		this.OnRegisteredForPushNotifications(token);
	}

	protected void FailedToRegisteredForPushNotifications(string error)
	{
		this.OnFailedToRegisteredForPushNotifications(error);
	}

	protected void PushNotificationsReceived(string payload)
	{
		this.OnPushNotificationsReceived(payload);
	}

	protected void PushNotificationsOpened(string payload)
	{
		this.OnPushNotificationsOpened(payload);
	}

	public virtual void ShowGDPRConsentUI()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: ShowGDPRConsentUI is not supported on this platform");
	}

	public virtual void ShowGDPRDeletionUI()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: ShowGDPRDeletionUI is not supported on this platform");
	}

	public virtual bool IsCommunicationEnabled()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: IsCommunicationEnabled is not supported on this platform");
		return false;
	}

	public virtual bool isDeviceDataRemoved()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: isDeviceDataRemoved is not supported on this platform");
		return false;
	}

	public virtual bool IsGDPRAvailable()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: IsAvailable is not supported on this platform");
		return false;
	}

	public virtual void SetCommunicationEnabled(bool enable)
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: SetCommunicationEnabled is not supported on this platform");
	}

	public virtual void RemoveAllDeviceData()
	{
		UnityEngine.Debug.Log("[Pushwoosh] Error: RemoveAllDeviceData is not supported on this platform");
	}

	protected void SetCommunicationEnableCallBack()
	{
		this.OnSetCommunicationEnable();
	}

	protected void FailedSetCommunicationEnableCallback(string error)
	{
		this.OnFailedSetCommunicationEnable(error);
	}

	protected void RemoveAllDataCallBack()
	{
		this.OnRemoveAllData();
	}

	protected void FailedRemoveAllDataCallback(string error)
	{
		this.OnFailedRemoveAllData(error);
	}

	protected virtual void Initialize()
	{
	}

	

	public void OnDestroy()
	{
		Pushwoosh.applicationIsQuitting = true;
	}

	

	private static object _lock = new object();

	private static bool applicationIsQuitting = false;

	public delegate void RegistrationSuccessHandler(string token);

	public delegate void RegistrationErrorHandler(string error);

	public delegate void GdprSuccessHandler();

	public delegate void GdprErrorHandler(string error);

	public delegate void NotificationHandler(string payload);

	public delegate void GetTagsHandler(IDictionary<string, object> tags, PushwooshException error);
}
