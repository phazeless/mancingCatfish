using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabObjectPool<T> : IObjectPool<T> where T : MonoBehaviour, IPooledObject<T>
{
	public PrefabObjectPool(Transform parent, List<T> prefabs)
	{
		this.parent = parent;
		this.prefabs = prefabs;
		foreach (T t in prefabs)
		{
			this.objectPools.Add(t, new ObjectPool<T>(new Func<object, T>(this.InstantiateCallback), 0, t));
			this.objectsByPool.Add(t, new List<T>());
		}
	}

	public T GetObject(T prefab)
	{
		if (this.objectPools.ContainsKey(prefab))
		{
			T @object = this.objectPools[prefab].GetObject();
			this.objectsByPool[prefab].Add(@object);
			return @object;
		}
		return (T)((object)null);
	}

	public void ReturnObject(T poolObject)
	{
		ObjectPool<T> objectPool = this.FindPoolOfObject(poolObject);
		if (objectPool != null)
		{
			object metaData = objectPool.MetaData;
			objectPool.ReturnObject(poolObject);
			this.objectsByPool[(T)((object)metaData)].Remove(poolObject);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Trying to return an object to a ObjectPool that doesn't exist.");
		}
	}

	private ObjectPool<T> FindPoolOfObject(T poolObject)
	{
		T key = (T)((object)null);
		foreach (KeyValuePair<T, List<T>> keyValuePair in this.objectsByPool)
		{
			T key2 = keyValuePair.Key;
			List<T> value = keyValuePair.Value;
			if (poolObject.Equals(value))
			{
				key = key2;
			}
		}
		if (this.objectPools.ContainsKey(key))
		{
			return this.objectPools[key];
		}
		return null;
	}

	private T InstantiateCallback(object metaData)
	{
		return UnityEngine.Object.Instantiate<T>((T)((object)metaData), this.parent, false);
	}

	private Dictionary<T, ObjectPool<T>> objectPools = new Dictionary<T, ObjectPool<T>>();

	private Dictionary<T, List<T>> objectsByPool = new Dictionary<T, List<T>>();

	private Transform parent;

	private List<T> prefabs = new List<T>();
}
