using System;
using System.Diagnostics;
using FullInspector;
using UnityEngine;

[fiInspectorOnly]
public abstract class BaseConsumable : ScriptableObjectWithId
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BaseConsumable, int> OnConsumed;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<BaseConsumable, int> OnGranted;

	public virtual Sprite Icon
	{
		get
		{
			return this.icon;
		}
	}

	public virtual Color IconBg
	{
		get
		{
			return this.iconBg;
		}
	}

	public virtual string Title
	{
		get
		{
			return this.title;
		}
	}

	public virtual string Description
	{
		get
		{
			return this.description;
		}
	}

	public abstract int MaxAmount { get; }

	public virtual void Consume(int amount)
	{
		if (this.OnConsumed != null)
		{
			this.OnConsumed(this, amount);
		}
	}

	public virtual void Grant(int amount)
	{
		if (this.OnGranted != null)
		{
			this.OnGranted(this, amount);
		}
	}

	[SerializeField]
	private Sprite icon;

	[SerializeField]
	private Color iconBg = Color.white;

	[SerializeField]
	private string title;

	[SerializeField]
	private string description;
}
