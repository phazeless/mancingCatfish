using System;
using UnityEngine;

[Serializable]
public class SwimCurvy : BaseSwimBehaviour
{
	public override void Start()
	{
		float? num = this.intialAngle;
		if (num == null)
		{
			this.intialAngle = new float?(base.transform.localEulerAngles.z);
		}
	}

	public override void Update()
	{
	}

	public override void FixedUpdate()
	{
		float num = this.speedster.ActualSpeed * Time.deltaTime * this.swimCurve.Evaluate(this.time) * 10f;
		this.rigidbody2D.MoveRotation(base.transform.eulerAngles.z + num);
		this.time += Time.deltaTime;
		if (this.time > 1f)
		{
			this.time = 0f;
		}
	}

	private float time;

	[SerializeField]
	private AnimationCurve swimCurve = new AnimationCurve();

	private float? intialAngle;
}
