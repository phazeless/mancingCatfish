using System;

namespace Soomla.Store
{
	public class UnexpectedStoreErrorEvent : SoomlaEvent
	{
		public UnexpectedStoreErrorEvent(UnexpectedStoreErrorEvent.ErrorCode errorCode) : this(errorCode, null)
		{
		}

		public UnexpectedStoreErrorEvent(int errorCode) : this((UnexpectedStoreErrorEvent.ErrorCode)errorCode, null)
		{
		}

		public UnexpectedStoreErrorEvent() : this(UnexpectedStoreErrorEvent.ErrorCode.GENERAL, null)
		{
		}

		public UnexpectedStoreErrorEvent(UnexpectedStoreErrorEvent.ErrorCode errorCode, object sender) : base(sender)
		{
			this.errorCode = errorCode;
		}

		public UnexpectedStoreErrorEvent.ErrorCode getErrorCode()
		{
			return this.errorCode;
		}

		private readonly UnexpectedStoreErrorEvent.ErrorCode errorCode;

		public enum ErrorCode
		{
			GENERAL,
			VERIFICATION_TIMEOUT,
			VERIFICATION_FAIL,
			PURCHASE_FAIL
		}
	}
}
