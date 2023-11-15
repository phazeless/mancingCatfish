using System;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class CatchFishBehaviour : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<FishBehaviour> OnFishEnterCatchArea;

	private void Awake()
	{
		base.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		FishBehaviour componentInParent = collider.GetComponentInParent<FishBehaviour>();
		bool flag = componentInParent != null && CatchFishBehaviour.OnFishEnterCatchArea != null;
		if (flag)
		{
			CatchFishBehaviour.OnFishEnterCatchArea(componentInParent);
		}
	}
}
