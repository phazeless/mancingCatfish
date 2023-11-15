using System;

namespace Soomla.Store
{
	public class SoomlaStoreInitializedEvent : SoomlaEvent
	{
		public SoomlaStoreInitializedEvent() : this(null)
		{
		}

		public SoomlaStoreInitializedEvent(object sender) : base(sender)
		{
		}
	}
}
