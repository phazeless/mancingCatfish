using System;

public abstract class BaseItemCustomLogic
{
	public virtual void Init(Item parentItem)
	{
		this.parentItem = parentItem;
	}

	public virtual void Update(Item item)
	{
		if (!this.hasLoaded)
		{
			this.parentItem = item;
			this.Load();
			this.hasLoaded = true;
		}
	}

	public abstract void Save();

	protected abstract void Load();

	public virtual void OnEquipped()
	{
	}

	public virtual void OnUnequipped()
	{
	}

	protected Item parentItem;

	private bool hasLoaded;
}
