using System;

namespace CloudOnce.Internal
{
	public enum KvStoreChangeReason
	{
		ServerChange,
		InitialSyncChange,
		QuotaViolationChange,
		AccountChange
	}
}
