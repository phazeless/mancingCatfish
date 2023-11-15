using System;
using UnityEngine;

[Serializable]
public class SwimTowards : BaseSwimBehaviour
{
	[HideInInspector]
	public float ActualTurnAngle { get; set; }

	[HideInInspector]
	public bool HasOverriddenTurnAngle { get; set; }

	public override void Start()
	{
		this.time = 0f;
		this.turnModifier = 10f;
		this.timeUntilAngleReached = 9999f;
		if (!this.HasOverriddenTurnAngle)
		{
			this.ActualTurnAngle = UnityEngine.Random.Range(this.minTurnAngle, this.maxTurnAngle);
		}
	}

	public override void FixedUpdate()
	{
		this.time += Time.deltaTime;
		if (this.time > this.timeUntilAngleReached)
		{
			return;
		}
		this.turnModifier = 10f - base.transform.localPosition.x * 1f;
		this.anglePerFrame = this.speedster.ActualSpeed * Time.deltaTime * this.turnModifier;
		this.timeUntilAngleReached = this.turnTime / this.anglePerFrame;
		this.rigidbody2D.MoveRotation(base.transform.eulerAngles.z + this.anglePerFrame * this.ActualTurnAngle);
	}

	[SerializeField]
	private float turnTime;

	public float minTurnAngle = -1f;

	public float maxTurnAngle = 1f;

	private float time;

	private Vector3 startAngle = Vector3.zero;

	private float turnModifier = 1f;

	private float timeUntilAngleReached = 9999f;

	private float anglePerFrame;
}
