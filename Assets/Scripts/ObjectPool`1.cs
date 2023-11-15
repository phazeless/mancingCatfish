using System;
using System.Collections.Generic;

public class ObjectPool<T> : IObjectPool<T> where T : IPooledObject<T>
{
	public ObjectPool(Func<object, T> instantiator) : this(instantiator, 0)
	{
	}

	public ObjectPool(Func<object, T> instantiator, int initialSize) : this(instantiator, 0, null)
	{
	}

	public ObjectPool(Func<object, T> instantiator, int initialSize, object metaData)
	{
		this.instantiator = instantiator;
		this.metaData = metaData;
		for (int i = 0; i < initialSize; i++)
		{
			this.ReturnObject(this.OnCreate(instantiator(metaData)));
		}
	}

	public object MetaData
	{
		get
		{
			return this.metaData;
		}
	}

	public int Count
	{
		get
		{
			return this.objects.Count;
		}
	}

	protected virtual T OnCreate(T createdObject)
	{
		createdObject.OnAddedToPool(this);
		return createdObject;
	}

	protected virtual T OnReturned(T returnedObject)
	{
		return returnedObject;
	}

	public T GetObject()
	{
		bool flag = false;
		return this.GetObject(out flag);
	}

	public T GetObject(out bool didCreate)
	{
		didCreate = (this.objects.Count == 0);
		T result = (!didCreate) ? this.objects.Pop() : this.OnCreate(this.instantiator(this.metaData));
		result.OnRetrieved(this);
		return result;
	}

	public void ReturnObject(T poolObject)
	{
		if (!this.IsInPool(poolObject))
		{
			this.objects.Push(this.OnReturned(poolObject));
		}
		poolObject.OnReturned(this);
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

	protected Func<object, T> instantiator;

	protected Stack<T> objects = new Stack<T>();

	protected object metaData;
}
