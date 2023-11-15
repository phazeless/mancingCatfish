using System;
using System.IO;

namespace SimpleJSON
{
	public class JSONBool : JSONNode
	{
		public JSONBool(bool aData)
		{
			this.m_Data = aData;
		}

		public JSONBool(string aData)
		{
			this.Value = aData;
		}

		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Boolean;
			}
		}

		public override bool IsBoolean
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
				return this.m_Data.ToString();
			}
			set
			{
				bool data;
				if (bool.TryParse(value, out data))
				{
					this.m_Data = data;
				}
			}
		}

		public override bool AsBool
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
			return (!this.m_Data) ? "false" : "true";
		}

		internal override string ToString(string aIndent, string aPrefix)
		{
			return (!this.m_Data) ? "false" : "true";
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(6);
			aWriter.Write(this.m_Data);
		}

		private bool m_Data;
	}
}
