using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

public class SwimBehaviourMono : BaseBehavior
{
	public void AddSwimBehaviour(ISwimBehaviour swimBehaviour)
	{
		ISwimBehaviour swimBehaviour2 = (ISwimBehaviour)swimBehaviour.Clone();
		swimBehaviour2.SetSwimPatternMono(this);
		swimBehaviour2.Awake();
		this.swimBehaviours.Add(swimBehaviour2);
	}

	public void AddSwimBehaviours(List<ISwimBehaviour> swimBehaviours)
	{
		for (int i = 0; i < swimBehaviours.Count; i++)
		{
			ISwimBehaviour swimBehaviour = (ISwimBehaviour)swimBehaviours[i].Clone();
			swimBehaviour.SetSwimPatternMono(this);
			swimBehaviour.Awake();
			this.swimBehaviours.Add(swimBehaviour);
		}
	}

	public T GetSwimBehaviour<T>() where T : ISwimBehaviour
	{
		foreach (ISwimBehaviour swimBehaviour in this.swimBehaviours)
		{
			if (swimBehaviour is T)
			{
				return (T)((object)swimBehaviour);
			}
		}
		return default(T);
	}

	public void ClearSwimBehaviours()
	{
		this.swimBehaviours.Clear();
	}

	public void Start()
	{
		for (int i = 0; i < this.swimBehaviours.Count; i++)
		{
			this.swimBehaviours[i].SetSwimPatternMono(this);
			this.swimBehaviours[i].Awake();
			this.swimBehaviours[i].Start();
		}
	}

	private void Update()
	{
		for (int i = 0; i < this.swimBehaviours.Count; i++)
		{
			this.swimBehaviours[i].Update();
		}
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < this.swimBehaviours.Count; i++)
		{
			this.swimBehaviours[i].FixedUpdate();
		}
	}

	[SerializeField]
	[ShowInInspector]
	private List<ISwimBehaviour> swimBehaviours = new List<ISwimBehaviour>();
}
