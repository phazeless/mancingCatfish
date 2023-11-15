using System;

public interface IPooledObject<T> where T : IPooledObject<T>
{
	void OnAddedToPool(ObjectPool<T> pool);

	void OnRetrieved(ObjectPool<T> pool);

	void OnReturned(ObjectPool<T> pool);
}
