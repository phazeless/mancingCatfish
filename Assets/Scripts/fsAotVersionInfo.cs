using System;
using FullSerializer.Internal;

namespace FullSerializer
{
	public struct fsAotVersionInfo
	{
		public bool IsConstructorPublic;

		public fsAotVersionInfo.Member[] Members;

		public struct Member
		{
			public Member(fsMetaProperty property)
			{
				this.MemberName = property.MemberName;
				this.JsonName = property.JsonName;
				this.StorageType = property.StorageType.CSharpName(true);
				this.OverrideConverterType = null;
				if (property.OverrideConverterType != null)
				{
					this.OverrideConverterType = property.OverrideConverterType.CSharpName();
				}
			}

			public override bool Equals(object obj)
			{
				return obj is fsAotVersionInfo.Member && this == (fsAotVersionInfo.Member)obj;
			}

			public override int GetHashCode()
			{
				return this.MemberName.GetHashCode() + 17 * this.JsonName.GetHashCode() + 17 * this.StorageType.GetHashCode() + ((!string.IsNullOrEmpty(this.OverrideConverterType)) ? (17 * this.OverrideConverterType.GetHashCode()) : 0);
			}

			public static bool operator ==(fsAotVersionInfo.Member a, fsAotVersionInfo.Member b)
			{
				return a.MemberName == b.MemberName && a.JsonName == b.JsonName && a.StorageType == b.StorageType && a.OverrideConverterType == b.OverrideConverterType;
			}

			public static bool operator !=(fsAotVersionInfo.Member a, fsAotVersionInfo.Member b)
			{
				return !(a == b);
			}

			public string MemberName;

			public string JsonName;

			public string StorageType;

			public string OverrideConverterType;
		}
	}
}
