using System;
using UnityEngine;

[Serializable]
public class WheelSlice
{
	public float size = 2f;

	public bool isFreeSpin;

	public bool isFishValueMultiplier;

	public bool isGemReward;

	public float amount;

	[HideInInspector]
	public Vector2 degreesFromTo;
}
