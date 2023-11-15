using System;

namespace ACE.Ads
{
	public class AdsBasicPluginSettings : BasicPluginSettings<AdsBasicPluginSettings>
	{
		public override string GetMainFolderName()
		{
			return "ACELib-Ads";
		}
	}
}
