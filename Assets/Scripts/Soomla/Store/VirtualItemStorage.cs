using System;

namespace Soomla.Store
{
	public abstract class VirtualItemStorage
	{
		protected virtual int _getBalance(VirtualItem item)
		{
			return 0;
		}

		protected virtual int _setBalance(VirtualItem item, int balance, bool notify)
		{
			return 0;
		}

		protected virtual int _add(VirtualItem item, int amount, bool notify)
		{
			return 0;
		}

		protected virtual int _remove(VirtualItem item, int amount, bool notify)
		{
			return 0;
		}

		protected static string TAG = "SOOMLA VirtualItemStorage";
	}
}
