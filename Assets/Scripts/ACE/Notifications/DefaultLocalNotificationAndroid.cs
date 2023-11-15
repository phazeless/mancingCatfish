using System;

namespace ACE.Notifications
{
	public class DefaultLocalNotificationAndroid : ILocalNotificationCapable
	{
		public void ClearLocalNotifications()
		{
			throw new NotImplementedException();
		}

		public void HandleReceivedNotifications(Action<string> payloadCallback)
		{
			throw new NotImplementedException();
		}

		public void ScheduleLocalNotification(string message, DateTime date)
		{
			throw new NotImplementedException();
		}

		public void ScheduleLocalNotification(string message, DateTime date, string payload)
		{
			throw new NotImplementedException();
		}
	}
}
