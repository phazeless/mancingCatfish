using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	private void Start()
	{
		if (this.isSpawnActive)
		{
			Canvas canvas = UnityEngine.Object.Instantiate<Canvas>(this.MainScreenCanvas);
			canvas.worldCamera = this.MainCamera;
		}
	}

	[Header("Cameras")]
	[SerializeField]
	private Camera MainCamera;

	[SerializeField]
	[Header("Canvases")]
	private Canvas MainScreenCanvas;

	[SerializeField]
	[Header("TestVariables")]
	private bool isSpawnActive = true;
}
