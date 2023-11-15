using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityThreading;

[ExecuteInEditMode]
public class UnityThreadHelper : MonoBehaviour
{
	public static void EnsureHelper()
	{
		object obj = UnityThreadHelper.syncRoot;
		lock (obj)
		{
			if (UnityThreadHelper.instance == null)
			{
				UnityThreadHelper.instance = (UnityEngine.Object.FindObjectOfType(typeof(UnityThreadHelper)) as UnityThreadHelper);
				if (UnityThreadHelper.instance == null)
				{
					UnityThreadHelper.instance = new GameObject("[UnityThreadHelper]")
					{
						hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable)
					}.AddComponent<UnityThreadHelper>();
					UnityThreadHelper.instance.EnsureHelperInstance();
				}
			}
		}
	}

	private static UnityThreadHelper Instance
	{
		get
		{
			UnityThreadHelper.EnsureHelper();
			return UnityThreadHelper.instance;
		}
	}

	public static Dispatcher Dispatcher
	{
		get
		{
			return UnityThreadHelper.Instance.CurrentDispatcher;
		}
	}

	public static TaskDistributor TaskDistributor
	{
		get
		{
			return UnityThreadHelper.Instance.CurrentTaskDistributor;
		}
	}

	public Dispatcher CurrentDispatcher
	{
		get
		{
			return this.dispatcher;
		}
	}

	public TaskDistributor CurrentTaskDistributor
	{
		get
		{
			return this.taskDistributor;
		}
	}

	private void EnsureHelperInstance()
	{
		this.dispatcher = (Dispatcher.MainNoThrow ?? new Dispatcher());
		this.taskDistributor = (TaskDistributor.MainNoThrow ?? new TaskDistributor("TaskDistributor"));
	}

	public static ActionThread CreateThread(Action<ActionThread> action, bool autoStartThread)
	{
		UnityThreadHelper.Instance.EnsureHelperInstance();
		Action<ActionThread> action2 = delegate(ActionThread currentThread)
		{
			try
			{
				action(currentThread);
			}
			catch (Exception message)
			{
				UnityEngine.Debug.LogError(message);
			}
		};
		ActionThread actionThread = new ActionThread(action2, autoStartThread);
		UnityThreadHelper.Instance.RegisterThread(actionThread);
		return actionThread;
	}

	public static ActionThread CreateThread(Action<ActionThread> action)
	{
		return UnityThreadHelper.CreateThread(action, true);
	}

	public static ActionThread CreateThread(Action action, bool autoStartThread)
	{
		return UnityThreadHelper.CreateThread(delegate(ActionThread thread)
		{
			action();
		}, autoStartThread);
	}

	public static ActionThread CreateThread(Action action)
	{
		return UnityThreadHelper.CreateThread(delegate(ActionThread thread)
		{
			action();
		}, true);
	}

	public static ThreadBase CreateThread(Func<ThreadBase, IEnumerator> action, bool autoStartThread)
	{
		UnityThreadHelper.Instance.EnsureHelperInstance();
		EnumeratableActionThread enumeratableActionThread = new EnumeratableActionThread(action, autoStartThread);
		UnityThreadHelper.Instance.RegisterThread(enumeratableActionThread);
		return enumeratableActionThread;
	}

	public static ThreadBase CreateThread(Func<ThreadBase, IEnumerator> action)
	{
		return UnityThreadHelper.CreateThread(action, true);
	}

	public static ThreadBase CreateThread(Func<IEnumerator> action, bool autoStartThread)
	{
		Func<ThreadBase, IEnumerator> action2 = (ThreadBase thread) => action();
		return UnityThreadHelper.CreateThread(action2, autoStartThread);
	}

	public static ThreadBase CreateThread(Func<IEnumerator> action)
	{
		Func<ThreadBase, IEnumerator> action2 = (ThreadBase thread) => action();
		return UnityThreadHelper.CreateThread(action2, true);
	}

	private void RegisterThread(ThreadBase thread)
	{
		if (this.registeredThreads.Contains(thread))
		{
			return;
		}
		this.registeredThreads.Add(thread);
	}

	private void OnDestroy()
	{
		foreach (ThreadBase threadBase in this.registeredThreads)
		{
			threadBase.Dispose();
		}
		if (this.dispatcher != null)
		{
			this.dispatcher.Dispose();
		}
		this.dispatcher = null;
		if (this.taskDistributor != null)
		{
			this.taskDistributor.Dispose();
		}
		this.taskDistributor = null;
		if (UnityThreadHelper.instance == this)
		{
			UnityThreadHelper.instance = null;
		}
	}

	private void Update()
	{
		if (this.dispatcher != null)
		{
			this.dispatcher.ProcessTasks();
		}
		ThreadBase[] array = (from thread in this.registeredThreads
		where !thread.IsAlive
		select thread).ToArray<ThreadBase>();
		foreach (ThreadBase threadBase in array)
		{
			threadBase.Dispose();
			this.registeredThreads.Remove(threadBase);
		}
	}

	private static UnityThreadHelper instance = null;

	private static object syncRoot = new object();

	private Dispatcher dispatcher;

	private TaskDistributor taskDistributor;

	private List<ThreadBase> registeredThreads = new List<ThreadBase>();
}
