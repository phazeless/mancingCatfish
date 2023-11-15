using System;

namespace Soomla.Store
{
	public class RestoreTransactionsFinishedEvent : SoomlaEvent
	{
		public RestoreTransactionsFinishedEvent(bool success) : this(success, null)
		{
		}

		public RestoreTransactionsFinishedEvent(bool success, object sender) : base(sender)
		{
			this.mSuccess = success;
		}

		public bool isSuccess()
		{
			return this.mSuccess;
		}

		private bool mSuccess;
	}
}
