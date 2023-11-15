using System;

namespace ACE.Notifications
{
	public class DefaultLocalNotificationUWP : ILocalNotificationCapable
	{
		public void ScheduleLocalNotification(string message, DateTime fireDate)
		{
			this.ScheduleLocalNotification(message, fireDate, string.Empty);
		}

		public void ScheduleLocalNotification(string message, DateTime fireDate, string payload)
		{
		}

		public void ClearLocalNotifications()
		{
		}

		public void HandleReceivedNotifications(Action<string> payloadCallback)
		{
		}
	}
}
