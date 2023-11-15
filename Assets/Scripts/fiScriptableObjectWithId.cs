using System;
using FullInspector;
using UnityEngine;

public class fiScriptableObjectWithId : BaseScriptableObject, IScriptableObjectWithId
{
	public string Id
	{
		get
		{
			return this.id;
		}
		protected set
		{
			this.id = value;
		}
	}

	[InspectorDisabled]
	[SerializeField]
	private string id;
}
