using System;
using System.Collections.Generic;
using System.Threading;

namespace UnityThreading
{
	public class Channel<T> : IDisposable
	{
		public Channel() : this(1)
		{
		}

		public Channel(int bufferSize)
		{
			if (bufferSize < 1)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "Must be greater or equal to 1.");
			}
			this.BufferSize = bufferSize;
		}

		public int BufferSize { get; private set; }

		~Channel()
		{
			this.Dispose();
		}

		public void Resize(int newBufferSize)
		{
			if (newBufferSize < 1)
			{
				throw new ArgumentOutOfRangeException("newBufferSize", "Must be greater or equal to 1.");
			}
			object obj = this.setSyncRoot;
			lock (obj)
			{
				if (!this.disposed)
				{
					if (WaitHandle.WaitAny(new WaitHandle[]
					{
						this.exitEvent,
						this.getEvent
					}) != 0)
					{
						this.buffer.Clear();
						if (newBufferSize != this.BufferSize)
						{
							this.BufferSize = newBufferSize;
						}
					}
				}
			}
		}

		public bool Set(T value)
		{
			return this.Set(value, int.MaxValue);
		}

		public bool Set(T value, int timeoutInMilliseconds)
		{
			object obj = this.setSyncRoot;
			bool result;
			lock (obj)
			{
				if (this.disposed)
				{
					result = false;
				}
				else
				{
					int num = WaitHandle.WaitAny(new WaitHandle[]
					{
						this.exitEvent,
						this.getEvent
					}, timeoutInMilliseconds);
					if (num == 258 || num == 0)
					{
						result = false;
					}
					else
					{
						this.buffer.Add(value);
						if (this.buffer.Count == this.BufferSize)
						{
							this.setEvent.Set();
							this.getEvent.Reset();
						}
						result = true;
					}
				}
			}
			return result;
		}

		public T Get()
		{
			return this.Get(int.MaxValue, default(T));
		}

		public T Get(int timeoutInMilliseconds, T defaultValue)
		{
			object obj = this.getSyncRoot;
			T result;
			lock (obj)
			{
				if (this.disposed)
				{
					result = defaultValue;
				}
				else
				{
					int num = WaitHandle.WaitAny(new WaitHandle[]
					{
						this.exitEvent,
						this.setEvent
					}, timeoutInMilliseconds);
					if (num == 258 || num == 0)
					{
						result = defaultValue;
					}
					else
					{
						T t = this.buffer[0];
						this.buffer.RemoveAt(0);
						if (this.buffer.Count == 0)
						{
							this.getEvent.Set();
							this.setEvent.Reset();
						}
						result = t;
					}
				}
			}
			return result;
		}

		public void Close()
		{
			object obj = this.disposeRoot;
			lock (obj)
			{
				if (!this.disposed)
				{
					this.exitEvent.Set();
				}
			}
		}

		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			object obj = this.disposeRoot;
			lock (obj)
			{
				this.exitEvent.Set();
				object obj2 = this.getSyncRoot;
				lock (obj2)
				{
					object obj3 = this.setSyncRoot;
					lock (obj3)
					{
						this.setEvent.Close();
						this.setEvent = null;
						this.getEvent.Close();
						this.getEvent = null;
						this.exitEvent.Close();
						this.exitEvent = null;
						this.disposed = true;
					}
				}
			}
		}

		private List<T> buffer = new List<T>();

		private object setSyncRoot = new object();

		private object getSyncRoot = new object();

		private object disposeRoot = new object();

		private ManualResetEvent setEvent = new ManualResetEvent(false);

		private ManualResetEvent getEvent = new ManualResetEvent(true);

		private ManualResetEvent exitEvent = new ManualResetEvent(false);

		private bool disposed;
	}
}
