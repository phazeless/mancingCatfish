using System;

namespace UnityThreading
{
	public enum TaskSortingSystem
	{
		NeverReorder,
		ReorderWhenAdded,
		ReorderWhenExecuted
	}
}
