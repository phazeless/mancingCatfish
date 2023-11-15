using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACE.Ads
{
	public class UnityAdSettings : UnityBaseSettings<UnityAdSettings>
	{
		public T GetAdProvider<T>() where T : IAdProvider
		{
			return (T)((object)this.adProviders.Find((IAdProvider x) => x is T));
		}

		public List<IAdProvider> GetAllAdProviders()
		{
			return this.adProviders;
		}

		public override string GetMainFolderName()
		{
			return "ACELib-Ads";
		}

		[SerializeField]
		private List<IAdProvider> adProviders = new List<IAdProvider>();
	}
}
