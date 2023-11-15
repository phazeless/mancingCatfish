using System;
using UnityEngine;
using UnityEngine.UI;

public class SoftMaskedImage : Image
{
	private void UpdateMask()
	{
		Vector4 value = new Vector4(base.sprite.textureRect.min.x / (float)base.sprite.texture.width, base.sprite.textureRect.min.y / (float)base.sprite.texture.height, base.sprite.textureRect.max.x / (float)base.sprite.texture.width, base.sprite.textureRect.max.y / (float)base.sprite.texture.height);
		this.material.SetVector("_Rect", value);
	}

	protected override void UpdateMaterial()
	{
		this.UpdateMask();
		base.UpdateMaterial();
	}
}
