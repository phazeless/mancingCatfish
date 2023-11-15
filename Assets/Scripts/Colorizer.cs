using System;
using UnityEngine;

[ExecuteInEditMode]
public class Colorizer : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (this.oldColor != this.TintColor)
		{
			this.ChangeColor(base.gameObject, this.TintColor);
		}
		this.oldColor = this.TintColor;
	}

	private void ChangeColor(GameObject effect, Color color)
	{
		Renderer[] componentsInChildren = effect.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			Material material;
			if (this.UseInstanceWhenNotEditorMode)
			{
				material = renderer.material;
			}
			else
			{
				material = renderer.sharedMaterial;
			}
			if (!(material == null) && material.HasProperty("_TintColor"))
			{
				color.a = material.GetColor("_TintColor").a;
				material.SetColor("_TintColor", color);
			}
		}
		Light componentInChildren = effect.GetComponentInChildren<Light>();
		if (componentInChildren != null)
		{
			componentInChildren.color = color;
		}
	}

	public Color TintColor;

	public bool UseInstanceWhenNotEditorMode = true;

	private Color oldColor;
}
