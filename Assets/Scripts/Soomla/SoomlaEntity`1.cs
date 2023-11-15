using System;
using UnityEngine;

namespace Soomla
{
	public abstract class SoomlaEntity<T>
	{
		protected SoomlaEntity(string id) : this(id, string.Empty, string.Empty)
		{
		}

		protected SoomlaEntity(string id, string name, string description)
		{
			this.Name = name;
			this.Description = description;
			this._id = id;
		}

		protected SoomlaEntity(AndroidJavaObject jniSoomlaEntity)
		{
			this.Name = jniSoomlaEntity.Call<string>("getName", new object[0]);
			this.Description = jniSoomlaEntity.Call<string>("getDescription", new object[0]);
			this._id = jniSoomlaEntity.Call<string>("getID", new object[0]);
		}

		protected SoomlaEntity(JSONObject jsonEntity)
		{
			if (jsonEntity["itemId"] == null)
			{
				SoomlaUtils.LogError("SOOMLA SoomlaEntity", "This is BAD! We don't have ID in the given JSONObject. Stopping here. JSON: " + jsonEntity.print(false));
				return;
			}
			if (jsonEntity["name"])
			{
				this.Name = jsonEntity["name"].str;
			}
			else
			{
				this.Name = string.Empty;
			}
			if (jsonEntity["description"])
			{
				this.Description = jsonEntity["description"].str;
			}
			else
			{
				this.Description = string.Empty;
			}
			this._id = jsonEntity["itemId"].str;
		}

		public string ID
		{
			get
			{
				return this._id;
			}
		}

		public virtual JSONObject toJSONObject()
		{
			if (string.IsNullOrEmpty(this._id))
			{
				SoomlaUtils.LogError("SOOMLA SoomlaEntity", "This is BAD! We don't have ID in the this SoomlaEntity. Stopping here.");
				return null;
			}
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject.AddField("name", this.Name);
			jsonobject.AddField("description", this.Description);
			jsonobject.AddField("itemId", this._id);
			jsonobject.AddField("className", SoomlaUtils.GetClassName(this));
			return jsonobject;
		}

		protected static bool isInstanceOf(AndroidJavaObject jniEntity, string classJniStr)
		{
			IntPtr clazz = AndroidJNI.FindClass(classJniStr);
			return AndroidJNI.IsInstanceOf(jniEntity.GetRawObject(), clazz);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			SoomlaEntity<T> soomlaEntity = obj as SoomlaEntity<T>;
			return soomlaEntity != null && this._id == soomlaEntity._id;
		}

		public bool Equals(SoomlaEntity<T> g)
		{
			return g != null && this._id == g._id;
		}

		public override int GetHashCode()
		{
			return this._id.GetHashCode();
		}

        public static bool operator ==(SoomlaEntity<T> a, SoomlaEntity<T> b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a._id == b._id;
        }

        public static bool operator !=(SoomlaEntity<T> a, SoomlaEntity<T> b)
		{
			return !(a == b);
		}

		public virtual T Clone(string newId)
		{
			JSONObject jsonobject = this.toJSONObject();
			jsonobject.SetField("itemId", JSONObject.CreateStringObject(newId));
			return (T)((object)Activator.CreateInstance(base.GetType(), new object[]
			{
				jsonobject
			}));
		}

		private const string TAG = "SOOMLA SoomlaEntity";

		public string Name;

		public string Description;

		protected string _id;
	}
}
