using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FullSerializer
{
	public static class fsJsonPrinter
	{
		private static void InsertSpacing(TextWriter stream, int count)
		{
			for (int i = 0; i < count; i++)
			{
				stream.Write("    ");
			}
		}

		private static string EscapeString(string str)
		{
			bool flag = false;
			int i = 0;
			while (i < str.Length)
			{
				char c = str[i];
				int num = Convert.ToInt32(c);
				if (num < 0 || num > 127)
				{
					flag = true;
					break;
				}
				switch (c)
				{
				case '\a':
				case '\b':
				case '\t':
				case '\n':
				case '\f':
				case '\r':
					goto IL_6D;
				default:
					if (c == '\0' || c == '"' || c == '\\')
					{
						goto IL_6D;
					}
					break;
				}
				IL_74:
				if (flag)
				{
					break;
				}
				i++;
				continue;
				IL_6D:
				flag = true;
				goto IL_74;
			}
			if (!flag)
			{
				return str;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c2 in str)
			{
				int num2 = Convert.ToInt32(c2);
				if (num2 < 0 || num2 > 127)
				{
					stringBuilder.Append(string.Format("\\u{0:x4} ", num2).Trim());
				}
				else
				{
					switch (c2)
					{
					case '\a':
						stringBuilder.Append("\\a");
						break;
					case '\b':
						stringBuilder.Append("\\b");
						break;
					case '\t':
						stringBuilder.Append("\\t");
						break;
					case '\n':
						stringBuilder.Append("\\n");
						break;
					default:
						if (c2 != '\0')
						{
							if (c2 != '"')
							{
								if (c2 != '\\')
								{
									stringBuilder.Append(c2);
								}
								else
								{
									stringBuilder.Append("\\\\");
								}
							}
							else
							{
								stringBuilder.Append("\\\"");
							}
						}
						else
						{
							stringBuilder.Append("\\0");
						}
						break;
					case '\f':
						stringBuilder.Append("\\f");
						break;
					case '\r':
						stringBuilder.Append("\\r");
						break;
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static void BuildCompressedString(fsData data, TextWriter stream)
		{
			switch (data.Type)
			{
			case fsDataType.Array:
			{
				stream.Write('[');
				bool flag = false;
				foreach (fsData data2 in data.AsList)
				{
					if (flag)
					{
						stream.Write(',');
					}
					flag = true;
					fsJsonPrinter.BuildCompressedString(data2, stream);
				}
				stream.Write(']');
				break;
			}
			case fsDataType.Object:
			{
				stream.Write('{');
				bool flag2 = false;
				foreach (KeyValuePair<string, fsData> keyValuePair in data.AsDictionary)
				{
					if (flag2)
					{
						stream.Write(',');
					}
					flag2 = true;
					stream.Write('"');
					stream.Write(keyValuePair.Key);
					stream.Write('"');
					stream.Write(":");
					fsJsonPrinter.BuildCompressedString(keyValuePair.Value, stream);
				}
				stream.Write('}');
				break;
			}
			case fsDataType.Double:
				stream.Write(fsJsonPrinter.ConvertDoubleToString(data.AsDouble));
				break;
			case fsDataType.Int64:
				stream.Write(data.AsInt64);
				break;
			case fsDataType.Boolean:
				if (data.AsBool)
				{
					stream.Write("true");
				}
				else
				{
					stream.Write("false");
				}
				break;
			case fsDataType.String:
				stream.Write('"');
				stream.Write(fsJsonPrinter.EscapeString(data.AsString));
				stream.Write('"');
				break;
			case fsDataType.Null:
				stream.Write("null");
				break;
			}
		}

		private static void BuildPrettyString(fsData data, TextWriter stream, int depth)
		{
			switch (data.Type)
			{
			case fsDataType.Array:
				if (data.AsList.Count == 0)
				{
					stream.Write("[]");
				}
				else
				{
					bool flag = false;
					stream.Write('[');
					stream.WriteLine();
					foreach (fsData data2 in data.AsList)
					{
						if (flag)
						{
							stream.Write(',');
							stream.WriteLine();
						}
						flag = true;
						fsJsonPrinter.InsertSpacing(stream, depth + 1);
						fsJsonPrinter.BuildPrettyString(data2, stream, depth + 1);
					}
					stream.WriteLine();
					fsJsonPrinter.InsertSpacing(stream, depth);
					stream.Write(']');
				}
				break;
			case fsDataType.Object:
			{
				stream.Write('{');
				stream.WriteLine();
				bool flag2 = false;
				foreach (KeyValuePair<string, fsData> keyValuePair in data.AsDictionary)
				{
					if (flag2)
					{
						stream.Write(',');
						stream.WriteLine();
					}
					flag2 = true;
					fsJsonPrinter.InsertSpacing(stream, depth + 1);
					stream.Write('"');
					stream.Write(keyValuePair.Key);
					stream.Write('"');
					stream.Write(": ");
					fsJsonPrinter.BuildPrettyString(keyValuePair.Value, stream, depth + 1);
				}
				stream.WriteLine();
				fsJsonPrinter.InsertSpacing(stream, depth);
				stream.Write('}');
				break;
			}
			case fsDataType.Double:
				stream.Write(fsJsonPrinter.ConvertDoubleToString(data.AsDouble));
				break;
			case fsDataType.Int64:
				stream.Write(data.AsInt64);
				break;
			case fsDataType.Boolean:
				if (data.AsBool)
				{
					stream.Write("true");
				}
				else
				{
					stream.Write("false");
				}
				break;
			case fsDataType.String:
				stream.Write('"');
				stream.Write(fsJsonPrinter.EscapeString(data.AsString));
				stream.Write('"');
				break;
			case fsDataType.Null:
				stream.Write("null");
				break;
			}
		}

		public static void PrettyJson(fsData data, TextWriter outputStream)
		{
			fsJsonPrinter.BuildPrettyString(data, outputStream, 0);
		}

		public static string PrettyJson(fsData data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string result;
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				fsJsonPrinter.BuildPrettyString(data, stringWriter, 0);
				result = stringBuilder.ToString();
			}
			return result;
		}

		public static void CompressedJson(fsData data, StreamWriter outputStream)
		{
			fsJsonPrinter.BuildCompressedString(data, outputStream);
		}

		public static string CompressedJson(fsData data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string result;
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				fsJsonPrinter.BuildCompressedString(data, stringWriter);
				result = stringBuilder.ToString();
			}
			return result;
		}

		private static string ConvertDoubleToString(double d)
		{
			if (double.IsInfinity(d) || double.IsNaN(d))
			{
				return d.ToString(CultureInfo.InvariantCulture);
			}
			string text = d.ToString(CultureInfo.InvariantCulture);
			if (!text.Contains(".") && !text.Contains("e") && !text.Contains("E"))
			{
				text += ".0";
			}
			return text;
		}
	}
}
