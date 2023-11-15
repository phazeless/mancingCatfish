using System;

namespace CloudOnce
{
	public class CloudRequestResult<T> where T : new()
	{
		public CloudRequestResult(T result)
		{
			this.Error = string.Empty;
			this.Result = result;
		}

		public CloudRequestResult(T result, string error)
		{
			this.Error = error;
			this.Result = result;
		}

		public string Error { get; private set; }

		public T Result { get; private set; }

		public bool HasError
		{
			get
			{
				return !string.IsNullOrEmpty(this.Error);
			}
		}
	}
}
