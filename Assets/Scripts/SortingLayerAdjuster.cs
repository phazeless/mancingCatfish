using System;
using UnityEngine;

public class SortingLayerAdjuster : MonoBehaviour
{
	private void Start()
	{
		this.trail.sortingLayerName = "TrailRenderer";
		this.trail.sortingOrder = -20;
	}

	public TrailRenderer trail;
}
