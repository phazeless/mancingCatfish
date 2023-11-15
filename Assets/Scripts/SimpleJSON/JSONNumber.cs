using System;
using System.IO;

namespace SimpleJSON
{
	public class JSONNumber : JSONNode
	{
		public JSONNumber(double aData)
		{
			this.m_Data = aData;
		}

		public JSONNumber(string aData)
		{
			this.Value = aData;
		}

		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Number;
			}
		}

		public override bool IsNumber
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
				double data;
				if (double.TryParse(value, out data))
				{
					this.m_Data = data;
				}
			}
		}

		public override double AsDouble
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
			return this.m_Data.ToString();
		}

		internal override string ToString(string aIndent, string aPrefix)
		{
			return this.m_Data.ToString();
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(4);
			aWriter.Write(this.m_Data);
		}

		private double m_Data;
	}
}
