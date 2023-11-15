using System;
using ACE.Notifications;

public class PushwooshPushProvider : BasePushProvider<PushwooshPushProviderSettings>
{
	public PushwooshPushProvider(IPushProviderSettings settings) : base(settings)
	{
	}

	protected override void OnInitialize()
	{
		Pushwoosh.ApplicationCode = base.PushProviderSettings.ApplicationCode;
		Pushwoosh.GcmProjectNumber = base.PushProviderSettings.GcmProjectNumber;
	
	}

	protected override ILocalNotificationCapable GetLocalNotificationImplementation()
	{
		return new PushwooshPushProvider.PushwooshAndroidLocalNotification();
	}

	private class PushwooshAndroidLocalNotification : ILocalNotificationCapable
	{
		public void ClearLocalNotifications()
		{
			
		}

		public void HandleReceivedNotifications(Action<string> payloadCallback)
		{
		}

		public void ScheduleLocalNotification(string message, DateTime date)
		{
			this.ScheduleLocalNotification(message, date, string.Empty);
		}

		public void ScheduleLocalNotification(string message, DateTime date, string payload)
		{
			
		}
	}
}
