using System;
using System.Collections.Generic;

namespace Soomla
{
	public class CustomEvent : SoomlaEvent
	{
		public CustomEvent(string name, Dictionary<string, string> extra) : this(name, extra, null)
		{
		}

		public CustomEvent(string name, Dictionary<string, string> extra, object sender) : base(sender)
		{
			this.Name = name;
			this.Extra = extra;
		}

		public string GetName()
		{
			return this.Name;
		}

		public Dictionary<string, string> GetExtra()
		{
			return this.Extra;
		}

		private string Name;

		private Dictionary<string, string> Extra;
	}
}
