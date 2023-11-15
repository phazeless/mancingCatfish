using System;
using System.Collections.Generic;

public class BasicObjectPool<T>
{
	public BasicObjectPool(Func<T> instantiator) : this(instantiator, 0)
	{
	}

	public BasicObjectPool(Func<T> instantiator, int initialSize)
	{
		this.instantiator = instantiator;
		for (int i = 0; i < initialSize; i++)
		{
			this.ReturnObject(instantiator());
		}
	}

	public int Count
	{
		get
		{
			return this.objects.Count;
		}
	}

	public T GetObject()
	{
		if (this.objects.Count == 0)
		{
			return this.instantiator();
		}
		return this.objects.Pop();
	}

	public void ReturnObject(T poolObject)
	{
		if (!this.IsInPool(poolObject))
		{
			this.objects.Push(poolObject);
		}
	}

	public void RemoveObject(T obj)
	{
		T[] array = this.objects.ToArray();
		this.objects.Clear();
		foreach (T t in array)
		{
			if (!EqualityComparer<T>.Default.Equals(t, obj))
			{
				this.objects.Push(t);
			}
		}
	}

	public bool IsInPool(T poolObject)
	{
		return this.objects.Contains(poolObject);
	}

	private Func<T> instantiator;

	private Stack<T> objects = new Stack<T>();
}
