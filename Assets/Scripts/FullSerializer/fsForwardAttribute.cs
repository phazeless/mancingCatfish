using System;

namespace FullSerializer
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public sealed class fsForwardAttribute : Attribute
	{
		public fsForwardAttribute(string memberName)
		{
			this.MemberName = memberName;
		}

		public string MemberName;
	}
}
