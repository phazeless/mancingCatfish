using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SimpleJSON
{
	public abstract class JSONNode
	{
		public virtual JSONNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual JSONNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual string Value
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		public virtual bool IsNumber
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsString
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsBoolean
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsArray
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsObject
		{
			get
			{
				return false;
			}
		}

		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		public virtual void Add(JSONNode aItem)
		{
			this.Add(string.Empty, aItem);
		}

		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		public virtual IEnumerable<JSONNode> Children
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<JSONNode> DeepChildren
		{
			get
			{
				foreach (JSONNode C in this.Children)
				{
					foreach (JSONNode D in C.DeepChildren)
					{
						yield return D;
					}
				}
				yield break;
			}
		}

		public override string ToString()
		{
			return "JSONNode";
		}

		public virtual string ToString(string aIndent)
		{
			return this.ToString(aIndent, string.Empty);
		}

		internal abstract string ToString(string aIndent, string aPrefix);

		public abstract JSONNodeType Tag { get; }

		public virtual double AsDouble
		{
			get
			{
				double result = 0.0;
				if (double.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual int AsInt
		{
			get
			{
				return (int)this.AsDouble;
			}
			set
			{
				this.AsDouble = (double)value;
			}
		}

		public virtual float AsFloat
		{
			get
			{
				return (float)this.AsDouble;
			}
			set
			{
				this.AsDouble = (double)value;
			}
		}

		public virtual bool AsBool
		{
			get
			{
				bool result = false;
				if (bool.TryParse(this.Value, out result))
				{
					return result;
				}
				return !string.IsNullOrEmpty(this.Value);
			}
			set
			{
				this.Value = ((!value) ? "false" : "true");
			}
		}

		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		public virtual JSONObject AsObject
		{
			get
			{
				return this as JSONObject;
			}
		}

		public static implicit operator JSONNode(string s)
		{
			return new JSONString(s);
		}

		public static implicit operator string(JSONNode d)
		{
			return (!(d == null)) ? d.Value : null;
		}

		public static implicit operator JSONNode(double n)
		{
			return new JSONNumber(n);
		}

		public static implicit operator double(JSONNode d)
		{
			return (!(d == null)) ? d.AsDouble : 0.0;
		}

		public static implicit operator JSONNode(float n)
		{
			return new JSONNumber((double)n);
		}

		public static implicit operator float(JSONNode d)
		{
			return (!(d == null)) ? d.AsFloat : 0f;
		}

		public static implicit operator JSONNode(int n)
		{
			return new JSONNumber((double)n);
		}

		public static implicit operator int(JSONNode d)
		{
			return (!(d == null)) ? d.AsInt : 0;
		}

		public static implicit operator JSONNode(bool b)
		{
			return new JSONBool(b);
		}

		public static implicit operator bool(JSONNode d)
		{
			return !(d == null) && d.AsBool;
		}

		public static bool operator ==(JSONNode a, object b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			bool flag = a is JSONNull || object.ReferenceEquals(a, null) || a is JSONLazyCreator;
			bool flag2 = b is JSONNull || object.ReferenceEquals(b, null) || b is JSONLazyCreator;
			return flag && flag2;
		}

		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static string Escape(string aText)
		{
			JSONNode.m_EscapeBuilder.Length = 0;
			if (JSONNode.m_EscapeBuilder.Capacity < aText.Length + aText.Length / 10)
			{
				JSONNode.m_EscapeBuilder.Capacity = aText.Length + aText.Length / 10;
			}
			foreach (char c in aText)
			{
				switch (c)
				{
				case '\b':
					JSONNode.m_EscapeBuilder.Append("\\b");
					break;
				case '\t':
					JSONNode.m_EscapeBuilder.Append("\\t");
					break;
				case '\n':
					JSONNode.m_EscapeBuilder.Append("\\n");
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							JSONNode.m_EscapeBuilder.Append(c);
						}
						else
						{
							JSONNode.m_EscapeBuilder.Append("\\\\");
						}
					}
					else
					{
						JSONNode.m_EscapeBuilder.Append("\\\"");
					}
					break;
				case '\f':
					JSONNode.m_EscapeBuilder.Append("\\f");
					break;
				case '\r':
					JSONNode.m_EscapeBuilder.Append("\\r");
					break;
				}
			}
			string result = JSONNode.m_EscapeBuilder.ToString();
			JSONNode.m_EscapeBuilder.Length = 0;
			return result;
		}

		private static void ParseElement(JSONNode ctx, string token, string tokenName, bool quoted)
		{
			if (quoted)
			{
				ctx.Add(tokenName, token);
				return;
			}
			string a = token.ToLower();
			double n;
			if (a == "false" || a == "true")
			{
				ctx.Add(tokenName, a == "true");
			}
			else if (a == "null")
			{
				ctx.Add(tokenName, null);
			}
			else if (double.TryParse(token, out n))
			{
				ctx.Add(tokenName, n);
			}
			else
			{
				ctx.Add(tokenName, token);
			}
		}

		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jsonnode = null;
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.Empty;
			bool flag = false;
			bool flag2 = false;
			while (i < aJSON.Length)
			{
				char c = aJSON[i];
				switch (c)
				{
				case '\t':
					goto IL_26C;
				case '\n':
				case '\r':
					break;
				default:
					switch (c)
					{
					case '[':
						if (flag)
						{
							stringBuilder.Append(aJSON[i]);
							goto IL_368;
						}
						stack.Push(new JSONArray());
						if (jsonnode != null)
						{
							jsonnode.Add(text, stack.Peek());
						}
						text = string.Empty;
						stringBuilder.Length = 0;
						jsonnode = stack.Peek();
						goto IL_368;
					case '\\':
						i++;
						if (flag)
						{
							char c2 = aJSON[i];
							switch (c2)
							{
							case 'r':
								stringBuilder.Append('\r');
								break;
							default:
								if (c2 != 'b')
								{
									if (c2 != 'f')
									{
										if (c2 != 'n')
										{
											stringBuilder.Append(c2);
										}
										else
										{
											stringBuilder.Append('\n');
										}
									}
									else
									{
										stringBuilder.Append('\f');
									}
								}
								else
								{
									stringBuilder.Append('\b');
								}
								break;
							case 't':
								stringBuilder.Append('\t');
								break;
							case 'u':
							{
								string s = aJSON.Substring(i + 1, 4);
								stringBuilder.Append((char)int.Parse(s, NumberStyles.AllowHexSpecifier));
								i += 4;
								break;
							}
							}
						}
						goto IL_368;
					case ']':
						break;
					default:
						switch (c)
						{
						case ' ':
							goto IL_26C;
						default:
							switch (c)
							{
							case '{':
								if (flag)
								{
									stringBuilder.Append(aJSON[i]);
									goto IL_368;
								}
								stack.Push(new JSONObject());
								if (jsonnode != null)
								{
									jsonnode.Add(text, stack.Peek());
								}
								text = string.Empty;
								stringBuilder.Length = 0;
								jsonnode = stack.Peek();
								goto IL_368;
							default:
								if (c != ',')
								{
									if (c != ':')
									{
										stringBuilder.Append(aJSON[i]);
										goto IL_368;
									}
									if (flag)
									{
										stringBuilder.Append(aJSON[i]);
										goto IL_368;
									}
									text = stringBuilder.ToString().Trim();
									stringBuilder.Length = 0;
									flag2 = false;
									goto IL_368;
								}
								else
								{
									if (flag)
									{
										stringBuilder.Append(aJSON[i]);
										goto IL_368;
									}
									if (stringBuilder.Length > 0)
									{
										JSONNode.ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
									}
									text = string.Empty;
									stringBuilder.Length = 0;
									flag2 = false;
									goto IL_368;
								}
								break;
							case '}':
								break;
							}
							break;
						case '"':
							flag ^= true;
							flag2 = (flag2 || flag);
							goto IL_368;
						}
						break;
					}
					if (flag)
					{
						stringBuilder.Append(aJSON[i]);
					}
					else
					{
						if (stack.Count == 0)
						{
							throw new Exception("JSON Parse: Too many closing brackets");
						}
						stack.Pop();
						if (stringBuilder.Length > 0)
						{
							JSONNode.ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
							flag2 = false;
						}
						text = string.Empty;
						stringBuilder.Length = 0;
						if (stack.Count > 0)
						{
							jsonnode = stack.Peek();
						}
					}
					break;
				}
				IL_368:
				i++;
				continue;
				IL_26C:
				if (flag)
				{
					stringBuilder.Append(aJSON[i]);
				}
				goto IL_368;
			}
			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return jsonnode;
		}

		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			this.Serialize(aWriter);
		}

		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToFile(string aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream fileStream = File.OpenWrite(aFileName))
			{
				this.SaveToStream(fileStream);
			}
		}

		public string SaveToBase64()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.SaveToStream(memoryStream);
				memoryStream.Position = 0L;
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}

		public static JSONNode Deserialize(BinaryReader aReader)
		{
			JSONNodeType jsonnodeType = (JSONNodeType)aReader.ReadByte();
			switch (jsonnodeType)
			{
			case JSONNodeType.Array:
			{
				int num = aReader.ReadInt32();
				JSONArray jsonarray = new JSONArray();
				for (int i = 0; i < num; i++)
				{
					jsonarray.Add(JSONNode.Deserialize(aReader));
				}
				return jsonarray;
			}
			case JSONNodeType.Object:
			{
				int num2 = aReader.ReadInt32();
				JSONObject jsonobject = new JSONObject();
				for (int j = 0; j < num2; j++)
				{
					string aKey = aReader.ReadString();
					JSONNode aItem = JSONNode.Deserialize(aReader);
					jsonobject.Add(aKey, aItem);
				}
				return jsonobject;
			}
			case JSONNodeType.String:
				return new JSONString(aReader.ReadString());
			case JSONNodeType.Number:
				return new JSONNumber(aReader.ReadDouble());
			case JSONNodeType.NullValue:
				return new JSONNull();
			case JSONNodeType.Boolean:
				return new JSONBool(aReader.ReadBoolean());
			default:
				throw new Exception("Error deserializing JSON. Unknown tag: " + jsonnodeType);
			}
		}

		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromStream(Stream aData)
		{
			JSONNode result;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				result = JSONNode.Deserialize(binaryReader);
			}
			return result;
		}

		public static JSONNode LoadFromFile(string aFileName)
		{
			JSONNode result;
			using (FileStream fileStream = File.OpenRead(aFileName))
			{
				result = JSONNode.LoadFromStream(fileStream);
			}
			return result;
		}

		public static JSONNode LoadFromBase64(string aBase64)
		{
			byte[] buffer = Convert.FromBase64String(aBase64);
			return JSONNode.LoadFromStream(new MemoryStream(buffer)
			{
				Position = 0L
			});
		}

		internal static StringBuilder m_EscapeBuilder = new StringBuilder();
	}
}
