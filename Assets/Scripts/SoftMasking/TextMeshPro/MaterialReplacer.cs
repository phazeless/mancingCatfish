using System;
using UnityEngine;

namespace SoftMasking.TextMeshPro
{
	[GlobalMaterialReplacer]
	public class MaterialReplacer : IMaterialReplacer
	{
		public int order
		{
			get
			{
				return 10;
			}
		}

		public Material Replace(Material material)
		{
			if (material && material.shader && material.shader.name.StartsWith("TextMeshPro/"))
			{
				Shader shader = Shader.Find("Soft Mask/" + material.shader.name);
				if (shader)
				{
					Material material2 = new Material(shader);
					material2.CopyPropertiesFromMaterial(material);
					return material2;
				}
			}
			return null;
		}
	}
}
