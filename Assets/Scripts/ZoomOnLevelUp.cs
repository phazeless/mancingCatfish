using System;
using UnityEngine;

[Serializable]
public class ZoomOnLevelUp : ILevelUpListener
{
	public void OnLevelUp(GameObject caller, Skill skill)
	{
		CameraMovement.Instance.ZoomTo(this.zoom, 3f);
	}

	[SerializeField]
	private float zoom = 1f;
}
