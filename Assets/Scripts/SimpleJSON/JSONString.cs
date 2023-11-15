using System;
using System.IO;

namespace SimpleJSON
{
	public class JSONString : JSONNode
	{
		public JSONString(string aData)
		{
			this.m_Data = aData;
		}

		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.String;
			}
		}

		public override bool IsString
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
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		public override string ToString()
		{
			return "\"" + JSONNode.Escape(this.m_Data) + "\"";
		}

		internal override string ToString(string aIndent, string aPrefix)
		{
			return "\"" + JSONNode.Escape(this.m_Data) + "\"";
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(3);
			aWriter.Write(this.m_Data);
		}

		private string m_Data;
	}
}
