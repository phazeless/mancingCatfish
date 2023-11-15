using System;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionHandler : MonoBehaviour
{
	private void Start()
	{
		if (this.mainCamera.aspect < 0.5625f)
		{
			for (int i = 0; i < this.canvasScalers.Length; i++)
			{
				this.canvasScalers[i].matchWidthOrHeight = 0f;
			}
		}
	}

	[SerializeField]
	private CanvasScaler[] canvasScalers;

	[SerializeField]
	private Camera mainCamera;

	[SerializeField]
	private RectTransform rect;
}
