using System;
using UnityEngine;

public abstract class UIListItem : MonoBehaviour, IPooledObject<UIListItem>
{
	public abstract void OnAddedToPool(ObjectPool<UIListItem> pool);

	public abstract void OnRetrieved(ObjectPool<UIListItem> pool);

	public abstract void OnReturned(ObjectPool<UIListItem> pool);

	public abstract void OnUpdateUI(IListItemContent content);

	public abstract void OnShouldRegisterListeners();

	public abstract void OnShouldUnregisterListeners();

	protected virtual void OnEnable()
	{
		this.OnShouldRegisterListeners();
	}

	protected virtual void OnDisable()
	{
		this.OnShouldUnregisterListeners();
	}

	protected virtual void OnDestroy()
	{
		this.OnShouldUnregisterListeners();
	}

	public abstract void SetSizes(float pixelRelation);
}
