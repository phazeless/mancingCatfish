using System;
using UnityEngine;

[Serializable]
public class SwimStraight : BaseSwimBehaviour
{
	public override void FixedUpdate()
	{
		if (!SwimStraight.isSimulatingBoatMovement)
		{
			this.rigidbody2D.MovePosition(base.transform.position + base.transform.up * Time.deltaTime * this.speedster.ActualSpeed);
		}
		else
		{
			this.rigidbody2D.MovePosition(base.transform.position + base.transform.up * Time.deltaTime * this.speedster.ActualSpeed + new Vector3(0f, -8f * BoatMovementHandler.boatMovementSpeed, 0f) * Time.deltaTime);
		}
	}

	public static bool isSimulatingBoatMovement;
}
