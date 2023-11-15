using System;
using System.Collections.Generic;

public class FishPool : ObjectPool<FishBehaviour>
{
	public FishPool(Func<object, FishBehaviour> instantiator, int initialSize, FishBehaviour prefab) : base(instantiator, initialSize, prefab)
	{
	}

	protected override FishBehaviour OnCreate(FishBehaviour createdObject)
	{
		this.allCreatedObjects.Add(createdObject);
		createdObject.OnAddedToPool(this);
		return createdObject;
	}

	protected override FishBehaviour OnReturned(FishBehaviour returnedObject)
	{
		returnedObject.gameObject.SetActive(false);
		return returnedObject;
	}

	public void ClearAndDestroyPoolObjects()
	{
		for (int i = 0; i < this.allCreatedObjects.Count; i++)
		{
			base.ReturnObject(this.allCreatedObjects[i]);
		}
	}

	private List<FishBehaviour> allCreatedObjects = new List<FishBehaviour>();
}
