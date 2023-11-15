using System;
using System.Collections;
using System.Threading;

namespace UnityThreading
{
	internal sealed class TaskWorker : ThreadBase
	{
		public TaskWorker(string name, TaskDistributor taskDistributor) : base(name, false)
		{
			this.TaskDistributor = taskDistributor;
			this.Dispatcher = new Dispatcher(false);
		}

		public TaskDistributor TaskDistributor { get; private set; }

		public bool IsWorking
		{
			get
			{
				return this.Dispatcher.IsWorking;
			}
		}

		protected override IEnumerator Do()
		{
			while (!this.exitEvent.InterWaitOne(0))
			{
				if (!this.Dispatcher.ProcessNextTask())
				{
					this.TaskDistributor.FillTasks(this.Dispatcher);
					if (this.Dispatcher.TaskCount == 0)
					{
						if (WaitHandle.WaitAny(new WaitHandle[]
						{
							this.exitEvent,
							this.TaskDistributor.NewDataWaitHandle
						}) == 0)
						{
							return null;
						}
						this.TaskDistributor.FillTasks(this.Dispatcher);
					}
				}
			}
			return null;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (this.Dispatcher != null)
			{
				this.Dispatcher.Dispose();
			}
			this.Dispatcher = null;
		}

		public Dispatcher Dispatcher;
	}
}
