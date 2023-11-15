using System;
using System.Collections.Generic;

namespace SoftMasking
{
	internal struct ClearListAtExit<T> : IDisposable
	{
		public ClearListAtExit(List<T> list)
		{
			this._list = list;
		}

		public void Dispose()
		{
			this._list.Clear();
		}

		private List<T> _list;
	}
}
