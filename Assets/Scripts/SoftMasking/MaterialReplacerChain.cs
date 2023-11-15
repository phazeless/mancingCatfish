using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoftMasking
{
	public class MaterialReplacerChain : IMaterialReplacer
	{
		public MaterialReplacerChain(IEnumerable<IMaterialReplacer> replacers, IMaterialReplacer yetAnother)
		{
			this._replacers = replacers.ToList<IMaterialReplacer>();
			this._replacers.Add(yetAnother);
			this.Initialize();
		}

		public int order { get; private set; }

		public Material Replace(Material material)
		{
			for (int i = 0; i < this._replacers.Count; i++)
			{
				Material material2 = this._replacers[i].Replace(material);
				if (material2 != null)
				{
					return material2;
				}
			}
			return null;
		}

		private void Initialize()
		{
			this._replacers.Sort((IMaterialReplacer a, IMaterialReplacer b) => a.order.CompareTo(b.order));
			this.order = this._replacers[0].order;
		}

		private readonly List<IMaterialReplacer> _replacers;
	}
}
