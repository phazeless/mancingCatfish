using System;
using UnityEngine;

public abstract class BaseGrantable : fiScriptableObjectWithId, IGrantable
{
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

	public abstract int Amount { get; }

	public abstract void Grant(string contentIdForAnalytics, ResourceChangeReason resourceChangeReason);

	[SerializeField]
	private Sprite icon;

	[SerializeField]
	private Color iconBg = Color.white;

	[SerializeField]
	private string title;

	[SerializeField]
	private string description;
}
