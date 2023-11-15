using System;

namespace ACE.Notifications
{
	public class DefaultLocalNotificationUnity : ILocalNotificationCapable
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
