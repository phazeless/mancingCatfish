using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoftMasking
{
	internal class MaterialReplacements
	{
		public MaterialReplacements(IMaterialReplacer replacer, Action<Material> applyParameters)
		{
			this._replacer = replacer;
			this._applyParameters = applyParameters;
		}

		public Material Get(Material original)
		{
			for (int i = 0; i < this._overrides.Count; i++)
			{
				MaterialReplacements.MaterialOverride materialOverride = this._overrides[i];
				if (object.ReferenceEquals(materialOverride.original, original))
				{
					Material material = materialOverride.Get();
					if (material)
					{
						material.CopyPropertiesFromMaterial(original);
						this._applyParameters(material);
					}
					return material;
				}
			}
			Material material2 = this._replacer.Replace(original);
			if (material2)
			{
				material2.hideFlags = HideFlags.HideAndDontSave;
				this._applyParameters(material2);
			}
			this._overrides.Add(new MaterialReplacements.MaterialOverride(original, material2));
			return material2;
		}

		public void Release(Material replacement)
		{
			for (int i = 0; i < this._overrides.Count; i++)
			{
				MaterialReplacements.MaterialOverride materialOverride = this._overrides[i];
				if (materialOverride.replacement == replacement && materialOverride.Release())
				{
					UnityEngine.Object.DestroyImmediate(replacement);
					this._overrides.RemoveAt(i);
					return;
				}
			}
		}

		public void ApplyAll()
		{
			for (int i = 0; i < this._overrides.Count; i++)
			{
				Material replacement = this._overrides[i].replacement;
				if (replacement)
				{
					this._applyParameters(replacement);
				}
			}
		}

		public void DestroyAllAndClear()
		{
			for (int i = 0; i < this._overrides.Count; i++)
			{
				UnityEngine.Object.DestroyImmediate(this._overrides[i].replacement);
			}
			this._overrides.Clear();
		}

		private readonly IMaterialReplacer _replacer;

		private readonly Action<Material> _applyParameters;

		private readonly List<MaterialReplacements.MaterialOverride> _overrides = new List<MaterialReplacements.MaterialOverride>();

		private class MaterialOverride
		{
			public MaterialOverride(Material original, Material replacement)
			{
				this.original = original;
				this.replacement = replacement;
				this._useCount = 1;
			}

			public Material original { get; private set; }

			public Material replacement { get; private set; }

			public Material Get()
			{
				this._useCount++;
				return this.replacement;
			}

			public bool Release()
			{
				return --this._useCount == 0;
			}

			private int _useCount;
		}
	}
}
