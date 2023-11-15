using System;
using System.Collections.Generic;
using System.Security;
using SimpleFirebaseUnity.MiniJSON;

namespace SimpleFirebaseUnity
{
	public class DataSnapshot
	{
		protected DataSnapshot()
		{
			this.val_dict = null;
			this.val_obj = null;
			this.keys = null;
			this.json = null;
		}

		public DataSnapshot(string _json = "")
		{
			object obj = (_json == null || _json.Length <= 0) ? null : Json.Deserialize(_json);
			if (obj is Dictionary<string, object>)
			{
				this.val_dict = (obj as Dictionary<string, object>);
			}
			else
			{
				this.val_obj = obj;
			}
			this.keys = null;
			this.json = _json;
		}

		public List<string> Keys
		{
			get
			{
				if (this.keys == null && this.val_dict != null)
				{
					this.keys = new List<string>(this.val_dict.Keys);
				}
				return this.keys;
			}
		}

		public string FirstKey
		{
			get
			{
				return (this.val_dict != null) ? this.Keys[0] : null;
			}
		}

		public string RawJson
		{
			get
			{
				return this.json;
			}
		}

		public object RawValue
		{
			get
			{
				return (this.val_dict != null) ? this.val_dict : this.val_obj;
			}
		}

		[SecuritySafeCritical]
		public T Value<T>()
		{
			T result;
			try
			{
				if (this.val_obj != null)
				{
					result = (T)((object)this.val_obj);
				}
				else
				{
					object obj = this.val_dict;
					result = (T)((object)obj);
				}
			}
			catch
			{
				result = default(T);
			}
			return result;
		}

		protected object val_obj;

		protected Dictionary<string, object> val_dict;

		protected List<string> keys;

		protected string json;
	}
}
