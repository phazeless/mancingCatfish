using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CloudOnce.Internal
{
	public class JSONObject
	{
		public JSONObject(JSONObject.Type t)
		{
			this.ObjectType = t;
			if (t != JSONObject.Type.Array)
			{
				if (t == JSONObject.Type.Object)
				{
					this.List = new List<JSONObject>();
					this.Keys = new List<string>();
				}
			}
			else
			{
				this.List = new List<JSONObject>();
			}
		}

		public JSONObject(bool b)
		{
			this.ObjectType = JSONObject.Type.Bool;
			this.B = b;
		}

		public JSONObject(float f)
		{
			this.ObjectType = JSONObject.Type.Number;
			this.F = f;
		}

		public JSONObject(Dictionary<string, string> dic)
		{
			this.ObjectType = JSONObject.Type.Object;
			this.Keys = new List<string>();
			this.List = new List<JSONObject>();
			foreach (KeyValuePair<string, string> keyValuePair in dic)
			{
				this.Keys.Add(keyValuePair.Key);
				this.List.Add(JSONObject.CreateStringObject(keyValuePair.Value));
			}
		}

		public JSONObject(Dictionary<string, JSONObject> dic)
		{
			this.ObjectType = JSONObject.Type.Object;
			this.Keys = new List<string>();
			this.List = new List<JSONObject>();
			foreach (KeyValuePair<string, JSONObject> keyValuePair in dic)
			{
				this.Keys.Add(keyValuePair.Key);
				this.List.Add(keyValuePair.Value);
			}
		}

		public JSONObject(JSONObject.AddJsonConents content)
		{
			content(this);
		}

		public JSONObject(IEnumerable<JSONObject> objs)
		{
			this.ObjectType = JSONObject.Type.Array;
			this.List = new List<JSONObject>(objs);
		}

		public JSONObject()
		{
		}

		public JSONObject(string str, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
		{
			this.Parse(str, maxDepth, storeExcessLevels, strict);
		}

		public List<JSONObject> List { get; private set; }

		public bool IsContainer
		{
			get
			{
				return this.ObjectType == JSONObject.Type.Array || this.ObjectType == JSONObject.Type.Object;
			}
		}

		public JSONObject.Type ObjectType { get; set; }

		public List<string> Keys { get; private set; }

		public string String { get; private set; }

		public float F { get; private set; }

		public bool B { get; private set; }

		public JSONObject this[int index]
		{
			get
			{
				return (this.List.Count <= index) ? null : this.List[index];
			}
			set
			{
				if (this.List.Count > index)
				{
					this.List[index] = value;
				}
			}
		}

		public JSONObject this[string index]
		{
			get
			{
				return this.GetField(index);
			}
			set
			{
				this.SetField(index, value);
			}
		}

		public static implicit operator bool(JSONObject o)
		{
			return o != null;
		}

		public static JSONObject StringObject(string val)
		{
			return JSONObject.CreateStringObject(val);
		}

		public static JSONObject Create()
		{
			return new JSONObject();
		}

		public static JSONObject Create(JSONObject.Type t)
		{
			JSONObject jsonobject = JSONObject.Create();
			jsonobject.ObjectType = t;
			if (t != JSONObject.Type.Array)
			{
				if (t == JSONObject.Type.Object)
				{
					jsonobject.List = new List<JSONObject>();
					jsonobject.Keys = new List<string>();
				}
			}
			else
			{
				jsonobject.List = new List<JSONObject>();
			}
			return jsonobject;
		}

		public static JSONObject Create(bool val)
		{
			JSONObject jsonobject = JSONObject.Create();
			jsonobject.ObjectType = JSONObject.Type.Bool;
			jsonobject.B = val;
			return jsonobject;
		}

		public static JSONObject Create(float val)
		{
			JSONObject jsonobject = JSONObject.Create();
			jsonobject.ObjectType = JSONObject.Type.Number;
			jsonobject.F = val;
			return jsonobject;
		}

		public static JSONObject Create(int val)
		{
			JSONObject jsonobject = JSONObject.Create();
			jsonobject.ObjectType = JSONObject.Type.Number;
			jsonobject.F = (float)val;
			return jsonobject;
		}

		public static JSONObject CreateStringObject(string val)
		{
			JSONObject jsonobject = JSONObject.Create();
			jsonobject.ObjectType = JSONObject.Type.String;
			jsonobject.String = val;
			return jsonobject;
		}

		public static JSONObject CreateBakedObject(string val)
		{
			JSONObject jsonobject = JSONObject.Create();
			jsonobject.ObjectType = JSONObject.Type.Baked;
			jsonobject.String = val;
			return jsonobject;
		}

		public static JSONObject Create(string val, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
		{
			JSONObject jsonobject = JSONObject.Create();
			jsonobject.Parse(val, maxDepth, storeExcessLevels, strict);
			return jsonobject;
		}

		public static JSONObject Create(JSONObject.AddJsonConents content)
		{
			JSONObject jsonobject = JSONObject.Create();
			content(jsonobject);
			return jsonobject;
		}

		public static JSONObject Create(Dictionary<string, string> dic)
		{
			JSONObject jsonobject = JSONObject.Create();
			jsonobject.ObjectType = JSONObject.Type.Object;
			jsonobject.Keys = new List<string>();
			jsonobject.List = new List<JSONObject>();
			foreach (KeyValuePair<string, string> keyValuePair in dic)
			{
				jsonobject.Keys.Add(keyValuePair.Key);
				jsonobject.List.Add(JSONObject.CreateStringObject(keyValuePair.Value));
			}
			return jsonobject;
		}

		public static JSONObject Create(Dictionary<string, float> dic)
		{
			JSONObject jsonobject = JSONObject.Create();
			jsonobject.ObjectType = JSONObject.Type.Object;
			jsonobject.Keys = new List<string>();
			jsonobject.List = new List<JSONObject>();
			foreach (KeyValuePair<string, float> keyValuePair in dic)
			{
				jsonobject.Keys.Add(keyValuePair.Key);
				jsonobject.List.Add(new JSONObject(keyValuePair.Value));
			}
			return jsonobject;
		}

		public void Absorb(JSONObject obj)
		{
			this.List.AddRange(obj.List);
			this.Keys.AddRange(obj.Keys);
			this.String = obj.String;
			this.F = obj.F;
			this.B = obj.B;
			this.ObjectType = obj.ObjectType;
		}

		public void Add(JSONObject obj)
		{
			if (obj)
			{
				if (this.ObjectType != JSONObject.Type.Array)
				{
					this.ObjectType = JSONObject.Type.Array;
					if (this.List == null)
					{
						this.List = new List<JSONObject>();
					}
				}
				this.List.Add(obj);
			}
		}

		public void AddField(string name, bool val)
		{
			this.AddField(name, JSONObject.Create(val));
		}

		public void AddField(string name, float val)
		{
			this.AddField(name, JSONObject.Create(val));
		}

		public void AddField(string name, string val)
		{
			this.AddField(name, JSONObject.CreateStringObject(val));
		}

		public void AddField(string name, JSONObject obj)
		{
			if (obj)
			{
				if (this.ObjectType != JSONObject.Type.Object)
				{
					if (this.Keys == null)
					{
						this.Keys = new List<string>();
					}
					if (this.ObjectType == JSONObject.Type.Array)
					{
						for (int i = 0; i < this.List.Count; i++)
						{
							this.Keys.Add(i + string.Empty);
						}
					}
					else if (this.List == null)
					{
						this.List = new List<JSONObject>();
					}
					this.ObjectType = JSONObject.Type.Object;
				}
				this.Keys.Add(name);
				this.List.Add(obj);
			}
		}

		public void RemoveField(string name)
		{
			if (this.Keys.IndexOf(name) > -1)
			{
				this.List.RemoveAt(this.Keys.IndexOf(name));
				this.Keys.Remove(name);
			}
		}

		public bool HasFields(params string[] names)
		{
			foreach (string item in names)
			{
				if (!this.Keys.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			return this.Print(false);
		}

		public string ToString(bool pretty)
		{
			return this.Print(pretty);
		}

		public Dictionary<string, string> ToDictionary()
		{
			if (this.ObjectType == JSONObject.Type.Object)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				int i = 0;
				while (i < this.List.Count)
				{
					JSONObject jsonobject = this.List[i];
					switch (jsonobject.ObjectType)
					{
					case JSONObject.Type.String:
						dictionary.Add(this.Keys[i], jsonobject.String);
						break;
					case JSONObject.Type.Number:
						dictionary.Add(this.Keys[i], jsonobject.F.ToString(CultureInfo.InvariantCulture));
						break;
					case JSONObject.Type.Object:
					case JSONObject.Type.Array:
						goto IL_C2;
					case JSONObject.Type.Bool:
						dictionary.Add(this.Keys[i], jsonobject.B + string.Empty);
						break;
					default:
						goto IL_C2;
					}
					IL_E7:
					i++;
					continue;
					IL_C2:
					UnityEngine.Debug.LogWarning("Omitting object: " + this.Keys[i] + " in dictionary conversion");
					goto IL_E7;
				}
				return dictionary;
			}
			return null;
		}

		private void Parse(string str, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
		{
			if (!string.IsNullOrEmpty(str))
			{
				str = str.Trim(JSONObject.s_whitespace);
				if (strict && str[0] != '[' && str[0] != '{')
				{
					this.ObjectType = JSONObject.Type.Null;
					return;
				}
				if (str.Length > 0)
				{
					if (string.Compare(str, "true", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.ObjectType = JSONObject.Type.Bool;
						this.B = true;
					}
					else if (string.Compare(str, "false", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.ObjectType = JSONObject.Type.Bool;
						this.B = false;
					}
					else if (string.Compare(str, "null", StringComparison.OrdinalIgnoreCase) == 0)
					{
						this.ObjectType = JSONObject.Type.Null;
					}
					else
					{
						if (str != null)
						{
							if (str == "\"INFINITY\"")
							{
								this.ObjectType = JSONObject.Type.Number;
								this.F = float.PositiveInfinity;
								goto IL_3C9;
							}
							if (str == "\"NEGINFINITY\"")
							{
								this.ObjectType = JSONObject.Type.Number;
								this.F = float.NegativeInfinity;
								goto IL_3C9;
							}
							if (str == "\"NaN\"")
							{
								this.ObjectType = JSONObject.Type.Number;
								this.F = float.NaN;
								goto IL_3C9;
							}
						}
						if (str[0] == '"')
						{
							this.ObjectType = JSONObject.Type.String;
							this.String = str.Substring(1, str.Length - 2);
						}
						else
						{
							int num = 1;
							int num2 = 0;
							char c = str[num2];
							if (c != '{')
							{
								if (c != '[')
								{
									try
									{
										this.F = Convert.ToSingle(str, CultureInfo.InvariantCulture);
										this.ObjectType = JSONObject.Type.Number;
									}
									catch (FormatException)
									{
										this.ObjectType = JSONObject.Type.Null;
									}
									return;
								}
								this.ObjectType = JSONObject.Type.Array;
								this.List = new List<JSONObject>();
							}
							else
							{
								this.ObjectType = JSONObject.Type.Object;
								this.Keys = new List<string>();
								this.List = new List<JSONObject>();
							}
							string item = string.Empty;
							bool flag = false;
							bool flag2 = false;
							int num3 = 0;
							while (++num2 < str.Length)
							{
								if (Array.IndexOf<char>(JSONObject.s_whitespace, str[num2]) <= -1)
								{
									if (str[num2] == '\\')
									{
										num2 += 2;
									}
									if (str[num2] == '"')
									{
										if (flag)
										{
											if (!flag2 && num3 == 0 && this.ObjectType == JSONObject.Type.Object)
											{
												item = str.Substring(num + 1, num2 - num - 1);
											}
											flag = false;
										}
										else
										{
											if (num3 == 0 && this.ObjectType == JSONObject.Type.Object)
											{
												num = num2;
											}
											flag = true;
										}
									}
									if (!flag)
									{
										if (this.ObjectType == JSONObject.Type.Object && num3 == 0 && str[num2] == ':')
										{
											num = num2 + 1;
											flag2 = true;
										}
										char c2 = str[num2];
										switch (c2)
										{
										case '[':
											goto IL_2F6;
										default:
											switch (c2)
											{
											case '{':
												goto IL_2F6;
											case '}':
												goto IL_301;
											}
											break;
										case ']':
											goto IL_301;
										}
										IL_30C:
										if ((str[num2] == ',' && num3 == 0) || num3 < 0)
										{
											flag2 = false;
											string text = str.Substring(num, num2 - num).Trim(JSONObject.s_whitespace);
											if (text.Length > 0)
											{
												if (this.ObjectType == JSONObject.Type.Object)
												{
													this.Keys.Add(item);
												}
												if (maxDepth != -1)
												{
													this.List.Add(JSONObject.Create(text, (maxDepth >= -1) ? (maxDepth - 1) : -2, false, false));
												}
												else if (storeExcessLevels)
												{
													this.List.Add(JSONObject.CreateBakedObject(text));
												}
											}
											num = num2 + 1;
											continue;
										}
										continue;
										IL_2F6:
										num3++;
										goto IL_30C;
										IL_301:
										num3--;
										goto IL_30C;
									}
								}
							}
						}
					}
					IL_3C9:;
				}
				else
				{
					this.ObjectType = JSONObject.Type.Null;
				}
			}
			else
			{
				this.ObjectType = JSONObject.Type.Null;
			}
		}

		private void SetField(string name, JSONObject obj)
		{
			if (this.HasField(name))
			{
				this.List.Remove(this[name]);
				this.Keys.Remove(name);
			}
			this.AddField(name, obj);
		}

		private JSONObject GetField(string name)
		{
			if (this.ObjectType == JSONObject.Type.Object)
			{
				for (int i = 0; i < this.Keys.Count; i++)
				{
					if (this.Keys[i] == name)
					{
						return this.List[i];
					}
				}
			}
			return null;
		}

		private bool HasField(string name)
		{
			return this.ObjectType == JSONObject.Type.Object && this.Keys.Any((string t) => t == name);
		}

		private string Print(bool pretty = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.Stringify(0, stringBuilder, pretty);
			return stringBuilder.ToString();
		}

		private void Stringify(int depth, StringBuilder builder, bool pretty = false)
		{
			if (depth++ > 100)
			{
				return;
			}
			switch (this.ObjectType)
			{
			case JSONObject.Type.Null:
				builder.Append("null");
				break;
			case JSONObject.Type.String:
				builder.AppendFormat("\"{0}\"", this.String);
				break;
			case JSONObject.Type.Number:
				if (float.IsInfinity(this.F))
				{
					builder.Append("\"INFINITY\"");
				}
				else if (float.IsNegativeInfinity(this.F))
				{
					builder.Append("\"NEGINFINITY\"");
				}
				else if (float.IsNaN(this.F))
				{
					builder.Append("\"NaN\"");
				}
				else
				{
					builder.Append(this.F.ToString(CultureInfo.InvariantCulture));
				}
				break;
			case JSONObject.Type.Object:
				builder.Append("{");
				if (this.List.Count > 0)
				{
					if (pretty)
					{
						builder.Append("\n");
					}
					for (int i = 0; i < this.List.Count; i++)
					{
						string arg = this.Keys[i];
						JSONObject jsonobject = this.List[i];
						if (jsonobject)
						{
							if (pretty)
							{
								for (int j = 0; j < depth; j++)
								{
									builder.Append("\t");
								}
							}
							builder.AppendFormat("\"{0}\":", arg);
							jsonobject.Stringify(depth, builder, pretty);
							builder.Append(",");
							if (pretty)
							{
								builder.Append("\n");
							}
						}
					}
					if (pretty)
					{
						builder.Length -= 2;
					}
					else
					{
						builder.Length--;
					}
				}
				if (pretty && this.List.Count > 0)
				{
					builder.Append("\n");
					for (int k = 0; k < depth - 1; k++)
					{
						builder.Append("\t");
					}
				}
				builder.Append("}");
				break;
			case JSONObject.Type.Array:
				builder.Append("[");
				if (this.List.Count > 0)
				{
					if (pretty)
					{
						builder.Append("\n");
					}
					foreach (JSONObject jsonobject2 in this.List)
					{
						if (jsonobject2)
						{
							if (pretty)
							{
								for (int l = 0; l < depth; l++)
								{
									builder.Append("\t");
								}
							}
							jsonobject2.Stringify(depth, builder, pretty);
							builder.Append(",");
							if (pretty)
							{
								builder.Append("\n");
							}
						}
					}
					if (pretty)
					{
						builder.Length -= 2;
					}
					else
					{
						builder.Length--;
					}
				}
				if (pretty && this.List.Count > 0)
				{
					builder.Append("\n");
					for (int m = 0; m < depth - 1; m++)
					{
						builder.Append("\t");
					}
				}
				builder.Append("]");
				break;
			case JSONObject.Type.Bool:
				builder.Append((!this.B) ? "false" : "true");
				break;
			case JSONObject.Type.Baked:
				builder.Append(this.String);
				break;
			}
		}

		private const int maxDepth = 100;

		private const string infinity = "\"INFINITY\"";

		private const string negInfinity = "\"NEGINFINITY\"";

		private const string nan = "\"NaN\"";

		private static readonly char[] s_whitespace = new char[]
		{
			' ',
			'\r',
			'\n',
			'\t'
		};

		public delegate void AddJsonConents(JSONObject self);

		public enum Type
		{
			Null,
			String,
			Number,
			Object,
			Array,
			Bool,
			Baked
		}
	}
}
