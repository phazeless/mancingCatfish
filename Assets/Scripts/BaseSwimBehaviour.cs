using System;
using UnityEngine;

public abstract class BaseSwimBehaviour : ISwimBehaviour, ICloneable
{
	protected GameObject gameObject
	{
		get
		{
			return this.swimPatternMono.gameObject;
		}
	}

	protected Transform transform
	{
		get
		{
			return this.swimPatternMono.transform;
		}
	}

	public void SetSwimPatternMono(SwimBehaviourMono swimPatternMono)
	{
		this.swimPatternMono = swimPatternMono;
	}

	public virtual void Awake()
	{
		this.speedster = this.gameObject.GetComponent<IHasSpeed>();
		this.rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
	}

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void FixedUpdate()
	{
	}

	public object Clone()
	{
		return base.MemberwiseClone();
	}

	private SwimBehaviourMono swimPatternMono;

	protected IHasSpeed speedster;

	protected Rigidbody2D rigidbody2D;
}
