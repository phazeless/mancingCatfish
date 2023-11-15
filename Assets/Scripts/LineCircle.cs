using System;
using UnityEngine;

public class LineCircle : MonoBehaviour
{
	private void Start()
	{
		this.line = base.gameObject.GetComponent<LineRenderer>();
		this.line.positionCount = this.segments + 1;
		this.line.useWorldSpace = false;
		this.CreatePoints();
	}

	private void CreatePoints()
	{
		float z = 0f;
		float num = 0f;
		for (int i = 0; i < this.segments + 1; i++)
		{
			float x = Mathf.Sin(0.0174532924f * num);
			float y = Mathf.Cos(0.0174532924f * num);
			this.line.SetPosition(i, new Vector3(x, y, z) * this.radius);
			num += 360f / (float)this.segments;
		}
	}

	public int segments;

	public float radius;

	private LineRenderer line;
}
