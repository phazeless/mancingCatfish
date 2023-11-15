using System;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFromAtlasShaderHelper : MonoBehaviour
{
	private void Start()
	{
		this.SetImage(this.startImage);
		base.Invoke("SetChangedImage", 1f);
	}

	private void SetChangedImage()
	{
		this.SetImage(this.changedImage);
	}

	public void SetImage(Sprite sprite)
	{
		this.image.sprite = sprite;
		Vector4 value = new Vector4(sprite.textureRect.min.x / (float)sprite.texture.width, sprite.textureRect.min.y / (float)sprite.texture.height, sprite.textureRect.max.x / (float)sprite.texture.width, sprite.textureRect.max.y / (float)sprite.texture.height);
		this.image.material.SetVector("_Rect", value);
	}

	public Sprite startImage;

	public Sprite changedImage;

	public Image image;
}
