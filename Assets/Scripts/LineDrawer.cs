using System;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
	private void Start()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			this.points[i].GetComponent<SpriteRenderer>().enabled = false;
		}
		this.UpdateLinePositions();
	}

	private void Update()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			if (this.points[i].hasChanged)
			{
				this.UpdateLinePositions();
				this.points[i].hasChanged = false;
			}
		}
	}

	private void UpdateLinePositions()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			if (this.lineRenderer.positionCount < this.points.Length)
			{
				this.lineRenderer.positionCount = this.points.Length;
			}
			this.lineRenderer.SetPosition(i, this.points[i].position);
		}
	}

	[SerializeField]
	private Transform[] points;

	[SerializeField]
	private LineRenderer lineRenderer;
}
