using System;
using UnityEngine;

namespace SimpleFirebaseUnity
{
	public struct FirebaseParam
	{
		public FirebaseParam(string _param = "")
		{
			this.param = _param;
		}

		public string Parameter
		{
			get
			{
				return this.param;
			}
		}

		public string SafeParameter
		{
			get
			{
				return WWW.EscapeURL(this.param);
			}
		}

		public FirebaseParam Add(string parameter)
		{
			if (this.param != null && this.param.Length > 0)
			{
				this.param += "&";
			}
			this.param += parameter;
			return this;
		}

		public FirebaseParam Add(string header, string value, bool quoted = true)
		{
			return (!quoted) ? this.Add(header + "=" + value) : this.Add(header + "=\"" + value + "\"");
		}

		public FirebaseParam Add(string header, int value)
		{
			return this.Add(header + "=" + value);
		}

		public FirebaseParam Add(string header, float value)
		{
			return this.Add(header + "=" + value);
		}

		public FirebaseParam Add(string header, bool value)
		{
			return this.Add(header + "=" + value);
		}

		public FirebaseParam OrderByChild(string key)
		{
			return this.Add("orderBy", key, true);
		}

		public FirebaseParam OrderByKey()
		{
			return this.Add("orderBy", "$key", true);
		}

		public FirebaseParam OrderByValue()
		{
			return this.Add("orderBy", "$value", true);
		}

		public FirebaseParam OrderByPriority()
		{
			return this.Add("orderBy", "$priority", true);
		}

		public FirebaseParam LimitToFirst(int lim)
		{
			return this.Add("limitToFirst", lim);
		}

		public FirebaseParam LimitToLast(int lim)
		{
			return this.Add("limitToLast", lim);
		}

		public FirebaseParam StartAt(string start)
		{
			return this.Add("startAt", start, true);
		}

		public FirebaseParam StartAt(int start)
		{
			return this.Add("startAt", start);
		}

		public FirebaseParam StartAt(bool start)
		{
			return this.Add("startAt", start);
		}

		public FirebaseParam StartAt(float start)
		{
			return this.Add("startAt", start);
		}

		public FirebaseParam EndAt(string end)
		{
			return this.Add("endAt", end, true);
		}

		public FirebaseParam EndAt(int end)
		{
			return this.Add("endAt", end);
		}

		public FirebaseParam EndAt(bool end)
		{
			return this.Add("endAt", end);
		}

		public FirebaseParam EndAt(float end)
		{
			return this.Add("endAt", end);
		}

		public FirebaseParam EqualTo(string at)
		{
			return this.Add("equalTo", at, true);
		}

		public FirebaseParam EqualTo(int at)
		{
			return this.Add("equalTo", at);
		}

		public FirebaseParam EqualTo(bool at)
		{
			return this.Add("equalTo", at);
		}

		public FirebaseParam EqualTo(float at)
		{
			return this.Add("equalTo", at);
		}

		public FirebaseParam PrintPretty()
		{
			return this.Add("print=pretty");
		}

		public FirebaseParam PrintSilent()
		{
			return this.Add("print=silent");
		}

		public FirebaseParam Shallow()
		{
			return this.Add("shallow=true");
		}

		public FirebaseParam Auth(string cred)
		{
			return this.Add("auth=" + cred);
		}

		public override string ToString()
		{
			return this.param;
		}

		public static FirebaseParam Empty
		{
			get
			{
				return default(FirebaseParam);
			}
		}

		private string param;
	}
}
