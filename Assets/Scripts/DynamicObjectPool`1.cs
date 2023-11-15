using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DynamicObjectPool<T> : IObjectPool<T> where T : MonoBehaviour, IPooledObject<T>
{
	public DynamicObjectPool(params T[] objects) : this(null, objects)
	{
	}

	public DynamicObjectPool(Transform parent, params T[] objects) : this(null, parent, objects)
	{
	}

	public DynamicObjectPool(Action<T> onObjectCreated, Transform parent, params T[] objects)
	{
		this.onObjectCreated = onObjectCreated;
		this.parent = parent;
		foreach (T t in objects)
		{
			Type type = t.GetType();
			if (!this.objectPools.ContainsKey(type))
			{
				this.objectPools.Add(type, new ObjectPool<T>(new Func<object, T>(this.InstantiateCallback), 3, t));
			}
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action<T> onObjectCreated;

	public T GetObject<K>(K prefab)
	{
		Type type = prefab.GetType();
		if (this.objectPools.ContainsKey(type))
		{
			return this.objectPools[type].GetObject();
		}
		return (T)((object)null);
	}

	public void ReturnObject(T poolObject)
	{
		Type type = poolObject.GetType();
		if (this.objectPools.ContainsKey(type))
		{
			this.objectPools[type].ReturnObject(poolObject);
		}
	}

	private T InstantiateCallback(object metaData)
	{
		T t = UnityEngine.Object.Instantiate<T>((T)((object)metaData));
		if (this.parent != null)
		{
			t.transform.SetParent(this.parent, false);
		}
		if (this.onObjectCreated != null)
		{
			this.onObjectCreated(t);
		}
		return t;
	}

	private Transform parent;

	private Dictionary<Type, ObjectPool<T>> objectPools = new Dictionary<Type, ObjectPool<T>>();
}
