using System;
using System.IO;

namespace SimpleJSON
{
	public class JSONNull : JSONNode
	{
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.NullValue;
			}
		}

		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		public override string Value
		{
			get
			{
				return "null";
			}
			set
			{
			}
		}

		public override bool AsBool
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public override string ToString()
		{
			return "null";
		}

		internal override string ToString(string aIndent, string aPrefix)
		{
			return "null";
		}

		public override bool Equals(object obj)
		{
			return object.ReferenceEquals(this, obj) || obj is JSONNull;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(5);
		}
	}
}
