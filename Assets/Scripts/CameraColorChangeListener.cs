using System;
using System.Diagnostics;
using UnityEngine;

public class CameraColorChangeListener : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<Color, Color> OnCameraColorChange;

	private void Awake()
	{
		this.mainCamera = base.GetComponent<Camera>();
		this.color = this.mainCamera.backgroundColor;
		this.previousColor = this.mainCamera.backgroundColor;
	}

	private void Start()
	{
		if (CameraColorChangeListener.OnCameraColorChange != null)
		{
			CameraColorChangeListener.OnCameraColorChange(this.color, this.color);
		}
	}

	private void Update()
	{
		if (this.color != this.mainCamera.backgroundColor)
		{
			this.previousColor = this.color;
			this.color = this.mainCamera.backgroundColor;
			if (CameraColorChangeListener.OnCameraColorChange != null)
			{
				CameraColorChangeListener.OnCameraColorChange(this.color, this.previousColor);
			}
		}
	}

	private Camera mainCamera;

	private Color color = default(Color);

	private Color previousColor = default(Color);
}
