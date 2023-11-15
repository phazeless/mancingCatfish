using System;
using FullInspector;
using UnityEngine;

[fiInspectorOnly]
public class ScriptableObjectWithId : ScriptableObject, IScriptableObjectWithId
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
